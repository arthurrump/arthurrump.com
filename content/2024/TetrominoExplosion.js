class TetrominoExplosion {
    particles;
    
    constructor(position, tetrominoCount, stopAt) {
        this.particles = new Array(tetrominoCount);
        
        // There is at least one element that falls straight down at the maximum
        // velocity, which we can follow.
        this.particles[tetrominoCount - 1] = new Particle(position.copy(), createVector(0, 5), Tetromino.random(), stopAt);

        for (let i = 0; i < tetrominoCount - 1; i++) {
            this.particles[i] = new Particle(position.copy(), createVector(random(-4, 4), random(-15, 5)), Tetromino.random());
        }        
    }
    
    followParticle() {
        return this.particles[this.particles.length - 1];
    }

    highestParticlePosition() {
        return this.particles.sort((a, b) => a.position.y - b.position.y)[0].position;
    }

    particleCount() {
        return this.particles.length;
    }

    update() {
        for (const particle of this.particles) {
            particle.update();
        }

        this.particles = this.particles.filter(p => !p.isDead);
    }

    display() {
        for (const particle of this.particles) {
            particle.display();
        }
    }
}