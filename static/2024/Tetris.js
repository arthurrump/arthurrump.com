class Tetris {
    width;
    height;
    grid;
    currentTetromino;
    currentLeft;
    currentTop;
    blockSize;
    speed;
    lastAutoDown;

    constructor(width, height, blockSize) {
        this.restart(width, height, blockSize);
    }

    restart(width = this.width, height = this.height, blockSize = this.blockSize) {
        this.width = width;
        this.height = height;
        this.grid = new Array(height);
        for (let row = 0; row < height; row++) {
            this.grid[row] = new Array(width);
        }
        this.blockSize = blockSize;
        this.currentTetromino = Tetromino.random();
        this.currentLeft = round(random(0, width - this.currentTetromino.width()));
        this.currentTop = 0;
        this.speed = 1000;
        this.lastAutoDown = Date.now();
    }

    displayWidth() {
        return this.width * this.blockSize;
    }

    displayHeight() {
        return this.height * this.blockSize;
    }

    canPlace(top, left, tetromino) {
        let blocks = tetromino.blocks();
        for (let row = 0; row < blocks.length; row++) {
            for (let col = 0; col < blocks[row].length; col++) {
                if (top + row >= this.height || blocks[row][col] && this.grid[top + row][left + col] != undefined) {
                    return false;
                }
            }
        }
        return true;
    }

    place(top, left, tetromino) {
        let blocks = tetromino.blocks();
        for (let row = 0; row < blocks.length; row++) {
            for (let col = 0; col < blocks[row].length; col++) {
                if (blocks[row][col]) {
                    this.grid[top + row][left + col] = tetromino.blockColor;
                }
            }
        }
    }

    rotate() {
        let blocks = this.currentTetromino.blocks();
        let ctWidth = blocks[0].length;
        let ctHeight = blocks.length;

        let newTop = this.currentTop;
        let newLeft = this.currentLeft;
        if (ctWidth == 4) {
            newTop -= 2;
            newLeft += 1;
        } else if (ctHeight == 4) {
            newTop += 2;
            newLeft -= 1;
        }

        if (newTop < 0) newTop = 0;
        if (newLeft < 0) newLeft = 0;
        if (newLeft + ctHeight >= this.width) newLeft = this.width - ctHeight;

        this.currentTetromino.rotate();
        if (this.canPlace(newTop, newLeft, this.currentTetromino)) {
            this.currentTop = newTop;
            this.currentLeft = newLeft;
        } else {
            this.currentTetromino.rotateBack();
        }
    }

    left() {
        if (this.currentLeft > 0 && this.canPlace(this.currentTop, this.currentLeft - 1, this.currentTetromino)) {
            this.currentLeft -= 1;
        }
    }

    right() {
        if (this.currentLeft + this.currentTetromino.width() < this.width && this.canPlace(this.currentTop, this.currentLeft + 1, this.currentTetromino)) {
            this.currentLeft += 1;
        }
    }

    down() {
        if (this.canPlace(this.currentTop + 1, this.currentLeft, this.currentTetromino)) {
            this.currentTop += 1;
        } else {
            this.next();
        }
    }

    next() {
        this.place(this.currentTop, this.currentLeft, this.currentTetromino);
        this.clean();
        if (!this.isGameOver()) {
            this.currentTetromino = Tetromino.random();
            this.currentLeft = round(random(0, this.width - this.currentTetromino.width()));
            this.currentTop = 0;
        }
    }

    clean() {
        function isFilled(row) {
            for (let i = 0; i < row.length; i++) {
                if (row[i] == undefined) {
                    return false;
                }
            }
            return true;
        }

        for (let row = 0; row < this.grid.length; row++) {
            if (isFilled(this.grid[row])) {
                for (let replaceRow = row; replaceRow > 0; replaceRow--) {
                    this.grid[replaceRow] = this.grid[replaceRow - 1];
                }
                this.grid[0] = new Array(this.width);
            }
        }
    }

    isGameOver() {
        for (let i = 0; i < this.grid[0].length; i++) {
            if (this.grid[0][i] != undefined) {
                return true;
            }
        }
        return false;
    }

    display() {
        push();

        noFill();
        let sw = 2;
        strokeWeight(sw);
        stroke(240, 240, 240);
        line(-sw, this.displayHeight() + sw, this.displayWidth() + sw, this.displayHeight() + sw);
        line(-sw, 0, -sw, this.displayHeight() + sw);
        line(this.displayWidth() + sw, 0, this.displayWidth() + sw, this.displayHeight() + sw);

        let currentBlocks = this.currentTetromino.blocks();
        for (let row = 0; row < currentBlocks.length; row++) {
            for (let col = 0; col < currentBlocks[row].length; col++) {
                if (currentBlocks[row][col]) {
                    Tetromino.displayBlock((this.currentLeft + col) * this.blockSize, (this.currentTop + row) * this.blockSize, this.currentTetromino.blockColor, this.blockSize);
                }
            }
        }

        for (let row = 0; row < this.grid.length; row++) {
            for (let col = 0; col < this.grid[row].length; col++) {
                if (this.grid[row][col] != undefined) {
                    Tetromino.displayBlock(col * this.blockSize, row * this.blockSize, this.grid[row][col], this.blockSize);
                }
            }
        }

        if (this.isGameOver()) {
            push();
            translate(this.displayWidth() / 2, this.displayHeight() / 2);

            strokeWeight(5);
            stroke(240, 240, 240);

            fill(0, 0, 0, 80);
            circle(0, 0, 100);

            noFill();
            arc(0, 0, 50, 50, -3 * QUARTER_PI, PI);
            line(-25, 0, -15, 10);
            line(-25, 0, -35, 10);

            pop();
        }

        pop();
    }

    update() {
        if (this.lastAutoDown + this.speed < Date.now()) {
            this.down();
            this.lastAutoDown = Date.now();
        }
    }
}