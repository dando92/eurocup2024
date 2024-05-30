class Score {
    constructor(player, scoreMap) {
        this.player = player
        this.scoreMap = scoreMap
    };
    serialize() {
        return JSON.stringify(this);
    }
}

module.exports = Score