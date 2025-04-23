<script lang="ts">
  import { Button } from "$lib/components/ui/button";
  import { Card, CardContent, CardHeader, CardTitle } from "$lib/components/ui/card";
  import { Progress } from "$lib/components/ui/progress/index.js";
  import { onMount } from "svelte";

  let isRecording = false;
  let mediaRecorder: MediaRecorder | null = null;
  let audioChunks: Blob[] = [];
  let audioUrl = "";
  let transcription = "";
  let loading = false;
  let error: string | null = null;
  let currentStep = 0;
  let stepProgress = 0;

  const steps = [
    { name: "Record", description: "Record your meeting audio" },
    { name: "Upload", description: "Uploading audio file" },
    { name: "Transcribe", description: "Transcribing audio to text" },
    { name: "Process", description: "Generating summary and tickets" },
    { name: "Complete", description: "Redirecting to meetings" }
  ];

  async function startRecording() {
    try {
      const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
      mediaRecorder = new MediaRecorder(stream);
      audioChunks = [];
      currentStep = 0;
      stepProgress = 0;

      mediaRecorder.ondataavailable = (event) => {
        if (event.data.size > 0) {
          audioChunks.push(event.data);
        }
      };

      mediaRecorder.onstop = async () => {
        const audioBlob = new Blob(audioChunks, { type: 'audio/wav' });
        audioUrl = URL.createObjectURL(audioBlob);
        await processAudio(audioBlob);
      };

      mediaRecorder.start();
      isRecording = true;
    } catch (error) {
      console.error("Error starting recording:", error);
      setError("Failed to access microphone. Please ensure you have granted microphone permissions.");
    }
  }

  function stopRecording() {
    if (mediaRecorder && isRecording) {
      mediaRecorder.stop();
      mediaRecorder.stream.getTracks().forEach(track => track.stop());
      isRecording = false;
    }
  }

  function setError(message: string) {
    error = message;
    loading = false;
    currentStep = 0;
    stepProgress = 0;
  }

  async function processAudio(audioBlob: Blob) {
    try {
      loading = true;
      error = null;

      // Step 1: Upload audio file
      currentStep = 1;
      stepProgress = 20;
      const formData = new FormData();
      formData.append('file', audioBlob, 'recording.wav');

      const uploadResponse = await fetch('/api/audio/upload', {
        method: 'POST',
        body: formData
      });

      if (!uploadResponse.ok) {
        throw new Error('Failed to upload audio file');
      }

      const { audioUrl } = await uploadResponse.json();
      stepProgress = 40;

      // Step 2: Transcribe audio
      currentStep = 2;
      const transcribeResponse = await fetch('/api/audio/transcribe', {
        method: 'POST',
        body: formData,
        headers: {
          'Accept': 'application/json'
        }
      });

      if (!transcribeResponse.ok) {
        throw new Error('Failed to transcribe audio');
      }

      const { transcription } = await transcribeResponse.json();
      stepProgress = 60;

      // Step 3: Process recording
      currentStep = 3;
      const processFormData = new FormData();
      processFormData.append('title', `Meeting ${new Date().toLocaleString()}`);
      processFormData.append('audioFile', audioBlob, 'recording.wav');

      const processResponse = await fetch('/api/recording/process', {
        method: 'POST',
        body: processFormData
      });

      if (!processResponse.ok) {
        throw new Error('Failed to process recording');
      }

      stepProgress = 80;
      const result = await processResponse.json();
      
      // Step 4: Complete
      currentStep = 4;
      stepProgress = 100;
      
      // Add a small delay before redirecting to show completion
      setTimeout(() => {
        window.location.href = '/meetings';
      }, 1000);
    } catch (e) {
      setError(e instanceof Error ? e.message : 'An error occurred');
      console.error('Error processing audio:', e);
    }
  }
</script>

<main class="container mx-auto px-4 py-8">
  <Card>
    <CardHeader>
      <CardTitle>Voice Recording Test</CardTitle>
    </CardHeader>
    <CardContent>
      <div class="space-y-6">
        <!-- Recording Controls -->
        <div class="flex flex-col gap-2">
          <div class="flex gap-2">
            <Button 
              on:click={startRecording} 
              disabled={isRecording || loading}
              class="bg-red-500 hover:bg-red-600"
            >
              {isRecording ? "Recording..." : "Start Recording"}
            </Button>
            <Button 
              on:click={stopRecording} 
              disabled={!isRecording || loading}
              class="bg-gray-500 hover:bg-gray-600"
            >
              Stop Recording
            </Button>
          </div>
          {#if isRecording}
            <div class="flex items-center gap-2">
              <div class="w-2 h-2 bg-red-500 rounded-full animate-pulse"></div>
              <span class="text-sm text-gray-600">Recording in progress...</span>
            </div>
          {/if}
        </div>

        <!-- Progress Steps -->
        {#if loading}
          <div class="space-y-4">
            <div class="flex justify-between items-center">
              {#each steps as step, i}
                <div class="flex flex-col items-center">
                  <div class="w-8 h-8 rounded-full flex items-center justify-center
                    {i === currentStep ? 'bg-blue-500 text-white' : 
                     i < currentStep ? 'bg-green-500 text-white' : 
                     'bg-gray-200 text-gray-600'}">
                    {i + 1}
                  </div>
                  <span class="text-xs mt-1 text-center {i === currentStep ? 'text-blue-500 font-medium' : 'text-gray-600'}">
                    {step.name}
                  </span>
                </div>
              {/each}
            </div>
            <Progress value={stepProgress} />
            <p class="text-sm text-gray-600 text-center">
              {steps[currentStep].description}...
            </p>
          </div>
        {/if}

        <!-- Error Message -->
        {#if error}
          <div class="p-4 bg-red-50 text-red-600 rounded-md">
            <p class="text-sm">{error}</p>
          </div>
        {/if}

        <!-- Audio Preview -->
        {#if audioUrl}
          <div class="space-y-2">
            <h3 class="font-medium">Recorded Audio:</h3>
            <audio controls src={audioUrl} class="w-full"></audio>
          </div>
        {/if}

        <!-- Transcription Preview -->
        {#if transcription}
          <div class="space-y-2">
            <h3 class="font-medium">Transcription:</h3>
            <p class="text-sm text-gray-600 whitespace-pre-wrap">{transcription}</p>
          </div>
        {/if}
      </div>
    </CardContent>
  </Card>
</main> 