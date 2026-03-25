<script setup lang="ts">
import { computed, nextTick, ref } from 'vue'
import allStreets from '@/assets/streets.json'

const searchText = ref('')
const searchInput = ref<HTMLInputElement | null>(null)
const suppressAutocompleteUntilNextType = ref(false)
const previousSearchValue = ref('')
const filterQuery = ref('')

function splitAddressInput(value: string) {
    const match = value.match(/^(\s*\d+\s*)/)
    const numberPrefix = match?.[1] ?? ''
    const streetPortion = value.slice(numberPrefix.length)

    return { numberPrefix, streetPortion }
}

function normalizeSearch(value: string) {
    const { streetPortion } = splitAddressInput(value)
    return streetPortion.trimStart().toLowerCase()
}

const filteredStreets = computed(() => {
    const query = filterQuery.value

    if (!query) {
        return allStreets
    }

    return allStreets.filter((street) => street.toLowerCase().startsWith(query))
})

function onSearchInput(event: Event) {
    const currentValue = searchText.value
    const previousValue = previousSearchValue.value
    const inputEvent = event as InputEvent
    const inputType = inputEvent.inputType ?? ''

    filterQuery.value = normalizeSearch(currentValue)

    const clearedOrDeleted = inputType.startsWith('delete') || currentValue.length < previousValue.length

    if (clearedOrDeleted) {
        suppressAutocompleteUntilNextType.value = true
        previousSearchValue.value = currentValue
        return
    }

    if (inputType === 'insertText' && suppressAutocompleteUntilNextType.value) {
        suppressAutocompleteUntilNextType.value = false
    } else if (suppressAutocompleteUntilNextType.value) {
        previousSearchValue.value = currentValue
        return
    }

    const normalized = normalizeSearch(currentValue)

    if (!normalized) {
        previousSearchValue.value = currentValue
        return
    }

    const suggestion = allStreets.find((street) => street.toLowerCase().startsWith(normalized))

    if (!suggestion) {
        previousSearchValue.value = currentValue
        return
    }

    const { numberPrefix } = splitAddressInput(currentValue)
    const completedValue = `${numberPrefix}${suggestion}`

    if (completedValue.toLowerCase() === currentValue.toLowerCase()) {
        previousSearchValue.value = currentValue
        return
    }

    searchText.value = completedValue
    previousSearchValue.value = completedValue

    nextTick(() => {
        if (!searchInput.value) {
            return
        }

        searchInput.value.setSelectionRange(currentValue.length, completedValue.length)
    })
}
</script>

<template>
    <div class="streets">
        <h1>Streets</h1>

        <label class="search-label" for="street-search">Street Search</label>
        <input
            id="street-search"
            ref="searchInput"
            v-model="searchText"
            class="search-input"
            type="text"
            placeholder="Start typing an address, e.g. 1 Brittany Oaks"
            autocomplete="off"
            @input="onSearchInput"
        />

        <ul v-if="filteredStreets.length > 0 && filteredStreets.length < 5">
            <li v-for="street in filteredStreets" :key="street">
                {{ street }}
            </li>
        </ul>
    </div>
</template>

<style scoped>
.streets {
    display: grid;
    gap: 0.75rem;
}

.search-label {
    font-weight: 600;
}

.search-input {
    width: 100%;
    max-width: 28rem;
    padding: 0.6rem 0.75rem;
    border: 1px solid #b5bcc7;
    border-radius: 0.4rem;
}

ul {
    margin: 0;
    padding-left: 1.2rem;
}
</style>