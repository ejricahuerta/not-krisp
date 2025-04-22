<script lang="ts">
  import { onMount } from 'svelte';
  
  export let text = '';
  export let delay = 50;
  export let className = '';
  export let repeat = true;
  export let repeatDelay = 2000;
  export let onComplete = () => {};
  
  let displayText = '';
  let currentIndex = 0;
  let isTyping = true;
  let isDeleting = false;
  let animationFrameId: number | undefined;
  let lastTime = 0;
  
  onMount(() => {
    const animate = (timestamp: number) => {
      if (!lastTime) lastTime = timestamp;
      const delta = timestamp - lastTime;
      
      if (delta >= delay) {
        if (isTyping) {
          if (currentIndex < text.length) {
            displayText += text[currentIndex];
            currentIndex++;
          } else {
            isTyping = false;
            if (repeat) {
              setTimeout(() => {
                isDeleting = true;
              }, repeatDelay);
            } else {
              onComplete();
            }
          }
        } else if (isDeleting) {
          if (currentIndex > 0) {
            displayText = displayText.slice(0, -1);
            currentIndex--;
          } else {
            isDeleting = false;
            isTyping = true;
            onComplete();
          }
        }
        lastTime = timestamp;
      }
      
      animationFrameId = requestAnimationFrame(animate);
    };
    
    animationFrameId = requestAnimationFrame(animate);
    
    return () => {
      if (animationFrameId) {
        cancelAnimationFrame(animationFrameId);
      }
    };
  });
</script>

<span class={className}>{displayText}<span class="animate-pulse">|</span></span>

<style>
  .animate-pulse {
    animation: pulse 1s cubic-bezier(0.4, 0, 0.6, 1) infinite;
  }
  
  @keyframes pulse {
    0%, 100% {
      opacity: 1;
    }
    50% {
      opacity: 0;
    }
  }
</style> 