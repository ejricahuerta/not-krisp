<script lang="ts">
  import { onMount } from "svelte";
  import type { Snippet } from "svelte";
  import { Button } from "$lib/components/ui/button";
  import Menu from "lucide-svelte/icons/menu";
  import X from "lucide-svelte/icons/x";

  onMount(() => {
    console.log("Mounted");
  });

  function closeWidget() {
    // Hide widget using electron API if available
    if (window.widgetAPI && typeof window.widgetAPI.hide === "function") {
      window.widgetAPI.hide();
    }
  }
</script>

<main class="flex flex-1 flex-col gap-4">
  <div class="flex gap-4 items-center justify-between sticky top-0">
    <span class="text-muted-foreground text-sm ms-4 mt-2">Tucked</span>
    <Button variant="ghost" size="icon" on:click={closeWidget}>
      <X class="h-4 w-4" />
    </Button>
  </div>
  <div class="flex flex-col gap-4 p-4">
    <h1 class="text-lg font-semibold md:text-2xl">Settings</h1>
  </div>
</main>
