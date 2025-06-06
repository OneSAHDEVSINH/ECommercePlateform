module.exports = function urlFix(req, res, next) {
  if (!req.url) {
    next();
    return;
  }

  // Save original URL for logging
  const originalUrl = req.url;

  try {
    // First try to pre-sanitize common problematic patterns
    if (req.url.includes('%')) {
      req.url = req.url
        .replace(/%(?![0-9A-F]{2})/gi, '%25') // Replace standalone % with %25
        .replace(/%F([^0-9A-F]|$)/gi, '%2F$1') // Fix %F -> %2F (common slash issue)
        .replace(/%([0-9A-F])([^0-9A-F]|$)/gi, '%0$1$2') // Fix single digit hex
        .replace(/%([^0-9A-F]{2})/gi, '%25$1') // Fix illegal hex sequences
        .replace(/%$/, '%25'); // Fix trailing %
    }

    // Now try to decode and re-encode to normalize
    req.url = encodeURI(decodeURI(req.url));

  } catch (e) {
    console.warn(`URL decode/encode failed for: ${originalUrl}, attempting recovery`);

    try {
      // More aggressive sanitization
      req.url = req.url
        // Handle various malformed patterns
        .replace(/%[^0-9A-F]/gi, '%25')  // Replace % followed by non-hex
        .replace(/%[0-9A-F]$/gi, '%250') // Fix trailing single hex
        .replace(/%[0-9A-F][^0-9A-F]/gi, (match) => {
          return `%${match.charAt(1)}0${match.charAt(2)}`;
        });

      // Verify the URL can now be properly decoded
      try {
        req.url = encodeURI(decodeURI(req.url));
        console.log(`Successfully sanitized URL: ${originalUrl} → ${req.url}`);
      } catch (innerError) {
        console.error(`Sanitization failed, removing non-ASCII: ${req.url}`);
        // Strip non-ASCII characters as a last resort
        req.url = req.url.replace(/[^\x00-\x7F]/g, '');
      }
    } catch (fixError) {
      console.error(`Complete URL sanitization failed: ${fixError.message}`);
      // Last resort: keep only safe URL characters
      req.url = req.url.replace(/[^\w\s\/\-\.]/g, '');

      // If nothing is left or it's still problematic, redirect to home
      if (!req.url || req.url === '') {
        req.url = '/';
      }
    }
  }

  next();
};
