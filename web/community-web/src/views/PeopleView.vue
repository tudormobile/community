<script setup lang="ts">
import { computed, onBeforeUnmount, onMounted, ref, watch } from 'vue'
import appConfig from '@/assets/config.json'
import usersAsset from '@/assets/users.json'
import familyIcon from '@/assets/icons/family.svg'
import userCheckIcon from '@/assets/icons/user_check.svg'
import MemberRow from '@/components/MemberRow.vue'
import PartyRow from '@/components/PartyRow.vue'
import TravelGroupHeader from '@/components/TravelGroupHeader.vue'
import { listThumbnails, type ThumbnailRecord } from '@/lib/thumbnailStore'
import type { AppConfig } from '@/types/config'
import type { EntityId, Member, Party, TravelGroup, UsersAsset } from '@/types/users'

type PartyStatus = 'all' | 'some' | 'none'

function normalizeId(id: EntityId | undefined, fallback: string): EntityId {
  return id ?? fallback
}

function cloneMember(member: Member, memberIndex: number, partyId: EntityId): Member {
  return {
    id: normalizeId(member.id, `${String(partyId)}-member-${memberIndex}`),
    name: member.name,
    present: Boolean(member.present),
  }
}

function cloneParty(party: Party, partyIndex: number, groupId: EntityId): Party {
  const partyId = normalizeId(party.id, `${String(groupId)}-party-${partyIndex}`)

  return {
    id: partyId,
    name: party.name,
    expanded: Boolean(party.expanded),
    members: party.members.map((member, memberIndex) => cloneMember(member, memberIndex, partyId)),
  }
}

function cloneTravelGroups(groups: TravelGroup[]): TravelGroup[] {
  return groups.map((group, groupIndex) => {
    const groupId = normalizeId(group.id, `group-${groupIndex}`)

    return {
      id: groupId,
      name: group.name,
      parties: group.parties.map((party, partyIndex) => cloneParty(party, partyIndex, groupId)),
    }
  })
}

const sourceGroups = (usersAsset as UsersAsset).travelGroups
const config = appConfig as AppConfig
const STORAGE_KEY = 'community-web.people.attendance.v1'
const SELECTED_GROUP_STORAGE_KEY = 'community-web.people.selected-group.v1'
const ALL_GROUPS = 'all'

function loadPersistedTravelGroups(): TravelGroup[] | null {
  try {
    const raw = localStorage.getItem(STORAGE_KEY)

    if (!raw) {
      return null
    }

    const parsed = JSON.parse(raw) as unknown

    if (!Array.isArray(parsed)) {
      return null
    }

    return cloneTravelGroups(parsed as TravelGroup[])
  } catch {
    return null
  }
}

const travelGroups = ref<TravelGroup[]>(loadPersistedTravelGroups() ?? cloneTravelGroups(sourceGroups))
const thumbnailSrcByPartyId = ref<Record<string, string>>({})
let previousThumbnailObjectUrls: string[] = []

function isValidGroupId(value: unknown): value is EntityId {
  return travelGroups.value.some((group) => group.id === value)
}

function loadPersistedSelectedGroupId(): EntityId | typeof ALL_GROUPS {
  try {
    const raw = localStorage.getItem(SELECTED_GROUP_STORAGE_KEY)

    if (!raw) {
      return ALL_GROUPS
    }

    const parsed = JSON.parse(raw) as unknown

    if (parsed === ALL_GROUPS) {
      return ALL_GROUPS
    }

    return isValidGroupId(parsed) ? parsed : ALL_GROUPS
  } catch {
    return ALL_GROUPS
  }
}

const selectedGroupId = ref<EntityId | typeof ALL_GROUPS>(loadPersistedSelectedGroupId())
const showIncompleteOnly = ref(false)

watch(
  travelGroups,
  (groups) => {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(groups))
  },
  { deep: true }
)

watch(selectedGroupId, (value) => {
  localStorage.setItem(SELECTED_GROUP_STORAGE_KEY, JSON.stringify(value))
})

function memberCount(party: Party): number {
  return party.members.length
}

function presentCount(party: Party): number {
  return party.members.reduce((count, member) => count + (member.present ? 1 : 0), 0)
}

