<script lang="ts">
    import { onMount } from 'svelte';
    import type { Meeting, Summary, Ticket } from '$lib/types';

    let meetings: Meeting[] = [];
    let selectedMeeting: Meeting | null = null;
    let summaries: Summary[] = [];
    let tickets: Ticket[] = [];
    let loading = false;
    let error: string | null = null;
    let audioRef: HTMLAudioElement;

    onMount(async () => {
        await loadMeetings();
    });

    async function loadMeetings() {
        try {
            loading = true;
            const response = await fetch('/api/meetings');
            if (!response.ok) throw new Error('Failed to load meetings');
            meetings = await response.json();
        } catch (e) {
            error = e instanceof Error ? e.message : 'An error occurred';
        } finally {
            loading = false;
        }
    }

    async function selectMeeting(meeting: Meeting) {
        selectedMeeting = meeting;
        await loadMeetingDetails(meeting.id);
    }

    async function loadMeetingDetails(meetingId: string) {
        try {
            loading = true;
            const [summariesResponse, ticketsResponse] = await Promise.all([
                fetch(`/api/summaries/meeting/${meetingId}`),
                fetch(`/api/tickets/meeting/${meetingId}`)
            ]);

            if (!summariesResponse.ok) throw new Error('Failed to load summaries');
            if (!ticketsResponse.ok) throw new Error('Failed to load tickets');

            summaries = await summariesResponse.json();
            tickets = await ticketsResponse.json();
        } catch (e) {
            error = e instanceof Error ? e.message : 'An error occurred';
        } finally {
            loading = false;
        }
    }

    function formatTime(seconds: number): string {
        const minutes = Math.floor(seconds / 60);
        const remainingSeconds = Math.floor(seconds % 60);
        return `${minutes}:${remainingSeconds.toString().padStart(2, '0')}`;
    }

    function handleTimeUpdate() {
        const progress = (audioRef.currentTime / audioRef.duration) * 100;
        const progressBar = document.getElementById('progress-bar');
        if (progressBar) {
            progressBar.style.width = `${progress}%`;
        }
    }
</script>

<div class="container mx-auto p-4">
    <h1 class="text-2xl font-bold mb-4">Meetings</h1>

    {#if error}
        <div class="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded mb-4">
            {error}
        </div>
    {/if}

    <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
        <!-- Meetings List -->
        <div class="bg-white rounded-lg shadow p-4">
            <h2 class="text-xl font-semibold mb-4">Recent Meetings</h2>
            {#if loading}
                <div class="text-center py-4">Loading...</div>
            {:else}
                <div class="space-y-2">
                    {#each meetings as meeting}
                        <button
                            class="w-full text-left p-3 rounded hover:bg-gray-100 {selectedMeeting?.id === meeting.id ? 'bg-blue-50 border border-blue-200' : ''}"
                            on:click={() => selectMeeting(meeting)}
                        >
                            <div class="font-medium">{meeting.title}</div>
                            <div class="text-sm text-gray-500">
                                {new Date(meeting.createdAt).toLocaleString()}
                            </div>
                        </button>
                    {/each}
                </div>
            {/if}
        </div>

        <!-- Meeting Details -->
        <div class="bg-white rounded-lg shadow p-4">
            {#if selectedMeeting}
                <h2 class="text-xl font-semibold mb-4">Meeting Details</h2>
                <div class="mb-6">
                    <h3 class="font-medium mb-2">Title</h3>
                    <p>{selectedMeeting.title}</p>
                </div>

                <!-- Audio Player -->
                <div class="mb-6">
                    <h3 class="font-medium mb-2">Recording</h3>
                    <div class="bg-gray-50 rounded-lg p-4">
                        <audio
                            bind:this={audioRef}
                            src={selectedMeeting.audioUrl}
                            on:timeupdate={handleTimeUpdate}
                            class="w-full mb-2"
                        ></audio>
                        <div class="flex items-center justify-between text-sm text-gray-600">
                            <button
                                class="p-2 rounded-full hover:bg-gray-200"
                                on:click={() => audioRef.play()}
                            >
                                <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" viewBox="0 0 20 20" fill="currentColor">
                                    <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM9.555 7.168A1 1 0 008 8v4a1 1 0 001.555.832l3-2a1 1 0 000-1.664l-3-2z" clip-rule="evenodd" />
                                </svg>
                            </button>
                            <div class="flex-1 mx-2">
                                <div class="h-1 bg-gray-200 rounded-full">
                                    <div id="progress-bar" class="h-1 bg-blue-500 rounded-full" style="width: 0%"></div>
                                </div>
                            </div>
                            <span id="current-time">0:00</span>
                        </div>
                    </div>
                </div>

                <!-- Summaries -->
                <div class="mb-6">
                    <h3 class="font-medium mb-2">Summaries</h3>
                    {#if summaries.length === 0}
                        <p class="text-gray-500">No summaries available</p>
                    {:else}
                        <div class="space-y-4">
                            {#each summaries as summary}
                                <div class="border rounded p-3">
                                    <p class="whitespace-pre-line">{summary.content}</p>
                                    <div class="text-sm text-gray-500 mt-2">
                                        {new Date(summary.createdAt).toLocaleString()}
                                    </div>
                                </div>
                            {/each}
                        </div>
                    {/if}
                </div>

                <!-- Tickets -->
                <div>
                    <h3 class="font-medium mb-2">Action Items</h3>
                    {#if tickets.length === 0}
                        <p class="text-gray-500">No action items</p>
                    {:else}
                        <div class="space-y-2">
                            {#each tickets as ticket}
                                <div class="border rounded p-3">
                                    <div class="font-medium">{ticket.title}</div>
                                    <p class="text-sm text-gray-600">{ticket.description}</p>
                                    <div class="flex items-center mt-2">
                                        <span class="text-xs px-2 py-1 rounded {ticket.status === 'Open' ? 'bg-yellow-100 text-yellow-800' : 'bg-green-100 text-green-800'}">
                                            {ticket.status}
                                        </span>
                                    </div>
                                </div>
                            {/each}
                        </div>
                    {/if}
                </div>
            {:else}
                <div class="text-center py-8 text-gray-500">
                    Select a meeting to view details
                </div>
            {/if}
        </div>
    </div>
</div>

<style>
    .container {
        max-width: 1200px;
    }
</style>
