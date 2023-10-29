import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { TweenMax } from 'gsap';

@Component({
  selector: 'app-dartboard',
  templateUrl: './dartboard.component.html',
  styleUrls: ['./dartboard.component.scss']
})
export class DartboardComponent implements OnInit {
  @Output() targetPressedEvent = new EventEmitter<number>();
  private dartboardWidth: number = 1400;
  private dartboardHeight: number = 1400;

  constructor() { }

  ngOnInit(): void {
    this.createSVG();
    this.showDartBoard(this.boardElemArray());
    const targets = document.getElementsByClassName("target");
    for (let i = 0; i < targets.length; i++) {
      targets[i].addEventListener("click", (e: any) => {
        let amount = e.target.getAttribute("value");
        e.target.classList.contains("double")
          ? (amount = 2 * amount)
          : e.target.classList.contains("triple")
            ? (amount = 3 * amount)
            : (amount = amount);
        this.targetPressedEvent.emit(parseFloat(amount))
      });
    }
  }

  createPointArray() {
    const angle = this.convertToRad(4.5);
    let array = [];
    const pOne = [this.dartboardWidth / 2, this.dartboardHeight / 2];
    array.push(pOne);
    this.DeterminePoint(angle, array, 300);
    this.DeterminePoint(angle, array, 350);
    this.DeterminePoint(angle, array, 550);
    this.DeterminePoint(angle, array, 600);
    this.DeterminePoint(angle, array, 625);
    return array;
  }

  convertToRad(num: any) {
    return Math.PI * num / 180;
  }

  DeterminePoint(angle: any, array: any, height: any) {
    for (let i = 0; i < 2; i++) {
      let point = [];
      let y = 2 * height * Math.sin(angle) * Math.sin(angle);
      let x = 2 * height * Math.sin(angle) * Math.cos(angle);
      i === 0
        ? (point = [this.dartboardWidth / 2 + x, this.dartboardHeight / 2 - height + y])
        : (point = [this.dartboardWidth / 2 - x, this.dartboardHeight / 2 - height + y]);
      array.push(point);
    }
  }

