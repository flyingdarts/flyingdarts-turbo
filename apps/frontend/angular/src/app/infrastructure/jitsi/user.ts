import { AnimationStyleMetadata } from '@angular/animations';

export class JitsiUser {
    room: String = 'principalRoom'; // set your room name
    name: String = ''; // set your room name

    constructor() {
        this.room = 'principalRoom';
    }

    getRoom() {
        return this.room;
    }

    setame(roomName: String) {
        this.room = roomName;
    }

    getName() {
        return this.name;
    }

    setName(name: String) {
        this.name = name;
    }
}
