import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { Division } from "../../../models/Division";
import { faPlus, faTrash } from "@fortawesome/free-solid-svg-icons";
import Select from "react-select";

type DivisionListProps = {
  divisions: Division[];
  selectedDivisionId: number;
  onDivisionSelect: (id: number) => void;
  onDivisionDelete: () => void;
  onDivisionCreate: () => void;
};

export default function DivisionList({
  divisions,
  selectedDivisionId,
  onDivisionSelect,
  onDivisionDelete,
  onDivisionCreate,
}: DivisionListProps) {
  return (
    <div className="flex flex-row gap-3">
      <Select
        className="min-w-[300px]"
        placeholder="Select division"
        options={divisions.map((d) => ({ value: d.id, label: d.name }))}
        onChange={(e) => onDivisionSelect(e?.value ?? -1)}
        value={
          selectedDivisionId >= 0
            ? {
                value: divisions.find((d) => d.id === selectedDivisionId)?.id,
                label: divisions.find((d) => d.id === selectedDivisionId)?.name,
              }
            : null
        }
      />
      <button
        onClick={onDivisionCreate}
        className="text-green-700"
        title="Create new division"
      >
        <FontAwesomeIcon icon={faPlus} />
      </button>
      <button
        onClick={onDivisionDelete}
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
  );
}
