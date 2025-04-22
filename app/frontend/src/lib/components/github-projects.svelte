<script lang="ts">
  import { onMount } from "svelte";
  import { github } from "$lib/stores/github";
  import { auth } from "$lib/stores/auth";

  import * as Card from "$lib/components/ui/card";
  import * as Alert from "$lib/components/ui/alert";
  import * as Skeleton from "$lib/components/ui/skeleton";

  export let active = false;
  let isLoading = true;
  let error: string | null = null;

  let API_URL = import.meta.env.VITE_API_URL;

  async function fetchProjects() {
    if (!active) {
      return;
    }

    try {
      if (!$auth.accessToken) {
        window.location.href = "/auth/login";
        return;
      }

      const response = await fetch(`${API_URL}/api/github/projects`, {
        headers: {
          Authorization: `Bearer ${$auth.accessToken}`,
        },
      });

      if (!response.ok) {
        const errorText = await response.text();
        console.error("API error:", errorText);
        throw new Error("Failed to fetch projects");
      }

      const projects = await response.json();
      console.log("Received projects:", projects);
      github.setProjects(projects);
    } catch (e) {
      console.error("Error fetching projects:", e);
      error =
        e instanceof Error
          ? e.message
          : "An error occurred while fetching projects";
      github.setError(error);
    } finally {
      isLoading = false;
      github.setLoading(false);
    }
  }

  $: if (active) {
    fetchProjects();
  }
</script>

<div class="space-y-4">
  <h2 class="text-2xl font-bold">Projects</h2>

  {#if error}
    <Alert.Root variant="destructive">
      <Alert.Title>Error</Alert.Title>
      <Alert.Description>{error}</Alert.Description>
    </Alert.Root>
  {/if}

  {#if isLoading}
    <div class="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
      {#each Array(6) as _}
        <Card.Root>
          <Card.Header>
            <Skeleton.Root class="h-4 w-3/4" />
          </Card.Header>
          <Card.Content>
            <Skeleton.Root class="h-4 w-full" />
            <Skeleton.Root class="h-4 w-2/3 mt-2" />
          </Card.Content>
        </Card.Root>
      {/each}
    </div>
  {:else if $github.projects.length === 0}
    <p class="text-muted-foreground">No projects found.</p>
  {:else}
    <div class="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
      {#each $github.projects as project}
        <Card.Root>
          <Card.Header>
            <Card.Title>
              <a
                href={project.html_url}
                target="_blank"
                rel="noopener noreferrer"
                class="hover:underline"
              >
                {project.name}
              </a>
            </Card.Title>
          </Card.Header>
          <Card.Content>
            {#if project.body}
              <p class="text-sm text-muted-foreground line-clamp-3">
                {project.body}
              </p>
            {/if}
            <div class="mt-4 flex items-center justify-between text-sm">
              <span class="text-muted-foreground">
                #{project.number}
              </span>
              <span class="capitalize">{project.state}</span>
            </div>
          </Card.Content>
        </Card.Root>
      {/each}
    </div>
  {/if}
</div>
