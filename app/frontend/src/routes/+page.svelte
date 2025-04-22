<script>
  import { Button } from "$lib/components/ui/button";
  import Nav from "$lib/components/nav.svelte";
  import Typewriter from "$lib/components/typewriter.svelte";
  import { Mic, FileText, Zap, Shield } from "lucide-svelte";
  import GitHubLogin from "$lib/components/github-login.svelte";

  let ctaWordUpper = [
    "Conversation",
    "Quick Updates",
    "Standups",
    "Huddles",
    "Nudges",
    "Quick Calls",
  ];
  let ctaWordLower = [
    "Code",
    "Dev Time",
    "Bug Fixes",
    "Changelog",
    "Pull Requests",
    "Releases",
    "Cofee break",
  ];

  let currentUpperIndex = 0;
  let currentLowerIndex = 0;

  function getNextUpperWord() {
    currentUpperIndex = (currentUpperIndex + 1) % ctaWordUpper.length;
    return ctaWordUpper[currentUpperIndex];
  }

  function getNextLowerWord() {
    currentLowerIndex = (currentLowerIndex + 1) % ctaWordLower.length;
    return ctaWordLower[currentLowerIndex];
  }
</script>

<Nav />

<div class="container mx-auto px-4 py-16">
  <!-- Hero Section -->
  <section class="mb-32 grid lg:grid-cols-6 grid-cols-1 w-full">
    <div class="lg:text-left text-center lg:col-span-4 text-dark">
      <h1 class="text-6xl font-bold mb-6 leading-tight">
        From <Typewriter
          text={ctaWordUpper[currentUpperIndex]}
          className="text-primary"
          delay={100}
          repeat={true}
          repeatDelay={2000}
          onComplete={getNextUpperWord}
        />
        <br />
        to
        <Typewriter
          text={ctaWordLower[currentLowerIndex]}
          className="text-primary"
          delay={100}
          repeat={true}
          repeatDelay={2000}
          onComplete={getNextLowerWord}
        />
      </h1>
      <p class="text-2xl mb-4 text-gray-600">
        We capture your calls, summarize key points, and turn action items into
        tickets — straight to GitHub, Jira, Trello, or Notion.
      </p>
      <div class="flex flex-col gap-4">
        <div>
          <Button
            size="lg"
            class="p-8 px-12 text-xl"
            on:click={() => (window.location.href = "/auth/login")}
            >Get Started for Free</Button
          >
        </div>
        <div>
          <p class="mt-4 text-sm text-gray-500">No credit card required</p>
        </div>
      </div>
    </div>
    <div class="lg:block hidden lg:col-span-2">
      <img 
        src="/images/hero.svg" 
        alt="Hero" 
        width="500"
        height="500"
        loading="lazy"
        class="w-full h-auto"
      />
    </div>
  </section>

  <!-- Value Propositions Section -->
  <section class="grid grid-cols-1 md:grid-cols-2 gap-12 mb-32">
    <div class="p-8 bg-gray-50 rounded-xl">
      <div class="flex items-center gap-3 mb-4">
        <Mic class="w-6 h-6 text-primary" />
        <h3 class="text-2xl font-semibold">Smart Listening</h3>
      </div>
      <p class="text-gray-600">
        Plug into your meetings and let not-krisp record, clean, and transcribe
        in real-time.
      </p>
    </div>
    <div class="p-8 bg-gray-50 rounded-xl">
      <div class="flex items-center gap-3 mb-4">
        <FileText class="w-6 h-6 text-primary" />
        <h3 class="text-2xl font-semibold">Summarization Engine</h3>
      </div>
      <p class="text-gray-600">
        No more "what did we decide again?" Our AI transforms messy
        conversations into clean action items.
      </p>
    </div>
    <div class="p-8 bg-gray-50 rounded-xl">
      <div class="flex items-center gap-3 mb-4">
        <Zap class="w-6 h-6 text-primary" />
        <h3 class="text-2xl font-semibold">Instant Ticket Sync</h3>
      </div>
      <p class="text-gray-600">
        Auto-create issues and tasks in your workflow tools—no more copy-paste
        fatigue.
      </p>
    </div>
    <div class="p-8 bg-gray-50 rounded-xl">
      <div class="flex items-center gap-3 mb-4">
        <Shield class="w-6 h-6 text-primary" />
        <h3 class="text-2xl font-semibold">Secure & Private</h3>
      </div>
      <p class="text-gray-600">
        No snooping. Data is encrypted, meetings are ephemeral, and your team
        stays in control.
      </p>
    </div>
  </section>

  <!-- Preview Section -->
  <section class="mb-32">
    <h2 class="text-4xl font-bold text-center mb-16">From babble to backlog</h2>
    <div
      class="bg-gray-100 rounded-2xl p-8 h-96 flex items-center justify-center"
    >
      <p class="text-gray-500">Dashboard Preview Coming Soon</p>
    </div>
  </section>

  <!-- How It Works Section -->
  <section class="mb-32">
    <h2 class="text-4xl font-bold text-center mb-16">How It Works</h2>
    <div class="grid grid-cols-1 md:grid-cols-3 gap-12">
      <div class="text-center">
        <div class="text-5xl font-bold mb-6 text-primary">1</div>
        <h3 class="text-2xl font-semibold mb-4">
          Connect your meeting platform
        </h3>
        <p class="text-gray-600">
          Integrate with Google Meet or Slack Huddles in seconds.
        </p>
      </div>
      <div class="text-center">
        <div class="text-5xl font-bold mb-6 text-primary">2</div>
        <h3 class="text-2xl font-semibold mb-4">Join any call</h3>
        <p class="text-gray-600">
          We listen, transcribe, and summarize automatically.
        </p>
      </div>
      <div class="text-center">
        <div class="text-5xl font-bold mb-6 text-primary">3</div>
        <h3 class="text-2xl font-semibold mb-4">Sync to your tools</h3>
        <p class="text-gray-600">
          One click to create tickets in your favorite project management tools.
        </p>
      </div>
    </div>
  </section>

  <!-- Integration Section -->
  <section class="mb-32">
    <h2 class="text-4xl font-bold text-center mb-16">
      Works with Your Favorite Tools
    </h2>
    <div class="flex flex-wrap justify-center gap-8 max-w-4xl mx-auto">
      <div
        class="w-32 h-32 bg-white rounded-lg shadow-sm hover:shadow-md transition-all duration-200 flex items-center justify-center p-6"
      >
        <img
          src="images/logo/google.svg"
          alt="Google Meet"
          width="48"
          height="48"
          loading="lazy"
          class="w-12 h-12 object-contain"
        />
      </div>
      <div
        class="w-32 h-32 bg-white rounded-lg shadow-sm hover:shadow-md transition-all duration-200 flex items-center justify-center p-6"
      >
        <img
          src="images/logo/slack.svg"
          alt="Slack"
          width="48"
          height="48"
          loading="lazy"
          class="w-12 h-12 object-contain"
        />
      </div>
      <div
        class="w-32 h-32 bg-white rounded-lg shadow-sm hover:shadow-md transition-all duration-200 flex items-center justify-center p-6"
      >
        <img
          src="images/logo/github.svg"
          alt="GitHub"
          width="48"
          height="48"
          loading="lazy"
          class="w-12 h-12 object-contain"
        />
      </div>
    </div>
  </section>

  <!-- Built For Section -->
  <section class="mb-32">
    <h2 class="text-4xl font-bold text-center mb-16">Built For</h2>
    <div class="grid grid-cols-1 md:grid-cols-2 gap-12">
      <div class="p-8 bg-gray-50 rounded-xl">
        <h3 class="text-2xl font-semibold mb-4">Product Teams</h3>
        <p class="text-gray-600">
          Tired of scattered decisions? Keep everything organized and
          actionable.
        </p>
      </div>
      <div class="p-8 bg-gray-50 rounded-xl">
        <h3 class="text-2xl font-semibold mb-4">Engineers</h3>
        <p class="text-gray-600">
          Want clear actionables? Get precise tasks from every meeting.
        </p>
      </div>
      <div class="p-8 bg-gray-50 rounded-xl">
        <h3 class="text-2xl font-semibold mb-4">Managers</h3>
        <p class="text-gray-600">
          Need async clarity? Stay in the loop without attending every meeting.
        </p>
      </div>
      <div class="p-8 bg-gray-50 rounded-xl">
        <h3 class="text-2xl font-semibold mb-4">Startups</h3>
        <p class="text-gray-600">
          Moving fast, staying lean? Automate your meeting follow-ups.
        </p>
      </div>
    </div>
  </section>

  <!-- CTA Section -->
  <section class="text-center bg-primary rounded-2xl p-16 mb-32">
    <h2 class="text-4xl font-bold mb-6 text-secondary">
      Ready to Transform Your Meetings?
    </h2>
    <p class="text-xl mb-8 text-secondary">
      Join teams who are already automating their meeting workflows.
    </p>
    <Button size="lg" class="px-8" variant="outline"
      >Get Started for Free</Button
    >
  </section>
</div>