function partyStatus(party: Party): PartyStatus {
  const total = memberCount(party)
  const present = presentCount(party)

  if (present === 0) {
    return 'none'
  }

  if (present === total) {
    return 'all'
  }

  return 'some'
}

function groupMemberCount(group: TravelGroup): number {
  return group.parties.reduce((count, party) => count + memberCount(party), 0)
}

function groupPresentCount(group: TravelGroup): number {
  return group.parties.reduce((count, party) => count + presentCount(party), 0)
}

const totalMembers = computed<number>(() => {
  return travelGroups.value.reduce((count, group) => count + groupMemberCount(group), 0)
})

const totalPresent = computed<number>(() => {
  return travelGroups.value.reduce((count, group) => count + groupPresentCount(group), 0)
})

const visibleTravelGroups = computed<TravelGroup[]>(() => {
  if (selectedGroupId.value === ALL_GROUPS) {
    return travelGroups.value
  }

  return travelGroups.value.filter((group) => group.id === selectedGroupId.value)
})

const filteredVisibleTravelGroups = computed<TravelGroup[]>(() => {
  if (!showIncompleteOnly.value) {
    return visibleTravelGroups.value
  }

  return visibleTravelGroups.value
    .map((group) => ({
      ...group,
      parties: group.parties.filter((party) => partyStatus(party) !== 'all'),
    }))
    .filter((group) => group.parties.length > 0)
})

function findParty(groupId: EntityId, partyId: EntityId): Party | null {
  const group = travelGroups.value.find((candidateGroup) => candidateGroup.id === groupId)

  if (!group) {
    return null
  }

  return group.parties.find((candidateParty) => candidateParty.id === partyId) ?? null
}

function toThumbnailMap(rows: ThumbnailRecord[]): Record<string, string> {
  for (const previousUrl of previousThumbnailObjectUrls) {
    URL.revokeObjectURL(previousUrl)
  }

  previousThumbnailObjectUrls = []

  const nextMap: Record<string, string> = {}

  for (const row of rows) {
    const objectUrl = URL.createObjectURL(row.blob)
    previousThumbnailObjectUrls.push(objectUrl)
    nextMap[row.partyId] = objectUrl
  }

  return nextMap
}

async function refreshPartyThumbnails() {
  try {
    const rows = await listThumbnails()
    thumbnailSrcByPartyId.value = toThumbnailMap(rows)
  } catch {
    thumbnailSrcByPartyId.value = {}
  }
}

function thumbnailSrcForParty(partyId: EntityId): string | null {
  return thumbnailSrcByPartyId.value[String(partyId)] ?? null
}

function markPartyPresent(groupId: EntityId, partyId: EntityId) {
  const party = findParty(groupId, partyId)

  if (!party) {
    return
  }

  party.members.forEach((member) => {
    member.present = true
  })
}

function togglePartyExpanded(groupId: EntityId, partyId: EntityId) {
  const party = findParty(groupId, partyId)

  if (!party) {
    return
  }

  party.expanded = !party.expanded
}

function updateMemberPresent(groupId: EntityId, partyId: EntityId, memberId: EntityId, nextValue: boolean) {
  const party = findParty(groupId, partyId)

  if (!party) {
    return
  }

  const member = party.members.find((candidateMember) => candidateMember.id === memberId)

  if (!member) {
    return
  }

  member.present = nextValue
}

function resetAttendance() {
  travelGroups.value.forEach((group) => {
    group.parties.forEach((party) => {
      party.members.forEach((member) => {
        member.present = false
      })
    })
  })
}

function groupStatus(group: TravelGroup): PartyStatus {
  const total = groupMemberCount(group)
  const present = groupPresentCount(group)

  if (present === 0) {
    return 'none'
  }

  if (present === total) {
    return 'all'
  }

  return 'some'
}

function resetGroup(groupId: EntityId) {
  const group = travelGroups.value.find((candidateGroup) => candidateGroup.id === groupId)

  if (!group) {
    return
  }

  group.parties.forEach((party) => {
    party.members.forEach((member) => {
      member.present = false
    })
  })
}

onMounted(async () => {
  await refreshPartyThumbnails()
})

onBeforeUnmount(() => {
  for (const previousUrl of previousThumbnailObjectUrls) {
    URL.revokeObjectURL(previousUrl)
  }

  previousThumbnailObjectUrls = []
})
</script>

