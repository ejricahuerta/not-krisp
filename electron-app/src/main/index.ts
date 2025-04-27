import { app, BrowserWindow } from 'electron';
import { join } from 'path';
import { setupAudioHandlers } from './audio';


//create a window with  no toolbar
async function createWindow() {
  const mainWindow = new BrowserWindow({
    width: 1200,
    height: 800,
    titleBarStyle: 'hidden',
    webPreferences: {
      nodeIntegration: false,
      contextIsolation: true,
      preload: join(__dirname, '../preload/index.js')
    }
  });

  // Set up audio handlers
  setupAudioHandlers(mainWindow);

  if (process.env.NODE_ENV === 'development') {
    await mainWindow.loadURL('http://localhost:5173');
    mainWindow.webContents.openDevTools();
  } else {
    await mainWindow.loadFile(join(__dirname, '../../dist/index.html'));
  }
}

app.whenReady().then(createWindow);

app.on('window-all-closed', () => {
  if (process.platform !== 'darwin') {
    app.quit();
  }
});

app.on('activate', () => {
  if (BrowserWindow.getAllWindows().length === 0) {
    createWindow();
  }
}); 