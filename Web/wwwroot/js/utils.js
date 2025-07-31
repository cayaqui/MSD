// =================================
// EzPro.SD - Utility Functions
// Funciones adicionales necesarias
// =================================

// Agregar a window.EzPro object
window.EzPro.Utils = {
    // Check if mobile view
    isMobile: function() {
        return window.innerWidth <= 991.98;
    },

    // Local Storage helpers
    setLocalStorage: function(key, value) {
        try {
            localStorage.setItem(key, value);
            return true;
        } catch (e) {
            console.error('Error saving to localStorage:', e);
            return false;
        }
    },

    getLocalStorage: function(key) {
        try {
            return localStorage.getItem(key);
        } catch (e) {
            console.error('Error reading from localStorage:', e);
            return null;
        }
    },

    removeLocalStorage: function(key) {
        try {
            localStorage.removeItem(key);
            return true;
        } catch (e) {
            console.error('Error removing from localStorage:', e);
            return false;
        }
    }
};

// Agregar a window.EzProInterop
window.EzProInterop.registerResizeHandler = function(dotNetRef) {
    let resizeTimer;
    const handleResize = function() {
        clearTimeout(resizeTimer);
        resizeTimer = setTimeout(function() {
            dotNetRef.invokeMethodAsync('OnWindowResize', window.EzPro.Utils.isMobile());
        }, 250);
    };

    window.addEventListener('resize', handleResize);
    
    // Return cleanup function
    return function() {
        window.removeEventListener('resize', handleResize);
    };
};

window.EzProInterop.registerKeyboardShortcuts = function(dotNetRef) {
    const handleKeydown = function(e) {
        // Ctrl+K or Cmd+K for search
        if ((e.ctrlKey || e.metaKey) && e.key === 'k') {
            e.preventDefault();
            dotNetRef.invokeMethodAsync('OpenSearchShortcut');
        }
        
        // Escape to close modals/dropdowns
        if (e.key === 'Escape') {
            // Close any open dropdowns
            const openDropdowns = document.querySelectorAll('.dropdown-menu.show');
            openDropdowns.forEach(dropdown => {
                dropdown.classList.remove('show');
            });
        }
    };

    document.addEventListener('keydown', handleKeydown);
    
    // Return cleanup function
    return function() {
        document.removeEventListener('keydown', handleKeydown);
    };
};

window.EzProInterop.initializeScrollbar = function(elementId) {
    const element = document.getElementById(elementId);
    if (element && typeof SimpleBar !== 'undefined') {
        // Check if SimpleBar is already initialized
        if (!element.SimpleBar) {
            new SimpleBar(element, {
                autoHide: true,
                scrollbarMinSize: 25,
                scrollbarMaxSize: 200
            });
        }
    }
};