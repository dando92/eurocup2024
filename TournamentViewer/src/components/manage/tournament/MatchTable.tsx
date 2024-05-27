import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { Match } from "../../../models/Match";
import { faTrash } from "@fortawesome/free-solid-svg-icons";

type MatchTableProps = {
  match: Match;
  onDeleteMatch: (matchId: number) => void;
};

export default function MatchTable({ match, onDeleteMatch }: MatchTableProps) {
  // Create a lookup table for scores and percentages
  const scoreTable: { [key: string]: { score: number; percentage: number } } =
    {};

  match.rounds.forEach((round) => {
    round.standings.forEach((standing) => {
      const key = `${standing.playerId}-${standing.songId}`;
      scoreTable[key] = {
        score: standing.score,
        percentage: standing.percentage,
      };
    });
  });

  return (
    <div className="flex flex-col w-full p-4 bg-gray-100 rounded-lg shadow-md">
      <div className="flex flex-row justify-center items-center">
        <h2 className="text-center text-4xl font-bold mb-6 text-blue-600">
          {match.name}
        </h2>
        <button
          onClick={() => onDeleteMatch(match.id)}
          className="ml-3 text-red-800 font-bold flex flex-row gap-2"
        >
          <FontAwesomeIcon icon={faTrash} />
        </button>
      </div>

      <div>
        <div
          className={`grid grid-cols-${
            match.songs.length + 2
          } w-full bg-blue-200 rounded-t-lg`}
          style={{
            gridTemplateColumns: `repeat(${match.songs.length + 2}, 1fr)`,
          }}
        >
          <div className="border border-blue-400 p-2">
            <div className="text-center font-bold text-blue-800"></div>
          </div>
          {match.songs.map((song, i) => (
            <div key={i} className="border border-blue-400 p-2">
              <div className="text-center font-bold text-blue-800">
                {song.title}
              </div>
            </div>
          ))}
          <div className="border border-blue-400 p-2">
            <div className="text-center font-bold text-blue-800">
              Total Points
            </div>
          </div>
        </div>

        {match.players.map((player, i) => (
          <div
            key={i}
            className={`grid grid-cols-${
              match.songs.length + 2
            } w-full odd:bg-white even:bg-gray-50`}
            style={{
              gridTemplateColumns: `repeat(${match.songs.length + 2}, 1fr)`,
            }}
          >
            <div className="border border-gray-300 p-2">
              <div className="text-center font-semibold text-gray-700">
                {player.name}
              </div>
            </div>
            {match.songs.map((song, j) => {
              const key = `${player.id}-${song.id}`;
              const scoreData = scoreTable[key];
              return (
                <div key={j} className="border border-gray-300 p-2">
                  <div className="text-center text-gray-600">
                    {scoreData
                      ? `${scoreData.score} (${scoreData.percentage}%)`
                      : "N/A"}
                  </div>
                </div>
              );
            })}
            <div className="border border-gray-300 p-2">
              <div className="text-center text-gray-600">
                Total Points for Player {i + 1}
              </div>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}
