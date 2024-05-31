const SYNCSTART_UDP_PORT = 53000;
const RECONNECT_INTERVAL = 5000;

const path = require("path");
const fs = require("fs");
const dgram = require("dgram");
const WebSocket = require("ws");
const sanitize = require("sanitize-filename");
const dotenv = require("dotenv");

const env = process.env.NODE_ENV;

// use .env if production, .env.development if development
const envFile = env === "production" ? ".env" : ".env.development";

dotenv.config({ path: path.resolve(__dirname, envFile) });

const WS_URL = process.env.WS_URL;

var isWsConnected = false;
var udpServer;
var ws;
let scoreSendingQueue = [];

const parseMessage = msg => {
  const [
    // "misc" information
    song,
    playerNumber,
    playerName,
    actualDancePoints,
    currentPossibleDancePoints,
    possibleDancePoints,
    formattedScore,
    life,
    isFailed,

    // tap note scores
    tapNoteNone,
    tapNoteHitMine,
    tapNoteAvoidMine,
    tapNoteCheckpointMiss,
    tapNoteMiss,
    tapNoteW5,
    tapNoteW4,
    tapNoteW3,
    tapNoteW2,
    tapNoteW1,
    tapNoteW0,
    tapNoteCheckpointHit,

    // hold note scores
    holdNoteNone,
    holdNoteLetGo,
    holdNoteHeld,
    holdNoteMissed,
    totalHoldsCount
  ] = msg.split("|");

  return {
    song,
    playerNumber: parseInt(playerNumber, 10),
    playerName,
    actualDancePoints: parseInt(actualDancePoints, 10),
    currentPossibleDancePoints: parseInt(currentPossibleDancePoints, 10),
    possibleDancePoints: parseInt(possibleDancePoints, 10),
    formattedScore,
    life: parseFloat(life),
    isFailed: isFailed === "1",

    tapNote: {
      none: parseInt(tapNoteNone, 10),
      hitMine: parseInt(tapNoteHitMine, 10),
      avoidMine: parseInt(tapNoteAvoidMine, 10),
      checkpointMiss: parseInt(tapNoteCheckpointMiss, 10),
      miss: parseInt(tapNoteMiss, 10),
      W5: parseInt(tapNoteW5, 10),
      W4: parseInt(tapNoteW4, 10),
      W3: parseInt(tapNoteW3, 10),
      W2: parseInt(tapNoteW2, 10),
      W1: parseInt(tapNoteW1, 10),
      W0: parseInt(tapNoteW0, 10),
      checkpointHit: parseInt(tapNoteCheckpointHit, 10)
    },

    holdNote: {
      none: parseInt(holdNoteNone, 10),
      letGo: parseInt(holdNoteLetGo, 10),
      held: parseInt(holdNoteHeld, 10),
      missed: parseInt(holdNoteMissed, 10)
    },

    totalHoldsCount: parseInt(totalHoldsCount, 10)
  };
};

const processMessage = async (address, msg, isFinalScore, isFinalMarathonScore) => {
  const parsedMessage = parseMessage(msg);
  const scoreKey = `${address} ${parsedMessage.playerNumber}`;
  const scoreData = Object.assign({}, parsedMessage, { id: scoreKey });

  // write json file for final score & final marathon score
  if (isFinalScore || isFinalMarathonScore) {
    const json = JSON.stringify(scoreData);
    const filename = sanitize(
      `${Date.now()}_${scoreData.song.replace("/", "_")}_${
        scoreData.playerName
      }.json`
    );
    const filePath = path.join(".", "scores", filename);
    fs.writeFileSync(filePath, json, "utf8");

    if (
      scoreData.tapNote.W0 === 0 &&
      scoreData.tapNote.W1 === 0 &&
      scoreData.tapNote.W2 === 0 &&
      scoreData.tapNote.W3 === 0 &&
      scoreData.tapNote.W4 === 0) {
      console.log(`Irrelevant score: ${scoreData.song} - player: ${scoreData.playerName}. Will not send`);
      return;
    }

    // Store score in queue
    console.log(`Storing score: ${scoreData.song} - player: ${scoreData.playerName}`);
    scoreSendingQueue.push(json);
  }
};

var setup_web_socket = function(){
    console.log('try connect');
    ws = new WebSocket(WS_URL);
    ws.on('open', function() {
        console.log('socket open');
        isWsConnected = true;
    });
    ws.on('error', function() {
    });
    ws.on('close', function() {
        if(isWsConnected) {
          console.log('socket close');
          isWsConnected = false;
        }

        setTimeout(setup_web_socket, RECONNECT_INTERVAL);
    });
};

var setup_udp_server = function(){
  udpServer = dgram.createSocket({ type: "udp4", reuseAddr: true });
  udpServer.on("message", async (buffer, rinfo) => {
    // we are interested only in score messages
    const isFinalScoreMessage = buffer[0] === 0x05;
    const isFinalMarathonScoreMessage = buffer[0] === 0x06;

    if (!isFinalScoreMessage && !isFinalMarathonScoreMessage) {
      return;
    }

    const scoreMessage = buffer.slice(1).toString("utf-8");

    try {
      await processMessage(
        rinfo.address,
        scoreMessage,
        isFinalScoreMessage,
        isFinalMarathonScoreMessage
      );
    } catch (e) {
      console.error(`ERROR: couldn't process message '${scoreMessage}'`, e);
    }
  });
  
  udpServer.bind(SYNCSTART_UDP_PORT);
}


// Poll in 5 second intervals and send scores
async function waitAndSendScores() {
  setTimeout(async function() {
    const queueLength = scoreSendingQueue.length;

    // Send accumulated scores to google sheets
    if (queueLength > 0 && isWsConnected) {
      try {
        for (let i = 0; i < queueLength; i++) {
          var json = scoreSendingQueue[i];
          console.log("Sending score to server");
          ws.send(json);
        }

        for (let i = 0; i < queueLength; i++) {
          scoreSendingQueue.shift();
        }
      } catch (e) {
        console.log("Error: could not send score", e);
      }
    }

    // Run again after 5 seconds
    await waitAndSendScores();
  }, 5000);
}

console.log("Starting TournamentManager.Bridge!");
waitAndSendScores();

console.log("WEBSOCKET_URL:", WS_URL);
setup_web_socket();

console.log("SYNCSTART_UDP_PORT:", SYNCSTART_UDP_PORT);
setup_udp_server();

