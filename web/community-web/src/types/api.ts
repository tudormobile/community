export interface ApiResponse<T> {
  isSuccess: boolean
  data: T | string | null
}

export interface DbxResponse<T> {
  success: boolean
  data: T | string | null
}

export interface ServiceVersion {
  name: string
  description: string
  copyright: string
  version: string
}

export interface Announcement {
  id: number
  title: string
  content: string
  date: string
}
