import { readFileSync, writeFileSync } from 'fs';
const pkg = JSON.parse(readFileSync(new URL('../package.json', import.meta.url), 'utf-8'));
const versionInfo = { name: pkg.name, version: pkg.version, author: pkg.author, copyright: pkg.copyright, license: pkg.license, buildDate: new Date() };
writeFileSync(new URL('../public/version.json', import.meta.url), JSON.stringify(versionInfo));
