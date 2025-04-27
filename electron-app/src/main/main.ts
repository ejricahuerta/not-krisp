import { BrowserWindow, app, Tray, Menu, ipcMain, screen, nativeImage } from 'electron';
import * as path from 'path';
import { getAllSettings, getSetting, setSetting, onSettingsChange } from './settings';
import { stateManager, AppState } from './state';
import { detectMeetingApps, detectBrowserMeetingWindows, resolveMeetingPriority } from './meetingDetection';

// Global variable declarations
let mainWindow: BrowserWindow | null = null;
let tray: Tray | null = null;
let settingsWindow: BrowserWindow | null = null;
let isQuitting = false;

// Real-time meeting detection polling
let lastDetectedMeetings: string[] = [];
function startMeetingDetectionPolling() {
  setInterval(async () => {
    try {
      const nativeMeetings = await detectMeetingApps();
      const browserMeetings = detectBrowserMeetingWindows();
      const { primary, conflicts } = resolveMeetingPriority(nativeMeetings, browserMeetings);
      // Compose a summary for UI/IPC
      const meetingSummary = {
        primary,
        conflicts,
        all: [
          ...nativeMeetings.map(m => ({ name: m.name, source: m.process, type: 'native' })),
          ...browserMeetings.map(m => ({ name: m.name, source: m.title, type: 'browser' })),
        ]
      };
      // Detect new meetings (by primary meeting change)
      const currentPrimaryKey = primary ? `${primary.name}:${primary.source}` : '';
      const lastPrimaryKey = lastDetectedMeetings[0] || '';
      if (currentPrimaryKey !== lastPrimaryKey) {
        if (mainWindow && mainWindow.isVisible()) {
          mainWindow.webContents.send('meeting:detected', meetingSummary);
        } else {
          openSettingsWindow();
        }
        // TODO: Show richer notification for meeting conflicts if conflicts.length > 0
        if (conflicts.length > 0) {
          console.warn('Meeting conflict detected:', conflicts);
        }
      }
      lastDetectedMeetings = [currentPrimaryKey];
    } catch (err) {
      console.error('Meeting detection polling error:', err);
    }
  }, 10000); // 10 seconds
}

// Create a basic tray icon as a fallback
function createBasicTrayIcon() {
  const size = 16;
  const icon = nativeImage.createEmpty();
  
  // Create a simple green dot icon
  const data = Buffer.alloc(size * size * 4); // RGBA buffer
  for (let y = 0; y < size; y++) {
    for (let x = 0; x < size; x++) {
      const offset = (y * size + x) * 4;
      const dx = x - size/2;
      const dy = y - size/2;
      const distance = Math.sqrt(dx*dx + dy*dy);
      
      if (distance < size/3) {
        // Green dot
        data[offset] = 0;     // R
        data[offset + 1] = 255; // G
        data[offset + 2] = 0;   // B
        data[offset + 3] = 255; // A
      } else {
        // Transparent
        data[offset + 3] = 0;
      }
    }
  }
  
  icon.addRepresentation({
    width: size,
    height: size,
    buffer: data,
    scaleFactor: 1.0
  });
  
  return icon;
}

// Add isQuitting property to app
declare global {
  namespace Electron {
    interface App {
      isQuitting: boolean;
    }
  }
}
app.isQuitting = false;

function updateTrayMenu() {
  if (!tray) return;
  const state = stateManager.getState();
  tray.setToolTip(`Not Krisp (${state})`);
  const contextMenu = Menu.buildFromTemplate([
    {
      label: state === 'Running' ? 'Pause' : 'Start',
      click: () => {
        stateManager.setState(state === 'Running' ? 'Paused' : 'Running');
      }
    },
    { type: 'separator' },
    {
      label: 'Show App',
      click: () => {
        if (!mainWindow) {
          createWindow();
        }
        mainWindow?.show();
        mainWindow?.focus();
      }
    },
    {
      label: 'Settings',
      click: () => {
        openSettingsWindow();
      }
    },
    { type: 'separator' },
    {
      label: 'Quit',
      click: () => {
        isQuitting = true;
        app.quit();
      }
    }
  ]);
  tray.setContextMenu(contextMenu);
}

