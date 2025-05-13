const { env } = require('process');

const target = env.ASPNETCORE_HTTPS_PORT ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}` :
  env.ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0] : 'https://localhost:44362';

const PROXY_CONFIG = [
  {
    context: [
      "/weatherforecast",
      "/api",
      "/auth",
      "/Auth"
    ],
    target: 'https://localhost:44362',
    secure: false,
    logLevel: "debug",
    changeOrigin: true,
    onProxyRes: function (proxyRes, req, res) {
      // For debugging purposes
      console.log('Response from backend:', proxyRes.statusCode);
    }
  }
]

module.exports = PROXY_CONFIG;
