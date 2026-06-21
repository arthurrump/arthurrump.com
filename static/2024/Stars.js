class Stars {
    stars = [];
    windowMinY = 0;
    windowMaxY = 0;

    display(y1, y2) {
        let minY = min(y1, y2);
        let maxY = max(y1, y2);

        if (minY < this.windowMinY) {
            for (let y = minY; y < this.windowMinY; y += 10) {
                if (random(0, 100) > 60) {
                    this.stars.push([random(0, desiredWidth), y, random(5, 15)]);
                }
            }
            this.windowMinY = minY;
        }

        if (maxY > this.windowMaxY) {
            for (let y = maxY; y > this.windowMaxY; y -= 10) {
                if (random(0, 100) > 60) {
                    this.stars.push([random(0, desiredWidth), y, random(5, 15)]);
                }
            }
            this.windowMaxY = maxY;
        }

        push();

        noStroke();
        fill(200, 200, 10);

        this.stars
            .filter(([ x, y, d ]) => minY < y && y < maxY)
            .forEach(([ x, y, d]) => circle(x, y, d));

        pop();
    }
}