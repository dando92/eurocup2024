class Phase {
  constructor(name) {
    this.name = name
    this.matches = []
  };
  getMatchByIndex(index) {
    return this.matches[index];
  }
  serialize() {
    return JSON.stringify(this);
  }
  static deserialize(json) {
    return Object.assign(new Phase, JSON.parse(json));
  }
}

module.exports = Phase