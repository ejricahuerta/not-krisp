<script lang="ts">
  import * as DropdownMenu from "$lib/components/ui/dropdown-menu";
  import * as Sheet from "$lib/components/ui/sheet";
  import { Button } from "$lib/components/ui/button";
  import * as Card from "$lib/components/ui/card";
  import { Input } from "$lib/components/ui/input";
  import {
    Search,
    Bell,
    House,
    Calendar,
    Ticket,
    AppWindow,
    Settings,
    CircleUser,
    Badge,
    Package2,
    Menu,
  } from "lucide-svelte";
  import { onMount } from "svelte";

  import "../app.css";
  import { goto } from "$app/navigation";
  import { page } from "$app/stores";
  import { auth } from "$lib/stores/auth";

  let { children } = $props();

  let isLoggedIn = $state(false);
  let loading = $state(true);

  onMount(() => {
    const storedToken = localStorage.getItem("accessToken");
    if (storedToken) {
      isLoggedIn = true;
    }
    setTimeout(() => {
      loading = false;
    }, 1000);
  });
</script>

{#if loading}
  <div
    class="fixed inset-0 flex justify-center items-center min-h-screen bg-background z-50"
  >
    <div class="relative w-3 h-3">
      <div
        class="absolute inset-0 bg-red-500/50 rounded-full animate-ping"
      ></div>
      <div class="absolute inset-0 bg-red-500 rounded-full"></div>
    </div>
    <span class="text-uppercase font-medium text-muted-foreground animate-fade"
      >&nbsp;Loading...&nbsp;</span
    >
  </div>
{:else if !isLoggedIn || $page.url.pathname === "/"}
  {@render children()}
{:else}
  <div
    class="grid min-h-screen w-full md:grid-cols-[220px_1fr] lg:grid-cols-[280px_1fr]"
  >
    <div class="bg-muted/40 hidden border-r md:block">
      <div class="flex h-full max-h-screen flex-col gap-2">
        <div class="flex h-14 items-center border-b px-4 lg:h-[60px] lg:px-6">
          <a href="/dashboard" class="flex items-center gap-2 font-semibold">
            <span class="">Not Krisp</span>
          </a>
          <Button variant="outline" size="icon" class="ml-auto h-8 w-8">
            <Bell class="h-4 w-4" />
            <span class="sr-only">Toggle notifications</span>
          </Button>
        </div>
        <div class="flex-1 overflow-y-auto">
          <nav class="grid items-start px-2 text-sm font-medium lg:px-4">
            <a
              href="/dashboard"
              class="text-muted-foreground hover:text-primary flex items-center gap-3 rounded-lg px-3 py-2 transition-all"
            >
              <House class="h-4 w-4" />
              Dashboard
            </a>
            <a
              href="/meetings"
              class="text-muted-foreground hover:text-primary flex items-center gap-3 rounded-lg px-3 py-2 transition-all"
            >
              <Calendar class="h-4 w-4" />
              Meetings
            </a>
          </nav>
        </div>
        <div class="mt-auto p-4">
          <Card.Root>
            <Card.Header class="p-2 pt-0 md:p-4">
              <Card.Title>Upgrade to Pro</Card.Title>
              <Card.Description>
                Unlock all features and get unlimited access to our support
                team.
              </Card.Description>
            </Card.Header>
            <Card.Content class="p-2 pt-0 md:p-4 md:pt-0">
              <Button size="sm" class="w-full">Upgrade</Button>
            </Card.Content>
          </Card.Root>
        </div>
      </div>
    </div>
    <div class="flex flex-col h-screen">
      <header
        class="bg-muted/40 flex h-14 items-center gap-4 border-b px-4 lg:h-[60px] lg:px-6 shrink-0"
      >
        <Sheet.Root>
          <Sheet.Trigger asChild let:builder>
            <Button
              variant="outline"
              size="icon"
              class="shrink-0 md:hidden"
              builders={[builder]}
            >
              <Menu class="h-5 w-5" />
              <span class="sr-only">Toggle navigation menu</span>
            </Button>
          </Sheet.Trigger>
          <Sheet.Content side="left" class="flex flex-col">
            <nav class="grid gap-2 text-lg font-medium">
              <a
                href="##"
                class="flex items-center gap-2 text-lg font-semibold"
              >
                <Package2 class="h-6 w-6" />
                <span class="sr-only">Acme Inc</span>
              </a>
              <a
                href="/dashboard"
                class="text-muted-foreground hover:text-foreground mx-[-0.65rem] flex items-center gap-4 rounded-xl px-3 py-2"
              >
                <House class="h-5 w-5" />
                Dashboard
              </a>
              <a
                href="/meetings"
                class="bg-muted text-foreground hover:text-foreground mx-[-0.65rem] flex items-center gap-4 rounded-xl px-3 py-2"
              >
                <Calendar class="h-5 w-5" />
                Meetings
              </a>
            </nav>
            <div class="mt-auto">
              <Card.Root>
                <Card.Header>
                  <Card.Title>Upgrade to Pro</Card.Title>
                  <Card.Description>
                    Unlock all features and get unlimited access to our support
                    team.
                  </Card.Description>
                </Card.Header>
                <Card.Content>
                  <Button size="sm" class="w-full">Upgrade</Button>
                </Card.Content>
              </Card.Root>
            </div>
          </Sheet.Content>
        </Sheet.Root>
        <div class="w-full flex-1">
          <form>
            <div class="relative">
              <Search
                class="text-muted-foreground absolute left-2.5 top-2.5 h-4 w-4"
              />
              <Input
                type="search"
                placeholder="Search..."
                class="bg-background w-full appearance-none pl-8 shadow-none md:w-2/3 lg:w-1/3"
              />
            </div>
          </form>
        </div>
        <DropdownMenu.Root>
          <DropdownMenu.Trigger asChild let:builder>
            <Button
              builders={[builder]}
              variant="secondary"
              size="icon"
              class="rounded-full"
            >
              {#if $auth.userInfo?.avatarUrl}
                <img
                  src={$auth.userInfo.avatarUrl}
                  alt={$auth.userInfo.name || $auth.userInfo.username}
                  class="w-5 h-5 rounded-full"
                />
              {:else}
                <CircleUser class="h-5 w-5" />
              {/if}
              <span class="sr-only">Toggle user menu</span>
            </Button>
          </DropdownMenu.Trigger>
          <DropdownMenu.Content align="end">
            <DropdownMenu.Label>
              {#if $auth.userInfo}
                <div class="flex flex-col space-y-1">
                  <p class="text-sm font-medium leading-none">
                    {$auth.userInfo.name || $auth.userInfo.username}
                  </p>
                  {#if $auth.userInfo.email}
                    <p class="text-xs leading-none text-muted-foreground">
                      {$auth.userInfo.email}
                    </p>
                  {/if}
                </div>
              {:else}
                My Account
              {/if}
            </DropdownMenu.Label>
            <DropdownMenu.Separator />
            <DropdownMenu.Item>Settings</DropdownMenu.Item>
            <DropdownMenu.Item>Support</DropdownMenu.Item>
            <DropdownMenu.Separator />
            <DropdownMenu.Item
              on:click={() => {
                auth.logout();
                window.location.href = "/auth/login";
              }}
            >
              Logout
            </DropdownMenu.Item>
          </DropdownMenu.Content>
        </DropdownMenu.Root>
      </header>
      <main class="">
        {@render children()}
      </main>
    </div>
  </div>
{/if}