function createTray() {
  if (tray) return;
  
  try {
    // Try multiple possible icon locations
    const possiblePaths = [
      path.join(__dirname,  'icon.png'),
      path.join(__dirname, '..', 'assets', 'icons', 'icon.png'),
      path.join(__dirname, '..', '..', 'src', 'assets', 'icons', 'icon.png')
    ];
    
    let icon = null;
    let loadedPath = '';
    
    // Try each path until we find a valid icon
    for (const iconPath of possiblePaths) {
      try {
        console.log('Trying icon path:', iconPath);
        const tempIcon = nativeImage.createFromPath(iconPath);
        if (!tempIcon.isEmpty()) {
          icon = tempIcon;
          loadedPath = iconPath;
          break;
        }
      } catch (e) {
        console.log('Failed to load icon from:', iconPath);
      }
    }
    
    if (!icon || icon.isEmpty()) {
      throw new Error('Could not load icon from any location');
    }
    
    console.log('Successfully loaded icon from:', loadedPath);
    tray = new Tray(icon);
  } catch (error) {
    console.warn('Failed to load tray icon, using fallback:', error);
    // Use a basic colored circle as fallback
    tray = new Tray(createBasicTrayIcon());
  }
  
  updateTrayMenu();
  
  tray.on('click', () => {
    if (!mainWindow) {
      createWindow();
    }
    if (mainWindow) {
      if (mainWindow.isVisible()) {
        mainWindow.hide();
      } else {
        mainWindow.show();
        mainWindow.focus();
      }
    }
  });
  
  stateManager.onChange(updateTrayMenu);
}

function openSettingsWindow() {
  if (settingsWindow) {
    settingsWindow.focus();
    if (mainWindow && mainWindow.isVisible()) {
      mainWindow.hide();
    }
    return;
  }
  if (mainWindow && mainWindow.isVisible()) {
    mainWindow.hide();
  }
  settingsWindow = new BrowserWindow({
    width: 300,
    height: 380,
    resizable: false,
    minimizable: false,
    maximizable: false,
    frame: false,
    parent: mainWindow || undefined,
    modal: !!mainWindow,
    webPreferences: {
      preload: path.join(__dirname, '../renderer/preload.js'),
      nodeIntegration: false,
      contextIsolation: true,
    },
    show: false,
    skipTaskbar: true,
  });
  settingsWindow.loadFile(path.join(__dirname, '../renderer/settings.html'));
  settingsWindow.on('closed', () => {
    settingsWindow = null;
  });

  settingsWindow.once('ready-to-show', () => {
    if (tray && settingsWindow) {
      const trayBounds = tray.getBounds();
      const display = screen.getDisplayNearestPoint({ x: trayBounds.x, y: trayBounds.y });
      const windowWidth = 300;
      const windowHeight = 380;

      let x = trayBounds.x;
      let y = trayBounds.y;

      // Determine taskbar position
      const isBottom = Math.abs(trayBounds.y + trayBounds.height - (display.bounds.y + display.workArea.height)) < 40;
      const isTop = Math.abs(trayBounds.y - display.bounds.y) < 40;
      const isLeft = Math.abs(trayBounds.x - display.bounds.x) < 40;
      const isRight = Math.abs(trayBounds.x + trayBounds.width - (display.bounds.x + display.workArea.width)) < 40;

      if (isBottom) {
        // Taskbar at bottom
        x = Math.round(trayBounds.x + trayBounds.width / 2 - windowWidth / 2);
        y = trayBounds.y - windowHeight;
      } else if (isTop) {
        // Taskbar at top
        x = Math.round(trayBounds.x + trayBounds.width / 2 - windowWidth / 2);
        y = trayBounds.y + trayBounds.height;
      } else if (isLeft) {
        // Taskbar at left
        x = trayBounds.x + trayBounds.width;
        y = Math.round(trayBounds.y + trayBounds.height / 2 - windowHeight / 2);
      } else if (isRight) {
        // Taskbar at right
        x = trayBounds.x - windowWidth;
        y = Math.round(trayBounds.y + trayBounds.height / 2 - windowHeight / 2);
      } else {
        // Fallback: center on tray icon
        x = Math.round(trayBounds.x + trayBounds.width / 2 - windowWidth / 2);
        y = Math.round(trayBounds.y + trayBounds.height / 2 - windowHeight / 2);
      }

      // Clamp to screen
      if (x < display.bounds.x) x = display.bounds.x;
      if (x + windowWidth > display.bounds.x + display.workArea.width) {
        x = display.bounds.x + display.workArea.width - windowWidth;
      }
      if (y < display.bounds.y) y = display.bounds.y;
      if (y + windowHeight > display.bounds.y + display.workArea.height) {
        y = display.bounds.y + display.workArea.height - windowHeight;
      }

      settingsWindow.setPosition(x, y, false);
    }
    if (settingsWindow) settingsWindow.show();
  });
}

