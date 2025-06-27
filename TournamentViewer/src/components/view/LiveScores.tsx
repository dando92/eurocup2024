import {
  HttpTransportType,
  HubConnection,
  HubConnectionBuilder,
} from "@microsoft/signalr";
import { useEffect, useState, useMemo } from "react";
import { RawScore } from "../../models/RawScore";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faChartLine, faHeartbeat } from "@fortawesome/free-solid-svg-icons";
import { LineChart, Line, ResponsiveContainer, YAxis } from 'recharts';

export default function LiveScores() {
  const [, setScoreUpdateConnection] = useState<HubConnection | null>(null);
  const [scores, setScores] = useState<RawScore[]>([]);
  const [showJudgements, setShowJudgements] = useState(true);
  const [showHealthChart, setShowHealthChart] = useState(false);
  const [healthHistory, setHealthHistory] = useState<{ [playerName: string]: { time: number; life: number; score: number }[] }>({});
  const [, setCurrentSong] = useState<string | null>(null);

  // Color palette for chart lines
  const playerColors = ['#10b981', '#3b82f6', '#f59e0b', '#ef4444', '#8b5cf6', '#06b6d4', '#84cc16', '#f97316'];

  // Function to get mini chart data for a specific player
  const getPlayerChartData = (playerName: string) => {
    const history = healthHistory[playerName];
    if (!history || history.length < 2) return [];

    // Keep only last 30 seconds of data for the mini chart
    const now = Date.now();
    const recentHistory = history.filter(entry => (now - entry.time) <= 30000);

    if (recentHistory.length < 2) return [];

    // Use the actual timestamp, but create a relative time from the first entry
    const firstTime = recentHistory[0].time;
    return recentHistory.map((entry) => ({
      time: (entry.time - firstTime) / 1000, // Convert to seconds from start
      life: entry.life * 100
    }));
  };

  // Function to get health status description
  const getHealthStatus = (life: number) => {
    if (life >= 0.8) return { status: 'Excellent', color: 'text-green-400' };
    if (life >= 0.5) return { status: 'Good', color: 'text-blue-400' };
    if (life >= 0.2) return { status: 'Warning', color: 'text-orange-400' };
    return { status: 'Critical', color: 'text-red-400' };
  };

  useEffect(() => {
    const conn = new HubConnectionBuilder()
      .withUrl(`${import.meta.env.VITE_PUBLIC_API_URL}../scoreupdatehub`, {
        skipNegotiation: true,
        transport: HttpTransportType.WebSockets,
      })
      .build();

    conn.on("OnScoreUpdate", (msg: RawScore) => {
      // Check if song has changed - if so, reset everything
      const newSong = msg.score.song;
      setCurrentSong((prevSong) => {
        if (prevSong && prevSong !== newSong) {
          console.log(`Song changed from "${prevSong}" to "${newSong}" - resetting match data`);
          // Reset scores and health history for new match
          setScores([msg]);
          setHealthHistory({});
          return newSong;
        }
        return prevSong || newSong;
      });

      // Only update scores if it's the same song or first song
      setScores((prev) => {
        // If this is a song change, scores were already reset above
        const isSameSong = prev.length === 0 || prev[0].score.song === newSong;
        if (!isSameSong) {
          return [msg]; // Already handled in setCurrentSong, but ensure consistency
        }
        
        const newScores = prev.filter(
          (score) => score.score.playerName !== msg.score.playerName
        );
        return [...newScores, msg];
      });

      // Track health history for charts
      setHealthHistory((prev) => {
        const playerHistory = prev[msg.score.playerName] || [];
        const newEntry = {
          time: Date.now(),
          life: msg.score.life,
          score: parseFloat(msg.score.formattedScore)
        };

        // Only add if there's a significant change in health (> 1%) or if it's the first entry
        const lastEntry = playerHistory[playerHistory.length - 1];
        const shouldAdd = !lastEntry || Math.abs(lastEntry.life - newEntry.life) > 0.01;

        if (!shouldAdd) return prev;

        // Keep only last 50 entries to avoid memory issues
        const updatedHistory = [...playerHistory, newEntry].slice(-50);

        return {
          ...prev,
          [msg.score.playerName]: updatedHistory
        };
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

  // Clean up health history and current song when scores are cleared
  useEffect(() => {
    if (scores.length === 0) {
      setHealthHistory({});
      setCurrentSong(null);
    }
  }, [scores.length]);

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
        <div className="flex gap-2">
          <button
            onClick={() => setShowJudgements((prev) => !prev)}
            className="text-slate-100 bg-slate-600 hover:bg-slate-500 px-3 py-1.5 text-sm rounded-lg transition-all duration-200 shadow-sm"
          >
            {showJudgements ? "Hide" : "Show"} judgements
          </button>
          <button
            onClick={() => setShowHealthChart((prev) => !prev)}
            className={`text-slate-100 px-3 py-1.5 text-sm rounded-lg transition-all duration-200 shadow-sm flex items-center gap-2 ${showHealthChart
                ? "bg-blue-600 hover:bg-blue-500"
                : "bg-green-600 hover:bg-green-500"
              }`}
          >
            <FontAwesomeIcon icon={showHealthChart ? faChartLine : faHeartbeat} />
            {showHealthChart ? "Chart View" : "Bar View"}
          </button>
        </div>
      </div>

      <div className="grid my-4 gap-3 grid-cols-1 sm:grid-cols-2 lg:grid-cols-3">
        {sortedScores.map((score, idx) => {
          // Use stable position for animations, but current index for styling
          const currentPosition = idx + 1;
          const isFirstPlace = idx === 0;
          const firstPlaceScore = sortedScores[0] ? parseFloat(sortedScores[0].score.formattedScore) : 0;
          const currentScore = parseFloat(score.score.formattedScore);
          const scoreDifference = firstPlaceScore - currentScore;

          return (
            <div
              key={score.score.playerName} // Use stable key to prevent animation reset
              className={`score-card position-stable flex flex-col items-start p-4 rounded-xl shadow-lg hover:scale-105 border ${score.score.isFailed
                  ? "bg-gradient-to-br from-red-900/80 to-red-800/60 border-red-700/50"
                  : "bg-gradient-to-br from-slate-800/90 to-slate-700/80 border-slate-600/30"
                } ${isFirstPlace
                  ? "ring-2 ring-amber-400/50 bg-gradient-to-br from-amber-900/30 to-slate-800/90 text-amber-100 shadow-xl animate-first-place-stable"
                  : "text-slate-100"
                }`}
              style={{
                transform: `translateY(${idx * 2}px)`,
                zIndex: sortedScores.length - idx,
              }}
            >
              <div className="flex flex-row gap-6 justify-between items-end w-full mb-3">
                <div className="flex items-center gap-2">
                  <span className={`text-sm font-medium px-2 py-1 rounded-md ${isFirstPlace
                      ? "bg-amber-400/20 text-amber-200"
                      : "bg-slate-600/50 text-slate-300"
                    }`}>
                    #{currentPosition}
                  </span>
                  <span className="font-semibold text-lg">{score.score.playerName}</span>
                  {score.score.isFailed && (
                    <span className="text-xs px-2 py-1 rounded-md bg-red-900/60 text-red-200 border border-red-700/50 font-medium animate-pulse">
                      FAILED
                    </span>
                  )}
                  {!isFirstPlace && scoreDifference > 0 && (
                    <span className={`text-xs px-2 py-1 rounded-md border score-difference ${
                      scoreDifference > 10 
                        ? "bg-red-900/40 text-red-200 border-red-700/40" 
                        : scoreDifference > 5 
                        ? "bg-orange-900/40 text-orange-200 border-orange-700/40"
                        : "bg-yellow-900/40 text-yellow-200 border-yellow-700/40"
                    }`}>
                      -{scoreDifference.toFixed(2)}%
                    </span>
                  )}
                </div>

                <span className="font-bold text-xl tracking-wide">
                  {score.score.formattedScore}%
                </span>
              </div>

              {/* Health trend mini chart OR health bar */}
              {showHealthChart ? (
                getPlayerChartData(score.score.playerName).length > 1 && (
                  <div className="w-full mb-3">
                    <div className="flex justify-between items-center mb-1">
                      <span className="text-xs text-slate-400">Health</span>
                      <div className="flex items-center gap-2">
                        <span className={`text-xs font-medium ${getHealthStatus(score.score.life).color}`}>
                          {getHealthStatus(score.score.life).status}
                        </span>
                        <span className="text-xs text-slate-400">
                          {Math.round(score.score.life * 100)}%
                        </span>
                      </div>
                    </div>
                    <div className="h-8 w-full mini-chart-container rounded relative overflow-hidden">
                      {/* Background gradient and reference lines */}
                      <div className="absolute inset-0 bg-gradient-to-t from-red-900/20 via-orange-900/10 to-green-900/20"></div>
                      {/* Reference lines at 25%, 50%, 75% */}
                      <div className="absolute inset-0">
                        <div className="absolute bottom-1/4 left-0 right-0 h-px bg-slate-600/30"></div>
                        <div className="absolute bottom-1/2 left-0 right-0 h-px bg-slate-600/40"></div>
                        <div className="absolute bottom-3/4 left-0 right-0 h-px bg-slate-600/30"></div>
                      </div>
                      <ResponsiveContainer width="100%" height="100%">
                        <LineChart data={getPlayerChartData(score.score.playerName)} margin={{ top: 2, right: 2, left: 2, bottom: 2 }}>
                          <YAxis domain={[0, 100]} hide />
                          <Line
                            type="monotone"
                            dataKey="life"
                            stroke={isFirstPlace ? "#fbbf24" : playerColors[idx % playerColors.length]}
                            strokeWidth={2}
                            dot={false}
                            activeDot={{ r: 2, fill: isFirstPlace ? "#fbbf24" : playerColors[idx % playerColors.length] }}
                            connectNulls={false}
                            isAnimationActive={false}
                          />
                        </LineChart>
                      </ResponsiveContainer>
                    </div>
                  </div>
                )
              ) : (
                <div className="w-full mb-3">
                  <div className="flex justify-between items-center mb-1">
                    <span className="text-xs text-slate-400">Health Status</span>
                    <div className="flex items-center gap-2">
                      <span className={`text-xs font-medium ${getHealthStatus(score.score.life).color}`}>
                        {getHealthStatus(score.score.life).status}
                      </span>
                      <span className="text-xs text-slate-400">
                        {Math.round(score.score.life * 100)}%
                      </span>
                    </div>
                  </div>
                  <div className="relative w-full h-3 rounded-full bg-slate-700/50 overflow-hidden shadow-inner">
                    <div
                      className={`absolute top-0 left-0 h-full transition-all duration-300 ease-out rounded-full ${score.score.life === 1
                          ? "bg-gradient-to-r from-emerald-500 to-emerald-400 shadow-sm"
                          : score.score.life < 0.2
                            ? "bg-gradient-to-r from-red-500 to-red-400 shadow-sm"
                            : score.score.life >= 0.5
                              ? "bg-gradient-to-r from-blue-500 to-blue-400 shadow-sm"
                              : "bg-gradient-to-r from-orange-500 to-orange-400 shadow-sm"
                        }`}
                      style={{ width: `${score.score.life * 100}%` }}
                    ></div>
                  </div>
                </div>
              )}

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

            </div>
          );
        })}
      </div>
    </div>
  );
}
