module.exports = {
  testEnvironment: 'jsdom',
  setupFilesAfterEnv: ['<rootDir>/src/setupTests.ts'],
  moduleNameMapper: {
    '\\.(css|less|scss|sass)$': 'identity-obj-proxy',
    '^axios$': 'axios/dist/axios.js'
  },
  transformIgnorePatterns: [
    'node_modules/(?!(axios)/.*)'
  ]
};