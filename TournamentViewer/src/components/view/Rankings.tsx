import { Team } from "../../models/Team.ts";
import { useEffect, useState } from "react";
import axios from "axios";
import { faMedal, faSpoon } from "@fortawesome/free-solid-svg-icons";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";

export default function Rankings() {
  const [teams, setTeams] = useState<Team[]>([]);

  useEffect(() => {
    axios.get("teams").then((response) => {
      setTeams(response.data);
    });
  }, []);

  return (
    <div>
      <div className={"flex flex-col gap-2"}>
        <h2 className={"text-rossoTesto text-2xl"}>Teams ranking</h2>
        <TeamRanking teams={teams} />
      </div>
    </div>
  );
}

function TeamRanking({ teams }: { teams: Team[] }) {
  const [sortedTeams, setSortedTeams] = useState<Team[]>([]);

  // gold, silver, bronze, wood in hex
  const colors = [
    "#dcb700", // gold
    "#C0C0C0", // silver
    "#CD7F32", // bronze
    "#da8446", // wood
  ];

  const fontAwesomeIcons = [faMedal, faMedal, faMedal, faSpoon];

  useEffect(() => {
    setSortedTeams([...teams].sort((a, b) => b.score - a.score));
  }, [teams]);

  return (
    <div className="rounded-lg shadow-md">
      {sortedTeams.map((team, index) => (
        <div
          key={team.id}
          className={`flex items-center gap-4 p-4 mb-2 rounded-md ${
            index % 2 === 0 ? "bg-red-900" : "bg-red-700"
          }`}
          style={{
            borderLeft: `4px solid ${colors[index] || "#ccc"}`,
          }}
        >
          <span style={{ color: colors[index] }} className="text-xl">
            <FontAwesomeIcon icon={fontAwesomeIcons[index] || faSpoon} />
          </span>
          <span className="flex-1 text-lg font-semibold text-gray-200">
            {team.name}
          </span>
          <span
            className="text-gray-300 text-2xl font-bold"
            style={{
              color: colors[index],
            }}
          >
            {team.score}
          </span>
        </div>
      ))}
    </div>
  );
}
