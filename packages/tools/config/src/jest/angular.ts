import type { Config } from 'jest';
import { jestBase } from './base';

/**
 * Angular-specific Jest configuration for the Flyingdarts workspace
 * Extends the base configuration with Angular-specific settings
 */

export const jestAngular: Config = {
  ...jestBase,
  preset: 'jest-preset-angular',
  // Angular-specific transform
  transform: {
    '^.+\\.(ts|js|html)$': [
      'jest-preset-angular',
      {
        tsconfig: 'tsconfig.spec.json',
        stringifyContentPathRegex: '\\.html$',
      },
    ],
  },

  // Angular-specific test patterns
  testMatch: ['**/src/**/*.spec.ts', '**/src/**/*.test.ts'],

  // Angular-specific coverage collection
  collectCoverageFrom: [
    '**/src/**/*.ts',
    '!**/src/**/*.d.ts',
    '!**/src/**/*.spec.ts',
    '!**/src/**/*.test.ts',
    '!**/src/main.ts',
    '!**/src/environments/**/*.ts',
    '!**/src/app/**/*.module.ts',
    '!**/src/app/**/*.routing.ts',
    '!**/src/app/**/*.routing.module.ts',
  ],

  // Angular-specific module name mapping
  moduleNameMapper: {
    '^src/(.*)$': '<rootDir>/src/$1',
    '^app/(.*)$': '<rootDir>/src/app/$1',
    '^assets/(.*)$': '<rootDir>/src/assets/$1',
    '^environments/(.*)$': '<rootDir>/src/environments/$1',
  },

  // Angular-specific globals
  globals: {
    'ts-jest': {
      tsconfig: 'tsconfig.spec.json',
    },
  },
};
