import { AudioAPI, AudioDevice } from '../../../preload';

declare global {
  interface Window {
    audio: AudioAPI;
  }
}

export { AudioAPI, AudioDevice }; 