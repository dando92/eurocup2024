import { useEffect, useState } from "react";
import { Phase } from "../../../models/Phase";
import Select from "react-select";
import axios from "axios";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faPlus, faTrash } from "@fortawesome/free-solid-svg-icons";

type PhaseListProps = {
  divisionId: number;
  onPhaseSelect: (phase: Phase | null) => void;
};

export default function PhaseList({
  divisionId,
  onPhaseSelect,
}: PhaseListProps) {
  const [phases, setPhases] = useState<Phase[]>([]);
  const [selectedPhaseId, setSelectedPhaseId] = useState<number>(-1);

  useEffect(() => {
    axios.get<Phase[]>(`divisions/${divisionId}/phases`).then((response) => {
      setPhases(response.data);
      if (response.data.length > 0) setSelectedPhaseId(response.data[0].id);
    });
  }, [divisionId]);

  const createPhase = () => {
    const name = prompt("Enter phase name");

    if (name) {
      axios.post<Phase>(`phases`, { divisionId, name }).then((response) => {
        setPhases([...phases, response.data]);
        setSelectedPhaseId(response.data.id);
      });
    }
  };
  const deletePhase = () => {
    if (
      window.confirm("Are you sure you want to delete this phase?")
    ) {
      axios.delete(`phases/${selectedPhaseId}`).then(() => {
        setPhases(phases.filter((d) => d.id !== selectedPhaseId));
        setSelectedPhaseId(-1);
      });
    }
  };

  return (
    <div className="flex flex-row gap-3">
      <Select
        className="min-w-[300px]"
        placeholder="Select phase"
        options={phases.map((p) => ({ value: p.id, label: p.name }))}
        onChange={(e) => {
          onPhaseSelect(phases.find((p) => p.id === selectedPhaseId) ?? null);
          setSelectedPhaseId(e?.value ?? -1);
        }}
        value={
          selectedPhaseId >= 0
            ? {
                value: phases.find((d) => d.id === selectedPhaseId)?.id,
                label: phases.find((d) => d.id === selectedPhaseId)?.name,
              }
            : null
        }
      />
      <button
        onClick={createPhase}
        className="text-green-700"
        title="Create new division"
      >
        <FontAwesomeIcon icon={faPlus} />
      </button>
      <button
        onClick={deletePhase}
        className="text-red-700 disabled:text-red-200"
        disabled={selectedPhaseId === -1}
        title={
          selectedPhaseId === -1 ? "plz select phase to delete" : "Delete phase"
        }
      >
        <FontAwesomeIcon icon={faTrash} />
      </button>
    </div>
  );
}
