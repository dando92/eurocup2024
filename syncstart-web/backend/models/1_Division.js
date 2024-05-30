const Phase = require('./4_Phase');

class Division {
  constructor(name) {
    this.name = name
    this.phases = []
  };
  
  addPhase(json) {
    this.phases.push(Phase.deserialize(json));
  }
  getPhase(index) {
    return this.phases[index];
  }
  serialize() {
    return JSON.stringify(this);
  }
  static deserialize(json) {
    return Object.assign(new Division, JSON.parse(json));
  }
}

module.exports = Division