import type { Config } from 'jest';

export const jestBase: Config = {
  setupFilesAfterEnv: ['<rootDir>/setup-jest.ts'],
  testEnvironment: 'jsdom',
  moduleFileExtensions: ['ts', 'html', 'js', 'json'],
  collectCoverageFrom: [
    '**/apps/**/angular/**/src/app/**/*.ts',
    '**/apps/**/angular/**/!src/main.ts',
    '**/apps/**/angular/**/!src/environments/**/*.ts',
    '**/apps/**/angular/**/!src/app/**/*.module.ts',
  ],
};
