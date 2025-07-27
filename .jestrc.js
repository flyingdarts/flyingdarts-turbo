{
    transform: {
        '^.+\\.tsx?$': [
          'ts-jest',
          // required due to custom location of tsconfig.json configuration file
          // https://kulshekhar.github.io/ts-jest/docs/getting-started/options/tsconfig
          {tsconfig: './config/tsconfig.json'},
        ],
      }
}