function createWindow() {
  if (mainWindow) return;
  
  mainWindow = new BrowserWindow({
    width: 800,
    height: 600,
    webPreferences: {
      preload: path.join(__dirname, '../renderer/preload.js'),
      nodeIntegration: false,
      contextIsolation: true,
    },
    show: false,
    frame: false,
    titleBarStyle: 'hidden',
  });

  mainWindow.loadFile(path.join(__dirname, '..', 'renderer', 'index.html'));

  // Handle window state
  mainWindow.addListener('show', () => {
    if (settingsWindow && settingsWindow.isVisible()) {
      settingsWindow.hide();
    }
  });

  mainWindow.on('close', function(event: Electron.Event) {
    if (!isQuitting) {
      event.preventDefault();
      mainWindow?.hide();
      return false;
    }
    return true;
  });

  mainWindow.on('closed', () => {
    mainWindow = null;
  });
}

// IPC handlers for settings
ipcMain.handle('settings:get', () => {
  return getAllSettings();
});
ipcMain.handle('settings:set', (_event, newSettings) => {
  Object.entries(newSettings).forEach(([key, value]) => {
    setSetting(key as any, value);
  });
  return getAllSettings();
});
// Broadcast settings changes to all renderer windows
onSettingsChange('startWithWindows', broadcastSettingsChange);
onSettingsChange('minimizeToTray', broadcastSettingsChange);
onSettingsChange('audioDeviceId', broadcastSettingsChange);
function broadcastSettingsChange() {
  const settings = getAllSettings();
  BrowserWindow.getAllWindows().forEach(win => {
    win.webContents.send('settings:changed', settings);
  });
}

// IPC handler for meeting app detection
ipcMain.handle('meeting:detectApps', async () => {
  return await detectMeetingApps();
});

// IPC handler for browser meeting window detection
ipcMain.handle('meeting:detectBrowserWindows', async () => {
  return detectBrowserMeetingWindows();
});

// Initialize app
app.whenReady().then(() => {
  // Create tray first
  createTray();
  
  // Create main window but don't show it yet
  createWindow();
  
  // Open settings window first
  openSettingsWindow();

  // Start real-time meeting detection polling
  startMeetingDetectionPolling();
});

app.on('before-quit', () => {
  isQuitting = true;
});

app.on('window-all-closed', () => {
  if (process.platform !== 'darwin') {
    if (isQuitting) {
      app.quit();
    }
  }
});

app.on('activate', () => {
  if (mainWindow === null) {
    createWindow();
  } else {
    mainWindow.show();
  }
}); 