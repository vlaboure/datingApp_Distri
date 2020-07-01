import { Photo } from './photo';

export interface User {
    id: number;
    userName: string;
    gender: string; 
    age: Date;
    created: Date;
    lastActive: Date;
    knownAs: string;
    photoUrl: string;
    city: string;
    country: string;
    interests?: string;
    introduction?: string;
    lookingFor?: string  ;
    photos?: Photo[]  ;
}
