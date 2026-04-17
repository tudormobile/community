import type { LocationData } from '@/types/location'

type PreparedLocation = {
  data: LocationData
  latRad: number
  lngRad: number
}

const EARTH_RADIUS_METERS = 6371000
const ROUGH_CANDIDATE_COUNT = 8

function toRadians(degrees: number): number {
  return (degrees * Math.PI) / 180
}

function haversineDistanceInMeters(fromLatRad: number, fromLngRad: number, to: PreparedLocation): number {
  const dLat = to.latRad - fromLatRad
  const dLng = to.lngRad - fromLngRad

  const haversine =
    Math.sin(dLat / 2) * Math.sin(dLat / 2) +
    Math.cos(fromLatRad) * Math.cos(to.latRad) * Math.sin(dLng / 2) * Math.sin(dLng / 2)

  return 2 * EARTH_RADIUS_METERS * Math.asin(Math.sqrt(haversine))
}

function equirectangularScore(fromLatRad: number, fromLngRad: number, to: PreparedLocation): number {
  const x = (to.lngRad - fromLngRad) * Math.cos((fromLatRad + to.latRad) / 2)
  const y = to.latRad - fromLatRad
  return x * x + y * y
}

export function createNearestLocationLookup(locations: LocationData[]) {
  const preparedLocations: PreparedLocation[] = locations.map((location) => ({
    data: location,
    latRad: toRadians(location.lat),
    lngRad: toRadians(location.lng),
  }))

  function findNearest(lat: number, lng: number): { location: LocationData; distanceMeters: number } | null {
    if (preparedLocations.length === 0) {
      return null
    }

    const latRad = toRadians(lat)
    const lngRad = toRadians(lng)
    const roughCandidates: Array<{ location: PreparedLocation; score: number }> = []

    for (const candidate of preparedLocations) {
      const score = equirectangularScore(latRad, lngRad, candidate)

      if (roughCandidates.length < ROUGH_CANDIDATE_COUNT) {
        roughCandidates.push({ location: candidate, score })

        if (roughCandidates.length === ROUGH_CANDIDATE_COUNT) {
          roughCandidates.sort((a, b) => b.score - a.score)
        }

        continue
      }

      const worstCandidate = roughCandidates.at(0)

      if (worstCandidate && score < worstCandidate.score) {
        roughCandidates[0] = { location: candidate, score }
        roughCandidates.sort((a, b) => b.score - a.score)
      }
    }

    const firstCandidate = roughCandidates.at(0)

    if (!firstCandidate) {
      return null
    }

    let bestLocation = firstCandidate.location
    let bestDistance = haversineDistanceInMeters(latRad, lngRad, bestLocation)

    for (const entry of roughCandidates) {
      const candidate = entry.location
      const candidateDistance = haversineDistanceInMeters(latRad, lngRad, candidate)

      if (candidateDistance < bestDistance) {
        bestLocation = candidate
        bestDistance = candidateDistance
      }
    }

    return {
      location: bestLocation.data,
      distanceMeters: bestDistance,
    }
  }

  return {
    findNearest,
  }
}