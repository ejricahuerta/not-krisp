<script lang="ts">
  import { toast } from './toast';
  import { AlertCircle, CheckCircle2 } from 'lucide-svelte';
  import { slide } from 'svelte/transition';
</script>

<div class="fixed bottom-4 right-4 flex flex-col gap-2">
  {#each $toast as t}
    <div
      class="flex items-center gap-2 rounded-lg bg-white p-4 shadow-lg"
      class:bg-red-50={t.variant === 'destructive'}
      class:text-red-600={t.variant === 'destructive'}
      transition:slide
    >
      {#if t.variant === 'destructive'}
        <AlertCircle class="h-5 w-5" />
      {:else}
        <CheckCircle2 class="h-5 w-5" />
      {/if}
      <div>
        <p class="font-medium">{t.title}</p>
        {#if t.description}
          <p class="text-sm opacity-80">{t.description}</p>
        {/if}
      </div>
    </div>
  {/each}
</div> 