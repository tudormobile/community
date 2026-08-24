<script setup lang="ts">
import type { AppConfig } from '@/types/config'
import appConfig from '@/assets/config.json'
import Search from '@/assets/icons/search.svg'
import AddPin from '@/assets/icons/add_alt.svg'
const config = appConfig as AppConfig

const props = defineProps<{
  searchTextValue: string;
}>();

const emit = defineEmits<{
  (e: 'update:searchTextValue', value: string): void;
  (e: 'add'): void;
}>();

const handleInput = (e: Event) => {
  const target = e.target as HTMLInputElement;
  emit('update:searchTextValue', target.value);
};

</script>

<template>
  <div class="search-container">
    <div class="search-pillbox">
      <div class="search-input-wrapper">
        <img class="search-icon" :src="Search" aria-hidden="true" alt="Search" />
        <input :value="searchTextValue" @input="handleInput" type="text" placeholder="Filter events"
          class="search-input" />
      </div>

      <div class="divider"></div>

      <!-- Remove the add button for now, since we don't have a way to add events yet. -->
      <!-- <button type="button" @click="emit('add')" class="add-button">
        <div class="add-icon-wrapper">
          <img class="add-icon" :src="AddPin" aria-hidden="true" alt="Add" />
        </div>
        <span class="add-label">Add</span>
      </button> -->
      <!-- End Remove -->
    </div>
  </div>
</template>

<style scoped>
.search-container {
  width: 100%;
  max-width: 450px;
}

.search-pillbox {
  background: white;
  border-radius: 9999px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
  display: flex;
  align-items: center;
  padding: 0px;
  border: 1px solid #e5e7eb;
}

.search-input-wrapper {
  flex: 1;
  display: flex;
  align-items: center;
  padding-left: 16px;
}

.search-icon {
  width: 20px;
  height: 20px;
  color: #9ca3af;
  margin-right: 12px;
}

.search-input {
  flex: 1;
  border: none;
  outline: none;
  font-size: 14px;
  padding: 12px 0;
  background: transparent;
}

.divider {
  width: 1px;
  height: 32px;
  background-color: #e5e7eb;
  margin: 0 8px;
}

.add-button {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 4px 16px;
  border-radius: 9999px;
  transition: all 0.2s;
  color: v-bind('config.theme.accent');
}

.add-button:hover {
  background-color: #f3f4f6;
}

.add-icon-wrapper {
  display: flex;
  align-items: center;
  justify-content: center;
}

.add-icon {
  width: 20px;
  height: 20px;
}

.add-label {
  font-size: 10px;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  margin-top: -2px;
}
</style>