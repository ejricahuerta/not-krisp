<script lang="ts">
  import { Button } from "$lib/components/ui/button/index.js";
  import { CheckCircle } from "lucide-svelte";
  import { onMount } from "svelte";

  let countdown = 5;
  let redirectTimer: number;

  onMount(() => {
    redirectTimer = window.setInterval(() => {
      countdown--;
      if (countdown <= 0) {
        window.clearInterval(redirectTimer);
        window.location.href = "/dashboard";
      }
    }, 1000);

    return () => {
      window.clearInterval(redirectTimer);
    };
  });
</script>

<div class="w-full lg:grid min-h-screen">
  <div class="flex items-center justify-center py-12">
    <div class="mx-auto grid w-[350px] gap-6">
      <div class="grid gap-2 text-center">
        <div class="flex justify-center">
          <CheckCircle class="w-16 h-16 text-green-500" />
        </div>
        <h1 class="text-3xl font-bold">Success!</h1>
        <p class="text-muted-foreground text-balance">
          You have been successfully authenticated.
        </p>
        <p class="text-sm text-muted-foreground">
          Redirecting to dashboard in {countdown} seconds...
        </p>
      </div>
      <div class="grid gap-4">
        <Button
          class="w-full"
          on:click={() => (window.location.href = "/dashboard")}
        >
          Go to Dashboard Now
        </Button>
      </div>
    </div>
  </div>
</div>
