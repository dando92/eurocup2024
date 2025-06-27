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
    <div className="text-slate-100 w-auto">
      <div className="flex flex-row gap-4 items-center mb-6">
        <h2 className="text-2xl font-semibold text-slate-200 bg-gradient-to-r from-slate-700 to-slate-600 px-4 py-2 rounded-lg shadow-sm">
          Now playing: {sortedScores[0].score.song.split("/")[1]}
        </h2>
        <div>
          <button
            onClick={() => setShowJudgements((prev) => !prev)}
            className="text-slate-100 bg-slate-600 hover:bg-slate-500 px-3 py-1.5 text-sm rounded-lg transition-all duration-200 shadow-sm"
          >
            {showJudgements ? "Hide" : "Show"} judgements
          </button>
        </div>
      </div>
      <div className="grid my-4 gap-3 grid-cols-1 sm:grid-cols-2 lg:grid-cols-3">
        {sortedScores.map((score, idx) => (
          <div
            key={score.score.playerName}
            className={`position-transition flex flex-col items-start p-4 rounded-xl shadow-lg transition-all duration-500 ease-in-out transform hover:scale-105 border ${
              score.score.isFailed 
                ? "bg-gradient-to-br from-red-900/80 to-red-800/60 border-red-700/50" 
                : "bg-gradient-to-br from-slate-800/90 to-slate-700/80 border-slate-600/30"
            } ${
              idx === 0 
                ? "ring-2 ring-amber-400/50 bg-gradient-to-br from-amber-900/30 to-slate-800/90 text-amber-100 shadow-xl animate-first-place" 
                : "text-slate-100"
            }`}
            style={{
              transform: `translateY(${idx * 1}px)`,
              zIndex: sortedScores.length - idx,
            }}
          >
            <div className="flex flex-row gap-6 justify-between items-end w-full mb-3">
              <div className="flex items-center gap-2">
                <span className={`text-sm font-medium px-2 py-1 rounded-md ${
                  idx === 0 
                    ? "bg-amber-400/20 text-amber-200" 
                    : "bg-slate-600/50 text-slate-300"
                }`}>
                  #{idx + 1}
                </span>
                <span className="font-semibold text-lg">{score.score.playerName}</span>
              </div>

              <span className="font-bold text-xl tracking-wide">
                {score.score.formattedScore}%
              </span>
            </div>
            {showJudgements && <div className="flex text-xs flex-wrap gap-1 mb-3 p-1 bg-slate-800/50 rounded-lg border border-slate-600/30">
              {score.score.tapNote.W0 > 0 && (
                <span className="text-blue-200 bg-blue-900/30 px-2 py-1 rounded-md font-medium">{score.score.tapNote.W0}FA</span>
              )}
              {score.score.tapNote.W1 > 0 && (
                <span className="text-slate-200 bg-slate-700/50 px-2 py-1 rounded-md font-medium">{score.score.tapNote.W1}FA</span>
              )}
              {score.score.tapNote.W2 > 0 && (
                <span className="text-amber-200 bg-amber-900/30 px-2 py-1 rounded-md font-medium">{score.score.tapNote.W2}EX</span>
              )}
              {score.score.tapNote.W3 > 0 && (
                <span className="text-emerald-200 bg-emerald-900/30 px-2 py-1 rounded-md font-medium">
                  {score.score.tapNote.W3}GR
                </span>
              )}
              {score.score.tapNote.W4 > 0 && (
                <span className="text-purple-200 bg-purple-900/30 px-2 py-1 rounded-md font-medium">{score.score.tapNote.W4}DE</span>
              )}
              {score.score.tapNote.W5 > 0 && (
                <span className="text-orange-200 bg-orange-900/30 px-2 py-1 rounded-md font-medium">
                  {score.score.tapNote.W5}WO
                </span>
              )}
              {score.score.tapNote.miss > 0 && (
                <span className="text-red-200 bg-red-900/30 px-2 py-1 rounded-md font-medium">
                  {score.score.tapNote.miss}MISS
                </span>
              )}
            </div>}
            <div className="w-full flex flex-row items-center gap-3">
              <FontAwesomeIcon icon={faHeart} className="text-slate-300 text-sm" />
              <div className="relative w-full h-3 rounded-full bg-slate-700/50 overflow-hidden shadow-inner">
                <div
                  className={`absolute top-0 left-0 h-full transition-all duration-300 ease-out rounded-full ${
                    score.score.life === 1
                      ? "bg-gradient-to-r from-emerald-500 to-emerald-400 shadow-sm"
                      : score.score.life < 0.2
                      ? "bg-gradient-to-r from-red-500 to-red-400 shadow-sm"
                      : "bg-gradient-to-r from-slate-500 to-slate-400 shadow-sm"
                  }`}
                  style={{ width: `${score.score.life * 100}%` }}
                ></div>
              </div>
              <span className="text-xs text-slate-400 font-medium min-w-[3rem]">
                {Math.round(score.score.life * 100)}%
              </span>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}
