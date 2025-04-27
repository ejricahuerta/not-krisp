import { contextBridge, ipcRenderer } from 'electron';
import { AudioDevice } from '../main/audio';

// Define the API types
export interface AudioAPI {
  getDevices: () => Promise<AudioDevice[]>;
  startRecording: (deviceId: string) => Promise<void>;
  stopRecording: () => Promise<string>;
  pauseRecording: () => Promise<void>;
  resumeRecording: () => Promise<void>;
  onAudioLevel: (callback: (level: number) => void) => void;
  onError: (callback: (error: string) => void) => void;
  onDevicesChanged: (callback: (devices: AudioDevice[]) => void) => void;
  onDeviceDisconnected: (callback: (deviceId: string) => void) => void;
  onDeviceSwitched: (callback: (deviceId: string) => void) => void;
}

// Expose the audio API to the renderer process
const audioAPI: AudioAPI = {
  getDevices: () => ipcRenderer.invoke('audio:getDevices'),
  startRecording: (deviceId) => ipcRenderer.invoke('audio:startRecording', deviceId),
  stopRecording: () => ipcRenderer.invoke('audio:stopRecording'),
  pauseRecording: () => ipcRenderer.invoke('audio:pauseRecording'),
  resumeRecording: () => ipcRenderer.invoke('audio:resumeRecording'),
  onAudioLevel: (callback) => {
    ipcRenderer.on('audio:level', (_, level) => callback(level));
  },
  onError: (callback) => {
    ipcRenderer.on('audio:error', (_, error) => callback(error));
  },
  onDevicesChanged: (callback) => {
    ipcRenderer.on('audio:devicesChanged', (_, devices) => callback(devices));
  },
  onDeviceDisconnected: (callback) => {
    ipcRenderer.on('audio:deviceDisconnected', (_, deviceId) => callback(deviceId));
  },
  onDeviceSwitched: (callback: (deviceId: string) => void) => {
    ipcRenderer.on('audio:deviceSwitched', (_, deviceId) => callback(deviceId));
  }
};

contextBridge.exposeInMainWorld('audio', audioAPI);

const meetingAPI = {
  detectApps: () => ipcRenderer.invoke('meeting:detectApps'),
  detectBrowserWindows: () => ipcRenderer.invoke('meeting:detectBrowserWindows'),
};

contextBridge.exposeInMainWorld('meeting', meetingAPI);

export {}; 