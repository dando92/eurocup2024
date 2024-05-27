import { useState } from "react";
import { Division } from "../../../models/Division";
import DivisionList from "./DivisionList";
import PhaseList from "./PhaseList";
import { Phase } from "../../../models/Phase";
import MatchesView from "./MatchesView";

export default function TournamentSettings() {
  const [selectedDivision, setSelectedDivision] = useState<Division | null>(
    null
  );
  const [selectedPhase, setSelectedPhase] = useState<Phase | null>(null);

  return (
    <div>
      <div className="flex flex-col justify-start gap-3">
        <div className="flex flex-row gap-3">
          <h2>Configure your tournament!</h2>
        </div>
        <DivisionList
          onDivisionSelect={(division) => setSelectedDivision(division)}
        />
        {selectedDivision && (
          <PhaseList
            onPhaseSelect={setSelectedPhase}
            divisionId={selectedDivision.id}
          />
        )}
        {selectedPhase && selectedDivision && <MatchesView division={selectedDivision} phaseId={selectedPhase.id} />}
      </div>
    </div>
  );
}
