<script lang="ts">
  import { Button } from "$lib/components/ui/button";
  let email = "";
  let isLoading = false;
  let error = "";
  let success = false;

  async function handleSubmit() {
    isLoading = true;
    error = "";
    success = false;

    try {
      console.log('Sending waitlist signup request');
      const response = await fetch('/api/waitlist', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ email }),
      });

      const data = await response.json();
      console.log('Response:', { status: response.status, data });

      if (!response.ok) {
        throw new Error(data.error || 'Failed to join waitlist');
      }

      success = true;
      email = ""; // Clear the form
    } catch (err: unknown) {
      console.error('Form submission error:', err);
      error = err instanceof Error ? err.message : 'An unexpected error occurred';
    } finally {
      isLoading = false;
    }
  }
</script>

<div class="bg-gradient-to-b from-background to-background/95">
  <div class="container mx-auto px-4">
    <!-- Hero Section -->
    <section class="min-h-screen grid grid-cols-1 md:grid-cols-2 gap-8">
      <div class="flex flex-col justify-center">
        <h1
          class="text-6xl font-bold mb-8 leading-tight bg-clip-text text-transparent bg-gradient-to-r from-white to-white/80"
        >
          Resolve Issues Faster. Automatically.
        </h1>
        <p class="text-2xl mb-12 mx-auto text-muted-foreground">
          Capture errors and conversations. <strong>Tucked</strong> creates and assigns
          tickets, with instant alerts. Get early access and reduce downtime.
        </p>
        <div class="">
          <form
            class="flex w-full max-w-md gap-4"
            on:submit|preventDefault={handleSubmit}
          >
            <input
              type="email"
              placeholder="Your Email Address"
              class="p-4 px-12 text-xl bg-card/50 backdrop-blur-sm border border-border rounded-lg text-white focus:outline-none focus:border-primary focus:ring-2 focus:ring-primary/20 transition-all"
              bind:value={email}
              disabled={isLoading}
              required
            />
            <Button
              type="submit"
              class="focus-visible:ring-ring inline-flex justify-center whitespace-nowrap font-medium transition-colors focus-visible:outline-none focus-visible:ring-1 disabled:pointer-events-none disabled:opacity-50 bg-primary text-primary-foreground hover:bg-primary/90 shadow h-10 rounded-md p-8 px-12 text-xl"
              disabled={isLoading}
            >
              {#if isLoading}
                Joining...
              {:else}
                Join Waitlist
              {/if}
            </Button>
          </form>
          {#if error}
            <p class="text-sm text-red-500 my-4">{error}</p>
          {/if}
          {#if success}
            <p class="text-sm text-green-500 my-4">
              Thanks for joining! We'll be in touch soon.
            </p>
          {:else}
            <p id="features" class="text-sm text-muted-foreground my-4">
              Be the first to streamline your issue resolution process.
            </p>
          {/if}
        </div>
      </div>
    </section>

    <!-- Features Section -->
    <section class="grid grid-cols-1 md:grid-cols-3 gap-8 mb-32">
      <div class="feature-card">
        <h3 class="text-2xl font-semibold mb-4 text-white">Capture Anything</h3>
        <p class="text-muted-foreground">
          From system logs and app errors to spoken team discussions.
        </p>
      </div>
      <div class="feature-card">
        <h3 class="text-2xl font-semibold mb-4 text-white">
          Automated Tickets
        </h3>
        <p class="text-muted-foreground">
          Intelligently creates and updates tickets based on captured data.
        </p>
      </div>
      <div class="feature-card">
        <h3 class="text-2xl font-semibold mb-4 text-white">
          Smart Notifications
        </h3>
        <p class="text-muted-foreground">
          Instantly alerts the right team members about new or updated tickets.
        </p>
      </div>
      <div class="feature-card">
        <h3 class="text-2xl font-semibold mb-4 text-white">
          Intelligent Assignment
        </h3>
        <p class="text-muted-foreground">
          Automatically assigns tickets to relevant individuals or teams.
        </p>
      </div>
      <div class="feature-card">
        <h3 class="text-2xl font-semibold mb-4 text-white">
          Seamless Integration
        </h3>
        <p class="text-muted-foreground">
          Works with your existing project management tools.
        </p>
      </div>
      <div class="feature-card">
        <h3 class="text-2xl font-semibold mb-4 text-white">
          Minimize Downtime
        </h3>
        <p class="text-muted-foreground">
          Proactively address issues for a more stable and efficient workflow.
        </p>
      </div>
    </section>

    <!-- Integration Section -->
    <section id="integrations" class="mb-32 overflow-hidden">
      <h2 class="text-4xl font-bold text-center mb-16 bg-clip-text text-transparent bg-gradient-to-r from-white to-white/80">
        Works with Your Favorite Tools
      </h2>
      <div class="flex animate-scroll">
        <!-- First set of logos -->
        <div class="flex min-w-full justify-around items-center gap-8 px-4">
          <img
            src="/logos/github.svg"
            alt="Github"
            class="h-12 w-auto opacity-50 hover:opacity-100 transition-opacity"
          />
          <img
            src="/logos/asana.svg"
            alt="Asana"
            class="h-12 w-auto opacity-50 hover:opacity-100 transition-opacity"
          />
          <img
            src="/logos/notion.svg"
            alt="Notion"
            class="h-12 w-auto opacity-50 hover:opacity-100 transition-opacity"
          />
          <img
            src="/logos/jira.svg"
            alt="Jira"
            class="h-12 w-auto opacity-50 hover:opacity-100 transition-opacity"
          />
          <img
            src="/logos/trello.svg"
            alt="Trello"
            class="h-12 w-auto opacity-50 hover:opacity-100 transition-opacity"
          />
        </div>
        <!-- Second set of logos -->
        <div class="flex min-w-full justify-around items-center gap-8 px-4">
          <img
            src="/logos/zoom.svg"
            alt="Zoom"
            class="h-12 w-auto opacity-50 hover:opacity-100 transition-opacity"
          />
          <img
            src="/logos/google-meet.svg"
            alt="Google Meet"
            class="h-12 w-auto opacity-50 hover:opacity-100 transition-opacity"
          />
          <img
            src="/logos/slack.svg"
            alt="Slack"
            class="h-12 w-auto opacity-50 hover:opacity-100 transition-opacity"
          />
          <img
            src="/logos/ms-teams.svg"
            alt="Microsoft Teams"
            class="h-12 w-auto opacity-50 hover:opacity-100 transition-opacity"
          />
          <img
            src="/logos/github.svg"
            alt="Github"
            class="h-12 w-auto opacity-50 hover:opacity-100 transition-opacity"
          />
        </div>
      </div>
    </section>

    <!-- Early Access CTA Section -->
    <section class="text-center feature-card mb-32 py-16">
      <h2 class="text-5xl font-bold mb-6 text-white">
        Tucked away — like I'm not here.
      </h2>
      <p class=" mb-8 text-muted-foreground px-5">
        Be one of the first to experience the power of automated issue
        resolution. Enter your email below to stay informed about our launch and
        gain early access.
      </p>
      <form
        class="flex max-w-md mx-auto gap-4"
        on:submit|preventDefault={handleSubmit}
      >
        <input
          type="email"
          placeholder="Your Email Address"
          class="flex-1 px-4 py-3 bg-background/50 backdrop-blur-sm border border-border rounded-lg text-white focus:outline-none focus:border-primary focus:ring-2 focus:ring-primary/20 transition-all"
          bind:value={email}
          disabled={isLoading}
          required
        />
        <Button
          class="focus-visible:ring-ring inline-flex items-center justify-center whitespace-nowrap font-medium transition-colors focus-visible:outline-none focus-visible:ring-1 disabled:pointer-events-none disabled:opacity-50 bg-primary text-primary-foreground hover:bg-primary/90 shadow h-10 rounded-md p-8 px-12 text-xl"
          disabled={isLoading}
        >
          {#if isLoading}
            Joining...
          {:else}
            Join Waitlist
          {/if}
        </Button>
      </form>
      {#if error}
        <p class="text-sm text-red-500 mt-4">{error}</p>
      {/if}
      {#if success}
        <p class="text-sm text-green-500 mt-4">
          Thanks for joining! We'll be in touch soon.
        </p>
      {/if}
    </section>
  </div>
</div>
