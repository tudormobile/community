import './assets/main.css'
import 'leaflet/dist/leaflet.css'
import 'leaflet.locatecontrol/dist/L.Control.Locate.min.css'
import 'v-calendar/style.css'

import { createApp } from 'vue'
import VCalendar from 'v-calendar'
import App from './App.vue'
import router from './router'

const app = createApp(App)

app.use(router)
app.use(VCalendar, {})

app.mount('#app')
