<script lang="ts">
  import { Button } from "$lib/components/ui/button";
  import { ArrowLeft } from "lucide-svelte";
  import { goto } from "$app/navigation";

  export let status: number;
  export let message: string;

  const errorMessages: Record<number, { title: string; description: string }> = {
    400: {
      title: "Bad Request",
      description: "The server couldn't understand your request. Please try again.",
    },
    401: {
      title: "Unauthorized",
      description: "You need to be logged in to access this page.",
    },
    403: {
      title: "Forbidden",
      description: "You don't have permission to access this resource.",
    },
    404: {
      title: "Page Not Found",
      description: "The page you're looking for doesn't exist or has been moved.",
    },
    500: {
      title: "Server Error",
      description: "Something went wrong on our end. Please try again later.",
    },
  };

  const errorInfo = errorMessages[status] || {
    title: "Error",
    description: message || "An unexpected error occurred.",
  };
</script>

<div class="fixed inset-0 flex items-center justify-center">
  <div class="mx-auto grid w-[350px] gap-6">
    <div class="grid gap-2 text-center">
      <div class="flex justify-center">
        <div class="text-[120px] font-bold text-muted-foreground/10">{status}</div>
      </div>
      <h1 class="text-3xl font-bold">{errorInfo.title}</h1>
      <p class="text-muted-foreground text-balance">{errorInfo.description}</p>
    </div>
    <div class="grid gap-4">
      {#if status === 401}
        <Button
          class="w-full"
          on:click={() => {
            goto("/auth/login");
          }}
        >
          Login
        </Button>
      {:else}
        <Button
          class="w-full"
          variant="outline"
          on:click={() => {
            goto("/");
          }}
        >
          <ArrowLeft class="mr-2 h-4 w-4" />
          Go Back Home
        </Button>
      {/if}
    </div>
  </div>
</div> 