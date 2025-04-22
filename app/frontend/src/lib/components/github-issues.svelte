<script lang="ts">
  import { onMount, onDestroy } from "svelte";
  import { github } from "$lib/stores/github";
  import { auth } from "$lib/stores/auth";
  import type { Issue as GitHubIssue } from "$lib/types/github";
  import { marked } from "marked";
  import DOMPurify from "dompurify";

  import * as Alert from "$lib/components/ui/alert";
  import * as Skeleton from "$lib/components/ui/skeleton";
  import * as Table from "$lib/components/ui/table";
  import { Badge } from "$lib/components/ui/badge";
  import * as Tooltip from "$lib/components/ui/tooltip/index.js";

  interface Label {
    name: string;
    color: string;
  }

  interface Assignee {
    login: string;
    avatar_url: string;
  }

  interface IssueWithRepo extends GitHubIssue {
    repository: string;
  }

  export let active = false;

  let isLoading = true;
  let error: string | null = null;
  let repositories: { owner: string; name: string; fullName: string }[] = [];
  let allIssues: IssueWithRepo[] = [];
  let visibleIssues: IssueWithRepo[] = [];
  let pageSize = 20;
  let currentPage = 1;
  let isInitialized = false;

  let API_URL = import.meta.env.VITE_API_URL;

  async function formatMarkdown(text: string | null): Promise<string> {
    if (!text) return "";
    // Sanitize the markdown to prevent XSS
    const html = await marked(text, { breaks: true });
    return DOMPurify.sanitize(html);
  }

  let formattedBodies: Record<string, string> = {};

  async function formatAllBodies() {
    const formatPromises = allIssues.map(async (issue) => {
      if (issue.body) {
        formattedBodies[issue.id.toString()] = await formatMarkdown(issue.body);
      }
    });
    await Promise.all(formatPromises);
  }

  async function fetchRepositories() {
    if (!active) {
      return;
    }

    try {
      if (!$auth.accessToken) {
        window.location.href = "/auth/login";
        return;
      }

      github.setLoading(true);
      const response = await fetch(`${API_URL}/api/github/repositories`, {
        headers: {
          Authorization: `Bearer ${$auth.accessToken}`,
        },
      });

      if (!response.ok) {
        const errorText = await response.text();
        console.error("Failed to fetch repositories:", errorText);
        throw new Error("Failed to fetch repositories");
      }

      const repos = await response.json();
      repositories = repos.map((repo: any) => {
        const owner = repo.owner?.login || repo.full_name?.split("/")[0];
        const name = repo.name;
        const fullName = repo.full_name || `${owner}/${name}`;
        return {
          owner,
          name,
          fullName,
        };
      });

      // Start fetching issues for all repositories
      fetchAllIssues();
    } catch (e) {
      console.error("Error fetching repositories:", e);
      error =
        e instanceof Error
          ? e.message
          : "An error occurred while fetching repositories";
      github.setError(error);
    } finally {
      github.setLoading(false);
      isLoading = false;
    }
  }

  async function fetchAllIssues() {
    if (!active || repositories.length === 0) return;

    allIssues = [];

    try {
      github.setLoading(true);
      const fetchPromises = repositories.map(async (repo) => {
        if (!repo.owner || !repo.name) {
          console.warn(`Skipping repository with missing owner or name:`, repo);
          return;
        }

        try {
          const response = await fetch(
            `${API_URL}/api/github/issues?repositoryOwner=${repo.owner}&repositoryName=${repo.name}`,
            {
              headers: {
                Authorization: `Bearer ${$auth.accessToken}`,
              },
            },
          );

          if (!response.ok) {
            const errorText = await response.text();
            console.error(
              `Failed to fetch issues for ${repo.fullName}:`,
              errorText,
            );
            return;
          }

          const issues: GitHubIssue[] = await response.json();
          const actualIssues = issues.filter((issue) => !issue.pull_request);

          // Add repository information to each issue
          const issuesWithRepo = actualIssues.map((issue) => ({
            ...issue,
            repository: repo.fullName,
          }));
          return issuesWithRepo;
        } catch (e) {
          console.error(`Error fetching issues for ${repo.fullName}:`, e);
          return;
        }
      });

      const results = await Promise.all(fetchPromises);
      allIssues = results.flat().filter(Boolean) as IssueWithRepo[];
      allIssues.sort(
        (a, b) =>
          new Date(b.updated_at).getTime() - new Date(a.updated_at).getTime(),
      );

      // Format all markdown at once
      await formatAllBodies();

      // Update visible issues
      updateVisibleIssues();

      github.setIssues(allIssues);
    } catch (e) {
      console.error("Error fetching issues:", e);
      error =
        e instanceof Error
          ? e.message
          : "An error occurred while fetching issues";
      github.setError(error);
    } finally {
      isLoading = false;
      github.setLoading(false);
    }
  }

  function updateVisibleIssues() {
    const start = (currentPage - 1) * pageSize;
    const end = start + pageSize;
    visibleIssues = allIssues.slice(start, end);
  }

  function nextPage() {
    if (currentPage * pageSize < allIssues.length) {
      currentPage++;
      updateVisibleIssues();
    }
  }

  function previousPage() {
    if (currentPage > 1) {
      currentPage--;
      updateVisibleIssues();
    }
  }

  onMount(() => {
    if (active && !isInitialized) {
      fetchRepositories();
      isInitialized = true;
    }
  });

  onDestroy(() => {
    // Cleanup if needed
    isInitialized = false;
  });

  $: if (active && !isInitialized) {
    fetchRepositories();
    isInitialized = true;
  }

  $: if (allIssues.length > 0) {
    updateVisibleIssues();
  }

  function getContrastColor(hexColor: string): string {
    // Remove the # if present
    const color = hexColor.replace("#", "");

    // Convert to RGB
    const r = parseInt(color.substring(0, 2), 16);
    const g = parseInt(color.substring(2, 4), 16);
    const b = parseInt(color.substring(4, 6), 16);

    // Calculate brightness using the formula: (0.299*R + 0.587*G + 0.114*B)
    const brightness = (0.299 * r + 0.587 * g + 0.114 * b) / 255;

    // Return white for dark colors, black for light colors
    return brightness > 0.5 ? "#000000" : "#FFFFFF";
  }
