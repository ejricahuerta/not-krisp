// Enumerate audio devices in the renderer process
async function listAudioDevices() {
  if (!navigator.mediaDevices || !navigator.mediaDevices.enumerateDevices) {
    throw new Error('MediaDevices API not supported');
  }
  const devices = await navigator.mediaDevices.enumerateDevices();
  return devices.filter(d => d.kind === 'audioinput' || d.kind === 'audiooutput');
}

async function selectAudioDevice(deviceId) {
  // Persist the selected device ID using the settings API
  await window.settingsAPI.setSettings({ audioDeviceId: deviceId });
}

async function getSelectedAudioDevice() {
  const settings = await window.settingsAPI.getSettings();
  return settings.audioDeviceId || null;
}

module.exports = {
  listAudioDevices,
  selectAudioDevice,
  getSelectedAudioDevice,
}; 