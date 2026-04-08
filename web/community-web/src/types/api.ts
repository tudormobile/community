export interface ApiResponse<T> {
  isSuccess: boolean
  data: T | string
}

export interface ServiceVersion {
  name: string
  description: string
  copyright: string
  version: string
}
