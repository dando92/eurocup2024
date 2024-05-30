import { useEffect, useState } from "react";
import { Match } from "../../models/Match";
import * as MatchesApi from "../../services/matches/matches.api";
import { Division } from "../../models/Division";
import { Phase } from "../../models/Phase";
import axios from "axios";
import MatchesView from "../manage/tournament/MatchesView";
import {
  HttpTransportType,
  HubConnection,
  HubConnectionBuilder,
} from "@microsoft/signalr";

export default function LivePhase() {
  const [loading, setLoading] = useState(true);
  const [phase, setPhase] = useState<Phase | null>(null);
  const [division, setDivision] = useState<Division | null>(null);
  const [activeMatch, setActiveMatch] = useState<Match | null>(null);
  const [, setConnection] = useState<HubConnection | null>(null);

  useEffect(() => {
    MatchesApi.getActiveMatch()
      .then((match) => {
        setActiveMatch(match);
        axios.get("/phases/" + match.phaseId).then((response) => {
          setPhase(response.data);
          axios
            .get("/divisions/" + response.data.divisionId)
            .then((response) => {
              setDivision(response.data);
            });
        });
      })
      .catch(() => {
        const conn = new HubConnectionBuilder()
          .withUrl(`${import.meta.env.VITE_PUBLIC_API_URL}../matchupdatehub`, {
            skipNegotiation: true,
            transport: HttpTransportType.WebSockets,
          })
          .build();

        conn.on("OnMatchUpdate", () => {
          // refresh page
          window.location.reload();
        });

        conn.start().then(() => {
          console.log("Now listening to match changes.");
        });

        setConnection(conn);
      })
      .finally(() => setLoading(false));
  }, []);

  return (
    <div>
      {!loading && !activeMatch && <p>No match in progress. Stay tuned!</p>}
      {division && phase && activeMatch && (
        <div>
          <h1
            className="text-center text-7xl text-lower font-bold"
            style={{ fontFamily: "rocket, sans-serif" }}
          >
            {division.name}
          </h1>
          <MatchesView
            showPastMatches={false}
            phaseId={phase.id}
            division={division}
          />
        </div>
      )}
    </div>
  );
}
