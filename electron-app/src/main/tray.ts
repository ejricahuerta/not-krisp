import { app, Tray, Menu, BrowserWindow, nativeImage } from 'electron';
import { join } from 'path';
import { getSetting } from './settings';

let tray: Tray | null = null;

// Add isQuitting property to app
declare global {
  namespace Electron {
    interface App {
      isQuitting: boolean;
    }
  }
}
app.isQuitting = false;

export function createTray(mainWindow: BrowserWindow): Tray {
  // Create tray icon
  //if mac os use the icon.png in the assets folder
  let iconPath = join(__dirname, 'assets', 'icons', 'icon.png');
  if (process.platform === 'darwin') {
    iconPath = join(__dirname, 'assets', 'icons', 'icon.ico');
  }

  const icon = nativeImage.createFromPath(iconPath).resize({ width: 18, height: 18 });
  tray = new Tray(icon);

  // Create context menu
  const contextMenu = Menu.buildFromTemplate([
    {
      label: 'Open Dashboard',
      click: () => {
        mainWindow.show();
      }
    },
    {
      label: 'Quit',
      click: () => {
        app.isQuitting = true;
        app.quit();
      }
    }
  ]);

  // Set tooltip and context menu
  tray.setToolTip('NotKrisp');
  tray.setContextMenu(contextMenu);

  // Handle click
  tray.on('click', () => {
    mainWindow.show();
  });

  // Handle window minimize/close
  mainWindow.on('minimize', () => {
    if (getSetting('minimizeToTray')) {
      mainWindow.hide();
    }
  });

  mainWindow.on('close', (event) => {
    if (getSetting('minimizeToTray') && !app.isQuitting) {
      event.preventDefault();
      mainWindow.hide();
    }
  });

  return tray;
}

export function destroyTray() {
  if (tray) {
    tray.destroy();
    tray = null;
  }
} 