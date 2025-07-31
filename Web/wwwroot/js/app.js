// =================================
// EzPro.SD - Main Application JavaScript
// Sistema de Control de Proyectos
// =================================

// Application namespace
window.EzPro = window.EzPro || {};

// Initialize application
window.EzPro.init = function () {
    console.log('EzPro.SD Application Initialized');

    // Initialize theme
    EzPro.Theme.init();

    // Initialize sidebar
    EzPro.Sidebar.init();

    // Initialize tooltips and popovers
    EzPro.initializeBootstrapComponents();
};

// Theme Management
window.EzPro.Theme = {
    init: function () {
        // Check for saved theme preference or default to 'light'
        const savedTheme = localStorage.getItem('theme') || 'light';
        this.setTheme(savedTheme);
    },

    setTheme: function (theme) {
        document.documentElement.setAttribute('data-bs-theme', theme);
        localStorage.setItem('theme', theme);

        // Update theme toggle button if exists
        const themeToggle = document.querySelector('.theme-toggle');
        if (themeToggle) {
            themeToggle.innerHTML = theme === 'light'
                ? '<i class="fas fa-moon"></i>'
                : '<i class="fas fa-sun"></i>';
        }
    },

    toggle: function () {
        const currentTheme = document.documentElement.getAttribute('data-bs-theme') || 'light';
        const newTheme = currentTheme === 'light' ? 'dark' : 'light';
        this.setTheme(newTheme);
    }
};

// Sidebar Management
window.EzPro.Sidebar = {
    init: function () {
        const sidebar = document.querySelector('.sidebar');
        const sidebarToggle = document.querySelector('.sidebar-toggle');
        const sidebarOverlay = document.querySelector('.sidebar-overlay');

        if (sidebarToggle) {
            sidebarToggle.addEventListener('click', () => this.toggle());
        }

        if (sidebarOverlay) {
            sidebarOverlay.addEventListener('click', () => this.hide());
        }

        // Handle window resize
        window.addEventListener('resize', () => {
            if (window.innerWidth > 991) {
                this.hideOverlay();
            }
        });

        // Initialize dropdown menus in sidebar
        this.initDropdowns();

        // Check saved state
        const isCollapsed = localStorage.getItem('sidebarCollapsed') === 'true';
        if (isCollapsed && sidebar) {
            sidebar.classList.add('collapsed');
        }
    },

    toggle: function () {
        const sidebar = document.querySelector('.sidebar');
        const isCollapsed = sidebar.classList.contains('collapsed');

        if (window.innerWidth <= 991) {
            // Mobile behavior
            sidebar.classList.toggle('show');
            this.toggleOverlay();
        } else {
            // Desktop behavior
            sidebar.classList.toggle('collapsed');
            localStorage.setItem('sidebarCollapsed', !isCollapsed);
        }
    },

    show: function () {
        const sidebar = document.querySelector('.sidebar');
        sidebar.classList.add('show');
        this.showOverlay();
    },

    hide: function () {
        const sidebar = document.querySelector('.sidebar');
        sidebar.classList.remove('show');
        this.hideOverlay();
    },

    collapse: function () {
        const sidebar = document.querySelector('.sidebar');
        if (sidebar) {
            sidebar.classList.add('collapsed');
        }
    },

    expand: function () {
        const sidebar = document.querySelector('.sidebar');
        if (sidebar) {
            sidebar.classList.remove('collapsed');
        }
    },

    isCollapsed: function () {
        const sidebar = document.querySelector('.sidebar');
        return sidebar ? sidebar.classList.contains('collapsed') : false;
    },

    showOverlay: function () {
        const overlay = document.querySelector('.sidebar-overlay');
        if (overlay) {
            overlay.classList.add('show');
            document.body.style.overflow = 'hidden';
        }
    },

    hideOverlay: function () {
        const overlay = document.querySelector('.sidebar-overlay');
        if (overlay) {
            overlay.classList.remove('show');
            document.body.style.overflow = '';
        }
    },

    toggleOverlay: function () {
        const overlay = document.querySelector('.sidebar-overlay');
        if (overlay && overlay.classList.contains('show')) {
            this.hideOverlay();
        } else {
            this.showOverlay();
        }
    },

    initDropdowns: function () {
        const dropdownToggles = document.querySelectorAll('.sidebar .nav-item.dropdown > .nav-link');

        dropdownToggles.forEach(toggle => {
            toggle.addEventListener('click', function (e) {
                e.preventDefault();
                const parent = this.parentElement;
                const wasOpen = parent.classList.contains('show');

                // Close all other dropdowns
                document.querySelectorAll('.sidebar .nav-item.dropdown.show').forEach(item => {
                    item.classList.remove('show');
                });

                // Toggle current dropdown
                if (!wasOpen) {
                    parent.classList.add('show');
                }
            });
        });
    }
};

