<script setup lang="ts">
import { ref, onMounted } from 'vue'
import type { DbxResponse, DbxVersion } from '@/types/api'
const status = ref<DbxVersion | null>(null)
const error = ref<string | null>(null)

const itemCount = ref<number | null>(null)
const eventCount = ref<number | null>(null)
const announcementCount = ref<number | null>(null)

onMounted(async () => {
    try {
        const additionalHeaders: Record<string, string> = JSON.parse(
            import.meta.env.VITE_API_ADDITIONAL_HEADERS
        )
        const response = await fetch(`${import.meta.env.VITE_API_BASE_URL}/dbx/status`, {
            headers: {
                [import.meta.env.VITE_API_KEY_NAME]: import.meta.env.VITE_API_KEY_VALUE,
                ...additionalHeaders,
            },
        })
        if (!response.ok) {
            error.value = `Request failed: ${response.status} ${response.statusText}`
            return
        }
        const result: DbxResponse<DbxVersion> = await response.json()
        if (result.success) {
            status.value = result.data as DbxVersion
            // For now, we'll just set these to dummy values since the API doesn't provide them yet
            itemCount.value = 1234
            eventCount.value = 567
            announcementCount.value = 89
        } else {
            error.value = result.data as string
        }
    } catch (e) {
        error.value = 'Offline'
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
        <div class="item">
            <span>Notices:</span>
            <span v-if="announcementCount !== null" class="status">{{ announcementCount.toLocaleString('en-US') }}</span>
            <span v-if="announcementCount === null && !error" class="query">Checking...</span>
            <span v-if="announcementCount === null && error" class="status">Unknown</span>
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
