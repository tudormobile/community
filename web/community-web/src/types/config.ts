import type { LocationData } from "./location";

export interface AppConfig {
  name: string;
  tagline: string;
  logoUrl: string;
  itemName: string;
  homeLocation: LocationData;
  theme: {
    primary: string;
    secondary: string;
    accent: string;
  };
}
