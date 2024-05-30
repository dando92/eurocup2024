class Match {
  constructor() {
    this.rounds = {}
  };
  getRoundBySongName(songName) {
    return this.rounds[songName];
  }
  serialize() {
    return JSON.stringify(this);
  }
  static deserialize(json) {
    return Object.assign(new Match, JSON.parse(json));
  }
}

module.exports = Match