import axios from "axios";
import { Match } from "../models/Match";

export function listByPhase(phaseId: number): Promise<Match[]> {
  return axios
    .get<Match[]>("tournament/expandphase/" + phaseId)
    .then((response) => {
      return response.data;
    });
}

export function get(id: number): Promise<Match> {
  return axios.get<Match>("matches/" + id).then((response) => {
    return response.data;
  });
}

export function create(match: Match): Promise<void> {
  return axios.post("matches", match);
}
