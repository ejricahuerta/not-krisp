<script lang="ts">
  import { onMount } from "svelte";
  import { github } from "../stores/github";
  import type { Repository } from "../types/github";
  import {
    Card,
    CardContent,
    CardDescription,
    CardHeader,
    CardTitle,
  } from "./ui/card";
  import { Skeleton } from "./ui/skeleton";
  import { auth } from "$lib/stores/auth";

  let repositories: Repository[] = [];
  let isLoading = false;
  let error: string | null = null;

  export let active = false;

  const apiUrl = import.meta.env.VITE_API_URL;
  if (!apiUrl) {
    console.error("API URL is not configured");
    error = "API URL is not configured";
  }

  onMount(async () => {
    if (apiUrl && active) {
      await fetchRepositories();
    }
  });

  async function fetchRepositories() {
    try {
      github.setLoading(true);
      github.setError(null);

      const accessToken = $auth.accessToken;
      if (!accessToken) {
        github.setError(
          "GitHub access token not found. Please authenticate first.",
        );
        return;
      }

      const response = await fetch(`${apiUrl}/api/github/repositories`, {
        method: "GET",
        headers: {
          Authorization: `Bearer ${accessToken}`,
          "Content-Type": "application/json",
        },
      });

      if (!response.ok) {
        if (response.status === 401) {
          github.setError("Unauthorized: Please authenticate again.");
        } else if (response.status === 403) {
          github.setError(
            "Forbidden: You do not have permission to access this resource.",
          );
        } else if (response.status === 404) {
          github.setError(
            "Not Found: The requested resource could not be found.",
          );
        } else {
          github.setError(`Error: ${response.status} ${response.statusText}`);
        }
        return;
      }

      const data = await response.json();
      github.setRepositories(data);
    } catch (err) {
      if (err instanceof Error) {
        github.setError("Network error: Please check your connection.");
      } else {
        github.setError("Error fetching repositories");
      }
    } finally {
      github.setLoading(false);
    }
  }

  $: repositories = $github.repositories;
  $: isLoading = $github.isLoading;
  $: error = $github.error;
</script>

<div class="space-y-4">
  {#if error}
    <div
      class="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded relative"
      role="alert"
    >
      <strong class="font-bold">Error!</strong>
      <span class="block sm:inline"> {error}</span>
    </div>
  {/if}

  {#if isLoading}
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
      {#each Array(6) as _}
        <Card>
          <CardHeader>
            <Skeleton class="h-4 w-3/4" />
            <Skeleton class="h-3 w-1/2" />
          </CardHeader>
          <CardContent>
            <Skeleton class="h-20 w-full" />
          </CardContent>
        </Card>
      {/each}
    </div>
  {:else if repositories.length > 0}
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
      {#each repositories as repo}
        <Card>
          <CardHeader>
            <CardTitle>
              <a
                href={repo.html_url}
                target="_blank"
                rel="noopener noreferrer"
                class="hover:underline"
              >
                {repo.name}
              </a>
            </CardTitle>
            <CardDescription>{repo.full_name}</CardDescription>
          </CardHeader>
          <CardContent>
            <p class="text-sm text-gray-600 mb-2">
              {repo.description || "No description available"}
            </p>
            <div class="flex items-center space-x-4 text-sm text-gray-500">
              <span>⭐ {repo.stargazers_count}</span>
              <span>🍴 {repo.forks_count}</span>
            </div>
          </CardContent>
        </Card>
      {/each}
    </div>
  {:else}
    <div class="text-center py-8">
      <p class="text-gray-500">No repositories found.</p>
    </div>
  {/if}
</div>
