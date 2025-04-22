<script>
  import { Button } from "$lib/components/ui/button";
  import Nav from "$lib/components/nav.svelte";
</script>

<Nav />

<div class="container mx-auto px-4 py-16">
  <!-- Hero Section -->
  <section class="text-center mb-32">
    <h1 class="text-6xl font-bold mb-8 leading-tight">The World's #1<br />AI Meeting Assistant</h1>
    <p class="text-2xl mb-12 max-w-3xl mx-auto text-gray-600">Record, transcribe, and summarize your calls so you can focus on the conversation. Get unlimited transcriptions and recordings on any conferencing app.</p>
    <div class="flex justify-center gap-6">
      <Button size="lg" class="px-8">Get Started for Free</Button>
      <Button variant="outline" size="lg" class="px-8">Book a Demo</Button>
    </div>
    <p class="mt-4 text-sm text-gray-500">No credit card required</p>
  </section>

  <!-- Features Section -->
  <section class="grid grid-cols-1 md:grid-cols-3 gap-12 mb-32">
    <div class="text-center p-8 bg-gray-50 rounded-xl">
      <h3 class="text-2xl font-semibold mb-4">AI Meeting Assistant</h3>
      <p class="text-gray-600">Automatically transcribe and summarize your meetings with AI-powered technology.</p>
    </div>
    <div class="text-center p-8 bg-gray-50 rounded-xl">
      <h3 class="text-2xl font-semibold mb-4">Meeting Recording</h3>
      <p class="text-gray-600">Record your meetings with crystal clear audio quality and easy playback.</p>
    </div>
    <div class="text-center p-8 bg-gray-50 rounded-xl">
      <h3 class="text-2xl font-semibold mb-4">Action Items</h3>
      <p class="text-gray-600">Automatically extract action items and create tickets in your favorite tools.</p>
    </div>
  </section>

  <!-- How It Works Section -->
  <section class="mb-32">
    <h2 class="text-4xl font-bold text-center mb-16">How It Works</h2>
    <div class="grid grid-cols-1 md:grid-cols-3 gap-12">
      <div class="text-center">
        <div class="text-5xl font-bold mb-6 text-blue-600">1</div>
        <h3 class="text-2xl font-semibold mb-4">Join Your Meeting</h3>
        <p class="text-gray-600">Start your meeting on any platform - Zoom, Google Meet, or Microsoft Teams.</p>
      </div>
      <div class="text-center">
        <div class="text-5xl font-bold mb-6 text-blue-600">2</div>
        <h3 class="text-2xl font-semibold mb-4">AI Takes Notes</h3>
        <p class="text-gray-600">Our AI assistant transcribes and summarizes the conversation in real-time.</p>
      </div>
      <div class="text-center">
        <div class="text-5xl font-bold mb-6 text-blue-600">3</div>
        <h3 class="text-2xl font-semibold mb-4">Get Action Items</h3>
        <p class="text-gray-600">Receive a summary with action items and create tickets automatically.</p>
      </div>
    </div>
  </section>

  <!-- CTA Section -->
  <section class="text-center bg-blue-50 rounded-2xl p-16 mb-32">
    <h2 class="text-4xl font-bold mb-6">Ready to Transform Your Meetings?</h2>
    <p class="text-xl mb-8 text-gray-600">Join thousands of teams who are already using our AI Meeting Assistant.</p>
    <Button size="lg" class="px-8">Get Started for Free</Button>
  </section>

  <!-- Integration Section -->
  <section class="mb-32">
    <h2 class="text-4xl font-bold text-center mb-16">Works with Your Favorite Tools</h2>
    <div class="grid grid-cols-2 md:grid-cols-4 gap-12">
      <div class="text-center p-6 bg-white rounded-xl shadow-sm">
        <h3 class="text-xl font-semibold mb-2">Zoom</h3>
      </div>
      <div class="text-center p-6 bg-white rounded-xl shadow-sm">
        <h3 class="text-xl font-semibold mb-2">Google Meet</h3>
      </div>
      <div class="text-center p-6 bg-white rounded-xl shadow-sm">
        <h3 class="text-xl font-semibold mb-2">Microsoft Teams</h3>
      </div>
      <div class="text-center p-6 bg-white rounded-xl shadow-sm">
        <h3 class="text-xl font-semibold mb-2">Slack</h3>
      </div>
    </div>
  </section>

  <!-- Trusted By Section -->
  <section class="mb-32">
    <h2 class="text-2xl font-semibold text-center mb-12 text-gray-600">Trusted by the world's largest global brands</h2>
    <div class="grid grid-cols-2 md:grid-cols-4 gap-8">
      <div class="h-12 bg-gray-100 rounded-lg"></div>
      <div class="h-12 bg-gray-100 rounded-lg"></div>
      <div class="h-12 bg-gray-100 rounded-lg"></div>
      <div class="h-12 bg-gray-100 rounded-lg"></div>
    </div>
  </section>
</div>
