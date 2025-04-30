import { Resend } from 'resend';
import { json } from '@sveltejs/kit';
import type { RequestHandler } from './$types';
import { VITE_RESEND_API_KEY } from '$env/static/private';

// Initialize Resend client properly
const resend = new Resend(VITE_RESEND_API_KEY);

export const POST: RequestHandler = async ({ request }) => {
    try {
        console.log('Waitlist signup received');
        const { email } = await request.json();

        if (!email) {
            console.error('No email provided');
            return json({ error: 'Email is required' }, { status: 400 });
        }

        console.log('Sending confirmation email to:', email);
        
        if (!VITE_RESEND_API_KEY) {
            console.error('VITE_RESEND_API_KEY is not set');
            return json({ error: 'Server configuration error' }, { status: 500 });
        }

        // Send confirmation email to the user
        const userEmailResult = await resend.emails.send({
            from: 'Tucked <waitlist@tucked.app>',
            to: email,
            subject: 'Welcome to Tucked Waitlist',
            html: `
        <div style="font-family: system-ui, sans-serif; color: #1a1a1a; max-width: 600px; margin: 0 auto;">
          <h1 style="color: #0B0B0B;">Welcome to Tucked! 👋</h1>
          <p>Thanks for joining our waitlist. We're excited to have you on board!</p>
          <p>We're building an AI-powered issue resolution and ticket management system that will help teams work more efficiently.</p>
          <p>We'll keep you updated on our progress and let you know as soon as we're ready to launch.</p>
          <p>Best regards,<br>The Tucked Team</p>
        </div>
      `
        });

        console.log('User email result:', userEmailResult);

        // Send notification to admin
        const adminEmailResult = await resend.emails.send({
            from: 'Tucked <waitlist@tucked.app>',
            to: 'admin@tucked.app',
            subject: 'New Waitlist Signup',
            html: `
        <div style="font-family: system-ui, sans-serif; color: #1a1a1a;">
          <h2>New Waitlist Signup</h2>
          <p>Email: ${email}</p>
          <p>Time: ${new Date().toISOString()}</p>
        </div>
      `
        });

        console.log('Admin email result:', adminEmailResult);

        return json({ success: true });
    } catch (error) {
        console.error('Waitlist error:', error);
        const errorMessage = error instanceof Error ? error.message : 'Failed to process signup';
        return json({ error: errorMessage }, { status: 500 });
    }
};
