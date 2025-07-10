import { createApp } from 'vue'
import { createPinia } from 'pinia'

import App from './App.vue'
import router from './router'

import '@/assets/css/main.scss'
import '@/assets/css/tailwind.css'
import './styles/element-override.css'
import {registerEcharts} from "@/plugins/echarts"
//不使用mock 请注释掉
import { mockXHR } from "@/mock/index";
import { getLocalStorage, setLocalStorage } from './utils'
import { StorageEnum } from './enums'
import 'ace-builds/src-noconflict/mode-xml'; // Load the language definition file used below
import 'ace-builds/src-noconflict/theme-chrome'; // Load the theme definition file used below
mockXHR()
if (!getLocalStorage(StorageEnum.BACKEND_NAME)) {
  setLocalStorage(StorageEnum.BACKEND_NAME, 'DS')
}
const app = createApp(App)
registerEcharts(app)
app.use(createPinia())
app.use(router)

app.mount('#app')
