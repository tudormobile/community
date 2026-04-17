<script setup lang="ts">
import { ref, onMounted } from 'vue'
import type { DbxIdStatus, DbxResponse, DbxVersion } from '@/types/api'

const eventsId = 'events'
const itemsId = 'items'

const eventsStatusUrl = `${import.meta.env.VITE_API_BASE_URL}/dbx/status/${eventsId}`
const itemsStatusUrl = `${import.meta.env.VITE_API_BASE_URL}/dbx/status/${itemsId}`

const status = ref<DbxVersion | null>(null)
const error = ref<string | null>(null)

const itemCount = ref<number | null>(null)
const eventCount = ref<number | null>(null)

function getRequestHeaders(): HeadersInit {
    const additionalHeaders: Record<string, string> = JSON.parse(
        import.meta.env.VITE_API_ADDITIONAL_HEADERS
    )

    return {
        [import.meta.env.VITE_API_KEY_NAME]: import.meta.env.VITE_API_KEY_VALUE,
        ...additionalHeaders,
    }
}

async function fetchDbxData<T>(url: string): Promise<T> {
    const response = await fetch(url, {
        headers: getRequestHeaders(),
    })

    if (!response.ok) {
        throw new Error(`Request failed: ${response.status} ${response.statusText}`)
    }

    const result: DbxResponse<T> = await response.json()
    if (!result.success) {
        throw new Error(String(result.data ?? 'Unknown API error'))
    }

    return result.data as T
}

onMounted(async () => {
    try {
        const [versionData, itemsData, eventsData] = await Promise.all([
            fetchDbxData<DbxVersion>(`${import.meta.env.VITE_API_BASE_URL}/dbx/status`),
            fetchDbxData<DbxIdStatus>(itemsStatusUrl),
            fetchDbxData<DbxIdStatus>(eventsStatusUrl),
        ])

        status.value = versionData
        itemCount.value = itemsData.count
        eventCount.value = eventsData.count
    } catch (e) {
        error.value = e instanceof Error ? e.message : 'Offline'
    }
})

</script>
<template>
    <div class="about-data">
        <div class="title">Data Service Information</div>
        <div class="item">
            <span>Service Status:</span>
            <span v-if="error" class="error">{{ error }}</span>
            <span v-if="status" class="status">Operational</span>
            <span v-if="!status && !error" class="query">Checking...</span>
        </div>
        <div class="item">
            <span>Version:</span>
            <span v-if="status" class="status">{{ status.version }}</span>
            <span v-if="!status && !error" class="query">Checking...</span>
            <span v-if="!status && error" class="status">Unknown</span>
        </div>
        <div class="item">
            <span>Item Count:</span>
            <span v-if="itemCount !== null" class="status">{{ itemCount.toLocaleString('en-US') }}</span>
            <span v-if="itemCount === null && !error" class="query">Checking...</span>
            <span v-if="itemCount === null && error" class="status">Unknown</span>
        </div>
        <div class="item">
            <span>Event Count:</span>
            <span v-if="eventCount !== null" class="status">{{ eventCount.toLocaleString('en-US') }}</span>
            <span v-if="eventCount === null && !error" class="query">Checking...</span>
            <span v-if="eventCount === null && error" class="status">Unknown</span>
        </div>
    </div>
</template>

<style scoped>
.about-data {
    display: grid;
    grid-template-columns: auto 1fr;
    column-gap: 1rem;
    row-gap: 0.1rem;
    align-items: baseline;
    padding-bottom: 2rem;
}

.title {
    font-size: 1rem;
    font-weight: bold;
    grid-column: 1 / -1;
}

.item {
    display: contents;
}

.item>span {
    font-size: 0.75rem;
}

.status {
    color: var(--text);
    font-weight: bold;
}

.error {
    color: var(--text-error);
    font-weight: bold;
}

.query {
    color: var(--text-subtle);
    font-weight: bold;
    font-style: italic;
}
</style>
