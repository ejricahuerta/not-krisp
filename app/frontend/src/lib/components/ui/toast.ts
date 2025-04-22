import { writable } from 'svelte/store';

export type Toast = {
  id: string;
  title: string;
  description?: string;
  variant?: 'default' | 'destructive';
};

function createToastStore() {
  const { subscribe, update } = writable<Toast[]>([]);

  return {
    subscribe,
    add: (toast: Omit<Toast, 'id'>) => {
      const id = Math.random().toString(36).substr(2, 9);
      update((toasts) => [...toasts, { ...toast, id }]);
      setTimeout(() => {
        update((toasts) => toasts.filter((t) => t.id !== id));
      }, 3000);
    },
    remove: (id: string) => {
      update((toasts) => toasts.filter((t) => t.id !== id));
    },
  };
}

export const toast = createToastStore(); 