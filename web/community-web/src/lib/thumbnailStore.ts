export type ThumbnailShape = 'square' | 'circle'

export type ThumbnailRecord = {
  partyId: string
  createdAt: number
  blob: Blob
  mimeType: string
  shape: ThumbnailShape
}

const DB_NAME = 'community-web-thumbnails'
const DB_VERSION = 1
const STORE_NAME = 'thumbnails'

const openDatabase = (): Promise<IDBDatabase> =>
  new Promise((resolve, reject) => {
    const request = indexedDB.open(DB_NAME, DB_VERSION)

    request.onupgradeneeded = () => {
      const db = request.result

      if (!db.objectStoreNames.contains(STORE_NAME)) {
        const store = db.createObjectStore(STORE_NAME, { keyPath: 'partyId' })
        store.createIndex('createdAt', 'createdAt', { unique: false })
      }
    }

    request.onsuccess = () => resolve(request.result)
    request.onerror = () => reject(request.error ?? new Error('Unable to open IndexedDB.'))
  })

const normalizePartyId = (partyId: string): string => partyId.trim()

export const saveThumbnail = async (
  partyId: string,
  blob: Blob,
  shape: ThumbnailShape,
): Promise<{ partyId: string; createdAt: number }> => {
  const normalizedPartyId = normalizePartyId(partyId)

  if (!normalizedPartyId) {
    throw new Error('A valid party ID is required to save a thumbnail.')
  }

  const db = await openDatabase()
  const createdAt = Date.now()

  return new Promise((resolve, reject) => {
    const tx = db.transaction(STORE_NAME, 'readwrite')
    const store = tx.objectStore(STORE_NAME)

    store.put({
      partyId: normalizedPartyId,
      createdAt,
      blob,
      mimeType: blob.type || 'image/png',
      shape,
    } satisfies ThumbnailRecord)

    tx.oncomplete = () => {
      db.close()
      resolve({ partyId: normalizedPartyId, createdAt })
    }
    tx.onerror = () => {
      db.close()
      reject(tx.error ?? new Error('Failed to save thumbnail.'))
    }
    tx.onabort = () => {
      db.close()
      reject(tx.error ?? new Error('Thumbnail save aborted.'))
    }
  })
}

export const listThumbnails = async (): Promise<ThumbnailRecord[]> => {
  const db = await openDatabase()

  return new Promise((resolve, reject) => {
    const tx = db.transaction(STORE_NAME, 'readonly')
    const store = tx.objectStore(STORE_NAME)
    const request = store.getAll()

    request.onsuccess = () => {
      const rows = (request.result ?? []) as ThumbnailRecord[]
      resolve(rows.sort((a, b) => b.createdAt - a.createdAt))
    }

    request.onerror = () => reject(request.error ?? new Error('Failed to read thumbnails.'))

    tx.oncomplete = () => db.close()
    tx.onerror = () => {
      db.close()
      reject(tx.error ?? new Error('Failed to read thumbnails.'))
    }
  })
}

export const removeThumbnail = async (partyId: string): Promise<void> => {
  const normalizedPartyId = normalizePartyId(partyId)

  if (!normalizedPartyId) {
    throw new Error('A valid party ID is required to remove a thumbnail.')
  }

  const db = await openDatabase()

  return new Promise((resolve, reject) => {
    const tx = db.transaction(STORE_NAME, 'readwrite')
    const store = tx.objectStore(STORE_NAME)

    store.delete(normalizedPartyId)

    tx.oncomplete = () => {
      db.close()
      resolve()
    }
    tx.onerror = () => {
      db.close()
      reject(tx.error ?? new Error('Failed to remove thumbnail.'))
    }
    tx.onabort = () => {
      db.close()
      reject(tx.error ?? new Error('Thumbnail removal aborted.'))
    }
  })
}
