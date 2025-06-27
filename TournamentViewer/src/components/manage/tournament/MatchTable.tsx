import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { Match } from "../../../models/Match";
import {
  faCircle,
  faInfoCircle,
  faPencil,
  faPlay,
  faPlus,
  faRefresh,
  faStickyNote,
  faTrash,
  faTrophy,
  faMedal,
  faAward,
  faBars,
  faTable,
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
  onAddStandingToMatch: (
    playerId: number,
    songId: number,
    percentage: number,
    score: number,
    isFailed: boolean
  ) => void;
  onEditMatchNotes: (matchId: number, notes: string) => void;
  onEditStanding: (
    playerId: number,
    songId: number,
    percentage: number,
    score: number,
    isFailed: boolean
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
  const [isMobileView, setIsMobileView] = useState(false);
  const [viewMode, setViewMode] = useState<'table' | 'cards'>('table');

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
    null
  );
  const [errorConnection, setErrorConnection] = useState<null | HubConnection>(
    null
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
    const checkMobileView = () => {
      setIsMobileView(window.innerWidth < 768);
      if (window.innerWidth < 768) {
        setViewMode('cards');
      } else {
        setViewMode('table');
      }
    };

    checkMobileView();
    window.addEventListener('resize', checkMobileView);
    return () => window.removeEventListener('resize', checkMobileView);
  }, []);

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

          error && toast.error(`Error: ${message} - ${error}`, {
            autoClose: false,
          }) 

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
    (a, b) => getTotalPoints(b.id) - getTotalPoints(a.id)
  );

  // Get player rank and badge
  const getPlayerRank = (playerId: number) => {
    const playerIndex = sortedPlayers.findIndex(p => p.id === playerId);
    return playerIndex + 1;
  };

  const getRankIcon = (rank: number) => {
    switch (rank) {
      case 1: return { icon: faTrophy, color: 'text-yellow-500' };
      case 2: return { icon: faMedal, color: 'text-gray-400' };
      case 3: return { icon: faAward, color: 'text-amber-600' };
      default: return null;
    }
  };

  // Mobile Card Component
  const PlayerCard = ({ player, rank }: { player: any; rank: number }) => {
    const rankInfo = getRankIcon(rank);
    const totalPoints = getTotalPoints(player.id);

    return (
      <div className={`bg-white rounded-xl shadow-lg p-4 mb-4 border-l-4 transition-all duration-300 hover:shadow-xl ${
        rank === 1 ? 'border-yellow-500 bg-gradient-to-r from-yellow-50 to-white' :
        rank === 2 ? 'border-gray-400 bg-gradient-to-r from-gray-50 to-white' :
        rank === 3 ? 'border-amber-600 bg-gradient-to-r from-amber-50 to-white' :
        'border-blue-500'
      }`}>
        <div className="flex items-center justify-between mb-3">
          <div className="flex items-center gap-3">
            {rankInfo && (
              <FontAwesomeIcon icon={rankInfo.icon} className={`${rankInfo.color} text-xl`} />
            )}
            <div>
              <h3 className="font-bold text-lg text-gray-800">{player.name}</h3>
              <p className="text-sm text-gray-500">Rank #{rank}</p>
            </div>
          </div>
          <div className="text-right">
            <div className="text-2xl font-bold text-blue-600">{totalPoints}</div>
            <div className="text-xs text-gray-500">total points</div>
          </div>
        </div>
        
        <div className="space-y-2">
          {match.songs.map((song, j) => {
            const key = `${player.id}-${song.id}`;
            const scoreData = scoreTable[key];
            return (
              <div key={j} className="flex items-center justify-between p-3 bg-gray-50 rounded-lg">
                <div className="flex-1">
                  <div className="font-medium text-sm text-gray-700 truncate">{song.title}</div>
                  {controls && isActive && (
                    <button
                      onClick={() => {
                        setEditSongId(song.id);
                        setAddSongToMatchModalOpen(true);
                      }}
                      className="text-xs text-blue-500 mt-1"
                      title="Change round song"
                    >
                      <FontAwesomeIcon icon={faRefresh} className="mr-1" />
                      Change song
                    </button>
                  )}
                </div>
                <div className="flex items-center gap-2">
                  {scoreData ? (
                    <div className="text-right">
                      <div className={`font-bold ${scoreData.isFailed ? 'text-red-500' : 'text-green-600'}`}>
                        {scoreData.score}
                      </div>
                      <div className="text-xs text-gray-500">
                        {scoreData.percentage}%{scoreData.isFailed ? ' F' : ''}
                      </div>
                      {controls && isActive && (
                        <div className="flex gap-1 mt-1">
                          <button
                            title="Edit standing manually"
                            className="text-xs text-blue-500"
                          >
                            <FontAwesomeIcon icon={faPencil} />
                          </button>
                          <button
                            title="Delete this standing"
                            className="text-xs text-red-500"
                            onClick={() => {
                              if (window.confirm("Are you sure you want to delete this standing?")) {
                                onDeleteStanding(player.id, song.id);
                              }
                            }}
                          >
                            <FontAwesomeIcon icon={faTrash} />
                          </button>
                        </div>
                      )}
                    </div>
                  ) : (
                    <div className="text-center">
                      <div className="text-gray-400 text-sm">-</div>
                      {controls && isActive && (
                        <button
                          title="Manually add score"
                          className="text-green-600 text-sm mt-1"
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
                      )}
                    </div>
                  )}
                </div>
              </div>
            );
          })}
        </div>
      </div>
    );
  };

  return (
    <div className="flex flex-col w-full p-2 md:p-4 my-3 rounded-lg">
      <div className="flex flex-col md:flex-row mb-6 justify-center items-center gap-4">
        <div className="text-center md:text-left">
          <h2 className="text-center text-2xl md:text-4xl font-bold text-blue-600">
            <div className="flex flex-row justify-center items-center gap-3">
              {isActive && (
                <FontAwesomeIcon
                  icon={faCircle}
                  className="text-red-800 text-xs animate-pulse"
                />
              )}
              <span className="text-xl md:text-xl">{match.name}</span>
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
            <p className="text-sm font-normal text-gray-600 flex flex-row items-center gap-1 justify-center md:justify-start mt-2">
              <FontAwesomeIcon icon={faInfoCircle} />
              {match.subtitle}
            </p>
          )}
        </div>
        
        {/* View Toggle for larger screens */}
        <div className="md:hidden flex gap-2">
          <button
            onClick={() => setViewMode('cards')}
            className={`px-3 py-2 rounded-lg transition-colors ${
              viewMode === 'cards' 
                ? 'bg-blue-500 text-white' 
                : 'bg-gray-200 text-gray-700'
            }`}
          >
            <FontAwesomeIcon icon={faBars} className="mr-2" />
            Cards
          </button>
          <button
            onClick={() => setViewMode('table')}
            className={`px-3 py-2 rounded-lg transition-colors ${
              viewMode === 'table' 
                ? 'bg-blue-500 text-white' 
                : 'bg-gray-200 text-gray-700'
            }`}
          >
            <FontAwesomeIcon icon={faTable} className="mr-2" />
            Table
          </button>
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
          <div className="bg-gray-100 rounded-xl p-3 flex flex-row gap-3 shadow-md">
            {!isActive && (
              <button
                onClick={() =>
                  onSetActiveMatch(division.id, phase.id, match.id)
                }
                title="Set as active match"
                className="bg-green-100 hover:bg-green-200 text-green-800 font-bold flex flex-row gap-2 px-3 py-2 rounded-lg transition-colors"
              >
                <FontAwesomeIcon icon={faPlay} />
                <span className="hidden sm:inline">Activate</span>
              </button>
            )}
            {isActive && (
              <button
                title="Add a new round/song to the match"
                onClick={() => {
                  setAddSongToMatchModalOpen(true);
                }}
                className="bg-green-100 hover:bg-green-200 text-green-800 font-bold flex flex-row gap-2 px-3 py-2 rounded-lg transition-colors"
              >
                <FontAwesomeIcon icon={faPlus} />
                <span className="hidden sm:inline">Add Round</span>
              </button>
            )}
            <button
              onClick={() => onDeleteMatch(match.id)}
              className="bg-red-100 hover:bg-red-200 text-red-800 font-bold flex flex-row gap-2 px-3 py-2 rounded-lg transition-colors"
            >
              <FontAwesomeIcon icon={faTrash} />
              <span className="hidden sm:inline">Delete</span>
            </button>
          </div>
        )}
      </div>
      
      <div className="flex flex-col gap-3">
        <Tab.Group>
          <Tab.List className="flex flex-row gap-4 md:gap-10 border-b mt-5 overflow-x-auto">
            <Tab
              className={({ selected }) =>
                classNames(
                  "py-2 px-4 text-lg whitespace-nowrap",
                  selected
                    ? "border-b-2 border-blue-500 font-bold text-blue-500"
                    : "text-gray-500"
                )
              }
            >
              Match Results
            </Tab>
            {controls && <Tab
              className={({ selected }) =>
                classNames(
                  "py-2 px-4 text-lg whitespace-nowrap",
                  selected
                    ? "border-b-2 border-blue-500 font-bold text-blue-500"
                    : "text-gray-500"
                )
              }
            >
              Errors & Logs
            </Tab>}
          </Tab.List>
          <Tab.Panels className="mt-3">
            <Tab.Panel>
              {/* Mobile Card View */}
              {viewMode === 'cards' ? (
                <div className="space-y-4">
                  <div className="text-center mb-6">
                    <h3 className="text-lg font-semibold text-gray-700 mb-2">Tournament Standings</h3>
                    <div className="text-sm text-gray-500">
                      {match.songs.length} round{match.songs.length !== 1 ? 's' : ''} • {match.players.length} player{match.players.length !== 1 ? 's' : ''}
                    </div>
                  </div>
                  {sortedPlayers.map((player, index) => (
                    <PlayerCard key={player.id} player={player} rank={index + 1} />
                  ))}
                </div>
              ) : (
                /* Desktop Table View */
                <div className="bg-white rounded-lg shadow-lg overflow-hidden">
                  <div className="overflow-x-auto">
                    <div 
                      className="min-w-full"
                      style={{ minWidth: `${(match.songs.length + 2) * 150}px` }}
                    >
                      {/* Table Header */}
                      <div
                        className="grid bg-gradient-to-r from-blue-600 to-blue-700 text-white"
                        style={{
                          gridTemplateColumns: `200px repeat(${match.songs.length}, 1fr) 120px`,
                        }}
                      >
                        <div className="p-4">
                          <div className="font-bold text-lg">Player</div>
                        </div>
                        {match.songs.map((song, i) => (
                          <div key={i} className="border-x border-blue-500 p-4">
                            <div className="text-center font-bold">
                              <div className="truncate">{song.title}</div>
                              {controls && isActive && (
                                <button
                                  onClick={() => {
                                    setEditSongId(song.id);
                                    setAddSongToMatchModalOpen(true);
                                  }}
                                  className="mt-2 text-blue-200 hover:text-white transition-colors"
                                  title="Change round song"
                                >
                                  <FontAwesomeIcon icon={faRefresh} />
                                </button>
                              )}
                            </div>
                          </div>
                        ))}
                        <div className="p-4">
                          <div className="text-center font-bold">
                            <FontAwesomeIcon icon={faTrophy} className="mr-2" />
                            Total
                          </div>
                        </div>
                      </div>

                      {/* Table Body */}
                      {sortedPlayers.map((player, i) => {
                        const rank = i + 1;
                        const rankInfo = getRankIcon(rank);
                        return (
                          <div
                            key={i}
                            className={`grid transition-colors hover:bg-blue-50 ${
                              i % 2 === 0 ? 'bg-gray-50' : 'bg-white'
                            } ${rank <= 3 ? 'border-l-4' : ''} ${
                              rank === 1 ? 'border-yellow-500' :
                              rank === 2 ? 'border-gray-400' :
                              rank === 3 ? 'border-amber-600' : ''
                            }`}
                            style={{
                              gridTemplateColumns: `200px repeat(${match.songs.length}, 1fr) 120px`,
                            }}
                          >
                            <div className="border border-gray-200 p-4">
                              <div className="flex items-center gap-3">
                                {rankInfo && (
                                  <FontAwesomeIcon 
                                    icon={rankInfo.icon} 
                                    className={`${rankInfo.color} text-lg`} 
                                  />
                                )}
                                <div>
                                  <div className="font-semibold text-gray-800">{player.name}</div>
                                  <div className="text-xs text-gray-500">Rank #{rank}</div>
                                </div>
                              </div>
                            </div>
                            {match.songs.map((song, j) => {
                              const key = `${player.id}-${song.id}`;
                              const scoreData = scoreTable[key];
                              return (
                                <div key={j} className="border border-gray-200 p-4">
                                  <div className="text-center flex flex-col gap-2 items-center text-gray-600">
                                    {scoreData ? (
                                      <div className="w-full">
                                        <div className={`text-lg font-bold ${
                                          scoreData?.isFailed
                                            ? "text-red-500"
                                            : "text-green-600"
                                        }`}>
                                          {scoreData.score}
                                        </div>
                                        <div className="text-xs text-gray-500">
                                          {scoreData.percentage}%{scoreData.isFailed ? " F" : ""}
                                        </div>
                                        {controls && isActive && (
                                          <div className="flex gap-1 justify-center mt-2">
                                            <button
                                              title="Edit standing manually"
                                              className="text-xs text-blue-500 hover:text-blue-700 transition-colors"
                                            >
                                              <FontAwesomeIcon icon={faPencil} />
                                            </button>
                                            <button
                                              title="Delete this standing"
                                              className="text-xs text-red-500 hover:text-red-700 transition-colors"
                                              onClick={() => {
                                                if (
                                                  window.confirm(
                                                    "Are you sure you want to delete this standing?"
                                                  )
                                                ) {
                                                  onDeleteStanding(player.id, song.id);
                                                }
                                              }}
                                            >
                                              <FontAwesomeIcon icon={faTrash} />
                                            </button>
                                          </div>
                                        )}
                                      </div>
                                    ) : (
                                      <div className="w-full">
                                        <div className="text-gray-400 text-lg">-</div>
                                        {controls && isActive && (
                                          <button
                                            title="Manually add score"
                                            className="text-green-600 hover:text-green-700 transition-colors mt-2"
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
                                        )}
                                      </div>
                                    )}
                                  </div>
                                </div>
                              );
                            })}
                            <div className="border border-gray-200 p-4">
                              <div className="text-center">
                                <div className="text-xl font-bold text-blue-600">
                                  {getTotalPoints(player.id)}
                                </div>
                                <div className="text-xs text-gray-500">points</div>
                              </div>
                            </div>
                          </div>
                        );
                      })}
                    </div>
                  </div>
                </div>
              )}
            </Tab.Panel>
            <Tab.Panel>
              {controls && (
                <div className="bg-white rounded-lg shadow-lg p-4">
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
