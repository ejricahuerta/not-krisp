<!-- Layout -->
<script>
  import "../app.css";
  import { Button } from "$lib/components/ui/button";
  import { onMount } from "svelte";

  onMount(() => {
    document.documentElement.classList.add("dark");
  });
</script>

<nav
  class="container mx-auto px-4 py-6 sticky top-0 z-50 bg-background/95 backdrop-blur-sm"
>
  <div class="flex items-center justify-between">
    <!-- Logo -->
    <div class="flex items-center">
      <img src="/logo.svg" alt="Tucked Logo" class="h-8 w-8 mr-3" />
      <span class="text-xl font-bold text-white"> Tucked</span>
    </div>

    <!-- Navigation Links -->
    <div class="hidden md:flex items-center space-x-8">
      <a href="#features" class="text-gray-400 hover:text-white">Features</a>
      <a href="#integrations" class="text-gray-400 hover:text-white"
        >Integrations</a
      >
      <a href="#qa" class="text-gray-400 hover:text-white">Q&A</a>
      <a href="/" class="text-gray-400 hover:text-white">Blogs</a>
    </div>

    <!-- Auth Buttons -->
    <div class="flex items-center space-x-4">
      <Button>Join Waitlist</Button>
    </div>
  </div>
</nav>
<div class="min-h-screen bg-gray-900">
  <slot />
</div>

<style>
  :global(html) {
    scroll-behavior: smooth;
  }
</style>
