import type { LocationData } from '@/types/location'

export interface CalendarEvent {
  id: string;
  title: string;
  description: string;
  start: Date;
  end: Date;
  location?: LocationData;
}