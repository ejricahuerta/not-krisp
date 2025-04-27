import psList from 'ps-list';
import { windowManager } from 'node-window-manager';
import { getSetting } from './settings';

// Mapping of meeting app names to process names
const MEETING_APP_PROCESS_MAP: Record<string, string[]> = {
  'Zoom': ['Zoom.exe', 'zoom.us', 'zoom', 'CptHost.exe'],
  'Microsoft Teams': ['Teams.exe', 'Teams_windows_x64.exe', 'Teams', 'Teams_mac'],
  'Google Meet': [], // Browser only
  'Cisco Webex': ['Webex.exe', 'webexhost.exe', 'webexmta.exe', 'atmgr.exe'],
  'GoToMeeting': ['GoToMeeting.exe', 'g2mstart.exe', 'g2mcomm.exe'],
  'Skype': ['Skype.exe', 'SkypeApp.exe', 'skypeforbusiness.exe'],
  'BlueJeans': ['BlueJeans.exe', 'BlueJeansHelper.exe'],
  'Slack': ['Slack.exe', 'slack'],
  'Discord': ['Discord.exe', 'discord'],
  'Jitsi Meet': [], // Browser only
};

function getMeetingAppsPriorityList() {
  const priority = getSetting('meetingAppPriority') as string[];
  return (priority && Array.isArray(priority) && priority.length > 0)
    ? priority
    : [
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
      ];
}

function getMeetingApps() {
  return getMeetingAppsPriorityList().map(name => ({
    name,
    processes: MEETING_APP_PROCESS_MAP[name] || [],
  }));
}

// List of browser meeting title patterns
const BROWSER_MEETING_PATTERNS = [
  { name: 'Google Meet', pattern: /Google Meet|meet\.google\.com/i },
  { name: 'Zoom Web', pattern: /Zoom Meeting|zoom\.us\/j\//i },
  { name: 'Teams Web', pattern: /Teams Meeting|teams\.microsoft\.com/i },
  { name: 'Webex Web', pattern: /Webex|webex\.com/i },
  { name: 'Slack Web', pattern: /Slack Call|slack\.com\/call/i },
  { name: 'Discord Web', pattern: /Discord|discord\.com\/channels\//i },
  { name: 'Jitsi Meet', pattern: /Jitsi Meet|meet\.jit\.si/i },
  // Add more as needed
];

export async function detectMeetingApps() {
  try {
    const processes = await psList();
    const detected: { name: string; process: string }[] = [];
    for (const app of getMeetingApps()) {
      for (const procName of app.processes) {
        if (processes.some(p => p.name === procName)) {
          detected.push({ name: app.name, process: procName });
        }
      }
    }
    return detected;
  } catch (err) {
    console.error('Error in detectMeetingApps:', err);
    return [];
  }
}

export function detectBrowserMeetingWindows() {
  try {
    const windows = windowManager.getWindows();
    const detected: { name: string; title: string }[] = [];
    for (const win of windows) {
      if (!win.isVisible() || !win.getTitle()) continue;
      const title = win.getTitle();
      for (const pattern of BROWSER_MEETING_PATTERNS) {
        if (pattern.pattern.test(title)) {
          detected.push({ name: pattern.name, title });
        }
      }
    }
    return detected;
  } catch (err) {
    console.error('Error in detectBrowserMeetingWindows:', err);
    return [];
  }
}

/**
 * Resolves meeting conflicts and selects the primary meeting based on user-configurable priority.
 * @param nativeMeetings Array of detected native meetings ({ name, process })
 * @param browserMeetings Array of detected browser meetings ({ name, title })
 * @returns { primary: { name: string, source: string }, conflicts: Array<{ name: string, source: string }> }
 */
export function resolveMeetingPriority(nativeMeetings: { name: string; process: string }[], browserMeetings: { name: string; title: string }[]) {
  const priorityList = getMeetingAppsPriorityList();
  // Flatten all detected meetings into a single array with source
  const allMeetings = [
    ...nativeMeetings.map(m => ({ name: m.name, source: m.process })),
    ...browserMeetings.map(m => ({ name: m.name, source: m.title })),
  ];
  // Sort detected meetings by their index in the priority list
  const sorted = allMeetings
    .filter(m => priorityList.includes(m.name))
    .sort((a, b) => priorityList.indexOf(a.name) - priorityList.indexOf(b.name));
  const primary = sorted[0] || null;
  const conflicts = sorted.slice(1);
  return { primary, conflicts };
}

// TODO: For future: Add platform-specific process detection for macOS/Linux if needed 