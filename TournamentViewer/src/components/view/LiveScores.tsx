import {
  HttpTransportType,
  HubConnection,
  HubConnectionBuilder,
} from "@microsoft/signalr";
import { useEffect, useState } from "react";
import { RawScore } from "../../models/RawScore";

export default function LiveScores() {
  const [, setScoreUpdateConnection] = useState<HubConnection | null>(null);
  const [scores, setScores] = useState<RawScore[]>([]);
  const [song, setSong] = useState<string | null>(null);

  useEffect(() => {
    const conn = new HubConnectionBuilder()
      .withUrl(`${import.meta.env.VITE_PUBLIC_API_URL}../scoreupdatehub`, {
        skipNegotiation: true,
        transport: HttpTransportType.WebSockets,
      })
      .build();

    conn.on("OnScoreUpdate", (msg: RawScore) => {
      // on every score update, add the score to the list of scores, and overwrite score of the same player
      setScores((prev) => {
        const newScores = prev.filter(
          (score) => score.score.playerName !== msg.score.playerName
        );
        return [...newScores, msg];
      });
    });

    conn.start().then(() => {
      console.log("Now listening to scores changes.");
    });

    setScoreUpdateConnection(conn);

    return () => {
      conn.stop();
    };

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  if (scores.length === 0) return <></>;

  return (
    <div>
      <h2>Now playing: {scores[0].score.song.split("/")[1]}</h2>
      <div className="grid grid-cols-1 lg:grid-cols-2">
        {scores
          .sort((a, b) => {
            const scoreA = +a.score.formattedScore;
            const scoreB = +b.score.formattedScore;

            // group by isFailed, then by score
            if (a.score.isFailed && !b.score.isFailed) return 1;
            if (!a.score.isFailed && b.score.isFailed) return -1;
            if (scoreA < scoreB) return 1;
            if (scoreA > scoreB) return -1;
            return 0;
          })
          .map((score, idx) => (
            <div
              key={score.score.playerName}
              className={`${
                score.score.isFailed ? "bg-red-800 " : "bg-upper "
              } text-white flex flex-col lg:flex-row justify-between lg:items-center gap-3 bg-upper p-3 m-3`}
            >
              <div className="flex flex-col">
                <span className="text-xl font-bold">
                  #{idx + 1} {score.score.playerName}
                </span>

                <div
                  className="bg-inherit h-2 mb-3 w-full"
                  style={{ position: "relative" }}
                >
                  <div
                    className={`${
                      score.score.life === 1 ? "bg-white" : "bg-lower"
                    } animate-pulse h-2`}
                    style={{
                      width: `${score.score.life * 100}%`,
                      position: "absolute",
                    }}
                  ></div>
                </div>
                <div className="lg:ml-auto flex overflow-hidden flex-row gap-3">
                  {score.score.tapNote.W0 > 0 && (
                    <span style={{ color: "lightblue" }}>
                      {score.score.tapNote.W0 + score.score.tapNote.W1}f
                    </span>
                  )}

                  {score.score.tapNote.W2 > 0 && (
                    <span style={{ color: "yellow" }}>
                      {score.score.tapNote.W2}e
                    </span>
                  )}
                  {score.score.tapNote.W3 > 0 && (
                    <span style={{ color: "lightgreen" }}>
                      {score.score.tapNote.W3}g
                    </span>
                  )}
                  {score.score.tapNote.W4 > 0 && (
                    <span style={{ color: "pink" }}>
                      {score.score.tapNote.W4}d
                    </span>
                  )}
                  {score.score.tapNote.W5 > 0 && (
                    <span style={{ color: "orange" }}>
                      {score.score.tapNote.W5}wo
                    </span>
                  )}
                  {score.score.tapNote.miss > 0 && (
                    <span style={{ color: "red" }}>
                      {score.score.tapNote.miss}m
                    </span>
                  )}
                </div>
              </div>
              <span className="lg:ml-3 font-bold text-xl">
                {score.score.formattedScore}%
              </span>
            </div>
          ))}
      </div>
    </div>
  );
}
