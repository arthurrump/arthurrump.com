class Particle {
    position; velocity; inner; stopAt; isDead;

    constructor(position, velocity, inner, stopAt = undefined) {
        this.position = position;
        this.velocity = velocity;
        this.inner = inner;
        this.stopAt = stopAt;
        this.isDead = false;
    }

    update() {
        this.position.add(this.velocity);
        this.velocity.y += gravity;
        this.velocity.x *= 0.99;

        if (this.stopAt != undefined && this.position.y > this.stopAt) {
            this.position.y = this.stopAt;
            this.velocity.mult(0);
        }

        if (this.position.y > desiredHeight + 100) {
            this.isDead = true;
            this.velocity.mult(0);
        }
    }

    display() {
        if (!this.isDead) {
            push();
            translate(this.position);
            
            this.inner.display();
            
            pop();
        }
    }
}
