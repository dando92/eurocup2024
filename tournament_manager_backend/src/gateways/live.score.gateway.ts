import {
    WebSocketGateway,
    WebSocketServer,
    OnGatewayConnection,
    OnGatewayDisconnect,
    SubscribeMessage
} from '@nestjs/websockets';
import { Server, Socket } from 'socket.io';

export class HoldNote {
    none: number;
    letGo: number;
    held: number;
    missed: number;
}

export class TapNote {
    none: number;
    hitMine: number;
    avoidMine: number;
    checkpointMiss: number;
    miss: number;
    W5: number;
    W4: number;
    W3: number;
    W2: number;
    W1: number;
    W0: number;
    checkpointHit: number;
}

export class LiveScore {
    song: string;
    playerNumber: number;
    playerName: string;
    actualDancePoints: number;
    currentPossibleDancePoints: number;
    possibleDancePoints: number;
    formattedScore: string;
    life: number;
    isFailed: boolean;
    tapNote: TapNote;
    holdNote: HoldNote;
    totalHoldsCount: number;
    id: string;
}

@WebSocketGateway({
    path: "/liveScore",
    cors: {
        origin: '*', // Adjust this for security in production
    },
})
export class LiveScoreGateway implements OnGatewayConnection, OnGatewayDisconnect {
    @WebSocketServer()
    server: Server;
    currentSong: string;
    scoreByPlayer: { [playerName: string]: LiveScore } = {};

    @SubscribeMessage('scoreChange')
    scoreChange(client: Socket, payload: any): void {
        console.log('Received message:', payload);
        const score: LiveScore = payload;

        if (this.currentSong != score.song) {
            this.scoreByPlayer = {};
            this.currentSong = score.song;
            this.UpdateNewSongStarted(this.currentSong);
        }

        this.scoreByPlayer[score.playerName] = score;

        this.UpdateLiveScore(score);
    }    @SubscribeMessage('scoreChange')

    @SubscribeMessage('onFinalResults')
    onFinalResults(client: Socket, payload: any): void {
        console.log('Received message:', payload);
        //TODO: Add
    }


    UpdateNewSongStarted(newSongTitle: string){
        this.server.emit('NewSongStarted', newSongTitle);
    }

    UpdateLiveScore(score: LiveScore) {
        this.server.emit('LiveScore', score);
    }
  
    handleConnection(client: Socket) {
        console.log(`Client connected to match gateway: ${client.id}`);
        // You could store connected clients here for later use
    }

    handleDisconnect(client: Socket) {
        console.log(`Client disconnected to match gateway: ${client.id}`);
    }
}
