class Tutorial {
    tetris;
    index;
    timeInIndex;
    doneAction;
    timeout;
    transition;
    fadeValue;

    tutorial = [
        { type: "left", action: () => this.tetris.left() },
        { type: "right", action: () => this.tetris.right() },
        { type: "up", action: () => this.tetris.rotate() },
        { type: "down", action: () => this.tetris.down() }
    ]

    constructor(tetris) {
        this.tetris = tetris;
        this.index = -1;
        this.timeout = 1000;
        this.fadeValue = 0;
        this.transition = "fadein"
    }

    hasStarted() {
        return this.index >= 0;
    }

    isDone() {
        return this.index >= this.tutorial.length;
    }

    start() {
        this.next();
    }

    next() {
        this.index = this.index + 1;
        this.timeInIndex = Date.now();
        this.doneAction = false;
        this.transition = "fadein";
    }

    update() {
        if (this.index >= 0 && this.index < this.tutorial.length) {
            if (this.transition == "fadein" && this.fadeValue < 255) {
                this.fadeValue += 10;
            } else if (this.transition == "fadeout" && this.fadeValue > 0) {
                this.fadeValue -= 10;
            }

            if (!this.doneAction && Date.now() - this.timeInIndex > this.timeout / 2) {
                this.tutorial[this.index].action();
                this.doneAction = true;
            }

            if (Date.now() - this.timeInIndex > this.timeout) {
                this.transition = "fadeout";
            }

            if (this.transition == "fadeout" && this.fadeValue <= 0) {
                this.next();
            }
        }
    }

    display() {
        if (this.index >= 0 && this.index < this.tutorial.length) {
            push();

            strokeWeight(5);
            stroke(240, 240, 240, this.fadeValue);
            noFill();

            rectMode(CENTER);
            square(0, 0, 100, 10);

            switch (this.tutorial[this.index].type) {
                case "right":
                    rotate(HALF_PI);
                    break;
                case "down":
                    rotate(PI);
                    break;
                case "left":
                    rotate(HALF_PI * 3);
                    break;
            }

            line(0, 25, 0, -25);
            line(0, -25, -15, -10);
            line(0, -25, 15, -10);

            pop();
        }
    }
}