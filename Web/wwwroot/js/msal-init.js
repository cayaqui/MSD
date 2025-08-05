// MSAL Initialization Helper
// This file helps ensure MSAL is properly initialized for Graph API calls

window.MsalHelper = {
    msalInstance: null,
    
    // Initialize MSAL after Blazor has started
    initialize: function() {
        console.log('[MsalHelper] Initializing MSAL helper...');
        
        // Check if AuthenticationService is available
        if (window.AuthenticationService && window.AuthenticationService.msalInstance) {
            this.msalInstance = window.AuthenticationService.msalInstance;
            console.log('[MsalHelper] MSAL instance found from AuthenticationService');
            
            // Also expose it globally for GraphHelper
            window.msalInstance = this.msalInstance;
            return true;
        }
        
        console.log('[MsalHelper] MSAL instance not available yet');
        return false;
    },
    
    // Wait for MSAL to be ready
    waitForMsal: async function(maxAttempts = 10, delay = 500) {
        console.log('[MsalHelper] Waiting for MSAL to be ready...');
        
        for (let i = 0; i < maxAttempts; i++) {
            if (this.initialize()) {
                console.log('[MsalHelper] MSAL is ready');
                return true;
            }
            
            await new Promise(resolve => setTimeout(resolve, delay));
        }
        
        console.warn('[MsalHelper] MSAL not available after waiting');
        return false;
    },
    
    // Get access token (wrapper for easier access)
    getAccessToken: async function(scopes = ["User.Read"]) {
        if (!this.msalInstance) {
            await this.waitForMsal();
        }
        
        if (!this.msalInstance) {
            console.warn('[MsalHelper] Cannot get access token - MSAL not initialized');
            return null;
        }
        
        try {
            const accounts = this.msalInstance.getAllAccounts();
            if (accounts.length === 0) {
                console.warn('[MsalHelper] No accounts found');
                return null;
            }
            
            const request = {
                scopes: scopes,
                account: accounts[0]
            };
            
            try {
                const response = await this.msalInstance.acquireTokenSilent(request);
                return response.accessToken;
            } catch (error) {
                console.warn('[MsalHelper] Silent token acquisition failed:', error);
                // Don't try interactive in automated scenarios
                return null;
            }
        } catch (error) {
            console.error('[MsalHelper] Error getting access token:', error);
            return null;
        }
    }
};

// Update GraphHelper to use MsalHelper
if (window.GraphHelper) {
    const originalGetAccessToken = window.GraphHelper.getAccessToken;
    
    window.GraphHelper.getAccessToken = async function() {
        console.log('[GraphHelper] Using MsalHelper for token acquisition');
        
        // First try MsalHelper
        const token = await window.MsalHelper.getAccessToken(["User.Read", "User.ReadBasic.All"]);
        if (token) {
            return token;
        }
        
        // Fallback to original method
        return originalGetAccessToken.call(this);
    };
}

// Initialize when the page is ready
// Blazor WebAssembly starts automatically, so we just need to wait a bit
document.addEventListener('DOMContentLoaded', () => {
    // Wait for Blazor and MSAL to initialize
    setTimeout(() => {
        console.log('[MsalHelper] Checking for MSAL availability...');
        window.MsalHelper.waitForMsal();
    }, 2000);
});

// Also try to initialize when this script loads (in case DOM is already ready)
if (document.readyState === 'complete' || document.readyState === 'interactive') {
    setTimeout(() => {
        console.log('[MsalHelper] Document ready, checking MSAL...');
        window.MsalHelper.waitForMsal();
    }, 2000);
}