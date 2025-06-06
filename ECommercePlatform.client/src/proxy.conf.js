const { env } = require('process');

const target = env.ASPNETCORE_HTTPS_PORT ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}` :
  env.ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0] : 'https://localhost:44362';

const PROXY_CONFIG = [
  {
    context: [
      "/weatherforecast",
      "/api/**",
      "/auth/**",
      "/Auth/**"
    ],
    target: target,
    secure: false,
    logLevel: "debug",
    changeOrigin: true,
    headers: {
      Connection: 'Keep-Alive'
    },
    onProxyReq: function (proxyReq, req, res) {
      // Log the original URL
      console.log('Proxying:', req.url);

      // Fix malformed URLs before sending to backend
      if (req.url && req.url.includes('%')) {
        try {
          // Attempt to decode
          decodeURIComponent(req.url);
        } catch (e) {
          console.error('Malformed URL detected in proxy:', req.url);
          // Send 302 redirect to 404 page
          res.writeHead(302, {
            'Location': '/404?error=malformed'
          });
          res.end();
          return;
        }
      }
    },
    onError: function (err, req, res) {
      console.error('Proxy error:', err);
      res.writeHead(500, {
        'Content-Type': 'text/plain'
      });
      res.end('Proxy error: ' + err.message);
    }
  }
];

module.exports = PROXY_CONFIG;
