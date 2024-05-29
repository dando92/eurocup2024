import { useEffect, useState } from "react";
import axios from "axios";
import { Phase } from "../../../models/Phase";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faCircle, faHandFist } from "@fortawesome/free-solid-svg-icons";
import CreateMatchModal from "./modals/CreateMatchModal";
import { Division } from "../../../models/Division";
import MatchTable from "./MatchTable";
import { useMatches } from "../../../services/matches/useMatches";

type MatchesViewProps = {
  phaseId: number;
  division: Division;
};

export default function MatchesView({ phaseId, division }: MatchesViewProps) {
  const [phase, setPhase] = useState<Phase | null>(null);
  const { state, actions } = useMatches(phaseId);

  const [createMatchModalOpened, setCreateMatchModalOpened] = useState(false);

  useEffect(() => {
    axios.get<Phase>(`/phases/${phaseId}`).then((response) => {
      setPhase(response.data);
      actions.list();
      actions.getActiveMatch();
    });

    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [phaseId]);

  return (
    <div className="mt-10">
      {phase && (
        <CreateMatchModal
          phase={phase}
          division={division}
          open={createMatchModalOpened}
          onClose={() => setCreateMatchModalOpened(false)}
          onCreate={actions.create}
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
        {state.matches.length === 0 && (
          <p className="text-center text-red-500 font-bold">
            No matches found.
          </p>
        )}

        {state.activeMatch && phase && (
          <div className="pb-20">
            <div className="flex flex-row justify-center items-center gap-3">
              <FontAwesomeIcon
                icon={faCircle}
                className="text-green-800 text-xs animate-pulse"
              />

              <h3 className="text-3xl text-center">Now playing:</h3>
            </div>
            <MatchTable
              controls
              division={division}
              phase={phase}
              isActive={true}
              onSetActiveMatch={actions.setActiveMatch}
              onDeleteMatch={actions.deleteMatch}
              onAddSongToMatchByRoll={actions.addSongToMatchByRoll}
              onAddSongToMatchBySongId={actions.addSongToMatchBySongId}
              onEditSongToMatchByRoll={actions.editSongToMatchByRoll}
              onEditSongToMatchBySongId={actions.editSongToMatchBySongId}
              match={state.activeMatch}
            />
          </div>
        )}

        <h3 className="text-3xl text-center">Past matches:</h3>
        {phase &&
          state.matches
            .filter((m) => m.id !== state.activeMatch?.id)
            .map((match) => (
              <MatchTable
                controls
                division={division}
                phase={phase}
                isActive={state.activeMatch?.id === match.id}
                onSetActiveMatch={actions.setActiveMatch}
                onDeleteMatch={actions.deleteMatch}
                onAddSongToMatchByRoll={actions.addSongToMatchByRoll}
                onAddSongToMatchBySongId={actions.addSongToMatchBySongId}
                onEditSongToMatchByRoll={actions.editSongToMatchByRoll}
                onEditSongToMatchBySongId={actions.editSongToMatchBySongId}
                key={match.id}
                match={match}
              />
            ))}
      </div>
    </div>
  );
}
