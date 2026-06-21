class Wish {
    state;
    transition;
    fadeValue;

    constructor() {
        this.state = "title";
        this.fadeValue = 0;
        this.transition = "fadein";
    }

    update() {
        if (this.transition == "fadein" && this.fadeValue < 255) {
            this.fadeValue += 5;
        } else if (this.transition == "fadeout" && this.fadeValue > 0) {
            this.fadeValue -= 5;
        } else if (this.transition == "fadeout" && this.state == "title") {
            this.state = "wish";
            this.transition = "fadein";
        }
    }

    switch() {
        this.transition = "fadeout";
    }

    display() {
        push();
  
        fill(240, 240, 240, this.fadeValue);
        textAlign(CENTER, TOP);
        
        if (this.state == "title") {
            textSize(64);
            textFont(fontPlayfair);
            let title = "Gelukkig";
            if (window.location.hash == "#en") {
                title = "Happy";
            }
            text(title, desiredWidth / 2, 32);
            
            textSize(150);
            textFont(fontPlayfairBoldItalic);
            text("2024", desiredWidth / 2, 32 + 32);
        } else if (this.state == "wish") {
            textAlign(CENTER, CENTER);
            textFont(fontPlayfair);
            textSize(48);
            let wish = "Dat alles dit jaar op zijn plek mag vallen."
            if (window.location.hash == "#en") {
                wish = "That everything may fall into place this year."
            }
            text(wish, 60, 32, desiredWidth - 120, 48 + 120)
        }
        
        pop();
    }
}