  createSVG() {
    const points = this.createPointArray();
    const svg = this.addAttributes("svg", [
      ["viewBox", `0 0 ${this.dartboardWidth} ${this.dartboardHeight}`],
      ["width", "100%"],
      ["height", "100%"],
      ["id", "dartboard"]
    ]);
    const zero = this.addAttributes("circle", [
      ["cx", String(this.dartboardWidth / 2)],
      ["cy", String(this.dartboardHeight / 2)],
      ["r", String(this.dartboardWidth / 2)],
      ["fill", "black"],
      ["id", "zero"],
      ["value", "0"]
    ]);
    zero.classList.add("target");
    const text = this.addAttributes("text", []);
    const numbers = [
      20, 17, 2, 15, 10, 6, 13, 4, 18, 1, 3, 19, 7, 16, 8, 11, 14, 9, 12, 5
    ];
    svg.appendChild(zero);
    let color;
    let paths = [
      `M${points[0][0]},${points[0][1]}L${points[1][0]},${points[1][1]
      }A300 300 0 0 0 ${points[2][0]},${points[2][1]}Z`,
      `M${points[2][0]},${points[2][1]}A300 300 0 0 1 ${points[1][0]},${points[1][1]
      }L${points[3][0]},${points[3][1]}A350 350 0 0 0 ${points[4][0]},${points[4][1]
      }Z`,
      `M${points[4][0]},${points[4][1]}A350 350 0 0 1 ${points[3][0]},${points[3][1]
      }L${points[5][0]},${points[5][1]}A550 550 0 0 0 ${points[6][0]},${points[6][1]
      }Z`,
      `M${points[6][0]},${points[6][1]}A550 550 0 0 1 ${points[5][0]},${points[5][1]
      }L${points[7][0]},${points[7][1]}A600 600 0 0 0 ${points[8][0]},${points[8][1]
      }Z`,
      `M${points[10][0]},${points[10][1]}A625 625 0 0 1${points[9][0]},${points[9][1]
      }`
    ];
    let classes = ["inner", "triple", "outer", "double", "number"];
    for (let j = 0; j < 20; j++) {
      for (let i = 0; i < 5; i++) {
        color =
          i === 4
            ? "transparent"
            : i % 2 === 0 && j % 2 === 0
              ? "black"
              : i % 2 !== 0 && j % 2 === 0
                ? "red"
                : i % 2 === 0 && j !== 0 ? "beige" : "green";
        const path = this.addAttributes("path", [
          ["d", paths[i]],
          ["class", classes[i]],
          ["fill", color],
          ["value", String(numbers[j])]
        ]);
        if (i === 4) {
          path.setAttribute("id", `text${j}`);
          path.style.opacity = "1";
          const textP = this.addAttributes("textPath", [
            ["href", `#text${j}`],
            ["class", "elem"],
            ["startOffset", "50%"],
            ["text-anchor", "middle"]
          ]);
          textP.innerHTML = String(numbers[j]);
          text.appendChild(textP);
        } else {
          path.classList.add("target");
        }
        svg.appendChild(path);
      }
    }
    svg.appendChild(text);
    const bull = this.addAttributes("circle", [
      ["cx", String(this.dartboardWidth / 2)],
      ["cy", String(this.dartboardHeight / 2)],
      ["r", "30"],
      ["fill", "red"],
      ["class", "target"],
      ["id", "dbull"],
      ["value", "50"]
    ]);
    const sbull = this.addAttributes("circle", [
      ["cx", String(this.dartboardWidth / 2)],
      ["cy", String(this.dartboardHeight / 2)],
      ["r", "60"],
      ["fill", "green"],
      ["class", "target"],
      ["id", "sbull"],
      ["value", "25"]
    ]);
    svg.appendChild(sbull);
    svg.appendChild(bull);
    document.getElementById("wrapper")!.appendChild(svg);
  }

  boardElemArray() {
    const a = document.getElementsByClassName("inner");
    const b = document.getElementsByClassName("triple");
    const c = document.getElementsByClassName("outer");
    const d = document.getElementsByClassName("double");
    const z = document.getElementsByClassName("number");
    return [a, b, c, d, z];
  }

  showDartBoard(array: any) {
    TweenMax.to("#dbull", 1, { opacity: 1 });
    TweenMax.to("#sbull", 2, { opacity: 1 });
    for (let i = 0; i < array.length; i++) {
      const item = array[i];
      const bb = item[0].getBBox();
      const to = `${this.dartboardWidth / 2 - bb.x} ${this.dartboardHeight / 2 - bb.y}`;
      setTimeout(
        () => {
          this.animationBoard(item, to);
        },
        i * 400,
        item,
        to
      );
    }
    TweenMax.to("#zero", 1, { opacity: 1, delay: 6 });
  }

  animationBoard(d: any, to: string) {
    TweenMax.to(d[10], 1, { rotation: 180, transformOrigin: to, opacity: 1 });
    for (let i = 1; i < 10; i++) {
      TweenMax.to(d[i], 1, {
        rotation: 180 - 18 * i,
        transformOrigin: to,
        opacity: 1,
        delay: 0.5 * i
      });
      TweenMax.to(d[i + 10], 1, {
        rotation: -180 + 18 * i,
        transformOrigin: to,
        opacity: 1,
        delay: 0.5 * i
      });
    }
    TweenMax.to(d[0], 1, {
      rotation: 0,
      transformOrigin: to,
      opacity: 1,
      delay: 5
    });
  }

  addAttributes(elem: any, array: any) {
    const item = document.createElementNS("http://www.w3.org/2000/svg", elem);
    for (let l = 0; l < array.length; l++) {
      item.setAttribute(array[l][0], array[l][1]);
    }
    return item;
  }

}