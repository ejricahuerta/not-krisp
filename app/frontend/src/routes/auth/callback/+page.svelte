<script lang="ts">
  import { Button } from "$lib/components/ui/button/index.js";
  import { Loader2 } from "lucide-svelte";
  import { onMount } from "svelte";
  import { toast } from "$lib/components/ui/toast";
  import { goto } from "$app/navigation";
  import { page } from "$app/stores";
  import { browser } from "$app/environment";
  import { auth } from "$lib/stores/auth";

  let countdown = 3;
  let redirectTimer: number;
  let isLoading = true;
  let error: string | null = null;
  let apiUrl: string;
  let isAuthenticated = false;
  let redirectUri: string;

  onMount(() => {
    if (!browser) return;

    apiUrl = import.meta.env.VITE_API_URL;
    redirectUri = `${$page.url.origin}/auth/callback`;

    // Check for existing token
    const storedToken = localStorage.getItem("accessToken");
    if (storedToken) {
      isAuthenticated = true;
      redirectToDashboard();
    } else {
      authenticate().then((newAccessToken) => {
        if (newAccessToken) {
          auth.setToken(newAccessToken);
          isAuthenticated = true;
          redirectToDashboard();
        } else {
          toast.add({
            title: "Authentication Failed",
            description: "Please try logging in again",
            variant: "destructive",
          });
          goto("/auth/login");
        }
      });
    }
  });

  async function authenticate() {
    if (!browser) return null;

    const urlParams = new URLSearchParams($page.url.search);
    const code = urlParams.get("code");
    console.log(
      "Code:",
      code
        ? code.substring(0, 4) + "..." + code.substring(code.length - 4)
        : "No code",
    );

    if (!code) {
      error = "No authentication code found";
      isLoading = false;
      return null;
    }

    try {
      const response = await fetch(
        `${apiUrl}/api/github/callback?code=${code}&redirectUri=${encodeURIComponent(redirectUri)}`,
        {
          method: "GET",
        },
      );

      if (!response.ok) {
        const errorData = await response.json().catch(() => ({}));
        error = errorData.message || "Authentication failed";
        isLoading = false;
        return null;
      }

      const data = await response.json();
      console.log("Auth response data:", {
        ...data,
        accessToken: data.accessToken
          ? data.accessToken.substring(0, 4) +
            "..." +
            data.accessToken.substring(data.accessToken.length - 4)
          : null,
      });
      isLoading = false;
      if (data.userInfo) {
        console.log("Setting user info:", data.userInfo);
        auth.setToken(data.accessToken, data.userInfo);
      } else {
        console.log("No user info in response");
        auth.setToken(data.accessToken);
      }
      return data.accessToken;
    } catch (error) {
      console.error("Authentication error:", error);
      error = "Failed to authenticate. Please try again.";
      isLoading = false;
      return null;
    }
  }

  function redirectToDashboard() {
    if (!browser) return;

    redirectTimer = window.setInterval(() => {
      countdown--;
      if (countdown <= 0) {
        window.clearInterval(redirectTimer);
        console.log("Redirecting to dashboard");
        //refresh the page
        window.location.href = "/dashboard";
      }
    }, 1000);

    return () => {
      window.clearInterval(redirectTimer);
    };
  }
</script>

<div class="w-full lg:grid min-h-screen">
  <div class="flex items-center justify-center py-12">
    <div class="mx-auto grid w-[350px] gap-6">
      {#if isLoading}
        <div class="grid gap-2 text-center">
          <div class="flex justify-center">
            <Loader2 class="w-16 h-16 text-primary animate-spin" />
          </div>
          <h1 class="text-3xl font-bold">Authenticating...</h1>
          <p class="text-muted-foreground text-balance">
            Please wait while we verify your credentials
          </p>
        </div>
      {:else if error}
        <div class="grid gap-2 text-center">
          <div class="flex justify-center">
            <svg
              xmlns="http://www.w3.org/2000/svg"
              width="64"
              height="64"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              stroke-width="2"
              stroke-linecap="round"
              stroke-linejoin="round"
              class="text-red-500"
              ><circle cx="12" cy="12" r="10" /><path d="m15 9-6 6" /><path
                d="m9 9 6 6"
              /></svg
            >
          </div>
          <h1 class="text-3xl font-bold">Error</h1>
          <p class="text-muted-foreground text-balance">
            {error}
          </p>
          <div class="grid gap-4">
            <Button class="w-full" on:click={() => goto("/auth/login")}>
              Back to Login
            </Button>
          </div>
        </div>
      {:else}
        <div class="grid gap-2 text-center">
          <div class="flex justify-center">
            <svg
              xmlns="http://www.w3.org/2000/svg"
              width="64"
              height="64"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              stroke-width="2"
              stroke-linecap="round"
              stroke-linejoin="round"
              class="text-green-500"
              ><circle cx="12" cy="12" r="10" /><path d="m9 12 2 2 4-4" /></svg
            >
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
      {/if}
    </div>
  </div>
</div>
