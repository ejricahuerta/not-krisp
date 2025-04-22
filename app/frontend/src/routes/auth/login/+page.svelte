<script lang="ts">
  import GitHubLogin from "$lib/components/github-login.svelte";
  import { page } from "$app/stores";
  import { browser } from "$app/environment";
  import { onMount } from "svelte";
  import { auth } from "$lib/stores/auth";
  import { goto } from "$app/navigation";

  let apiUrl = import.meta.env.VITE_API_URL;
  let redirectUri: string;

  let states = [
    "Recording voice...",
    "Transcribing...",
    "Summarizing...",
    "Generating summary...",
    "Creating list of actions...",
    "Generating tickets...",
  ];

  let currentState = 0;
  let interval: number;

  onMount(() => {
    // TODO: Redirect to dashboard if user is already logged in
    if ($auth.isAuthenticated) {
      goto("/dashboard");
    }

    if (browser) {
      redirectUri = `${$page.url.origin}/auth/callback`;
      interval = window.setInterval(() => {
        currentState = (currentState + 1) % states.length;
      }, 2000);
    }

    return () => {
      if (interval) {
        window.clearInterval(interval);
      }
    };
  });
</script>

<main class="w-full lg:grid lg:grid-cols-2 min-h-screen">
  <div
    class="absolute top-12 left-12 z-20 flex items-center text-lg font-medium text-white"
  >
    Not Krisp
  </div>
  <div class="bg-[#09090b] hidden lg:block relative overflow-hidden">
    <div class="absolute inset-0 flex items-end justify-center">
      <div
        class="w-[1200px] h-[1200px] -mb-[600px] bg-gradient-radial from-purple-500/25 via-purple-800/5 to-transparent"
      ></div>
    </div>
    <div class="absolute inset-0 flex flex-col items-center justify-center">
      <div class="mt-auto mb-16 flex items-center gap-3">
        <div class="relative w-3 h-3">
          <div
            class="absolute inset-0 bg-red-500/40 rounded-full animate-ping"
          ></div>
          <div class="absolute inset-0 bg-red-500 rounded-full"></div>
        </div>
        <span class="text-lg font-medium text-white/90 animate-fade"
          >{states[currentState]}</span
        >
      </div>
      <p class="text-white/60 text-center text-sm max-w-md px-4 pb-8">
        This demo shows the real-time processing of your voice meetings - from
        recording to generating actionable tickets.
      </p>
    </div>
  </div>
  <div class="flex items-center justify-center py-12">
    <div class="mx-auto grid w-[350px] gap-6">
      <div class="grid gap-2 text-center">
        <h1 class="text-3xl font-bold">Welcome</h1>
        <p class="text-muted-foreground text-balance">
          Sign in with GitHub to continue
        </p>
      </div>
      <div class="grid gap-4">
        <GitHubLogin />
        <p class="text-center text-sm text-muted-foreground">
          More login providers coming soon
        </p>
      </div>
    </div>
  </div>
</main>
