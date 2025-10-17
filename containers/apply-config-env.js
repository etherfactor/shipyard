// Patches a JSON file using env vars like CONFIG__oauth__authority=http://...
// Usage: node apply-config-env.js /path/to/config.base.json /path/to/config.json
const fs = require('fs');

const src = process.argv[2];
const dest = process.argv[3] || src;

const coerce = (val) => {
  if (val === 'true' || val === 'false') return val === 'true';
  if (val === 'null') return null;
  if (/^-?\d+(\.\d+)?$/.test(val)) return Number(val);
  // allow explicit JSON with prefix
  if (val.startsWith('json:')) {
    try { return JSON.parse(val.slice(5)); } catch { /* fall through */ }
  }
  return val;
};

const setPath = (obj, pathParts, value) => {
  let cur = obj;
  for (let i = 0; i < pathParts.length - 1; i++) {
    const key = pathParts[i];
    if (!(key in cur) || typeof cur[key] !== 'object' || cur[key] === null) cur[key] = {};
    cur = cur[key];
  }
  cur[pathParts[pathParts.length - 1]] = value;
};

const data = JSON.parse(fs.readFileSync(src, 'utf8'));

for (const [k, v] of Object.entries(process.env)) {
  if (!k.startsWith('CONFIG__')) continue;
  // CONFIG__oauth__authority -> ["oauth","authority"]
  const parts = k.split('__').slice(1).filter(Boolean);
  if (parts.length === 0) continue;
  setPath(data, parts, coerce(v));
}

fs.writeFileSync(dest, JSON.stringify(data, null, 2));
