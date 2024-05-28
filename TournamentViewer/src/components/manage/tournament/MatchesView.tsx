import { useEffect, useState } from "react";
import { Match } from "../../../models/Match";
import axios from "axios";
import { Phase } from "../../../models/Phase";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faHandFist } from "@fortawesome/free-solid-svg-icons";
import CreateMatchModal from "./CreateMatchModal";
import { Division } from "../../../models/Division";
import MatchTable from "./MatchTable";

type MatchesViewProps = {
  phaseId: number;
  division: Division;
};

export default function MatchesView({ phaseId, division }: MatchesViewProps) {
  const [phase, setPhase] = useState<Phase | null>(null);
  const [matches, setMatches] = useState<Match[]>([]);

  const [activeMatch, setActiveMatch] = useState<Match | null>(null);

  const [createMatchModalOpened, setCreateMatchModalOpened] = useState(false);

  useEffect(() => {
    axios.get<Phase>("phases/" + phaseId).then((response) => {
      setPhase(response.data);
    });

    axios.get<Match[]>("tournament/expandphase/" + phaseId).then((response) => {
      setMatches(response.data);
    });

    axios.get<Match>("tournament/activematch").then((response) => {
      setActiveMatch(response.data);
    });
  }, [phaseId]);

  const createMatch = () => {
    setCreateMatchModalOpened(false);

    axios.get<Match[]>("tournament/expandphase/" + phaseId).then((response) => {
      setMatches(response.data);
    });
  };

  const deleteMatch = (matchId: number) => {
    if (window.confirm("Are you sure you want to delete this match?")) {
      axios.delete("matches/" + matchId).then(() => {
        setMatches(matches.filter((m) => m.id !== matchId));
      });
    }
  };

  const setActiveMatchAction = (
    divisionId: number,
    phaseId: number,
    matchId: number
  ) => {
    axios
      .post("tournament/setactivematch", { divisionId, phaseId, matchId })
      .then(() => {
        setActiveMatch(matches.find((m) => m.id === matchId) || null);
      });
  };

  return (
    <div className="mt-10">
      {phase && (
        <CreateMatchModal
          phase={phase}
          division={division}
          open={createMatchModalOpened}
          onClose={() => setCreateMatchModalOpened(false)}
          onCreate={() => createMatch()}
        />
      )}
      <h1 className="text-center text-3xl">
        Overall View of Phase &quot;{phase?.name}&quot;
      </h1>
      <div className="mt-2 w-full bg-gray-200 p-2 rounded-lg">
        <button
          onClick={() => setCreateMatchModalOpened(true)}
          className="text-green-800 font-bold flex flex-row gap-2 items-center"
        >
          <FontAwesomeIcon icon={faHandFist} />
          <span>New match</span>
        </button>
      </div>
      <div className="w-full mt-10">
        {matches.length === 0 && (
          <p className="text-center text-red-500 font-bold">
            No matches found.
          </p>
        )}

        {phase &&
          matches.map((match) => (
            <MatchTable
              division={division}
              phase={phase}
              isActive={activeMatch?.id === match.id}
              onSetActiveMatch={setActiveMatchAction}
              onDeleteMatch={deleteMatch}
              key={match.id}
              match={match}
            />
          ))}
      </div>
    </div>
  );
}
