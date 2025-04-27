import { BrowserWindow, ipcMain } from 'electron';
import { Readable } from 'stream';
import { pipeline } from 'stream/promises';
import { createWriteStream } from 'fs';
import { join } from 'path';
import { getUserDataPath } from './utils';

export interface AudioDevice {
  id: string;
  name: string;
  type: 'input' | 'output';
}

export type RecordingStatus = 'stopped' | 'recording' | 'paused';

class AudioRecorder {
  private mediaStream: MediaStream | null = null;
  private mediaRecorder: MediaRecorder | null = null;
  private audioChunks: Blob[] = [];
  private status: RecordingStatus = 'stopped';
  private levelMonitorTimer: NodeJS.Timeout | null = null;
  private mainWindow: BrowserWindow | null = null;

  constructor(mainWindow: BrowserWindow) {
    this.mainWindow = mainWindow;
  }

  async getDevices(): Promise<AudioDevice[]> {
    try {
      const devices = await navigator.mediaDevices.enumerateDevices();
      return devices
        .filter(device => device.kind === 'audioinput' || device.kind === 'audiooutput')
        .map(device => ({
          id: device.deviceId,
          name: device.label || `Audio Device ${device.deviceId}`,
          type: device.kind === 'audioinput' ? 'input' : 'output'
        }));
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : 'Failed to get audio devices';
      throw new Error(errorMessage);
    }
  }

  async startRecording(deviceId: string): Promise<void> {
    if (this.status === 'recording') {
      throw new Error('Recording is already in progress');
    }

    try {
      this.mediaStream = await navigator.mediaDevices.getUserMedia({
        audio: {
          deviceId: { exact: deviceId }
        }
      });

      const audioContext = new AudioContext();
      const source = audioContext.createMediaStreamSource(this.mediaStream);
      const analyser = audioContext.createAnalyser();
      source.connect(analyser);

      this.mediaRecorder = new MediaRecorder(this.mediaStream);
      this.audioChunks = [];

      this.mediaRecorder.ondataavailable = (event) => {
        if (event.data.size > 0) {
          this.audioChunks.push(event.data);
        }
      };

      this.mediaRecorder.start();
      this.status = 'recording';

      // Monitor audio levels
      const dataArray = new Uint8Array(analyser.frequencyBinCount);
      this.levelMonitorTimer = setInterval(() => {
        analyser.getByteFrequencyData(dataArray);
        const average = dataArray.reduce((a, b) => a + b) / dataArray.length;
        const normalizedLevel = average / 255;
        this.mainWindow?.webContents.send('audio:level', normalizedLevel);
      }, 100);

    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : 'Failed to start recording';
      throw new Error(errorMessage);
    }
  }

  async stopRecording(): Promise<string> {
    if (this.status !== 'recording' && this.status !== 'paused') {
      throw new Error('No active recording to stop');
    }

    return new Promise((resolve, reject) => {
      if (!this.mediaRecorder) {
        reject(new Error('MediaRecorder not initialized'));
        return;
      }

      this.mediaRecorder.onstop = async () => {
        try {
          const blob = new Blob(this.audioChunks, { type: 'audio/webm' });
          const buffer = await blob.arrayBuffer();
          const readable = new Readable();
          readable._read = () => {};
          readable.push(Buffer.from(buffer));
          readable.push(null);

          const timestamp = new Date().toISOString().replace(/[:.]/g, '-');
          const fileName = `recording-${timestamp}.webm`;
          const filePath = join(getUserDataPath(), 'recordings', fileName);

          await pipeline(readable, createWriteStream(filePath));
          this.cleanup();
          resolve(filePath);
        } catch (error) {
          const errorMessage = error instanceof Error ? error.message : 'Failed to save recording';
          reject(new Error(errorMessage));
        }
      };

      this.mediaRecorder.stop();
    });
  }

  pauseRecording(): void {
    if (this.status !== 'recording') {
      throw new Error('No active recording to pause');
    }

    this.mediaRecorder?.pause();
    this.status = 'paused';
  }

  resumeRecording(): void {
    if (this.status !== 'paused') {
      throw new Error('No paused recording to resume');
    }

    this.mediaRecorder?.resume();
    this.status = 'recording';
  }

  private cleanup(): void {
    if (this.levelMonitorTimer) {
      clearInterval(this.levelMonitorTimer);
      this.levelMonitorTimer = null;
    }

    this.mediaStream?.getTracks().forEach(track => track.stop());
    this.mediaStream = null;
    this.mediaRecorder = null;
    this.audioChunks = [];
    this.status = 'stopped';
  }
}

export function setupAudioHandlers(mainWindow: BrowserWindow): void {
  const recorder = new AudioRecorder(mainWindow);

  ipcMain.handle('audio:getDevices', async () => {
    try {
      return await recorder.getDevices();
    } catch (error) {
      mainWindow.webContents.send('audio:error', error instanceof Error ? error.message : 'Failed to get devices');
      throw error;
    }
  });

  ipcMain.handle('audio:startRecording', async (_, deviceId: string) => {
    try {
      await recorder.startRecording(deviceId);
    } catch (error) {
      mainWindow.webContents.send('audio:error', error instanceof Error ? error.message : 'Failed to start recording');
      throw error;
    }
  });

  ipcMain.handle('audio:stopRecording', async () => {
    try {
      return await recorder.stopRecording();
    } catch (error) {
      mainWindow.webContents.send('audio:error', error instanceof Error ? error.message : 'Failed to stop recording');
      throw error;
    }
  });

  ipcMain.handle('audio:pauseRecording', () => {
    try {
      recorder.pauseRecording();
    } catch (error) {
      mainWindow.webContents.send('audio:error', error instanceof Error ? error.message : 'Failed to pause recording');
      throw error;
    }
  });

  ipcMain.handle('audio:resumeRecording', () => {
    try {
      recorder.resumeRecording();
    } catch (error) {
      mainWindow.webContents.send('audio:error', error instanceof Error ? error.message : 'Failed to resume recording');
      throw error;
    }
  });

  // Device change handling
  if (typeof navigator !== 'undefined' && navigator.mediaDevices) {
    navigator.mediaDevices.addEventListener('devicechange', async () => {
      try {
        const devices = await recorder.getDevices();
        mainWindow.webContents.send('audio:devicesChanged', devices);
      } catch (error) {
        mainWindow.webContents.send('audio:error', error instanceof Error ? error.message : 'Failed to get devices after change');
      }
    });
  }
}