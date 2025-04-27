import Store from 'electron-store';

export interface AppSettings {
  startWithWindows: boolean;
  minimizeToTray: boolean;
  audioDeviceId: string | null;
  micDeviceId: string | null;
  speakerDeviceId: string | null;
  microphoneDeviceId: string;
  audioQuality: 'low' | 'medium' | 'high';
  noiseSuppression: boolean;
  echoCancellation: boolean;
  // Audio threshold settings
  audioThreshold: number;
  audioHysteresis: number;
  autoRecordingEnabled: boolean;
  meetingAppPriority: string[];
}

const defaultSettings: AppSettings = {
  startWithWindows: false,
  minimizeToTray: true,
  audioDeviceId: null,
  micDeviceId: null,
  speakerDeviceId: null,
  microphoneDeviceId: 'default',
  audioQuality: 'medium',
  noiseSuppression: true,
  echoCancellation: true,
  // Default threshold settings
  audioThreshold: 0.1, // 10% of max volume
  audioHysteresis: 0.02, // 2% hysteresis to prevent rapid toggling
  autoRecordingEnabled: true,
  meetingAppPriority: [
    'Zoom',
    'Microsoft Teams',
    'Google Meet',
    'Cisco Webex',
    'GoToMeeting',
    'Skype',
    'BlueJeans',
    'Slack',
    'Discord',
    'Jitsi Meet',
  ],
};

const schema = {
  startWithWindows: { type: 'boolean', default: defaultSettings.startWithWindows },
  minimizeToTray: { type: 'boolean', default: defaultSettings.minimizeToTray },
  audioDeviceId: { type: ['string', 'null'], default: defaultSettings.audioDeviceId },
  micDeviceId: { type: ['string', 'null'], default: defaultSettings.micDeviceId },
  speakerDeviceId: { type: ['string', 'null'], default: defaultSettings.speakerDeviceId },
  microphoneDeviceId: { type: 'string', default: defaultSettings.microphoneDeviceId },
  audioQuality: { 
    type: 'string', 
    enum: ['low', 'medium', 'high'], 
    default: defaultSettings.audioQuality 
  },
  noiseSuppression: { type: 'boolean', default: defaultSettings.noiseSuppression },
  echoCancellation: { type: 'boolean', default: defaultSettings.echoCancellation },
  // Audio threshold schema
  audioThreshold: { 
    type: 'number', 
    minimum: 0,
    maximum: 1,
    default: defaultSettings.audioThreshold 
  },
  audioHysteresis: { 
    type: 'number',
    minimum: 0,
    maximum: 0.5,
    default: defaultSettings.audioHysteresis 
  },
  autoRecordingEnabled: { 
    type: 'boolean', 
    default: defaultSettings.autoRecordingEnabled 
  },
  meetingAppPriority: {
    type: 'array',
    items: { type: 'string' },
    default: defaultSettings.meetingAppPriority
  },
};

// Type assertion to any to work around type issues with electron-store
const store = new Store<AppSettings>({ schema }) as any;

type SettingsKey = keyof AppSettings;

export function getSetting<K extends SettingsKey>(key: K): AppSettings[K] {
  return store.get(key);
}

export function setSetting<K extends SettingsKey>(key: K, value: AppSettings[K]) {
  store.set(key, value);
}

export function onSettingsChange<K extends SettingsKey>(key: K, callback: (value: AppSettings[K]) => void) {
  store.onDidChange(key, callback);
}

export function getAllSettings(): AppSettings {
  return {
    startWithWindows: getSetting('startWithWindows'),
    minimizeToTray: getSetting('minimizeToTray'),
    audioDeviceId: getSetting('audioDeviceId'),
    micDeviceId: getSetting('micDeviceId'),
    speakerDeviceId: getSetting('speakerDeviceId'),
    microphoneDeviceId: getSetting('microphoneDeviceId'),
    audioQuality: getSetting('audioQuality'),
    noiseSuppression: getSetting('noiseSuppression'),
    echoCancellation: getSetting('echoCancellation'),
    audioThreshold: getSetting('audioThreshold'),
    audioHysteresis: getSetting('audioHysteresis'),
    autoRecordingEnabled: getSetting('autoRecordingEnabled'),
    meetingAppPriority: getSetting('meetingAppPriority'),
  };
} 