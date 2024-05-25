import { useEffect, useState } from "react";
import { Division } from "../../../models/Division";
import axios from "axios";
import DivisionList from "./DivisionList";

export default function TournamentSettings() {
  const [divisions, setDivisions] = useState<Division[]>([]);
  const [selectedDivisionId, setSelectedDivisionId] = useState<number>(-1);

  useEffect(() => {
    axios.get<Division[]>("divisions").then((response) => {
      setDivisions(response.data);
      response.data.length > 0 && setSelectedDivisionId(response.data[0].id);
    });
  }, []);

  // Division functions
  const createDivision = () => {
    const name = prompt("Enter division name");

    if (name) {
      axios.post<Division>("divisions", { name }).then((response) => {
        setDivisions([...divisions, response.data]);
        setSelectedDivisionId(response.data.id);
      });
    }
  };

  const deleteDivision = () => {
    // ask the user double confirmation because it's a dangerous action
    if (
      window.confirm("WARNING!! Are you sure you want to delete this division?")
    ) {
      if (
        window.confirm(
          "WARNING!! This action is irreversible. Are you really sure?"
        )
      ) {
        axios.delete(`divisions/${selectedDivisionId}`).then(() => {
          setDivisions(divisions.filter((d) => d.id !== selectedDivisionId));
          setSelectedDivisionId(-1);
        });
      }
    }
  };
  // End Division functions

  return (
    <div>
      <div className="flex flex-col justify-start gap-3">
        <div className="flex flex-row gap-3">
          <h2>Configure your tournament!</h2>
        </div>
        <DivisionList
          divisions={divisions}
          selectedDivisionId={selectedDivisionId}
          onDivisionSelect={setSelectedDivisionId}
          onDivisionDelete={deleteDivision}
          onDivisionCreate={createDivision}
        />
      </div>
    </div>
  );
}
