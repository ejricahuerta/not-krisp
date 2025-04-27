export type AppState = 'Running' | 'Paused';

class StateManager {
  private state: AppState = 'Paused';
  private listeners: Array<(state: AppState) => void> = [];

  getState() {
    return this.state;
  }

  setState(newState: AppState) {
    if (this.state !== newState) {
      this.state = newState;
      this.notify();
    }
  }

  onChange(listener: (state: AppState) => void) {
    this.listeners.push(listener);
  }

  private notify() {
    for (const listener of this.listeners) {
      listener(this.state);
    }
  }
}

export const stateManager = new StateManager(); 