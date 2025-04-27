import { contextBridge, ipcRenderer } from 'electron';
import { AudioDevice, RecordingStatus } from '../main/audio';

export interface AudioAPI {
  getAudioDevices: () => Promise<AudioDevice[]>;
  startRecording: (deviceId: string) => Promise<void>;
  stopRecording: () => Promise<void>;
  pauseRecording: () => Promise<void>;
  resumeRecording: () => Promise<void>;
  onRecordingStatus: (callback: (status: RecordingStatus) => void) => void;
  onAudioLevel: (callback: (level: number) => void) => void;
}

const audioAPI: AudioAPI = {
  getAudioDevices: () => ipcRenderer.invoke('audio:getDevices'),
  startRecording: (deviceId) => ipcRenderer.invoke('audio:startRecording', deviceId),
  stopRecording: () => ipcRenderer.invoke('audio:stopRecording'),
  pauseRecording: () => ipcRenderer.invoke('audio:pauseRecording'),
  resumeRecording: () => ipcRenderer.invoke('audio:resumeRecording'),
  onRecordingStatus: (callback) => {
    ipcRenderer.on('audio:recordingStatus', (_, status) => callback(status));
  },
  onAudioLevel: (callback) => {
    ipcRenderer.on('audio:level', (_, level) => callback(level));
  },
};

contextBridge.exposeInMainWorld('audio', audioAPI); 