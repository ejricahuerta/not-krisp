<script lang="ts">
  import { Button } from "$lib/components/ui/button/index.js";
  import { toast } from "$lib/components/ui/toast";
  import { page } from "$app/stores";
  import { browser } from "$app/environment";
  import { goto } from "$app/navigation";

  let isLoading = $state(false);
  const apiUrl = import.meta.env.VITE_API_URL;

  async function handleGitHubLogin() {
    if (!browser) return;
    
    isLoading = true;
    try {
      // Get the redirect URI
      const redirectUri = `${$page.url.origin}/auth/callback`;
      
      // Fetch the GitHub auth URL
      const response = await fetch(
        `${apiUrl}/api/github/authorize?redirectUri=${encodeURIComponent(redirectUri)}`,
        {
          method: 'GET',
          headers: {
            'Content-Type': 'application/json',
          },
        }
      );

      if (!response.ok) {
        throw new Error('Failed to get GitHub authorization URL');
      }

      const data = await response.json();
      const githubAuthUrl = data.url;
      
      // Redirect to GitHub
      window.location.href = githubAuthUrl;
    } catch (error) {
      console.error("Failed to initiate GitHub login:", error);
      toast.add({
        title: "Error",
        description:
          error instanceof Error
            ? error.message
            : "Failed to connect with GitHub",
        variant: "destructive",
      });
    } finally {
      setTimeout(() => {
        isLoading = false;
      }, 10000);
    }
  }
</script>

<Button
  variant="outline"
  on:click={handleGitHubLogin}
  disabled={isLoading}
  class="flex items-center gap-2 w-full"
>
  <svg
    xmlns="http://www.w3.org/2000/svg"
    width="24"
    height="24"
    fill="currentColor"
    class="bi bi-github"
    viewBox="0 0 16 16"
  >
    <path d="M8 0C3.58 0 0 3.58 0 8c0 3.54 2.29 6.53 5.47 7.59.4.07.55-.17.55-.38 0-.19-.01-.82-.01-1.49-2.01.37-2.53-.49-2.69-.94-.09-.23-.48-.94-.82-1.13-.28-.15-.68-.52-.01-.53.63-.01 1.08.58 1.23.82.72 1.21 1.87.87 2.33.66.07-.52.28-.87.51-1.07-1.78-.2-3.64-.89-3.64-3.95 0-.87.31-1.59.82-2.15-.08-.2-.36-1.02.08-2.12 0 0 .67-.21 2.2.82.64-.18 1.32-.27 2-.27.68 0 1.36.09 2 .27 1.53-1.04 2.2-.82 2.2-.82.44 1.1.16 1.92.08 2.12.51.56.82 1.27.82 2.15 0 3.07-1.87 3.75-3.65 3.95.29.25.54.73.54 1.48 0 1.07-.01 1.93-.01 2.2 0 .21.15.46.55.38A8.012 8.012 0 0 0 16 8c0-4.42-3.58-8-8-8z"/>
  </svg>
  {isLoading ? "Connecting..." : "Continue with GitHub"}
</Button>
