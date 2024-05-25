import { useEffect, useState } from "react";
import { Player } from "../../../models/Player";
import axios from "axios";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faPlus, faTrash } from "@fortawesome/free-solid-svg-icons";

export default function PlayersList() {
  const [players, setPlayers] = useState<Player[]>([]);
  const [selectedPlayerId, setSelectedPlayerId] = useState<number>(-1);

  const [search, setSearch] = useState<string>("");

  useEffect(() => {
    axios.get<Player[]>("players").then((response) => {
      setPlayers(response.data.sort((a, b) => a.name.localeCompare(b.name)));
    });
  }, []);

  const getSelectedPlayer = () => {
    return players.find((p) => p.id === selectedPlayerId);
  };

  const createPlayer = () => {
    const name = prompt("Enter player name");

    if (name) {
      axios.post<Player>("players", { name }).then((response) => {
        setPlayers([...players, response.data]);
      });
    }
  };

  const deletePlayer = (id: number) => {
    if (window.confirm("Are you sure you want to delete this player?")) {
      axios.delete(`players/${id}`).then(() => {
        setPlayers(players.filter((p) => p.id !== id));
        setSelectedPlayerId(-1);
      });
    }
  };

  return (
    <div>
      <div className="flex flex-col justify-start gap-3">
        <div className="flex flex-row gap-3">
          <h2>Players List</h2>
          <button
            onClick={createPlayer}
            title="Add new player"
            className="w-4 text-green-700"
          >
            <FontAwesomeIcon icon={faPlus} />
          </button>
        </div>
        <div className="flex flex-row gap-5">
          <div className="bg-gray-100 w-[200px] h-[400px] overflow-auto">
            <input
              className="p-1 w-full border-blu border outline-none"
              type="search"
              placeholder="Search player..."
              value={search}
              onChange={(e) => setSearch(e.target.value)}
            />
            {players
              .filter((p) =>
                search.length < 0
                  ? true
                  : p.name.toLowerCase().includes(search.toLowerCase())
              )
              .map((player) => {
                return (
                  <div
                    key={player.id}
                    role="button"
                    onClick={() => setSelectedPlayerId(player.id)}
                    className={`${
                      selectedPlayerId === player.id
                        ? "bg-middle text-white"
                        : "hover:bg-lower hover:text-white"
                    } cursor-pointer py-2 px-3 flex justify-between items-center gap-3 `}
                  >
                    <span>{player.name}</span>
                    <button
                      onClick={(e) => {
                        e.stopPropagation();
                        deletePlayer(player.id);
                      }}
                      className="text-sm"
                    >
                      <FontAwesomeIcon icon={faTrash} />
                    </button>
                  </div>
                );
              })}
            {search.length > 0 &&
              players.filter((p) =>
                p.name.toLowerCase().includes(search.toLowerCase())
              ).length === 0 && (
                <div className="text-center py-2 text-red-500">
                  No player found
                </div>
              )}
          </div>
          <div>
            {selectedPlayerId < 0 && (
              <div>Select a player from the list to view informations.</div>
            )}
            {selectedPlayerId >= 0 && (
              <PlayerItem player={getSelectedPlayer() as Player} />
            )}
          </div>
        </div>
      </div>
    </div>
  );
}

function PlayerItem({ player }: { player: Player }) {
  return (
    <div>
      <h3 className="text-2xl">Player Information</h3>
      <div>
        <span>Name: </span>
        <span>{player.name}</span>
      </div>
      <h3 className="mt-3">Player Scores</h3>
      <p>No scores on record for this player.</p>
    </div>
  );
}
