import { promises as fs } from 'fs';
import { createWriteStream } from 'fs';
import path from 'path';
import { fileURLToPath } from 'url';
import https from 'https';

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

const LOGOS_DIR = path.join(__dirname, '../static/logos');

// Create logos directory if it doesn't exist
try {
  await fs.access(LOGOS_DIR);
} catch {
  await fs.mkdir(LOGOS_DIR, { recursive: true });
}

const logos = {
  'github': 'https://raw.githubusercontent.com/simple-icons/simple-icons/develop/icons/github.svg',
  'asana': 'https://raw.githubusercontent.com/simple-icons/simple-icons/develop/icons/asana.svg',
  'notion': 'https://raw.githubusercontent.com/simple-icons/simple-icons/develop/icons/notion.svg',
  'jira': 'https://raw.githubusercontent.com/simple-icons/simple-icons/develop/icons/jira.svg',
  'trello': 'https://raw.githubusercontent.com/simple-icons/simple-icons/develop/icons/trello.svg',
  'zoom': 'https://raw.githubusercontent.com/simple-icons/simple-icons/develop/icons/zoom.svg',
  'google-meet': 'https://raw.githubusercontent.com/simple-icons/simple-icons/develop/icons/googlemeet.svg',
  'slack': 'https://raw.githubusercontent.com/simple-icons/simple-icons/develop/icons/slack.svg'
};

const downloadLogo = (name, url) => {
  return new Promise((resolve, reject) => {
    const filePath = path.join(LOGOS_DIR, `${name}.svg`);
    https.get(url, async (response) => {
      let data = '';
      response.on('data', (chunk) => {
        data += chunk;
      });
      response.on('end', async () => {
        // Modify SVG for dark mode by adding white fill
        const modifiedSvg = data.replace('<svg', '<svg fill="white"');
        await fs.writeFile(filePath, modifiedSvg);
        console.log(`Downloaded and modified ${name}.svg`);
        resolve();
      });
    }).on('error', (err) => {
      console.error(`Error downloading ${name}.svg:`, err.message);
      reject(err);
    });
  });
};

await Promise.all(
  Object.entries(logos).map(([name, url]) => downloadLogo(name, url))
); 