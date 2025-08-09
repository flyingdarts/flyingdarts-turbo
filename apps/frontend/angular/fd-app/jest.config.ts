import { configPackage } from '@flyingdarts/config';
import type { Config } from 'jest';

const config: Config = {
  ...configPackage.jest.angular,
  rootDir: '.',
};
export default config;
