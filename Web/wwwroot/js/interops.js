// =================================
// EzPro.SD - Blazor JavaScript Interop
// Sistema de Control de Proyectos
// =================================

window.EzProInterop = {};

// EzDropdown component functions
window.EzDropdown = {
    initialize: function (dropdownId, dotNetRef) {
        const dropdownElement = document.getElementById(dropdownId);
        if (!dropdownElement) return false;

        // Initialize Bootstrap dropdown
        const dropdown = new bootstrap.Dropdown(dropdownElement);

        // Store reference for later disposal
        dropdownElement._dropdown = dropdown;
        dropdownElement._dotNetRef = dotNetRef;

        // Add event listeners
        dropdownElement.addEventListener('shown.bs.dropdown', async function () {
            if (dotNetRef) {
                await dotNetRef.invokeMethodAsync('OnDropdownShown');
            }
        });

        dropdownElement.addEventListener('hidden.bs.dropdown', async function () {
            if (dotNetRef) {
                await dotNetRef.invokeMethodAsync('OnDropdownHidden');
            }
        });

        return true;
    },

    show: function (dropdownId) {
        const dropdownElement = document.getElementById(dropdownId);
        if (dropdownElement && dropdownElement._dropdown) {
            dropdownElement._dropdown.show();
            return true;
        }
        return false;
    },

    hide: function (dropdownId) {
        const dropdownElement = document.getElementById(dropdownId);
        if (dropdownElement && dropdownElement._dropdown) {
            dropdownElement._dropdown.hide();
            return true;
        }
        return false;
    },

    toggle: function (dropdownId) {
        const dropdownElement = document.getElementById(dropdownId);
        if (dropdownElement && dropdownElement._dropdown) {
            dropdownElement._dropdown.toggle();
            return true;
        }
        return false;
    },

    dispose: function (dropdownId) {
        const dropdownElement = document.getElementById(dropdownId);
        if (dropdownElement) {
            if (dropdownElement._dropdown) {
                dropdownElement._dropdown.dispose();
                dropdownElement._dropdown = null;
            }
            if (dropdownElement._dotNetRef) {
                dropdownElement._dotNetRef = null;
            }
        }
    }
};

// EzModal component functions
window.EzModal = {
    initialize: function (modalId, dotNetRef) {
        const modalElement = document.getElementById(modalId);
        if (!modalElement) return false;

        // Initialize Bootstrap modal
        const modal = new bootstrap.Modal(modalElement, {
            backdrop: 'static',
            keyboard: false
        });

        // Store references
        modalElement._modal = modal;
        modalElement._dotNetRef = dotNetRef;

        // Add event listeners
        modalElement.addEventListener('shown.bs.modal', async function () {
            if (dotNetRef) {
                await dotNetRef.invokeMethodAsync('OnModalShown');
            }
        });

        modalElement.addEventListener('hidden.bs.modal', async function () {
            if (dotNetRef) {
                await dotNetRef.invokeMethodAsync('OnModalHidden');
            }
        });

        return true;
    },

    show: function (modalId) {
        const modalElement = document.getElementById(modalId);
        if (modalElement && modalElement._modal) {
            modalElement._modal.show();
            return true;
        }
        return false;
    },

    hide: function (modalId) {
        const modalElement = document.getElementById(modalId);
        if (modalElement && modalElement._modal) {
            modalElement._modal.hide();
            return true;
        }
        return false;
    },

    registerEscapeHandler: function (dotNetRef) {
        document.addEventListener('keydown', async function (e) {
            if (e.key === 'Escape' && dotNetRef) {
                await dotNetRef.invokeMethodAsync('HandleEscapeKey');
            }
        });
    },

    dispose: function (modalId) {
        const modalElement = document.getElementById(modalId);
        if (modalElement) {
            if (modalElement._modal) {
                modalElement._modal.dispose();
                modalElement._modal = null;
            }
            if (modalElement._dotNetRef) {
                modalElement._dotNetRef = null;
            }
        }
    }
};

