const {
  app,
  BrowserWindow,
  Tray,
  Menu,
  screen,
  ipcMain,
  contextBridge,
} = require("electron");
const path = require("path");
const waitOn = require("wait-on");
const fs = require("fs");

let mainWindow, widgetWindow, tray;

async function createWindows() {
  // Dashboard
  mainWindow = new BrowserWindow({
    width: 1200,
    height: 800,
    show: false,
    toolbar: false,
  });
  await waitOn({ resources: ["http://localhost:5173"] });
  await mainWindow.loadURL("http://localhost:5173/dashboard").then(() => {
    console.log("Loaded");
  });

  // Widget
  widgetWindow = new BrowserWindow({
    width: 300,
    height: 200,
    frame: false,
    skipTaskbar: true,
    alwaysOnTop: true,
    show: false,
    webPreferences: {
      preload: path.join(__dirname, "widget-preload.js"),
      contextIsolation: true,
      nodeIntegration: false,
    },
  });
  await widgetWindow.loadURL("http://localhost:5173/widget").then(() => {
    console.log("Widget Loaded");
    const { width, height } = widgetWindow.getBounds();
    const { workArea } = screen.getPrimaryDisplay();
    const x = workArea.x + workArea.width - width;
    const y = workArea.y + workArea.height - height;
    widgetWindow.setPosition(x, y);
    widgetWindow.show();
  });
}

function setupTray() {
  tray = new Tray(path.join(__dirname, "static", "favicon.png"));
  const menu = Menu.buildFromTemplate([
    { label: "          Dashboard           ", click: () => mainWindow.show() },
    {
      label: "          Toggle Widget           ",
      click: () => {
        widgetWindow.isVisible() ? widgetWindow.hide() : widgetWindow.show();
      },
    },
    { type: "separator" },
    { label: "          Quit            ", click: () => app.quit() },
  ]);
  tray.setContextMenu(menu);
  tray.setToolTip("Tucked");
  tray.on("click", () => {
    const { width, height } = widgetWindow.getBounds();
    const { workArea } = screen.getPrimaryDisplay();
    const x = workArea.x + workArea.width - width;
    const y = workArea.y + workArea.height - height;
    widgetWindow.setPosition(x, y);
    widgetWindow.show();
  });
}

// Listen for hide event
ipcMain.on("hide-widget", () => {
  if (widgetWindow) widgetWindow.hide();
});

app.whenReady().then(async () => {
  await createWindows();
  setupTray();
});

app.on("window-all-closed", () => app.quit());
