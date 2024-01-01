class Tetromino {
    type;
    rotation;
    blockColor;

    blockSize;

    constructor(type, rotation, color, blockSize = 30) {
        this.type = type;
        this.rotation = rotation;
        this.blockColor = color;
        this.blockSize = blockSize;
    }

    static random() {
        let type = random([ "I", "O", "T", "J", "L", "S", "Z" ]);
        let rotation = random([ 0, 90, 180, 270 ]);
        let blockColor = random([ color(240, 0, 0), color(160, 0, 240), color(0, 240, 0), color(240, 240, 0), color(240, 160, 0), color(0, 0, 240), color(0, 240, 240) ]);
        return new Tetromino(type, rotation, blockColor);
    }

    static rotateBlocksCounterclockwise(blocks) {
        // https://stackoverflow.com/a/17428705
        return blocks[0].map((_, col) => blocks.map(row => row[row.length - 1 - col]));
    }

    static displayBlock(x, y, color, blockSize = 30) {
        push();

        strokeWeight(blockSize / 10);
        stroke(red(color) / 2, green(color) / 2, blue(color) / 2);
        fill(color);
        
        let innerSize = blockSize - (blockSize / 20);
        square(x, y, innerSize);

        pop();
    }

    rotate() {
        this.rotation = (this.rotation + 270) % 360;
    }

    rotateBack() {
        this.rotation = (this.rotation + 90) % 360;
    }

    blocks() {
        let blocks;

        // https://en.wikipedia.org/wiki/Tetromino#One-sided_tetrominoes
        switch (this.type) {
            case "I":
                blocks =
                    [ [ true, true, true, true ] ];
                break;
            case "O":
                blocks =
                    [ [ true, true ],
                      [ true, true ] ];
                break;
            case "T":
                blocks =
                    [ [ true, true, true ],
                      [ false, true, false ] ];
                break;
            case "J":
                blocks =
                    [ [ true, true, true ],
                      [ false, false, true ] ];
                break;
            case "L":
                blocks =
                    [ [ true, true, true ],
                      [ true, false, false ] ];
                break;
            case "S":
                blocks =
                    [ [ false, true, true ],
                      [ true, true, false ] ];
                break;
            case "Z":
                blocks =
                    [ [ true, true, false ],
                      [ false, true, true ] ];
                break;
        }
        
        switch (this.rotation) {
            // Note: the cases fall through on purpose!
            case 270:
                blocks = Tetromino.rotateBlocksCounterclockwise(blocks);
            case 180:
                blocks = Tetromino.rotateBlocksCounterclockwise(blocks);
            case 90:
                blocks = Tetromino.rotateBlocksCounterclockwise(blocks);
            case 0:
        }

        return blocks;
    }

    width() {
        return this.blocks()[0].length;
    }

    height() {
        return this.blocks().length;
    }

    display() {
        let blocks = this.blocks();
        let xOffset = blocks[0].length * this.blockSize / 2;
        let yOffset = blocks.length * this.blockSize;

        blocks.forEach((row, rowIndex) => {
            row.forEach((block, blockIndex) => {
                if (block) {
                    Tetromino.displayBlock(blockIndex * this.blockSize - xOffset, rowIndex * this.blockSize - yOffset, this.blockColor, this.blockSize);
                }
            });
        });
    }
}
