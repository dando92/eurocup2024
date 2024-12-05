import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { Match } from "../../../models/Match";
import {
  faBan,
  faCircle,
  faInfoCircle,
  faPencil,
  faPlay,
  faPlus,
  faRefresh,
  faStickyNote,
  faTrash,
} from "@fortawesome/free-solid-svg-icons";
import { Division } from "../../../models/Division";
import { Phase } from "../../../models/Phase";
import AddEditSongToMatchModal from "./modals/AddEditSongToMatchModal";
import { useEffect, useState } from "react";
import AddStandingToMatchModal from "./modals/AddStandingToMatchModal";
import {
  HttpTransportType,
  HubConnection,
  HubConnectionBuilder,
} from "@microsoft/signalr";
import { toast } from "react-toastify";
import EditMatchNotesModal from "./modals/EditMatchNotesModal";
import { Log } from "../../../models/Log";
import LogViewer from "../../layout/LogViewer";
import { Tab } from "@headlessui/react";
import { classNames } from "../../../pages/ManagePage";

type MatchTableProps = {
  division: Division;
  phase: Phase;
  match: Match;
  isActive: boolean;
  controls?: boolean;
  onGetActiveMatch: () => void;
  onSetActiveMatch: (
    divisionId: number,
    phaseId: number,
    matchId: number,
  ) => void;
  onDeleteMatch: (matchId: number) => void;
  onAddSongToMatchByRoll: (
    divisionId: number,
    phaseId: number,
    matchId: number,
    group: string,
    level: string,
  ) => void;
  onAddSongToMatchBySongId: (
    divisionId: number,
    phaseId: number,
    matchId: number,
    songId: number,
  ) => void;
  onEditSongToMatchByRoll: (
    divisionId: number,
    phaseId: number,
    matchId: number,
    group: string,
    level: string,
    editSongId: number,
  ) => void;
  onEditSongToMatchBySongId: (
    divisionId: number,
    phaseId: number,
    matchId: number,
    songId: number,
    editSongId: number,
  ) => void;
  onAddStandingToMatch: (
    playerId: number,
    songId: number,
    percentage: number,
    score: number,
    isFailed: boolean,
  ) => void;
  onEditMatchNotes: (matchId: number, notes: string) => void;
  onEditStanding: (
    playerId: number,
    songId: number,
    percentage: number,
    score: number,
    isFailed: boolean,
  ) => void;
  onDeleteStanding: (playerId: number, songId: number) => void;
};