// Bootstrap Components Initialization
window.EzPro.initializeBootstrapComponents = function () {
    // Initialize tooltips
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // Initialize popovers
    const popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
    popoverList = popoverTriggerList.map(function (popoverTriggerEl) {
        return new bootstrap.Popover(popoverTriggerEl);
    });

    // Initialize toasts
    const toastElList = [].slice.call(document.querySelectorAll('.toast'));
    toastElList.map(function (toastEl) {
        return new bootstrap.Toast(toastEl);
    });
};

// Utility Functions
window.EzPro.Utils = {
    // Format currency
    formatCurrency: function (amount, currency = 'USD') {
        return new Intl.NumberFormat('es-CL', {
            style: 'currency',
            currency: currency
        }).format(amount);
    },

    // Format date
    formatDate: function (date, format = 'short') {
        if (!date) return '';
        const d = new Date(date);

        if (format === 'short') {
            return d.toLocaleDateString('es-CL');
        } else if (format === 'long') {
            return d.toLocaleDateString('es-CL', {
                weekday: 'long',
                year: 'numeric',
                month: 'long',
                day: 'numeric'
            });
        } else if (format === 'time') {
            return d.toLocaleTimeString('es-CL', {
                hour: '2-digit',
                minute: '2-digit'
            });
        }

        return d.toLocaleString('es-CL');
    },

    // Format number
    formatNumber: function (number, decimals = 2) {
        return new Intl.NumberFormat('es-CL', {
            minimumFractionDigits: decimals,
            maximumFractionDigits: decimals
        }).format(number);
    },

    // Format percentage
    formatPercentage: function (value, decimals = 1) {
        return `${this.formatNumber(value, decimals)}%`;
    },

    // Debounce function
    debounce: function (func, wait) {
        let timeout;
        return function executedFunction(...args) {
            const later = () => {
                clearTimeout(timeout);
                func(...args);
            };
            clearTimeout(timeout);
            timeout = setTimeout(later, wait);
        };
    },

    // Copy to clipboard
    copyToClipboard: async function (text) {
        try {
            await navigator.clipboard.writeText(text);
            return true;
        } catch (err) {
            console.error('Failed to copy text: ', err);
            return false;
        }
    }
};

// Modal Helper
window.EzPro.Modal = {
    show: function (modalId) {
        const modalEl = document.getElementById(modalId);
        if (modalEl) {
            const modal = new bootstrap.Modal(modalEl);
            modal.show();
            return modal;
        }
        return null;
    },

    hide: function (modalId) {
        const modalEl = document.getElementById(modalId);
        if (modalEl) {
            const modal = bootstrap.Modal.getInstance(modalEl);
            if (modal) {
                modal.hide();
            }
        }
    },

    toggle: function (modalId) {
        const modalEl = document.getElementById(modalId);
        if (modalEl) {
            const modal = bootstrap.Modal.getInstance(modalEl) || new bootstrap.Modal(modalEl);
            modal.toggle();
            return modal;
        }
        return null;
    }
};

// File Upload Helper
window.EzPro.FileUpload = {
    validateFile: function (file, allowedExtensions, maxSize) {
        const extension = '.' + file.name.split('.').pop().toLowerCase();
        const sizeInMB = file.size / (1024 * 1024);

        if (allowedExtensions && !allowedExtensions.includes(extension)) {
            return {
                valid: false,
                error: `Tipo de archivo no permitido. Extensiones permitidas: ${allowedExtensions.join(', ')}`
            };
        }

        if (maxSize && file.size > maxSize) {
            return {
                valid: false,
                error: `El archivo excede el tamaño máximo permitido de ${maxSize / (1024 * 1024)} MB`
            };
        }

        return { valid: true };
    },

    readAsDataURL: function (file) {
        return new Promise((resolve, reject) => {
            const reader = new FileReader();
            reader.onload = e => resolve(e.target.result);
            reader.onerror = reject;
            reader.readAsDataURL(file);
        });
    }
};

// Initialize on DOM ready
document.addEventListener('DOMContentLoaded', function () {
    // Initialize application
    EzPro.init();
});

// Blazor interop
window.initializeApp = function () {
    EzPro.init();
};

window.initializeBootstrap = function () {
    EzPro.initializeBootstrapComponents();
};

// Agregar al app.js existente
window.getWindowWidth = function () {
    return window.innerWidth;
};

window.setTheme = function (theme) {
    document.documentElement.setAttribute('data-theme', theme);
    localStorage.setItem('theme', theme);
};

window.addResizeListener = function (dotNetRef) {
    let resizeTimer;
    window.addEventListener('resize', function () {
        clearTimeout(resizeTimer);
        resizeTimer = setTimeout(function () {
            dotNetRef.invokeMethodAsync('OnWindowResize', window.innerWidth);
        }, 250);
    });
};