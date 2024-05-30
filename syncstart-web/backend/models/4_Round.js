class Round {
  constructor() {
    this.scores = {}
  };
  getScoreByPlayerName(playerName) {
    return this.scores[playerName];
  }
  addScore(playerName, scoreData){
    this.scores[playerName] = scoreData;
  }
  serialize() {
    return JSON.stringify(this);
  }
  static deserialize(json) {
    return Object.assign(new Round, JSON.parse(json));
  }
}

module.exports = Round