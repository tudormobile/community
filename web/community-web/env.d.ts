/// <reference types="vite/client" />

interface ImportMetaEnv {
  readonly VITE_API_APP_NAME: string
  readonly VITE_API_KEY_NAME: string
  readonly VITE_API_KEY_VALUE: string
  readonly VITE_API_BASE_URL: string
  readonly VITE_API_ADDITIONAL_HEADERS: string
}

interface ImportMeta {
  readonly env: ImportMetaEnv
}
