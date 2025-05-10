const { contextBridge, ipcRenderer } = require('electron');

contextBridge.exposeInMainWorld('widgetAPI', {
  hide: () => ipcRenderer.send('hide-widget')
});
  