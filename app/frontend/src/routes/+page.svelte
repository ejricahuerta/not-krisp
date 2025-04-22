<script>
  import { Button } from "$lib/components/ui/button";
  import Nav from "$lib/components/nav.svelte";
  import Typewriter from "$lib/components/typewriter.svelte";
  import { Mic, FileText, Zap, Shield } from "lucide-svelte";
  import GitHubLogin from "$lib/components/github-login.svelte";
  import { auth } from "$lib/stores/auth";
    import { onMount } from "svelte";

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
    <div class="p-8 bg-gray-100 rounded-xl">
      <div class="flex items-center gap-3 mb-4">
        <Mic class="w-6 h-6 text-primary" />
        <h3 class="text-2xl font-semibold">Smart Listening</h3>
      </div>
      <p class="text-gray-600">
        Plug into your meetings and let not-krisp record, clean, and transcribe
        in real-time.
      </p>
    </div>
    <div class="p-8 bg-gray-100 rounded-xl">
      <div class="flex items-center gap-3 mb-4">
        <FileText class="w-6 h-6 text-primary" />
        <h3 class="text-2xl font-semibold">Summarization Engine</h3>
      </div>
      <p class="text-gray-600">
        No more "what did we decide again?" Our AI transforms messy
        conversations into clean action items.
      </p>
    </div>
    <div class="p-8 bg-gray-100 rounded-xl">
      <div class="flex items-center gap-3 mb-4">
        <Zap class="w-6 h-6 text-primary" />
        <h3 class="text-2xl font-semibold">Instant Ticket Sync</h3>
      </div>
      <p class="text-gray-600">
        Auto-create issues and tasks in your workflow tools—no more copy-paste
        fatigue.
      </p>
    </div>
    <div class="p-8 bg-gray-100 rounded-xl">
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
        class="w-32 h-32 bg-gray-100 rounded-lg shadow-sm hover:shadow-md transition-all duration-200 flex items-center justify-center p-6"
      >
        <svg
          xmlns="http://www.w3.org/2000/svg"
          width="40"
          height="40"
          fill="currentColor"
          class="bi bi-google"
          viewBox="0 0 16 16"
        >
          <path
            d="M15.545 6.558a9.4 9.4 0 0 1 .139 1.626c0 2.434-.87 4.492-2.384 5.885h.002C11.978 15.292 10.158 16 8 16A8 8 0 1 1 8 0a7.7 7.7 0 0 1 5.352 2.082l-2.284 2.284A4.35 4.35 0 0 0 8 3.166c-2.087 0-3.86 1.408-4.492 3.304a4.8 4.8 0 0 0 0 3.063h.003c.635 1.893 2.405 3.301 4.492 3.301 1.078 0 2.004-.276 2.722-.764h-.003a3.7 3.7 0 0 0 1.599-2.431H8v-3.08z"
          />
        </svg>
      </div>
      <div
        class="w-32 h-32 bg-gray-100 rounded-lg shadow-sm hover:shadow-md transition-all duration-200 flex items-center justify-center p-6"
      >
        <svg
          xmlns="http://www.w3.org/2000/svg"
          width="40"
          height="40"
          fill="currentColor"
          class="bi bi-slack"
          viewBox="0 0 16 16"
        >
          <path
            d="M3.362 10.11c0 .926-.756 1.681-1.681 1.681S0 11.036 0 10.111.756 8.43 1.68 8.43h1.682zm.846 0c0-.924.756-1.68 1.681-1.68s1.681.756 1.681 1.68v4.21c0 .924-.756 1.68-1.68 1.68a1.685 1.685 0 0 1-1.682-1.68zM5.89 3.362c-.926 0-1.682-.756-1.682-1.681S4.964 0 5.89 0s1.68.756 1.68 1.68v1.682zm0 .846c.924 0 1.68.756 1.68 1.681S6.814 7.57 5.89 7.57H1.68C.757 7.57 0 6.814 0 5.89c0-.926.756-1.682 1.68-1.682zm6.749 1.682c0-.926.755-1.682 1.68-1.682S16 4.964 16 5.889s-.756 1.681-1.68 1.681h-1.681zm-.848 0c0 .924-.755 1.68-1.68 1.68A1.685 1.685 0 0 1 8.43 5.89V1.68C8.43.757 9.186 0 10.11 0c.926 0 1.681.756 1.681 1.68zm-1.681 6.748c.926 0 1.682.756 1.682 1.681S11.036 16 10.11 16s-1.681-.756-1.681-1.68v-1.682h1.68zm0-.847c-.924 0-1.68-.755-1.68-1.68s.756-1.681 1.68-1.681h4.21c.924 0 1.68.756 1.68 1.68 0 .926-.756 1.681-1.68 1.681z"
          />
        </svg>
      </div>
      <div
        class="w-32 h-32 bg-gray-100 rounded-lg shadow-sm hover:shadow-md transition-all duration-200 flex items-center justify-center p-6"
      >
        <svg
          xmlns="http://www.w3.org/2000/svg"
          width="40"
          height="40"
          fill="currentColor"
          class="bi bi-github"
          viewBox="0 0 16 16"
        >
          <path
            d="M8 0C3.58 0 0 3.58 0 8c0 3.54 2.29 6.53 5.47 7.59.4.07.55-.17.55-.38 0-.19-.01-.82-.01-1.49-2.01.37-2.53-.49-2.69-.94-.09-.23-.48-.94-.82-1.13-.28-.15-.68-.52-.01-.53.63-.01 1.08.58 1.23.82.72 1.21 1.87.87 2.33.66.07-.52.28-.87.51-1.07-1.78-.2-3.64-.89-3.64-3.95 0-.87.31-1.59.82-2.15-.08-.2-.36-1.02.08-2.12 0 0 .67-.21 2.2.82.64-.18 1.32-.27 2-.27s1.36.09 2 .27c1.53-1.04 2.2-.82 2.2-.82.44 1.1.16 1.92.08 2.12.51.56.82 1.27.82 2.15 0 3.07-1.87 3.75-3.65 3.95.29.25.54.73.54 1.48 0 1.07-.01 1.93-.01 2.2 0 .21.15.46.55.38A8.01 8.01 0 0 0 16 8c0-4.42-3.58-8-8-8"
          />
        </svg>
      </div>
    </div>
  </section>

  <!-- Built For Section -->
  <section class="mb-32">
    <h2 class="text-4xl font-bold text-center mb-16">Built For</h2>
    <div class="grid grid-cols-1 md:grid-cols-2 gap-12">
      <div class="p-8 bg-gray-100 rounded-xl">
        <h3 class="text-2xl font-semibold mb-4">Product Teams</h3>
        <p class="text-gray-600">
          Tired of scattered decisions? Keep everything organized and
          actionable.
        </p>
      </div>
      <div class="p-8 bg-gray-100 rounded-xl">
        <h3 class="text-2xl font-semibold mb-4">Engineers</h3>
        <p class="text-gray-600">
          Want clear actionables? Get precise tasks from every meeting.
        </p>
      </div>
      <div class="p-8 bg-gray-100 rounded-xl">
        <h3 class="text-2xl font-semibold mb-4">Managers</h3>
        <p class="text-gray-600">
          Need async clarity? Stay in the loop without attending every meeting.
        </p>
      </div>
      <div class="p-8 bg-gray-100 rounded-xl">
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
