// MSAL Debug Helper
console.log('[MSAL Debug] Script loaded');

// Override console methods to capture MSAL logs
const originalLog = console.log;
const originalWarn = console.warn;
const originalError = console.error;

console.log = function() {
    const args = Array.from(arguments);
    const message = args.join(' ');
    
    // Capture MSAL-specific logs
    if (message.includes('MSAL') || message.includes('msal') || message.includes('authentication')) {
        originalLog.apply(console, ['[MSAL Debug Captured]'].concat(args));
    }
    
    originalLog.apply(console, arguments);
};

console.warn = function() {
    const args = Array.from(arguments);
    const message = args.join(' ');
    
    if (message.includes('MSAL') || message.includes('msal') || message.includes('authentication')) {
        originalWarn.apply(console, ['[MSAL Debug Warning]'].concat(args));
    }
    
    originalWarn.apply(console, arguments);
};

console.error = function() {
    const args = Array.from(arguments);
    const message = args.join(' ');
    
    if (message.includes('MSAL') || message.includes('msal') || message.includes('authentication')) {
        originalError.apply(console, ['[MSAL Debug Error]'].concat(args));
    }
    
    originalError.apply(console, arguments);
};

// Monitor for MSAL-specific events
window.addEventListener('message', function(event) {
    if (event.data && (event.data.type === 'msal' || event.origin.includes('login.microsoftonline.com'))) {
        console.log('[MSAL Debug] Message event:', event.data);
    }
});

console.log('[MSAL Debug] Monitoring active');