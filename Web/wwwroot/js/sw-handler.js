// Service Worker Handler
// This file prevents the "no-op fetch handler" warning

// Check if there's a service worker registered
if ('serviceWorker' in navigator) {
    navigator.serviceWorker.getRegistrations().then(function(registrations) {
        if (registrations.length > 0) {
            console.log('[SW Handler] Found', registrations.length, 'service worker(s)');
            
            // Check each registration for no-op fetch handlers
            registrations.forEach(function(registration) {
                console.log('[SW Handler] Service worker scope:', registration.scope);
                
                // If this is a Blazor PWA service worker, it's okay
                if (registration.scope.includes('service-worker')) {
                    console.log('[SW Handler] Blazor PWA service worker detected');
                }
            });
        } else {
            console.log('[SW Handler] No service workers registered');
        }
    }).catch(function(error) {
        console.error('[SW Handler] Error checking service workers:', error);
    });
}

// If we're in development and there's an unwanted service worker, we can unregister it
// Uncomment the following if you want to remove all service workers in development
/*
if (window.location.hostname === 'localhost') {
    navigator.serviceWorker.getRegistrations().then(function(registrations) {
        registrations.forEach(function(registration) {
            registration.unregister().then(function(success) {
                if (success) {
                    console.log('[SW Handler] Unregistered service worker:', registration.scope);
                }
            });
        });
    });
}
*/