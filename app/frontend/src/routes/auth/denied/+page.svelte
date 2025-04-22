<script lang="ts">
  import { Button } from "$lib/components/ui/button/index.js";
  import { XCircle } from "lucide-svelte";
</script>

<div class="w-full lg:grid lg:min-h-[600px] xl:min-h-[800px]">
  <div class="flex items-center justify-center py-12">
    <div class="mx-auto grid w-[350px] gap-6">
      <div class="grid gap-2 text-center">
        <div class="flex justify-center">
          <XCircle class="w-16 h-16 text-red-500" />
        </div>
        <h1 class="text-3xl font-bold">Access Denied</h1>
        <p class="text-muted-foreground text-balance">
          You don't have permission to access this resource.
        </p>
      </div>
      <div class="grid gap-4">
        <Button
          class="w-full"
          on:click={() => (window.location.href = "/auth/login")}
        >
          Try Again
        </Button>
      </div>
    </div>
  </div>
</div>