export default function MatchTable({
  division,
  phase,
  match,
  isActive,
  controls = false,
  onGetActiveMatch,
  onDeleteMatch,
  onSetActiveMatch,
  onAddSongToMatchByRoll,
  onAddSongToMatchBySongId,
  onEditSongToMatchByRoll,
  onEditSongToMatchBySongId,
  onAddStandingToMatch,
  onEditMatchNotes,
  onDeleteStanding,
}: MatchTableProps) {
  // Create a lookup table for scores and percentages
  const scoreTable: {
    [key: string]: { score: number; percentage: number; isFailed: boolean };
  } = {};

  const [logs, setLogs] = useState<Log[]>([]);
  const [addSongToMatchModalOpen, setAddSongToMatchModalOpen] = useState(false);
  const [editSongId, setEditSongId] = useState<number | null>(null);

  const [addStandingToMatchModalOpen, setAddStandingToMatchModalOpen] =
    useState(false);

  const [EditMatchNotesModalOpen, setEditMatchNotesModalOpen] = useState(false);

  const [songIdPlayerId, setSongIdPlayerId] = useState<{
    playerId: number;
    playerName: string;
    songId: number;
    songTitle: string;
  }>({ songId: 0, playerId: 0, playerName: "", songTitle: "" });

  const [scoreConnection, setScoreConnection] = useState<null | HubConnection>(
    null,
  );
  const [errorConnection, setErrorConnection] = useState<null | HubConnection>(
    null,
  );

  match.rounds.forEach((round) => {
    round.standings.forEach((standing) => {
      const key = `${standing.playerId}-${standing.songId}`;
      scoreTable[key] = {
        score: standing.score,
        percentage: standing.percentage,
        isFailed: standing.isFailed,
      };
    });
  });

  useEffect(() => {
    if (scoreConnection === null && isActive) {
      const conn = new HubConnectionBuilder()
        .withUrl(`${import.meta.env.VITE_PUBLIC_API_URL}../matchupdatehub`, {
          skipNegotiation: true,
          transport: HttpTransportType.WebSockets,
        })
        .build();

      conn.on("OnMatchUpdate", () => {
        onGetActiveMatch();
      });

      conn.start().then(() => {
        console.log("Now listening to match changes.");
        toast.info("Now listening to match changes.");
      });

      if (!errorConnection && controls) {
        const errConn = new HubConnectionBuilder()
          .withUrl(`${import.meta.env.VITE_PUBLIC_API_URL}../logupdatehub`, {
            skipNegotiation: true,
            transport: HttpTransportType.WebSockets,
          })
          .build();

        errConn.on("OnLogUpdate", ({ message, error }: Log) => {
          console.log(message, error);

          error &&
            toast.error(`Error: ${message} - ${error}`, {
              autoClose: false,
            });

          setLogs((prevLogs) => [
            ...prevLogs,
            { message, error: error, timestamp: new Date().toISOString() },
          ]);
        });

        errConn.start().then(() => {
          console.log("Now listening to log changes.");
          toast.info("Now listening to log changes.");
        });

        setErrorConnection(errConn);
      }

      setScoreConnection(conn);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [isActive, scoreConnection, errorConnection]);

  // Calculate total points for each player
  const getTotalPoints = (playerId: number) => {
    return match.rounds
      .map((round) => round.standings.find((s) => s.playerId === playerId))
      .reduce((acc, standing) => {
        if (standing) {
          return acc + standing.score;
        }
        return acc;
      }, 0);
  };

  // Sort players by total points
  const sortedPlayers = [...match.players].sort(
    (a, b) => getTotalPoints(b.id) - getTotalPoints(a.id),
  );

  return (
    <div className="flex flex-col w-full p-4 my-3 rounded-lg">
      <div className="flex flex-row mb-6 justify-center items-center">
        <div>
          <h2 className="text-center text-4xl font-bold text-rossoTesto">
            <div className="flex flex-row justify-center items-center gap-3">
              {isActive && (
                <FontAwesomeIcon
                  icon={faCircle}
                  className="text-green-200 text-xs animate-pulse"
                />
              )}
              <span className="text-xl">{match.name}</span>
              {controls && (
                <button
                  className="text-lg"
                  title={match.notes ? match.notes : "Add notes"}
                  onClick={() => setEditMatchNotesModalOpen(true)}
                >
                  <FontAwesomeIcon icon={faStickyNote} />
                </button>
              )}
            </div>
          </h2>
          {match.subtitle && (
            <p className="text-sm font-normal text-rossoTesto flex flex-row items-center gap-1">
              <FontAwesomeIcon icon={faInfoCircle} />
              {match.subtitle}
            </p>
          )}
        </div>
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
        <AddStandingToMatchModal
          open={addStandingToMatchModalOpen}
          playerId={songIdPlayerId.playerId}
          isManualMatch={match.isManualMatch}
          songId={songIdPlayerId.songId}
          playerName={songIdPlayerId.playerName}
          songTitle={songIdPlayerId.songTitle}
          onClose={() => {
            setAddStandingToMatchModalOpen(false);
            setSongIdPlayerId({
              playerId: 0,
              songId: 0,
              playerName: "",
              songTitle: "",
            });
          }}
          onAddStandingToMatch={onAddStandingToMatch}
        />
        <EditMatchNotesModal
          match={match}
          open={EditMatchNotesModalOpen}
          onClose={() => setEditMatchNotesModalOpen(false)}
          onSave={onEditMatchNotes}
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
      <div className="flex flex-col gap-3">
        <Tab.Group>
          <Tab.List className="flex flex-row gap-10 border-b mt-5">
            <Tab
              className={({ selected }) =>
                classNames(
                  "py-2 px-4 text-lg",
                  selected
                    ? "border-b-2 border-blue-500 font-bold text-blue-500"
                    : "text-gray-500",
                )
              }
            >
              Match
            </Tab>
            {controls && (
              <Tab
                className={({ selected }) =>
                  classNames(
                    "py-2 px-4 text-lg",
                    selected
                      ? "border-b-2 border-blue-500 font-bold text-blue-500"
                      : "text-gray-500",
                  )
                }
              >
                Errors & Logs
              </Tab>
            )}
          </Tab.List>
          <Tab.Panels className="mt-3">
            <Tab.Panel>
              <div
                style={{ minWidth: match.songs.length * 200 }}
                className={`shadow-lg overflow-auto lg:min-w-fit`}
              >
                <div
                  className={`grid grid-cols-${
                    match.songs.length + 2
                  } w-full bg-rossoTesto rounded-t-lg`}
                  style={{
                    gridTemplateColumns: `repeat(${
                      match.songs.length + 2
                    }, 1fr)`,
                  }}
                >
                  <div className=" border-rossoTag p-2">
                    <div className="text-center font-bold text-rossoTag"></div>
                  </div>
                  {match.songs.map((song, i) => (
                    <div key={i} className="border-x border-rossoTag p-2">
                      <div className="text-center font-bold text-blue-100">
                        {song.title}
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
                  <div className=" p-2">
                    <div className="text-center font-bold text-blue-100">
                      Total Points
                    </div>
                  </div>
                </div>

                {sortedPlayers.map((player, i) => (
                  <div
                    key={i}
                    className={`grid grid-cols-${
                      match.songs.length + 2
                    } w-full odd:bg-white even:bg-gray-50`}
                    style={{
                      gridTemplateColumns: `repeat(${
                        match.songs.length + 2
                      }, 1fr)`,
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

                      const playerDisabled =
                        scoreData?.isFailed && scoreData?.percentage === -1;

                      if (playerDisabled) {
                        return (
                          <div
                            className={
                              "flex flex-row gap-3 items-center justify-center bg-gray-400"
                            }
                          >
                            <p className={"font-bold"}>Disabled</p>
                            {controls && (
                              <button
                                className={"underline font-red-800 text-xs"}
                                onClick={() =>
                                  onDeleteStanding(player.id, song.id)
                                }
                              >
                                Reactivate
                              </button>
                            )}
                          </div>
                        );
                      }

                      return (
                        <div key={j} className="border border-gray-300 p-2">
                          <div className="text-center  justify-center flex flex-row gap-3 items-center text-gray-600">
                            <p
                              className={`${
                                scoreData?.isFailed
                                  ? "text-red-500 font-bold"
                                  : "text-black"
                              }`}
                            >
                              {scoreData && !playerDisabled
                                ? `${scoreData.score} (${
                                    scoreData.percentage
                                  }%) ${scoreData.isFailed ? "F" : ""}`
                                : "-"}
                              {controls && isActive && scoreData && (
                                <>
                                  <button
                                    title="Edit standing manually"
                                    className="text-xs ml-3 text-blu"
                                  >
                                    <FontAwesomeIcon icon={faPencil} />
                                  </button>
                                  <button
                                    title="Delete this standing"
                                    className="text-xs ml-3 text-red-500"
                                    onClick={() => {
                                      if (
                                        window.confirm(
                                          "Are you sure you want to delete this standing?",
                                        )
                                      ) {
                                        onDeleteStanding(player.id, song.id);
                                      }
                                    }}
                                  >
                                    <FontAwesomeIcon icon={faTrash} />
                                  </button>
                                </>
                              )}
                            </p>

                            {!scoreData && controls && isActive && (
                              <>
                                <button
                                  title="Manually add score"
                                  className="text-green-700"
                                  onClick={() => {
                                    setAddStandingToMatchModalOpen(true);
                                    setSongIdPlayerId({
                                      playerId: player.id,
                                      songId: song.id,
                                      playerName: player.name,
                                      songTitle: song.title,
                                    });
                                  }}
                                >
                                  <FontAwesomeIcon icon={faPlus} />
                                </button>
                                <button
                                  title="Disable player for this round"
                                  className="text-xs ml-3 text-red-500"
                                  onClick={() => {
                                    onAddStandingToMatch(
                                      player.id,
                                      song.id,
                                      -1,
                                      0,
                                      true,
                                    );
                                  }}
                                >
                                  <FontAwesomeIcon icon={faBan} />
                                </button>
                              </>
                            )}
                          </div>
                        </div>
                      );
                    })}
                    <div className="border border-gray-300 p-2">
                      <div className="text-center text-gray-600">
                        {match.rounds
                          .map((round) =>
                            round.standings.find(
                              (s) => s.playerId === player.id,
                            ),
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
            </Tab.Panel>
            <Tab.Panel>
              {controls && (
                <div>
                  <LogViewer logs={logs} />
                </div>
              )}
            </Tab.Panel>
          </Tab.Panels>
        </Tab.Group>
      </div>
    </div>
  );
}
