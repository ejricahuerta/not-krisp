<script lang="ts">
  import { github } from "$lib/stores/github";
  import { onMount } from "svelte";
  import { auth } from "$lib/stores/auth";
  import * as Tabs from "$lib/components/ui/tabs/index";
  import * as Card from "$lib/components/ui/card/index";
  import GithubRepositories from "$lib/components/github-repositories.svelte";
  import GithubProjects from "$lib/components/github-projects.svelte";
  import GithubIssues from "$lib/components/github-issues.svelte";

  let selectedTab = "repositories";

  onMount(async () => {
    if (!$auth.accessToken) {
      window.location.href = "/auth/login";
    }
  });
</script>

<div class="container mx-auto px-4 py-8">
  <div class="mb-8">
    <h1 class="text-3xl font-bold">GitHub Dashboard</h1>
    <p class="text-muted-foreground">
      View and manage your GitHub repositories, projects, and issues
    </p>
  </div>

  <Tabs.Root bind:value={selectedTab} class="space-y-4">
    <Tabs.List>
      <Tabs.Trigger value="repositories">Repositories</Tabs.Trigger>
      <Tabs.Trigger value="projects">Projects</Tabs.Trigger>
      <Tabs.Trigger value="issues">Issues</Tabs.Trigger>
    </Tabs.List>

    <Tabs.Content value="repositories">
      <Card.Root>
        <Card.Header>
          <Card.Title>Repositories</Card.Title>
          <Card.Description>
            View and manage your GitHub repositories
          </Card.Description>
        </Card.Header>
        <Card.Content>
          <GithubRepositories active={selectedTab === "repositories"} />
        </Card.Content>
      </Card.Root>
    </Tabs.Content>

    <Tabs.Content value="projects">
      <Card.Root>
        <Card.Header>
          <Card.Title>Projects</Card.Title>
          <Card.Description>View and manage your GitHub projects</Card.Description>
        </Card.Header>
        <Card.Content>
          <GithubProjects active={selectedTab === "projects"} />
        </Card.Content>
      </Card.Root>
    </Tabs.Content>

    <Tabs.Content value="issues">
      <Card.Root>
        <Card.Header>
          <Card.Title>Issues</Card.Title>
          <Card.Description>View and manage your GitHub issues</Card.Description>
        </Card.Header>
        <Card.Content>
          <GithubIssues active={selectedTab === "issues"} />
        </Card.Content>
      </Card.Root>
    </Tabs.Content>
  </Tabs.Root>
</div>
