<script setup lang="ts">
import { computed } from 'vue'

type ToggleValue = 'first' | 'second'

const props = withDefaults(
	defineProps<{
		firstLabel: string
		secondLabel: string
		firstIcon?: string
		secondIcon?: string
		modelValue?: ToggleValue
		ariaLabel?: string
	}>(),
	{
		modelValue: 'first',
		ariaLabel: 'View toggle',
	},
)

const emit = defineEmits<{
	(e: 'update:modelValue', value: ToggleValue): void
	(e: 'change', value: ToggleValue): void
}>()

const selected = computed({
	get: () => props.modelValue,
	set: (value: ToggleValue) => {
		if (value === props.modelValue) {
			return
		}

		emit('update:modelValue', value)
		emit('change', value)
	},
})

function select(value: ToggleValue) {
	selected.value = value
}
</script>

<template>
	<div class="items-toggle-wrapper">
	<div class="items-toggle" role="group" :aria-label="ariaLabel">
		<button
			type="button"
			class="toggle-button"
			:class="{ active: selected === 'first' }"
			:aria-pressed="selected === 'first'"
			@click="select('first')"
		>
			<img v-if="firstIcon" :src="firstIcon" class="toggle-icon" alt="" />
			{{ firstLabel }}
		</button>

		<button
			type="button"
			class="toggle-button"
			:class="{ active: selected === 'second' }"
			:aria-pressed="selected === 'second'"
			@click="select('second')"
		>
			<img v-if="secondIcon" :src="secondIcon" class="toggle-icon" alt="" />
			{{ secondLabel }}
		</button>
	</div>
	</div>
</template>

<style scoped>
.items-toggle-wrapper {
	display: flex;
	justify-content: center;
	width: 100%;
}

.items-toggle {
	display: inline-grid;
	grid-template-columns: 1fr 1fr;
	gap: 0.25rem;
	padding: 0.25rem;
	border: 1px solid #d0d5dd;
	border-radius: 999px;
	background: #f8fafc;
}

.toggle-button {
	border: 0;
	border-radius: 999px;
	padding: 0.45rem 0.9rem;
	background: transparent;
	display: flex;
	align-items: center;
	gap: 0.35rem;
	color: #475467;
	font-weight: 600;
	cursor: pointer;
	transition: background-color 150ms ease, color 150ms ease, box-shadow 150ms ease;
}

.toggle-button:hover {
	background: #eef2f7;
}

.toggle-button.active {
	background: #ffffff;
	color: #101828;
	box-shadow: 0 1px 2px rgba(16, 24, 40, 0.12);
}

.toggle-icon {
	width: 1rem;
	height: 1rem;
	opacity: 0.6;
}

.toggle-button.active .toggle-icon {
	opacity: 1;
}
</style>
