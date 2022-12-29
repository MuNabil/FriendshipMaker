// To contain a massage that we send it from the API in MessageDto
export interface Message {
    id: number;
    senderId: number;
    senderUsername: string;
    senderPhotoUrl: string;
    recipientId: number;
    recipientUsername: string;
    recipientPhotoUrl: string;
    content: string;
    readAt?: Date;
    sendAt: Date;
}
