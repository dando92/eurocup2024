import { useState } from "react";
import { Division } from "../../../models/Division";
import DivisionList from "./DivisionList";
import { Phase } from "../../../models/Phase";
import { HttpTransportType, HubConnectionBuilder } from "@microsoft/signalr";
import PhaseList from "./PhaseList";
import MatchesView from "./MatchesView";

export default function TournamentSettings() {
  const [selectedDivision, setSelectedDivision] = useState<Division | null>(
    null
  );
  const [selectedPhase, setSelectedPhase] = useState<Phase | null>(null);

  const connection = new HubConnectionBuilder()
    .withUrl(`${import.meta.env.VITE_PUBLIC_API_URL}../matchupdatehub`, {
      skipNegotiation: true,
      transport: HttpTransportType.WebSockets,
    })
    .build();

  connection.on("OnMatchUpdate", (message) => {
    console.log(message);
  });

  connection.start().then(() => {
    console.log("Connection started!");
  });

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
        {selectedPhase && selectedDivision && (
          <MatchesView division={selectedDivision} phaseId={selectedPhase.id} />
        )}
      </div>
    </div>
  );
}
