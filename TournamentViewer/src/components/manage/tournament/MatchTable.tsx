import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { Match } from "../../../models/Match";
import {
  faPlay,
  faPlus,
  faRefresh,
  faTrash,
} from "@fortawesome/free-solid-svg-icons";
import { Division } from "../../../models/Division";
import { Phase } from "../../../models/Phase";
import AddEditSongToMatchModal from "./modals/AddEditSongToMatchModal";
import { useState } from "react";

type MatchTableProps = {
  division: Division;
  phase: Phase;
  match: Match;
  isActive: boolean;
  controls?: boolean;
  onSetActiveMatch: (
    divisionId: number,
    phaseId: number,
    matchId: number
  ) => void;
  onDeleteMatch: (matchId: number) => void;
  onAddSongToMatchByRoll: (
    divisionId: number,
    phaseId: number,
    matchId: number,
    group: string,
    level: string
  ) => void;
  onAddSongToMatchBySongId: (
    divisionId: number,
    phaseId: number,
    matchId: number,
    songId: number
  ) => void;
  onEditSongToMatchByRoll: (
    divisionId: number,
    phaseId: number,
    matchId: number,
    group: string,
    level: string,
    editSongId: number
  ) => void;
  onEditSongToMatchBySongId: (
    divisionId: number,
    phaseId: number,
    matchId: number,
    songId: number,
    editSongId: number
  ) => void;
};

export default function MatchTable({
  division,
  phase,
  match,
  isActive,
  controls = false,
  onDeleteMatch,
  onSetActiveMatch,
  onAddSongToMatchByRoll,
  onAddSongToMatchBySongId,
  onEditSongToMatchByRoll,
  onEditSongToMatchBySongId,
}: MatchTableProps) {
  // Create a lookup table for scores and percentages
  const scoreTable: { [key: string]: { score: number; percentage: number } } =
    {};

  const [addSongToMatchModalOpen, setAddSongToMatchModalOpen] = useState(false);
  const [editSongId, setEditSongId] = useState<number | null>(null);

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
    <div className="flex flex-col w-full p-4 my-3 bg-gray-200 rounded-lg shadow-md">
      <div className="flex flex-row mb-6 justify-center items-center">
        <h2 className="text-center text-4xl font-bold text-blue-600">
          &nbsp;{match.name}
        </h2>
        <AddEditSongToMatchModal
          songId={editSongId}
          phaseId={phase.id}
          matchId={match.id}
          divisionId={division.id}
          open={addSongToMatchModalOpen}
          onAddSongToMatchByRoll={onAddSongToMatchByRoll}
          onAddSongToMatchBySongId={onAddSongToMatchBySongId}
          onEditSongToMatchByRoll={onEditSongToMatchByRoll}
          onEditSongToMatchBySongId={onEditSongToMatchBySongId}
          onClose={() => {
            setAddSongToMatchModalOpen(false);
            setEditSongId(null);
          }}
        />
        {controls && (
          <div className="ml-3 bg-gray-300 rounded-xl p-3 flex flex-row gap-3">
            {!isActive && (
              <button
                onClick={() =>
                  onSetActiveMatch(division.id, phase.id, match.id)
                }
                title="Set as active match"
                className="text-green-800 font-bold flex flex-row gap-2"
              >
                <FontAwesomeIcon icon={faPlay} />
              </button>
            )}
            {isActive && (
              <button
                title="Add a new round/song to the match"
                onClick={() => {
                  setAddSongToMatchModalOpen(true);
                }}
                className=" text-green-800 font-bold flex flex-row gap-2"
              >
                <FontAwesomeIcon icon={faPlus} />
              </button>
            )}
            <button
              onClick={() => onDeleteMatch(match.id)}
              className="ml-3 text-red-800 font-bold flex flex-row gap-2"
            >
              <FontAwesomeIcon icon={faTrash} />
            </button>
          </div>
        )}
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
                {song.title}{" "}
                {controls && isActive && (
                  <>
                    <button
                      onClick={() => {
                        setEditSongId(song.id);
                        setAddSongToMatchModalOpen(true);
                      }}
                      className="ml-3"
                      title="Change round song"
                    >
                      <FontAwesomeIcon icon={faRefresh} />
                    </button>
                  </>
                )}
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
                  <div className="text-center  justify-center flex flex-row gap-3 items-center text-gray-600">
                    {scoreData
                      ? `${scoreData.score} (${scoreData.percentage}%)`
                      : "-"}

                    {!scoreData && controls && isActive && (
                      <button
                        title="Manually add score"
                        className="text-green-700"
                      >
                        <FontAwesomeIcon icon={faPlus} />
                      </button>
                    )}
                  </div>
                </div>
              );
            })}
            <div className="border border-gray-300 p-2">
              <div className="text-center text-gray-600">
                {match.rounds
                  .map((round) =>
                    round.standings.find((s) => s.playerId === player.id)
                  )
                  .reduce((acc, standing) => {
                    if (standing) {
                      return acc + standing.score;
                    }
                    return acc;
                  }, 0)}
              </div>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}
