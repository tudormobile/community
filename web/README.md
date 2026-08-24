# Web Applications

## App: community-web app

### Routing on Cloudflare Workers
For Worker deployments, SPA deep-link fallback is configured in `community-web/wrangler.toml` via `assets.not_found_handling = "single-page-application"`.
The Cloudflare Pages-style `public/_redirects` file is intentionally not used.

### Build and Test
#### Compile and run in hot-load development mode
```sh
npm install
npm run dev
-or-
npm run host
```
#### Compile and Minify for Production
```sh
npm run build
```