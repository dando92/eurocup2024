import { useState } from "react";
import OkModal from "../../../layout/OkModal";
import { Match } from "../../../../models/Match";

type EditMatchNotesModalProps = {
  open: boolean;
  match: Match;
  onClose: () => void;
  onSave: (matchId: number, notes: string) => void;
};

export default function EditMatchNotesModal({
  open,
  match,
  onClose,
  onSave,
}: EditMatchNotesModalProps) {
  const [notes, setNotes] = useState(match.notes || "");

  const handleSave = () => {
    onSave(match.id, notes);
    onClose();
  };

  return (
    <OkModal
      title={`Edit notes for match ${match.name}`}
      onClose={onClose}
      onOk={handleSave}
      open={open}
    >
      <textarea
      className="border border-blu p-3 outline-none rounded-lg"
        value={notes}
        onChange={(e) => setNotes(e.target.value)}
        style={{ width: "100%", height: "200px" }}
      />
    </OkModal>
  );
}
