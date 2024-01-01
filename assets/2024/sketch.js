let desiredWidth = 640;
let desiredHeight = 960;
let margin = 24;

let gravity = 0.2;

let canvasScale;
let rocket;
let stars;
let tetrominoExplosion;
let tetris;
let wish;
let tutorial;

let fontPlayfair;
let fontPlayfairBoldItalic;

function preload() {
  fontPlayfair = loadFont("assets/PlayfairDisplay/PlayfairDisplay-Regular.ttf");
  fontPlayfairBoldItalic = loadFont("assets/PlayfairDisplay/PlayfairDisplay-BoldItalic.ttf");
}

function setup() {
  canvasScale = min(1, min((windowWidth - margin) / desiredWidth, (windowHeight - margin) / desiredHeight));
  createCanvas(canvasScale * desiredWidth, canvasScale * desiredHeight);

  rocket = new Rocket(createVector(desiredWidth / 2, 0));
  stars = new Stars();
  wish = new Wish();
}

function draw() {
  background(10, 10, 50);

  let windowY = 0;
  if (!rocket.isDead) {
    rocket.update();
    windowY = rocket.position.y - desiredHeight / 2;
  } 
  
  if (rocket.isDead) {
    if (tetrominoExplosion == undefined) {
      tetrominoExplosion = new TetrominoExplosion(rocket.position, 100, desiredHeight - 120);

      let tetromino = tetrominoExplosion.followParticle().inner;
      let tetrisHeight = 20;
      let tetrisWidth = 11;
      if (tetromino.width() % 2 == 0) {
        tetrisWidth += 1;
      }
      tetris = new Tetris(tetrisWidth, tetrisHeight, 30);
      let top = tetrisHeight - tetromino.height();
      let left = (tetrisWidth - tetromino.width()) / 2;
      tetris.place(top, left, tetromino);

      tutorial = new Tutorial(tetris);
      
      wish.switch();
    }

    tetrominoExplosion.update();
    
    windowY = min(0, tetrominoExplosion.followParticle().position.y - desiredHeight / 2);

    if (tetrominoExplosion.particleCount() <= 1) {
      if (!tutorial.hasStarted()) {
        tutorial.start();
      } else {
        tutorial.update();
      }

      if (tutorial.isDone()) {
        tetris.update();
      }
    }
  }
  
  push();
  scale(canvasScale);
  
  push();
  translate(0, -windowY);
  
  stars.display(windowY, windowY + desiredHeight);
  
  if (!rocket.isDead) {
    rocket.display();
  } else if (tetrominoExplosion.particleCount() > 1) {
    tetrominoExplosion.display();
  } else {
    push();
    translate((desiredWidth - tetris.displayWidth()) / 2, desiredHeight - 120 - tetris.displayHeight());
    tetris.display();

    translate(tetris.displayWidth() / 2, tetris.displayHeight() / 2);
    tutorial.display();
    pop();
  }
  
  pop();

  wish.update();
  wish.display();

  pop();
}

function windowResized() {
  canvasScale = min(1, min((windowWidth - margin) / desiredWidth, (windowHeight - margin) / desiredHeight));
  resizeCanvas(canvasScale * desiredWidth, canvasScale * desiredHeight);
}

function keyPressed() {
  if (tetris != undefined) {
    if (keyCode == UP_ARROW) {
      tetris.rotate();
    } else if (keyCode == LEFT_ARROW) {
      tetris.left();
    } else if (keyCode == RIGHT_ARROW) {
      tetris.right();
    } else if (keyCode == DOWN_ARROW) {
      tetris.down();
    } else if ((key == ' ' || key == 'r')) {
      tetris.restart();
    }
  }
}

let touchStartPoint;
let touchMovePoint;

function touchStarted() {
  if (tetris != undefined && touches.length == 1) {
    touchStartPoint = touches[0];
  }
  return false;
}

function touchMoved() {
  if (tetris != undefined && touches.length == 1) {
    touchMovePoint = touches[0];
  }
  return false;
}

function touchEnded() {
  if (tetris != undefined && touchStartPoint != undefined) {
    if (touchMovePoint != undefined) {
      let dx = touchMovePoint.x - touchStartPoint.x;
      let dy = touchMovePoint.y - touchStartPoint.y;
      if (abs(dx) > 100 && abs(dx) > 2 * abs(dy)) { // Horizontal swipe
        if (dx > 0) { // Swipe right
          tetris.right();
        } else { // Swipe left
          tetris.left();
        }
      } else if (abs(dy) > 100 && abs(dy) > 2 * abs(dx)) { // Vertical swipe
        if (dy > 0) { // Swipe down
          tetris.down();
        } else {
          tetris.rotate();
        }
      } else if (tetris.isGameOver()) {
        tetris.restart();
      }
    } else if (tetris.isGameOver()) {
      tetris.restart();
    }
  }
  return false;
}

function mouseClicked() {
  if (tetris != undefined && tetris.isGameOver()) {
    tetris.restart();
  }
}
