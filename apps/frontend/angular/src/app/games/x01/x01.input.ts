
export class X01Input {
  Sum: number;
  Input: string;
  constructor(sum: number, input: string) {
    this.Sum = sum;
    this.Input = input;
  }

  public next(score: number) {
    this.Input += score;
    this.Sum = Number(this.Input)
  }
  public reset(): void {
    this.Sum = 0
    this.Input = ""
  }

  public getSum(): string {
    return this.Sum.toString();
  }
}
