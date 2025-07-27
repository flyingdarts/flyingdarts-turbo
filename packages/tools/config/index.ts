/**
 * @flyingdarts/config - Shared configuration files for the Flyingdarts workspace
 *
 * This package provides base configurations that can be extended and customized
 * by individual projects in the workspace.
 */

import path from "path";

// Import Jest configurations
import { jestAngular } from "./src/jest/angular";
import {
  tsconfigAngular,
  tsconfigBase,
  tsconfigStrict,
} from "./src/typescript";
import { jestBase } from "./src/jest/base";

export interface ConfigPackage {
  jest: {
    base: any;
    angular: any;
  };
  typescript: {
    base: any;
    strict: any;
    node: any;
  };
}

// Export base configurations
export const configPackage: ConfigPackage = {
  // Jest configurations
  jest: {
    base: jestBase,
    angular: jestAngular,
  },

  // TypeScript configurations (placeholder - these would need to be created)
  typescript: {
    base: tsconfigBase,
    strict: tsconfigStrict,
    node: tsconfigAngular,
  },
};

export default configPackage;
