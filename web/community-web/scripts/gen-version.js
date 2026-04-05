import { readFileSync, writeFileSync } from 'fs';
const pkg = JSON.parse(readFileSync(new URL('../package.json', import.meta.url), 'utf-8'));
const versionInfo = { name: pkg.name, version: pkg.version, copyright: pkg.author, license: pkg.license, buildDate: new Date() };
writeFileSync('./public/version.json', JSON.stringify(versionInfo));
