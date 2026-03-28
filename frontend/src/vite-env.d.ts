/// <reference types="vite/client" />

interface ImportMetaEnv {
  readonly VITE_AUTH_API: string
  readonly VITE_RESULTS_API: string
  readonly VITE_ACH_API: string
}

interface ImportMeta {
  readonly env: ImportMetaEnv
}
