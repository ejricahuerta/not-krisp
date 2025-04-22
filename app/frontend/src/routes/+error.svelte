<script lang="ts">
  import ErrorPage from "$lib/components/error-page.svelte";
  import { page } from "$app/stores";
</script>

<ErrorPage status={$page.status} message={$page.error?.message || "An unexpected error occurred"} /> 