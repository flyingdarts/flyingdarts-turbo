import type { Config } from "jest";
import { configPackage } from "@flyingdarts/config";

const config: Config = {
  ...configPackage.jest.angular,
  rootDir: ".",
};
export default config;
