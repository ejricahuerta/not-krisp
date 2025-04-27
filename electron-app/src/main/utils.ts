import { app } from 'electron';
import { join } from 'path';
import * as fs from 'fs';

export function getUserDataPath(): string {
  const userDataPath = app.getPath('userData');
  const recordingsPath = join(userDataPath, 'recordings');
  
  // Create recordings directory if it doesn't exist
  if (!fs.existsSync(recordingsPath)) {
    fs.mkdirSync(recordingsPath, { recursive: true });
  }
  
  return recordingsPath;
} 