// Initialize Bootstrap components after Blazor render
window.EzProInterop.initializeBootstrap = function () {
    // Tooltips
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl, {
            trigger: 'hover'
        });
    });

    // Popovers
    const popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
    popoverTriggerList.map(function (popoverTriggerEl) {
        return new bootstrap.Popover(popoverTriggerEl);
    });
};

// Modal functions
window.EzProInterop.showModal = function (modalId) {
    const modalElement = document.getElementById(modalId);
    if (modalElement) {
        const modal = new bootstrap.Modal(modalElement);
        modal.show();
        return true;
    }
    return false;
};

window.EzProInterop.hideModal = function (modalId) {
    const modalElement = document.getElementById(modalId);
    if (modalElement) {
        const modal = bootstrap.Modal.getInstance(modalElement);
        if (modal) {
            modal.hide();
            return true;
        }
    }
    return false;
};

// Toast notifications
window.EzProInterop.showToast = function (message, type = 'info', duration = 5000) {
    // Create toast container if it doesn't exist
    let toastContainer = document.getElementById('toast-container');
    if (!toastContainer) {
        toastContainer = document.createElement('div');
        toastContainer.id = 'toast-container';
        toastContainer.className = 'toast-container position-fixed top-0 end-0 p-3';
        toastContainer.style.zIndex = '1080';
        document.body.appendChild(toastContainer);
    }

    // Create toast element
    const toastId = 'toast-' + Date.now();
    const toastHtml = `
        <div id="${toastId}" class="toast" role="alert" aria-live="assertive" aria-atomic="true">
            <div class="toast-header bg-${type} text-white">
                <i class="fas fa-${getToastIcon(type)} me-2"></i>
                <strong class="me-auto">${getToastTitle(type)}</strong>
                <button type="button" class="btn-close btn-close-white" data-bs-dismiss="toast"></button>
            </div>
            <div class="toast-body">
                ${message}
            </div>
        </div>
    `;

    toastContainer.insertAdjacentHTML('beforeend', toastHtml);

    const toastElement = document.getElementById(toastId);
    const toast = new bootstrap.Toast(toastElement, {
        autohide: true,
        delay: duration
    });

    // Remove toast from DOM after it's hidden
    toastElement.addEventListener('hidden.bs.toast', function () {
        toastElement.remove();
    });

    toast.show();
};

function getToastIcon(type) {
    const icons = {
        'success': 'check-circle',
        'danger': 'exclamation-circle',
        'warning': 'exclamation-triangle',
        'info': 'info-circle',
        'primary': 'bell'
    };
    return icons[type] || 'info-circle';
}

function getToastTitle(type) {
    const titles = {
        'success': 'Éxito',
        'danger': 'Error',
        'warning': 'Advertencia',
        'info': 'Información',
        'primary': 'Notificación'
    };
    return titles[type] || 'Notificación';
}

// Clipboard functions
window.EzProInterop.copyToClipboard = async function (text) {
    try {
        await navigator.clipboard.writeText(text);
        EzProInterop.showToast('Copiado al portapapeles', 'success', 3000);
        return true;
    } catch (err) {
        console.error('Error al copiar:', err);
        EzProInterop.showToast('Error al copiar al portapapeles', 'danger', 3000);
        return false;
    }
};

// Local storage functions
window.EzProInterop.localStorage = {
    setItem: function (key, value) {
        try {
            localStorage.setItem(key, JSON.stringify(value));
            return true;
        } catch (e) {
            console.error('Error saving to localStorage:', e);
            return false;
        }
    },

    getItem: function (key) {
        try {
            const item = localStorage.getItem(key);
            return item ? JSON.parse(item) : null;
        } catch (e) {
            console.error('Error reading from localStorage:', e);
            return null;
        }
    },

    removeItem: function (key) {
        try {
            localStorage.removeItem(key);
            return true;
        } catch (e) {
            console.error('Error removing from localStorage:', e);
            return false;
        }
    },

    clear: function () {
        try {
            localStorage.clear();
            return true;
        } catch (e) {
            console.error('Error clearing localStorage:', e);
            return false;
        }
    }
};

