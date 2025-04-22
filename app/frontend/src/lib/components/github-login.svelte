<script lang="ts">
  import { Button } from "$lib/components/ui/button";
  import { Github } from "lucide-svelte";
  import { toast } from "$lib/components/ui/toast";

  let isLoading = false;
  const apiUrl = import.meta.env.VITE_API_URL;
  const githubClientId = import.meta.env.VITE_GITHUB_CLIENT_ID;
  console.log(apiUrl);
  console.log(githubClientId.substring(0, 5) + "...");
  async function handleGitHubLogin() {
    isLoading = true;
    try {
      // src/routes/integrations/github/+page.svelte or similar

      const redirectUri = `${apiUrl}/api/github/callback`; // Backend route

      const githubAuthUrl = `https://github.com/login/oauth/authorize?client_id=${githubClientId}&redirect_uri=${encodeURIComponent(redirectUri)}&scope=repo`;

      // Redirect browser (not fetch!)
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
      isLoading = false;
    }
  }
</script>

<Button
  variant="outline"
  on:click={handleGitHubLogin}
  disabled={isLoading}
  class="flex items-center gap-2 w-full"
>
  <Github class="w-4 h-4" />
  {isLoading ? "Connecting..." : "Continue with GitHub"}
</Button>
