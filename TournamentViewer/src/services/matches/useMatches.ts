import { useReducer } from "react";
import { initialState, matchesReducer } from "./matches.reducer";
import * as MatchesApi from "./matches.api";
import { CreateMatchRequest } from "../../models/requests/match-requests";

export function useMatches(phaseId: number) {
  const [state, dispatch] = useReducer(matchesReducer, initialState);

  async function list() {
    try {
      const items = await MatchesApi.listByPhase(phaseId);
      dispatch({ type: "onListMatches", payload: items });
    } catch (error) {
      console.error("Error listing matches by phase:", error);
      throw new Error("Unable to list matches by phase.");
    }
  }

  async function create(request: CreateMatchRequest) {
    try {
      const item = await MatchesApi.create(request);
      dispatch({ type: "onCreateMatch", payload: item });
    } catch (error) {
      console.error("Error creating match:", error);
      throw new Error("Unable to create match.");
    }
  }

  async function setActiveMatch(
    divisionId: number,
    phaseId: number,
    matchId: number
  ) {
    try {
      await MatchesApi.setActiveMatch({ divisionId, phaseId, matchId });
      dispatch({
        type: "onSetActiveMatch",
        payload: state.matches.find((m) => m.id === matchId)!,
      });
    } catch (error) {
      console.error("Error setting active match:", error);
      throw new Error("Unable to set active match.");
    }
  }

  async function deleteMatch(matchId: number) {
    try {
      await MatchesApi.deleteMatch(matchId);
      dispatch({
        type: "onDeleteMatch",
        payload: state.matches.find((m) => m.id === matchId)!,
      });
    } catch (error) {
      console.error("Error deleting match:", error);
      throw new Error("Unable to delete match.");
    }
  }

  async function addSongToMatchByRoll(
    divisionId: number,
    phaseId: number,
    matchId: number,
    group: string,
    level: string
  ) {
    try {
      const item = await MatchesApi.addSongToActiveMatch({
        divisionId,
        phaseId,
        matchId,
        group,
        level,
      });
      dispatch({ type: "onAddSongToMatch", payload: item });
    } catch (error) {
      console.error("Error adding song to match:", error);
      throw new Error("Unable to add song to match.");
    }
  }

  async function addSongToMatchBySongId(
    divisionId: number,
    phaseId: number,
    matchId: number,
    songId: number
  ) {
    try {
      const item = await MatchesApi.addSongToActiveMatch({
        divisionId,
        phaseId,
        matchId,
        songId,
      });
      dispatch({ type: "onAddSongToMatch", payload: item });
    } catch (error) {
      console.error("Error adding song to match:", error);
      throw new Error("Unable to add song to match.");
    }
  }

  async function addStandingToMatch(
    playerId: number,
    songId: number,
    percentage: number,
    isFailed: boolean
  ) {
    try {
      const item = await MatchesApi.addStandingToActiveMatch({
        playerId,
        songId,
        percentage,
        isFailed,
      });
      dispatch({ type: "onAddStandingToMatch", payload: item });
    } catch (error) {
      console.error("Error adding standing to match:", error);
      throw new Error("Unable to add standing to match.");
    }
  }

  async function deleteStandingFromMatch(songId: number) {
    try {
      const item = await MatchesApi.deleteStandingsFromActiveMatch(songId);
      dispatch({ type: "onDeleteStandingFromMatch", payload: item });
    } catch (error) {
      console.error("Error deleting standing from match:", error);
      throw new Error("Unable to delete standing from match.");
    }
  }

  async function deleteStandingsForPlayerFromMatch(
    songId: number,
    playerId: number
  ) {
    try {
      const item = await MatchesApi.deleteStandingsForPlayerFromActiveMatch(
        songId,
        playerId
      );
      dispatch({ type: "onDeleteStandingFromMatch", payload: item });
    } catch (error) {
      console.error("Error deleting standings for player from match:", error);
      throw new Error("Unable to delete standings for player from match.");
    }
  }

  return {
    state,
    actions: {
      list,
      create,
      setActiveMatch,
      deleteMatch,
      addSongToMatchByRoll,
      addSongToMatchBySongId,
      addStandingToMatch,
      deleteStandingFromMatch,
      deleteStandingsForPlayerFromMatch,
    },
  };
}