// File download
window.EzProInterop.downloadFile = function (filename, content, mimeType) {
    const blob = new Blob([content], { type: mimeType });
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = filename;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    window.URL.revokeObjectURL(url);
};

// Print functions
window.EzProInterop.print = function (elementId) {
    const element = document.getElementById(elementId);
    if (element) {
        const printWindow = window.open('', '_blank');
        printWindow.document.write('<html><head><title>Imprimir</title>');

        // Copy all stylesheets
        const styles = document.querySelectorAll('link[rel="stylesheet"], style');
        styles.forEach(style => {
            printWindow.document.write(style.outerHTML);
        });

        printWindow.document.write('</head><body>');
        printWindow.document.write(element.innerHTML);
        printWindow.document.write('</body></html>');
        printWindow.document.close();

        printWindow.onload = function () {
            printWindow.print();
            printWindow.close();
        };
    }
};

// Scroll functions
window.EzProInterop.scrollToTop = function () {
    window.scrollTo({ top: 0, behavior: 'smooth' });
};

window.EzProInterop.scrollToElement = function (elementId) {
    const element = document.getElementById(elementId);
    if (element) {
        element.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
};

// Focus management
window.EzProInterop.focusElement = function (elementId) {
    const element = document.getElementById(elementId);
    if (element) {
        element.focus();
        return true;
    }
    return false;
};

// Window dimensions
window.EzProInterop.getWindowDimensions = function () {
    return {
        width: window.innerWidth,
        height: window.innerHeight
    };
};

// Prevent default form submission (for Blazor forms)
window.EzProInterop.preventDefaultFormSubmit = function (formId) {
    const form = document.getElementById(formId);
    if (form) {
        form.addEventListener('submit', function (e) {
            e.preventDefault();
            return false;
        });
    }
};

// Confirmation dialog
window.EzProInterop.confirm = function (message, title = 'Confirmar') {
    return window.confirm(message);
};

// Set page title
window.EzProInterop.setPageTitle = function (title) {
    document.title = title + ' - EzPro.SD';
};

// Toggle full screen
window.EzProInterop.toggleFullScreen = function () {
    if (!document.fullscreenElement) {
        document.documentElement.requestFullscreen();
    } else {
        if (document.exitFullscreen) {
            document.exitFullscreen();
        }
    }
};

// Check if element is in viewport
window.EzProInterop.isInViewport = function (elementId) {
    const element = document.getElementById(elementId);
    if (!element) return false;

    const rect = element.getBoundingClientRect();
    return (
        rect.top >= 0 &&
        rect.left >= 0 &&
        rect.bottom <= (window.innerHeight || document.documentElement.clientHeight) &&
        rect.right <= (window.innerWidth || document.documentElement.clientWidth)
    );
};

// EzTooltip component functions
window.EzTooltip = {
    initialize: function (tooltipId, options) {
        const element = document.getElementById(tooltipId);
        if (!element) return false;

        const tooltipOptions = {
            placement: options.placement || 'top',
            trigger: options.trigger || 'hover',
            html: options.html || false,
            delay: options.delay || 0,
            animation: true
        };

        const tooltip = new bootstrap.Tooltip(element, tooltipOptions);
        element._tooltip = tooltip;

        return true;
    },

    show: function (tooltipId) {
        const element = document.getElementById(tooltipId);
        if (element && element._tooltip) {
            element._tooltip.show();
            return true;
        }
        return false;
    },

    hide: function (tooltipId) {
        const element = document.getElementById(tooltipId);
        if (element && element._tooltip) {
            element._tooltip.hide();
            return true;
        }
        return false;
    },

    update: function (tooltipId, newContent) {
        const element = document.getElementById(tooltipId);
        if (element && element._tooltip) {
            element._tooltip.setContent({ '.tooltip-inner': newContent });
            return true;
        }
        return false;
    },

    dispose: function (tooltipId) {
        const element = document.getElementById(tooltipId);
        if (element && element._tooltip) {
            element._tooltip.dispose();
            element._tooltip = null;
        }
    }
};

// EzTabControl component functions
window.EzTabControl = {
    initialize: function (tabsId) {
        const tabsContainer = document.getElementById(tabsId);
        if (!tabsContainer) return false;

        // Initialize Bootstrap tabs
        const tabTriggers = tabsContainer.querySelectorAll('[data-bs-toggle="tab"]');
        tabTriggers.forEach(trigger => {
            new bootstrap.Tab(trigger);
        });

        return true;
    },

    showTab: function (tabId) {
        const tabElement = document.getElementById(tabId);
        if (tabElement) {
            const tab = bootstrap.Tab.getInstance(tabElement) || new bootstrap.Tab(tabElement);
            tab.show();
            return true;
        }
        return false;
    }
};

// EzDateRangePicker component functions
window.EzDateRangePicker = {
    initialize: function (pickerId, options) {
        // This is a placeholder for date range picker functionality
        // You might want to integrate a library like daterangepicker.js
        return true;
    }
};

// EzToast component functions
window.EzToast = {
    show: function (toastId) {
        const toastElement = document.getElementById(toastId);
        if (toastElement) {
            const toast = new bootstrap.Toast(toastElement);
            toast.show();
            return true;
        }
        return false;
    },

    hide: function (toastId) {
        const toastElement = document.getElementById(toastId);
        if (toastElement) {
            const toast = bootstrap.Toast.getInstance(toastElement);
            if (toast) {
                toast.hide();
                return true;
            }
        }
        return false;
    }
};

// EzFileUpload component functions
window.EzFileUpload = {
    initializeDropZone: function (dropZoneId, dotNetRef) {
        const dropZone = document.getElementById(dropZoneId);
        if (!dropZone) return false;

        // Prevent default drag behaviors
        ['dragenter', 'dragover', 'dragleave', 'drop'].forEach(eventName => {
            dropZone.addEventListener(eventName, preventDefaults, false);
            document.body.addEventListener(eventName, preventDefaults, false);
        });

        // Highlight drop zone when item is dragged over it
        ['dragenter', 'dragover'].forEach(eventName => {
            dropZone.addEventListener(eventName, highlight, false);
        });

        ['dragleave', 'drop'].forEach(eventName => {
            dropZone.addEventListener(eventName, unhighlight, false);
        });

        // Handle dropped files
        dropZone.addEventListener('drop', handleDrop, false);

        function preventDefaults(e) {
            e.preventDefault();
            e.stopPropagation();
        }

        function highlight(e) {
            dropZone.classList.add('drag-over');
        }

        function unhighlight(e) {
            dropZone.classList.remove('drag-over');
        }

        async function handleDrop(e) {
            const dt = e.dataTransfer;
            const files = dt.files;

            if (dotNetRef && files.length > 0) {
                await dotNetRef.invokeMethodAsync('HandleDroppedFiles', Array.from(files));
            }
        }

        return true;
    }
};

// Cleanup functions
window.EzProInterop.disposeTooltips = function () {
    const tooltips = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltips.forEach(el => {
        const tooltip = bootstrap.Tooltip.getInstance(el);
        if (tooltip) {
            tooltip.dispose();
        }
    });
};

window.EzProInterop.disposePopovers = function () {
    const popovers = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
    popovers.forEach(el => {
        const popover = bootstrap.Popover.getInstance(el);
        if (popover) {
            popover.dispose();
        }
    });
};