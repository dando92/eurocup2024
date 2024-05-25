import { useState } from "react";
import { Division } from "../../../models/Division";
import DivisionList from "./DivisionList";

export default function TournamentSettings() {
  const [selectedDivision, setSelectedDivision] = useState<Division | null>(
    null
  );

  return (
    <div>
      <div className="flex flex-col justify-start gap-3">
        <div className="flex flex-row gap-3">
          <h2>Configure your tournament!</h2>
        </div>
        <DivisionList
          onDivisionSelect={(division) => setSelectedDivision(division)}
        />
      </div>
    </div>
  );
}
