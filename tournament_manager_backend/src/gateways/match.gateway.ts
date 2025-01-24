
// app.gateway.ts
import {
  WebSocketGateway,
  WebSocketServer,
  OnGatewayConnection,
  OnGatewayDisconnect,
} from '@nestjs/websockets';
import { Server, Socket } from 'socket.io';
import { Match } from '../crud/entities/match.entity';

@WebSocketGateway({
  path: "/matchupdatehub",
  cors: {
    origin: '*', // Adjust this for security in production
  },
})
export class MatchGateway implements OnGatewayConnection, OnGatewayDisconnect {
  @WebSocketServer()
  server: Server;

  handleConnection(client: Socket) {
    console.log(`Client connected to match gateway: ${client.id}`);
    // You could store connected clients here for later use
  }

  handleDisconnect(client: Socket) {
    console.log(`Client disconnected to match gateway: ${client.id}`);
  }

  UpdateMatch(match: Match) {
    this.server.emit('UpdateMatch', match);
  }

  OnMatchUpdate(match: Match) {
    if(!match) {
      return;
    }
      
    const msg = {matchId: match.id};

    if(match.phase)
      msg['phaseId'] = match.phase;

    if(match.phase.division)
      msg['divisionId'] = match.phase.division.id;

    this.server.emit('OnMatchUpdate', msg);
  }  
}
