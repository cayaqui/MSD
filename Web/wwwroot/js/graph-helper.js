// Helper functions for Microsoft Graph integration

window.GraphHelper = {
    // Get access token for Microsoft Graph
    getAccessToken: async function () {
        try {
            console.log('[GraphHelper] Attempting to get access token for Microsoft Graph...');
            
            // Try multiple ways to get the MSAL instance
            let msalInstance = window.AuthenticationService?.msalInstance;
            
            // If not found, try the global msal instance
            if (!msalInstance && window.msal) {
                msalInstance = window.msal;
            }
            
            // If still not found, wait a bit and try again (in case it's still initializing)
            if (!msalInstance) {
                await new Promise(resolve => setTimeout(resolve, 1000));
                msalInstance = window.AuthenticationService?.msalInstance || window.msal;
            }
            
            if (!msalInstance) {
                console.warn('[GraphHelper] MSAL instance not found - Graph API features will be unavailable');
                return null;
            }

            // Define the request for Graph API
            const request = {
                scopes: ["User.Read", "User.ReadBasic.All", "profile", "openid"],
                account: msalInstance.getAllAccounts()[0]
            };

            try {
                // Try to get token silently first
                const response = await msalInstance.acquireTokenSilent(request);
                console.log('[GraphHelper] Token acquired silently');
                return response.accessToken;
            } catch (silentError) {
                console.warn('[GraphHelper] Silent token acquisition failed, trying interactive...');
                
                // If silent fails, try interactive
                try {
                    const response = await msalInstance.acquireTokenPopup(request);
                    console.log('[GraphHelper] Token acquired via popup');
                    return response.accessToken;
                } catch (interactiveError) {
                    console.error('[GraphHelper] Interactive token acquisition failed:', interactiveError);
                    return null;
                }
            }
        } catch (error) {
            console.error('[GraphHelper] Error getting access token:', error);
            return null;
        }
    },

    // Get user photo from Microsoft Graph
    getUserPhoto: async function () {
        try {
            console.log('[GraphHelper] Getting user photo...');
            
            const accessToken = await this.getAccessToken();
            if (!accessToken) {
                console.warn('[GraphHelper] No access token available');
                return null;
            }

            const response = await fetch('https://graph.microsoft.com/v1.0/me/photo/$value', {
                headers: {
                    'Authorization': `Bearer ${accessToken}`
                }
            });

            if (response.ok) {
                const blob = await response.blob();
                const reader = new FileReader();
                
                return new Promise((resolve) => {
                    reader.onloadend = function() {
                        console.log('[GraphHelper] User photo retrieved successfully');
                        resolve(reader.result);
                    };
                    reader.readAsDataURL(blob);
                });
            } else {
                console.warn('[GraphHelper] Failed to get user photo:', response.status);
                return null;
            }
        } catch (error) {
            console.error('[GraphHelper] Error getting user photo:', error);
            return null;
        }
    },

    // Get user profile from Microsoft Graph
    getUserProfile: async function () {
        try {
            console.log('[GraphHelper] Getting user profile...');
            
            const accessToken = await this.getAccessToken();
            if (!accessToken) {
                console.warn('[GraphHelper] No access token available');
                return null;
            }

            const response = await fetch('https://graph.microsoft.com/v1.0/me', {
                headers: {
                    'Authorization': `Bearer ${accessToken}`
                }
            });

            if (response.ok) {
                const data = await response.json();
                console.log('[GraphHelper] User profile retrieved successfully');
                return data;
            } else {
                console.warn('[GraphHelper] Failed to get user profile:', response.status);
                return null;
            }
        } catch (error) {
            console.error('[GraphHelper] Error getting user profile:', error);
            return null;
        }
    }
};