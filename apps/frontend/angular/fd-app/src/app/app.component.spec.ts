import { AppComponent } from "./app.component";
import { of } from "rxjs";

describe("AppComponent", () => {
  let component: AppComponent;
  let mockStore: any;

  beforeEach(() => {
    mockStore = {
      select: jest.fn().mockReturnValue(of(false)),
    };

    component = new AppComponent(mockStore);
  });

  it("should be created", () => {
    expect(component).toBeTruthy();
  });

  it("should have title 'flyingdarts'", () => {
    expect(component.title).toBe("flyingdarts");
  });
});
