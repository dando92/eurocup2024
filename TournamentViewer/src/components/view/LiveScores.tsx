import {
  HttpTransportType,
  HubConnection,
  HubConnectionBuilder,
} from "@microsoft/signalr";
import { useEffect, useState, useMemo } from "react";
import { RawScore } from "../../models/RawScore";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faHeart } from "@fortawesome/free-solid-svg-icons";

export default function LiveScores() {
  const [, setScoreUpdateConnection] = useState<HubConnection | null>(null);
  const [scores, setScores] = useState<RawScore[]>([]);
  const [showJudgements, setShowJudgements] = useState(true);

  useEffect(() => {
    const conn = new HubConnectionBuilder()
      .withUrl(`${import.meta.env.VITE_PUBLIC_API_URL}../scoreupdatehub`, {
        skipNegotiation: true,
        transport: HttpTransportType.WebSockets,
      })
      .build();

    conn.on("OnScoreUpdate", (msg: RawScore) => {
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
  }, []);

  const sortedScores = useMemo(() => {
    return scores.sort((a, b) => {
      const scoreA = +a.score.formattedScore;
      const scoreB = +b.score.formattedScore;

      if (a.score.isFailed && !b.score.isFailed) return 1;
      if (!a.score.isFailed && b.score.isFailed) return -1;
      return scoreB - scoreA;
    });
  }, [scores]);

  if (scores.length === 0) return <></>;

  return (
    <div className="text-bianco w-auto">
      <div className="flex flex-row gap-3 items-center">
        <h2 className="text-blu">
          Now playing: {sortedScores[0].score.song.split("/")[1]}
        </h2>
        <div>
          <button
            onClick={() => setShowJudgements((prev) => !prev)}
            className="text-bianco bg-blu p-0.5 text-xs rounded-md"
          >
            {showJudgements ? "Hide" : "Show"} judgements
          </button>
        </div>
      </div>
      <div className="grid my-2 border-b pb-2  grid-cols-1 sm:grid-cols-3 lg:grid-cols-4 gap-1">
        {sortedScores.map((score, idx) => (
          <div
            key={score.score.playerName}
            className={`flex flex-col items-start p-2  rounded-md shadow-md transition-transform transform ${
              score.score.isFailed ? "bg-red-900" : "bg-upper"
            } ${
              idx === 0 ? "text-yellow-300 animate-first-place" : "text-white"
            }`}
          >
            <div className="flex flex-row gap-5 justify-between items-end w-full">
              <span className="text-xl">
                <span className="italic">#{idx + 1}</span>{" "}
                <span className="font-bold">{score.score.playerName}</span>
              </span>

              <span className=" font-bold text-xl">
                {score.score.formattedScore}%
              </span>
            </div>
            {showJudgements && <div className=" flex text-xs text-ellipsis flex-wrap gap-3  text-bianco">
              {score.score.tapNote.W0 > 0 && (
                <span className="text-blue-200">{score.score.tapNote.W0}FA</span>
              )}
              {score.score.tapNote.W1 > 0 && (
                <span>{score.score.tapNote.W1}FA</span>
              )}
              {score.score.tapNote.W2 > 0 && (
                <span className="text-yellow-300">{score.score.tapNote.W2}EX</span>
              )}
              {score.score.tapNote.W3 > 0 && (
                <span className="text-green-300">
                  {score.score.tapNote.W3}GR
                </span>
              )}
              {score.score.tapNote.W4 > 0 && (
                <span className="text-pink-300">{score.score.tapNote.W4}DE</span>
              )}
              {score.score.tapNote.W5 > 0 && (
                <span className="text-orange-300">
                  {score.score.tapNote.W5}WO
                </span>
              )}
              {score.score.tapNote.miss > 0 && (
                <span className="text-red-300">
                  {score.score.tapNote.miss}MISS
                </span>
              )}
            </div>}
            <div className="w-full flex flex-row items-center gap-3">
              <FontAwesomeIcon icon={faHeart} className="text-white" />
              <div className="relative w-full h-2 my-2 rounded-md bg-grigio overflow-hidden">
                <div
                  className={`absolute top-0 left-0 h-full transition-all ${
                    score.score.life === 1
                      ? "bg-green-500"
                      : score.score.life < 0.2
                      ? "bg-red-500"
                      : "bg-lower"
                  }`}
                  style={{ width: `${score.score.life * 100}%` }}
                ></div>
              </div>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}
