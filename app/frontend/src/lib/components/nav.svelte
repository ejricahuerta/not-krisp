<script>
  import { Button } from "$lib/components/ui/button";
</script>

<nav class="container mx-auto px-4 py-6">
  <div class="flex items-center justify-between">
    <!-- Logo -->
    <div class="flex items-center">
      <div class="h-8 w-8 bg-blue-600 rounded-full mr-3"></div>
      <span class="text-xl font-bold">Not-Krisp</span>
    </div>

    <!-- Navigation Links -->
    <div class="hidden md:flex items-center space-x-8">
      <a href="#" class="text-gray-600 hover:text-gray-900">Products</a>
      <a href="#" class="text-gray-600 hover:text-gray-900">Use Cases</a>
      <a href="#" class="text-gray-600 hover:text-gray-900">Pricing</a>
      <a href="#" class="text-gray-600 hover:text-gray-900">Blog</a>
      <a href="#" class="text-gray-600 hover:text-gray-900">How it works?</a>
    </div>

    <!-- Auth Buttons -->
    <div class="flex items-center space-x-4">
      <Button variant="ghost">Sign in</Button>
      <Button>Get Started</Button>
    </div>
  </div>
</nav> 