<template>
  <main class="people-view">
    <header class="people-toolbar">
      <div class="toolbar-actions">
        <button type="button" class="reset-button" @click="resetAttendance">RESET ALL</button>

        <button
          type="button"
          class="group-filter-button incomplete-filter-button"
          :class="{ active: showIncompleteOnly }"
          :aria-pressed="showIncompleteOnly"
          aria-label="Show only parties that are not fully checked off"
          @click="showIncompleteOnly = !showIncompleteOnly"
        >
          <img :src="userCheckIcon" alt="" class="filter-icon" />
          <span>Missing</span>
        </button>
      </div>

      <p class="global-total">{{ totalPresent }} / {{ totalMembers }}</p>
    </header>

    <section class="group-filter">
      <button
        v-for="group in travelGroups"
        :key="`filter-${String(group.id)}`"
        type="button"
        class="group-filter-button"
        :class="{ active: selectedGroupId === group.id }"
        @click="selectedGroupId = group.id ?? ALL_GROUPS"
      >
        {{ group.name }}
      </button>

      <button
        type="button"
        class="group-filter-button"
        :class="{ active: selectedGroupId === ALL_GROUPS }"
        @click="selectedGroupId = ALL_GROUPS"
      >
        All Groups
      </button>
    </section>

    <section v-for="group in filteredVisibleTravelGroups" :key="String(group.id)" class="travel-group">
      <TravelGroupHeader
        :name="group.name"
        :present-count="groupPresentCount(group)"
        :total-count="groupMemberCount(group)"
        :status="groupStatus(group)"
        @reset-group="resetGroup(group.id ?? '')"
      />

      <div class="party-list">
        <PartyRow
          v-for="party in group.parties"
          :key="`${String(group.id)}-${String(party.id)}`"
          :party-id="party.id ?? ''"
          :name="party.name"
          :present-count="presentCount(party)"
          :total-count="memberCount(party)"
          :status="partyStatus(party)"
          :expanded="Boolean(party.expanded)"
          :expand-icon-src="familyIcon"
          :accent-color="config.theme.accent"
          :thumbnail-src="thumbnailSrcForParty(party.id ?? '')"
          @tap-party="markPartyPresent(group.id ?? '', party.id ?? '')"
          @toggle-expanded="togglePartyExpanded(group.id ?? '', party.id ?? '')"
          @thumbnail-saved="refreshPartyThumbnails"
          @thumbnail-removed="refreshPartyThumbnails"
        >
          <template #members>
            <MemberRow
              v-for="member in party.members"
              :key="`${String(party.id)}-${String(member.id)}`"
              :name="member.name"
              :present="member.present"
              @update:present="updateMemberPresent(group.id ?? '', party.id ?? '', member.id ?? '', $event)"
            />
          </template>
        </PartyRow>
      </div>
    </section>
  </main>
</template>

<style scoped>
.people-view {
  display: flex;
  flex-direction: column;
  gap: 0.85rem;
}

.people-toolbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 0.75rem;
}

.toolbar-actions {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  flex-wrap: wrap;
}

.reset-button {
  min-height: 2.4rem;
  padding: 0.45rem 0.8rem;
  font-weight: 700;
  letter-spacing: 0.02em;
}

.global-total {
  font-weight: 700;
  color: var(--text-strong);
}

.travel-group {
  display: flex;
  flex-direction: column;
  gap: 0.45rem;
}

.group-filter {
  display: flex;
  gap: 0.45rem;
  overflow-x: auto;
  padding: 0.1rem 0;
}

.group-filter-button {
  min-height: 2.2rem;
  padding: 0.4rem 0.75rem;
  white-space: nowrap;
}

.incomplete-filter-button {
  display: inline-flex;
  align-items: center;
  gap: 0.35rem;
}

.filter-icon {
  width: 1rem;
  height: 1rem;
}

.group-filter-button.active {
  border-color: var(--brand);
  background: color-mix(in srgb, var(--brand) 12%, white);
  color: var(--text-strong);
  font-weight: 700;
}

.party-list {
  display: flex;
  flex-direction: column;
  gap: 0.45rem;
}
</style>