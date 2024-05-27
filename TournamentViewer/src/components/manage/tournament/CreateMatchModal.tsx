import { useEffect, useState } from "react";
import OkModal from "../../layout/OkModal";
import { Player } from "../../../models/Player";
import axios from "axios";
import Select from "react-select";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faMinusCircle, faPlusCircle } from "@fortawesome/free-solid-svg-icons";
import { Song } from "../../../models/Song";

type CreateMatchModal = {
  open: boolean;
  onClose: () => void;
  onCreate: () => void;
  phaseId: number;
};

export default function CreateMatchModal({
  open,
  onClose,
  onCreate,
  phaseId,
}: CreateMatchModal) {
  const [players, setPlayers] = useState<Player[]>([]);
  const [songs, setSongs] = useState<Song[]>([]);

  const [matchName, setMatchName] = useState<string>("");

  const [selectedPlayers, setSelectedPlayers] = useState<Player[]>([]);

  const [songAddType, setSongAddType] = useState<"title" | "roll">("roll");
  // if by roll
  const [selectedSongDifficulties, setSelectedSongDifficulties] = useState<
    string[]
  >([]);
  const [difficultyInput, setDifficultyInput] = useState<string>("");

  // if by title
  const [selectedSongs, setSelectedSongs] = useState<Song[]>([]);

  useEffect(() => {
    open &&
      axios.get<Player[]>(`players`).then((response) => {
        setPlayers(response.data);
      });

    open &&
      axios.get<Song[]>(`songs`).then((response) => {
        setSongs(response.data);
      });
  }, [open]);

  return (
    <OkModal
      okText="Create match"
      title="Create Match"
      open={open}
      onClose={onClose}
      onOk={onCreate}
    >
      <div className="flex flex-col w-full gap-3">
        <div className="w-full">
          <h3>Name</h3>
          <input
            className="w-full border border-gray-300 px-2 py-2 rounded-lg"
            type="text"
            value={matchName}
            onChange={(e) => setMatchName(e.target.value)}
            placeholder="Type match name"
          />
        </div>
        <div className="w-full">
          <h3>Players</h3>
          <Select
            isMulti
            options={players.map((p) => ({ value: p.id, label: p.name }))}
            onChange={(e) => {
              setSelectedPlayers(
                e.map((p) => players.find((pl) => pl.id === p.value)!)
              );
            }}
            value={selectedPlayers.map((p) => ({ value: p.id, label: p.name }))}
            menuPortalTarget={document.body}
            styles={{ menuPortal: (base) => ({ ...base, zIndex: 9999 }) }}
          />
        </div>
        <div className="w-full">
          <h3>Songs</h3>
          <div className="flex flex-row gap-3 mb-2">
            <div className="flex flex-row gap-1">
              <input
                type="radio"
                id="title"
                name="songAddType"
                value="title"
                checked={songAddType === "title"}
                onChange={() => setSongAddType("title")}
              />
              <label htmlFor="title">By titles</label>
            </div>
            <div className="flex flex-row gap-1">
              <input
                type="radio"
                id="roll"
                name="songAddType"
                value="roll"
                checked={songAddType === "roll"}
                onChange={() => setSongAddType("roll")}
              />
              <label htmlFor="roll">By roll</label>
            </div>
          </div>
          {songAddType === "roll" && (
            <div>
              {selectedSongDifficulties.length > 0 && (
                <div className="flex my-2 flex-col gap-2 w-96">
                  {selectedSongDifficulties.map((d, i) => (
                    <div key={i} className="flex flex-row items-center gap-2">
                      <span className="w-6 font-bold">{d}</span>
                      <button
                        onClick={() =>
                          setSelectedSongDifficulties(
                            selectedSongDifficulties.filter(
                              (_, index) => index !== i
                            )
                          )
                        }
                        className="text-red-700 text-sm"
                      >
                        <FontAwesomeIcon icon={faMinusCircle} />
                      </button>
                    </div>
                  ))}
                </div>
              )}
              <div>
                <input
                  value={difficultyInput}
                  onChange={(e) => setDifficultyInput(e.target.value)}
                  className="border border-gray-300 px-2 py-2 mr-2 rounded-lg"
                  type="number"
                  placeholder="Type difficulty"
                />
                <button
                  onClick={() => {
                    setSelectedSongDifficulties([
                      ...selectedSongDifficulties,
                      difficultyInput,
                    ]);
                    setDifficultyInput("");
                  }}
                  className="text-green-700 text-lg"
                >
                  <FontAwesomeIcon icon={faPlusCircle} />
                </button>
              </div>
            </div>
          )}
          {songAddType === "title" && (
            <div>
              <Select
                isMulti
                options={songs.map((s) => ({ value: s.id, label: s.title }))}
                onChange={(e) => {
                  setSelectedSongs(
                    e.map((s) => songs.find((song) => song.id === s.value)!)
                  );
                }}
                value={selectedSongs.map((s) => ({
                  value: s.id,
                  label: s.title,
                }))}
                menuPortalTarget={document.body}
                styles={{ menuPortal: (base) => ({ ...base, zIndex: 9999 }) }}
              />
            </div>
          )}
        </div>
      </div>
    </OkModal>
  );
}
