import { defineConfig } from 'vite';

export default defineConfig({
  server: {
    middlewareMode: false,
    hmr: {
      overlay: false // Disable error overlay for malformed URIs
    }
  },
  plugins: [
    {
      name: 'fix-malformed-urls',
      configureServer(server) {
        server.middlewares.use((req, res, next) => {
          if (!req.url) {
            return next();
          }

          // First try to sanitize known problematic patterns before attempting to decode
          const originalUrl = req.url;
          try {
            // Pre-sanitize URL
            if (req.url.includes('%')) {
              req.url = req.url
                .replace(/%(?![0-9A-F]{2})/gi, '%25') // Replace standalone % with %25
                .replace(/%F([^0-9A-F]|$)/gi, '%2F$1') // Fix %F -> %2F (common slash issue)
                .replace(/%([0-9A-F])([^0-9A-F]|$)/gi, '%0$1$2') // Fix single digit hex
                .replace(/%([^0-9A-F]{2})/gi, '%25$1') // Fix illegal hex sequences
                .replace(/%$/, '%25'); // Fix trailing %
            }

            // Now try to decode
            decodeURI(req.url);
            next();
          } catch (e) {
            console.warn(`URL decode failed for: ${originalUrl}, attempting recovery`);

            try {
              // More aggressive sanitization
              req.url = req.url
                // Replace all potentially problematic characters
                .replace(/%[^0-9A-F]/gi, '%25')
                .replace(/%[0-9A-F]$/gi, '%250') // Fix trailing single hex
                .replace(/%[0-9A-F][^0-9A-F]/gi, (match) => {
                  return `%${match.charAt(1)}0${match.charAt(2)}`;
                });

              // Verify it's now valid
              try {
                decodeURI(req.url);
                console.log(`Successfully fixed URL: ${req.url}`);
              } catch (innerError) {
                // If still malformed, use fallback
                console.error(`Failed to fix URL, using fallback: ${req.url}`);
                req.url = '/';
              }
            } catch (fixError) {
              // Last resort fallback
              console.error(`Error during URL fixing: ${fixError}`);
              req.url = '/';
            }
            next();
          }
        });
      }
    }
  ]
});
