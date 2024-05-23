import { useEffect, useState } from "react";
import { Player } from "../../models/Player";
import axios from "axios";

export default function PlayersList() {
  const [players, setPlayers] = useState<Player[]>([]);

  useEffect(() => {
    axios.get<Player[]>("players").then((response) => {
      setPlayers(response.data);
    });
  }, [])

  return <div>
    <pre>{JSON.stringify(players, null, 2)}</pre>
  </div>;
}
