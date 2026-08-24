export type EntityId = string | number

export interface Member {
  id?: EntityId
  name: string
  present: boolean
}

export interface Party {
  id?: EntityId
  name: string
  expanded?: boolean
  members: Member[]
}

export interface TravelGroup {
  id?: EntityId
  name: string
  parties: Party[]
}

export interface UsersAsset {
  travelGroups: TravelGroup[]
}