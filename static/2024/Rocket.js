class Rocket {
    position; rotation; velocity;

    isDead;

    constructor(position) {
        this.position = position;

        this.rotation = 0;
        this.velocity = createVector(0, -60);
    }

    display() {
        push();
        translate(this.position);
        rotate(this.rotation);
        translate(0, -75);

        noStroke();
        fill(240, 0, 0);
        triangle(0, 0, -50, 70, 50, 70);

        let colors = [ color(240, 240, 0), color(240, 0, 240) ]
        for (let i = 0; i < 5; i++) {
            fill(colors[i % colors.length]);
            rect(-30 + i * 12, 70, 12, 150);
        }

        pop();
    }

    update() {
        this.fly();
        this.wiggle();
        if (this.velocity.y > -2) {
            this.isDead = true;
        }
    }

    fly() {
        this.position.add(this.velocity);
        this.velocity.y += gravity;
    }

    wiggle() {
        this.rotation = noise(frameCount / 5) / 20;
    }
}