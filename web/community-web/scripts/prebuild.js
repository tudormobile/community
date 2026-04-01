console.log("Running prebuild script to set package version");
const now = new Date();
const patch = `${(now.getFullYear() % 100).toString().padStart(2, '0')}${(now.getMonth() + 1).toString().padStart(2, '0')}-web.${now.getDate().toString()}`;
console.log(`Computed patch version: ${patch}`);

import { readFileSync, writeFileSync } from 'fs';
const packageJsonPath = './package.json';
const packageJson = JSON.parse(readFileSync(packageJsonPath, 'utf-8'));
const version = packageJson.version.split(".", 2).join(".") + `.${patch}`;
console.log(`Computed package version: ${version}`);

packageJson.version = version;
writeFileSync(packageJsonPath, JSON.stringify(packageJson, null, 2) + '\n', 'utf-8');

const indexHtmlPath = './index.html';
const indexHtml = readFileSync(indexHtmlPath, 'utf-8');
const updatedIndexHtml = indexHtml.replace(
  /<!-- Version Code .+ -->/,
  `<!-- Version Code ${version} -->`
);
writeFileSync(indexHtmlPath, updatedIndexHtml, 'utf-8');
console.log(`Updated index.html with version: ${version}`);