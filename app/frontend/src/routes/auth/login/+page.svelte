<script lang="ts">
  import { Button } from "$lib/components/ui/button/index.js";
  import { Input } from "$lib/components/ui/input/index.js";
  import { Label } from "$lib/components/ui/label/index.js";
  import GitHubLogin from "$lib/components/github-login.svelte";
  import { Github, Mail } from "lucide-svelte";
</script>

<main class="w-full lg:grid lg:grid-cols-2 min-h-screen">
  <div class="bg-muted hidden lg:block">
    <img
      src="/images/hero.svg"
      alt="Hero"
      width="1920"
      height="1080"
      class="h-full w-full object-cover dark:brightness-[0.2] dark:grayscale"
    />
  </div>
  <div class="flex items-center justify-center py-12">
    <div class="mx-auto grid w-[350px] gap-6">
      <div class="grid gap-2 text-center">
        <h1 class="text-3xl font-bold">Welcome Back</h1>
        <p class="text-muted-foreground text-balance">
          Sign in to your account to continue
        </p>
      </div>
      <div class="grid gap-4">
        <div class="grid gap-2">
          <Label for="email">Email</Label>
          <Input id="email" type="email" placeholder="m@example.com" required />
        </div>
        <div class="grid gap-2">
          <div class="flex items-center">
            <Label for="password">Password</Label>
            <a href="##" class="ml-auto inline-block text-sm underline">
              Forgot your password?
            </a>
          </div>
          <Input id="password" type="password" required />
        </div>
        <Button type="submit" class="w-full">
          <Mail class="w-4 h-4 mr-2" />
          Login with Email
        </Button>
        <div class="relative">
          <div class="absolute inset-0 flex items-center">
            <div class="w-full border-t border-border"></div>
          </div>
          <div class="relative flex justify-center text-xs uppercase">
            <span class="bg-background px-2 text-muted-foreground">
              Or continue with
            </span>
          </div>
        </div>
        <GitHubLogin />
      </div>
      <div class="mt-4 text-center text-sm">
        Don&apos;t have an account?
        <a href="/signup" class="underline"> Sign up </a>
      </div>
    </div>
  </div>
</main>
