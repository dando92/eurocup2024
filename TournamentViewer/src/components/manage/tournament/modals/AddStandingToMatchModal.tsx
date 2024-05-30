import { useState } from "react";
import OkModal from "../../../layout/OkModal";

type AddStandingToMatchModalProps = {
  playerId: number;
  songId: number;
  playerName: string;
  songTitle: string;
  open: boolean;
  onClose: () => void;
  onAddStandingToMatch: (
    playerId: number,
    songId: number,
    percentage: number,
    isFailed: boolean
  ) => void;
};

export default function AddStandingToMatchModal({
  playerId,
  songId,
  playerName,
  songTitle,
  open,
  onClose,
  onAddStandingToMatch,
}: AddStandingToMatchModalProps) {
  const [percentage, setPercentage] = useState<number>(0);
  const [isFailed, setIsFailed] = useState<boolean>(false);

  const onSubmit = () => {
    onAddStandingToMatch(playerId, songId, percentage, false);
    onClose();
  };

  return (
    <OkModal
      title="Add new standing"
      open={open}
      onClose={onClose}
      onOk={onSubmit}
    >
      <h2>
        {playerName} for {songTitle}
      </h2>
      <div className="flex flex-col gap-4">
        <div>
          <label className="block text-sm font-medium text-gray-700">
            Percentage
          </label>
          <input
            type="number"
            value={percentage}
            onChange={(e) => setPercentage(parseInt(e.target.value))}
            className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
          />
        </div>
        <div>
          <label className="block text-sm font-medium text-gray-700">
            Failed
          </label>
          <input
            type="checkbox"
            checked={isFailed}
            onChange={(e) => setIsFailed(e.target.checked)}
            className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
          />
        </div>
      </div>
    </OkModal>
  );
}