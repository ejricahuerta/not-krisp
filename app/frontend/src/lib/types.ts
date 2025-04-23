export interface Meeting {
    id: string;
    title: string;
    audioUrl: string;
    createdAt: string;
    updatedAt: string;
}

export interface Summary {
    id: string;
    meetingId: string;
    content: string;
    status: string;
    createdAt: string;
    updatedAt: string;
}

export interface Ticket {
    id: string;
    meetingId: string;
    title: string;
    description: string;
    status: string;
    platform: string;
    externalId: string;
    url: string;
    metadata: Record<string, string>;
    createdAt: string;
    updatedAt: string;
} 