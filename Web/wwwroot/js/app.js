// App JavaScript Functions

window.ezproApp = {
    // Initialize tooltips
    initTooltips: function() {
        var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
        tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl);
        });
    },

    // Toggle sidebar
    toggleSidebar: function() {
        const sidebar = document.querySelector('.sidebar');
        const mainContent = document.querySelector('.main-content');
        const mainHeader = document.querySelector('.main-header');
        
        if (sidebar) {
            sidebar.classList.toggle('sidebar-mini');
            mainContent?.classList.toggle('sidebar-mini');
            mainHeader?.classList.toggle('sidebar-mini');
            
            // Save state to localStorage
            const isMini = sidebar.classList.contains('sidebar-mini');
            localStorage.setItem('sidebarMini', isMini);
        }
    },

    // Initialize sidebar state
    initSidebar: function() {
        const isMini = localStorage.getItem('sidebarMini') === 'true';
        if (isMini) {
            document.querySelector('.sidebar')?.classList.add('sidebar-mini');
            document.querySelector('.main-content')?.classList.add('sidebar-mini');
            document.querySelector('.main-header')?.classList.add('sidebar-mini');
        }
    },

    // Show loading
    showLoading: function(elementId) {
        const element = document.getElementById(elementId);
        if (element) {
            element.innerHTML = '<div class="loading-wrapper"><div class="loading-spinner"></div></div>';
        }
    },

    // Hide loading
    hideLoading: function(elementId) {
        const element = document.getElementById(elementId);
        if (element) {
            element.innerHTML = '';
        }
    },

    // Chart.js default configuration
    setChartDefaults: function() {
        if (window.Chart) {
            Chart.defaults.font.family = getComputedStyle(document.documentElement).getPropertyValue('--font-family');
            Chart.defaults.color = getComputedStyle(document.documentElement).getPropertyValue('--gray-700');
        }
    },

    // Create a simple line chart
    createLineChart: function(canvasId, labels, datasets) {
        const ctx = document.getElementById(canvasId);
        if (!ctx) return null;

        return new Chart(ctx, {
            type: 'line',
            data: {
                labels: labels,
                datasets: datasets
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        display: true,
                        position: 'top',
                    }
                },
                scales: {
                    x: {
                        grid: {
                            display: false
                        }
                    },
                    y: {
                        beginAtZero: true,
                        grid: {
                            borderDash: [2, 2]
                        }
                    }
                }
            }
        });
    },

    // Create a simple bar chart
    createBarChart: function(canvasId, labels, datasets) {
        const ctx = document.getElementById(canvasId);
        if (!ctx) return null;

        return new Chart(ctx, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: datasets
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        display: true,
                        position: 'top',
                    }
                },
                scales: {
                    x: {
                        grid: {
                            display: false
                        }
                    },
                    y: {
                        beginAtZero: true,
                        grid: {
                            borderDash: [2, 2]
                        }
                    }
                }
            }
        });
    },

    // Create a doughnut chart
    createDoughnutChart: function(canvasId, labels, data, colors) {
        const ctx = document.getElementById(canvasId);
        if (!ctx) return null;

        return new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels: labels,
                datasets: [{
                    data: data,
                    backgroundColor: colors
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        position: 'bottom',
                    }
                }
            }
        });
    },

    // Format currency
    formatCurrency: function(amount, currency = 'USD') {
        return new Intl.NumberFormat('en-US', {
            style: 'currency',
            currency: currency,
            minimumFractionDigits: 0,
            maximumFractionDigits: 0
        }).format(amount);
    },

    // Format percentage
    formatPercentage: function(value, decimals = 1) {
        return value.toFixed(decimals) + '%';
    },

    // Copy to clipboard
    copyToClipboard: function(text) {
        if (navigator.clipboard) {
            navigator.clipboard.writeText(text).then(function() {
                console.log('Copied to clipboard');
            }).catch(function(err) {
                console.error('Failed to copy: ', err);
            });
        }
    },

    // Download file from byte array
    downloadFile: function(fileName, byteArray) {
        const blob = new Blob([byteArray], { type: 'application/octet-stream' });
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = fileName;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        window.URL.revokeObjectURL(url);
    },

    // Debounce function
    debounce: function(func, wait) {
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

    // Initialize on document ready
    init: function() {
        this.initSidebar();
        this.initTooltips();
        this.setChartDefaults();
    }
};

// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', function() {
    window.ezproApp.init();
});

// Make downloadFile available globally for Blazor
window.downloadFile = window.ezproApp.downloadFile;