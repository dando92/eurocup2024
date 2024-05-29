import axios from "axios";
import { Match } from "../../models/Match";
import {
  AddSongToMatchRequest,
  AddStandingToMatchRequest,
  CreateMatchRequest,
  SetActiveMatchRequest,
} from "../../models/requests/match-requests";

export async function listByPhase(phaseId: number): Promise<Match[]> {
  try {
    const response = await axios.get<Match[]>(
      "tournament/expandphase/" + phaseId
    );
    return response.data;
  } catch (error) {
    console.error("Error listing matches by phase:", error);
    throw new Error("Unable to list matches by phase.");
  }
}

export async function create(request: CreateMatchRequest): Promise<Match> {
  try {
    const response = await axios.post<Match>("matches", request);
    return response.data;
  } catch (error) {
    console.error("Error creating match:", error);
    throw new Error("Unable to create match.");
  }
}

export async function setActiveMatch(
  request: SetActiveMatchRequest
): Promise<void> {
  try {
    const response = await axios.post("tournament/setactivematch", request);
    return response.data;
  } catch (error) {
    console.error("Error setting active match:", error);
    throw new Error("Unable to set active match.");
  }
}

export async function deleteMatch(matchId: number): Promise<void> {
  try {
    await axios.delete("matches/" + matchId);
  } catch (error) {
    console.error("Error deleting match:", error);
    throw new Error("Unable to delete match.");
  }
}

export async function addSongToActiveMatch(
  request: AddSongToMatchRequest
): Promise<Match> {
  try {
    const response = await axios.post("tournament/addround", request);
    return response.data;
  } catch (error) {
    console.error("Error adding song to active match:", error);
    throw new Error("Unable to add song to active match.");
  }
}

export async function addStandingToActiveMatch(
  request: AddStandingToMatchRequest
): Promise<Match> {
  try {
    const response = await axios.post("tournament/addstanding", request);
    return response.data;
  } catch (error) {
    console.error("Error adding standing to active match:", error);
    throw new Error("Unable to add standing to active match.");
  }
}

export async function deleteStandingsFromActiveMatch(
  songId: number
): Promise<Match> {
  try {
    const response = await axios.delete("tournament/deletestanding/" + songId);
    return response.data;
  } catch (error) {
    console.error("Error deleting standings from active match:", error);
    throw new Error("Unable to delete standings from active match.");
  }
}

export async function deleteStandingsForPlayerFromActiveMatch(
  songId: number,
  playerId: number
): Promise<Match> {
  try {
    const response = await axios.delete(
      `tournament/deletestanding/${songId}/${playerId}`,
      {
        data: { songId, playerId },
      }
    );

    return response.data;
  } catch (error) {
    console.error(
      "Error deleting standings for player from active match:",
      error
    );
    throw new Error("Unable to delete standings for player from active match.");
  }
}
