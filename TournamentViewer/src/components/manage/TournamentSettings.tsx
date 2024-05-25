import { useEffect, useState } from "react";
import { Division } from "../../models/Division";
import axios from "axios";
import Select from "react-select";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faPlus, faTrash } from "@fortawesome/free-solid-svg-icons";

export default function TournamentSettings() {
  const [divisions, setDivisions] = useState<Division[]>([]);
  const [selectedDivisionId, setSelectedDivisionId] = useState<number>(-1);

  useEffect(() => {
    axios.get<Division[]>("divisions").then((response) => {
      setDivisions(response.data);
      response.data.length > 0 && setSelectedDivisionId(response.data[0].id);
    });
  }, []);

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

  return (
    <div>
      <div className="flex flex-col justify-start gap-3">
        <div className="flex flex-row gap-3">
          <h2>Configure your tournament!</h2>
        </div>
        <div className="flex flex-row gap-3">
          <Select
            className="min-w-[300px]"
            placeholder="Select division"
            options={divisions.map((d) => ({ value: d.id, label: d.name }))}
            onChange={(e) => setSelectedDivisionId(e?.value ?? -1)}
            value={
              selectedDivisionId >= 0
                ? {
                    value: divisions.find((d) => d.id === selectedDivisionId)
                      ?.id,
                    label: divisions.find((d) => d.id === selectedDivisionId)
                      ?.name,
                  }
                : null
            }
          />
          <button
            onClick={createDivision}
            className="text-green-700"
            title="Create new division"
          >
            <FontAwesomeIcon icon={faPlus} />
          </button>
          <button
            onClick={deleteDivision}
            className="text-red-700 disabled:text-red-200"
            disabled={selectedDivisionId === -1}
            title={
              selectedDivisionId === -1
                ? "plz select division to delete"
                : "Delete division"
            }
          >
            <FontAwesomeIcon icon={faTrash} />
          </button>
        </div>
      </div>
    </div>
  );
}
