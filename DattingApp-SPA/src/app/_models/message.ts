export interface Message {
    id: number;
    senderId: number;
    senderKnownAs: string;
    senderPhotoUrl: string;
    receptId: number;
    receptKnownAs: string;
    receptPhotoUrl: string;
    isRead: boolean;
    content: string;
    messageSent: Date;
    dateRead: Date;
}