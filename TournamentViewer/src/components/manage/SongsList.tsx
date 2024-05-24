import { faPlus, faTrash } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { useEffect, useState } from "react";
import { Song } from "../../models/Song";
import axios from "axios";
import Select from "react-select";

export default function SongsList() {
  const [songs, setSongs] = useState<Song[]>([]);
  const [groups, setGroups] = useState<string[]>([]);

  const [search, setSearch] = useState<string>("");
  const [selectedGroupName, setSelectedGroupName] = useState<string>("");

  const [selectedSongId, setSelectedSongId] = useState<number>(-1);

  useEffect(() => {
    axios.get<Song[]>("songs").then((response) => {
      const { data } = response;
      setSongs(data);
      setGroups([...new Set(data.map((s) => s.group))]);
      if (data.length > 0) setSelectedGroupName(data[0].group);
    });
  }, []);

  return (
    <div>
      <div className="flex flex-col justify-start gap-3">
        <div className="flex flex-row gap-3">
          <h2>Songs List</h2>
          <button
            title={!selectedGroupName ? "plz select group" : ""}
            disabled={!selectedGroupName}
            className="disabled:opacity-50 w-4 text-green-700"
          >
            <FontAwesomeIcon icon={faPlus} />
          </button>
        </div>
        <Select
          options={groups.map((g) => {
            return { value: g, label: g };
          })}
          placeholder="Select group..."
          className="w-[300px]"
          value={
            selectedGroupName
              ? { value: selectedGroupName, label: selectedGroupName }
              : null
          }
          onChange={(selected) =>
            selected
              ? setSelectedGroupName(selected.value)
              : setSelectedGroupName("")
          }
        ></Select>
        <div className="flex flex-row gap-3">
          <div className="relative bg-gray-100 w-[400px] h-[400px] overflow-auto">
            <input
              className="p-1 w-full sticky inset-0 border-blu border outline-none"
              type="search"
              placeholder="Search song..."
              value={search}
              onChange={(e) => setSearch(e.target.value)}
            />
            {songs
              .filter((s) => {
                const isInGroup = s.group === selectedGroupName;

                const found =
                  search.length < 0
                    ? true
                    : s.title.toLowerCase().includes(search.toLowerCase());

                return isInGroup && found;
              })
              .map((song) => {
                return (
                  <div
                    key={song.id}
                    role="button"
                    onClick={() => setSelectedSongId(song.id)}
                    className={`${
                      selectedSongId === song.id
                        ? "bg-middle text-white"
                        : "hover:bg-lower hover:text-white"
                    } cursor-pointer py-2 px-3 flex justify-between items-center gap-3 `}
                  >
                    <span>{song.title}</span>
                    <button
                      onClick={(e) => {
                        e.stopPropagation();
                        // TODO deleteSong(player.id);
                      }}
                      className="text-sm"
                    >
                      <FontAwesomeIcon icon={faTrash} />
                    </button>
                  </div>
                );
              })}
            {search.length > 0 &&
              songs.filter((s) =>
                s.title.toLowerCase().includes(search.toLowerCase())
              ).length === 0 && (
                <div className="text-center py-2 text-red-500">
                  No song found
                </div>
              )}
          </div>
          <div></div>
        </div>
      </div>
    </div>
  );
}
