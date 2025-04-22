<script>
  import { Button } from "$lib/components/ui/button";
  import { auth } from "$lib/stores/auth";
  import { goto } from "$app/navigation";

  let isAuthenticated = $state(false);

  $effect(() => {
    if ($auth.isAuthenticated) {
      isAuthenticated = true;
    }
  });
</script>

<nav class="container mx-auto px-4 py-6">
  <div class="flex items-center justify-between">
    <!-- Logo -->
    <div class="flex items-center">
      <div class="h-8 w-8 bg-primary rounded-full mr-3"></div>
      <span class="text-xl font-bold">Not Krisp</span>
    </div>

    <!-- Navigation Links -->
    <div class="hidden md:flex items-center space-x-8">
      <a href="/products" class="text-gray-600 hover:text-gray-900">Products</a>
      <a href="/use-cases" class="text-gray-600 hover:text-gray-900"
        >Use Cases</a
      >
      <a href="/pricing" class="text-gray-600 hover:text-gray-900">Pricing</a>
      <a href="/blog" class="text-gray-600 hover:text-gray-900">Blog</a>
      <a href="/how-it-works" class="text-gray-600 hover:text-gray-900"
        >How it works?</a
      >
    </div>

    <!-- Auth Buttons -->
    <div class="flex items-center space-x-4">
      {#if isAuthenticated}
        <Button variant="ghost" on:click={() => goto("/dashboard")}
          >Dashboard</Button
        >
        <Button variant="ghost" on:click={() => goto("/auth/logout")}
          >Logout</Button
        >
      {:else}
        <Button variant="ghost" on:click={() => goto("/auth/login")}
          >Sign in</Button
        >
        <Button on:click={() => goto("/auth/login")}>Get Started</Button>
      {/if}
    </div>
  </div>
</nav>