</script>

<div class="space-y-4">
  {#if error}
    <Alert.Root variant="destructive">
      <Alert.Title>Error</Alert.Title>
      <Alert.Description>{error}</Alert.Description>
    </Alert.Root>
  {/if}

  {#if isLoading}
    <div class="space-y-2">
      {#each Array(5) as _}
        <Skeleton.Root class="h-12 w-full" />
      {/each}
    </div>
  {:else if allIssues.length === 0}
    <p class="text-muted-foreground">No issues found.</p>
  {:else}
    <Table.Root class="w-full">
      <Table.Header>
        <Table.Row>
          <Table.Head class="w-1/12">Repository</Table.Head>
          <Table.Head class="w-5/12">Title and Description</Table.Head>
          <Table.Head class="w-1/12">Status</Table.Head>
          <Table.Head class="w-2/12">Labels</Table.Head>
          <Table.Head class="w-2/12">Assignees</Table.Head>
        </Table.Row>
      </Table.Header>
      <Table.Body>
        {#each visibleIssues as issue}
          <Table.Row>
            <Table.Cell>
              <span class="text-sm text-muted-foreground"
                >{issue.repository}</span
              >
            </Table.Cell>
            <Table.Cell>
              <div class="space-y-2">
                <a
                  href={issue.html_url}
                  target="_blank"
                  rel="noopener noreferrer"
                  class="font-medium hover:underline block"
                >
                  {issue.title}
                </a>
                {#if issue.body}
                  <Tooltip.Root>
                    <Tooltip.Trigger asChild>
                      <div
                        class="prose prose-sm max-w-none text-muted-foreground line-clamp-2"
                      >
                        {@html formattedBodies[issue.id.toString()] || ""}
                      </div>
                    </Tooltip.Trigger>
                    <Tooltip.Content class="max-w-md">
                      <div
                        class="prose prose-sm max-w-none text-muted-foreground p-2"
                      >
                        {@html formattedBodies[issue.id.toString()] || ""}
                      </div>
                    </Tooltip.Content>
                  </Tooltip.Root>
                {/if}
              </div>
            </Table.Cell>
            <Table.Cell>
              {#if issue.state === "open"}
                <Badge variant="default">{issue.state}</Badge>
              {:else if issue.state === "closed"}
                <Badge variant="default">{issue.state}</Badge>
              {:else}
                <Badge variant="secondary">{issue.state}</Badge>
              {/if}
            </Table.Cell>
            <Table.Cell>
              <div class="flex flex-wrap gap-2">
                {#each issue.labels as label}
                  <Badge
                    variant="secondary"
                    style="background-color: #{label.color}; color: {getContrastColor(
                      label.color,
                    )}"
                  >
                    {label.name}
                  </Badge>
                {/each}
              </div>
            </Table.Cell>
            <Table.Cell>
              <div class="flex items-center space-x-2">
                {#each issue.assignees as assignee}
                  {#if assignee.avatar_url}
                    <img
                      src={assignee.avatar_url}
                      alt={assignee.login}
                      class="w-6 h-6 rounded-full"
                      title={assignee.login}
                    />
                  {/if}
                {/each}
              </div>
            </Table.Cell>
          </Table.Row>
        {/each}
      </Table.Body>
    </Table.Root>
  {/if}
</div>

<style>
  :global(.prose) {
    max-width: none;
  }
  :global(.prose p) {
    margin: 0;
  }
  :global(.prose a) {
    color: inherit;
    text-decoration: underline;
  }
  :global(.prose code) {
    background-color: var(--muted);
    padding: 0.2em 0.4em;
    border-radius: 0.25rem;
    font-size: 0.875em;
  }
  :global(.prose pre) {
    background-color: var(--muted);
    padding: 1rem;
    border-radius: 0.5rem;
    overflow-x: auto;
  }
  :global(.prose ul),
  :global(.prose ol) {
    margin: 0;
    padding-left: 1.5rem;
  }
</style>
