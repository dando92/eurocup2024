import { useEffect, useState } from "react";
import { Match } from "../../../models/Match";
import axios from "axios";
import { Phase } from "../../../models/Phase";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faHandFist } from "@fortawesome/free-solid-svg-icons";
import CreateMatchModal from "./CreateMatchModal";
import { Division } from "../../../models/Division";

type MatchesViewProps = {
  phaseId: number;
  division: Division
};

export default function MatchesView({ phaseId, division }: MatchesViewProps) {
  const [phase, setPhase] = useState<Phase | null>(null); // [1
  const [matches, setMatches] = useState<Match[]>([]);

  const [createMatchModalOpened, setCreateMatchModalOpened] = useState(false);

  useEffect(() => {
    axios.get<Phase>("phases/" + phaseId).then((response) => {
      setPhase(response.data);
    });

    axios.get<Match[]>("tournament/expandphase/" + phaseId).then((response) => {
      setMatches(response.data);
    });
  }, [phaseId]);

  const createMatch = () => {};

  return (
    <div className="mt-10">
      {phase && <CreateMatchModal
        phase={phase}
        division={division}
        open={createMatchModalOpened}
        onClose={() => setCreateMatchModalOpened(false)}
        onCreate={() => createMatch()}
      />}
      <h1 className="text-center text-3xl">
        Overall View of Phase &quot;{phase?.name}&quot;
      </h1>
      <div className="mt-2 w-full bg-gray-200 p-2 rounded-lg">
        <button onClick={() => setCreateMatchModalOpened(true)} className="text-green-800 font-bold flex flex-row gap-2 items-center">
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
      </div>
    </div>
  );
}
