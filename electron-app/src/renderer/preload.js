// Preload script for Electron renderer process
// Exposes safe APIs for settings and audio device management

const { contextBridge, ipcRenderer } = require('electron');

// Settings API
contextBridge.exposeInMainWorld('settingsAPI', {
  getSettings: () => ipcRenderer.invoke('settings:get'),
  setSettings: (settings) => ipcRenderer.invoke('settings:set', settings),
  onSettingsChange: (callback) => {
    ipcRenderer.on('settings:changed', (_event, newSettings) => callback(newSettings));
  }
});

// Audio Device API
contextBridge.exposeInMainWorld('audioAPI', {
  // Device enumeration
  getAudioDevices: () => ipcRenderer.invoke('audio:getDevices'),
  onDevicesChange: (callback) => {
    ipcRenderer.on('audio:devicesChanged', (_event, devices) => callback(devices));
  },
  
  // Recording control
  startRecording: () => ipcRenderer.invoke('audio:startRecording'),
  stopRecording: () => ipcRenderer.invoke('audio:stopRecording'),
  pauseRecording: () => ipcRenderer.invoke('audio:pauseRecording'),
  resumeRecording: () => ipcRenderer.invoke('audio:resumeRecording'),
  
  // Recording status
  onRecordingStatus: (callback) => {
    ipcRenderer.on('audio:recordingStatus', (_event, status) => callback(status));
  },
  
  // Audio analysis
  onAudioLevel: (callback) => {
    ipcRenderer.on('audio:level', (_event, level) => callback(level));
  },
  
  // Error handling
  onError: (callback) => {
    ipcRenderer.on('audio:error', (_event, error) => callback(error));
  }
});

// Meeting Detection API
contextBridge.exposeInMainWorld('meetingAPI', {
  detectApps: () => ipcRenderer.invoke('meeting:detectApps'),
  detectBrowserWindows: () => ipcRenderer.invoke('meeting:detectBrowserWindows'),
  onMeetingDetected: (callback) => {
    ipcRenderer.on('meeting:detected', (_event, meetingSummary) => callback(meetingSummary));
  }
}); 