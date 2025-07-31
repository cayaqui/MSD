// =================================
// EzPro.SD - Charts JavaScript
// Sistema de Control de Proyectos
// =================================

window.EzPro = window.EzPro || {};
window.EzPro.Charts = {};

// Chart.js default configuration
Chart.defaults.font.family = "'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif";
Chart.defaults.font.size = 12;
Chart.defaults.color = '#6c757d';
Chart.defaults.plugins.legend.labels.usePointStyle = true;
Chart.defaults.plugins.legend.labels.padding = 15;

// Theme colors
window.EzPro.Charts.colors = {
    primary: '#845adf',
    secondary: '#23b7e5',
    success: '#26bf94',
    info: '#5b85f7',
    warning: '#f5b849',
    danger: '#e54c65',
    light: '#f0f1f5',
    dark: '#1c2126',
    // Chart specific colors
    planned: '#3b82f6',
    actual: '#10b981',
    forecast: '#f59e0b',
    budget: '#6366f1',
    earned: '#8b5cf6',
    spent: '#ef4444'
};

// Alias for easier access
const chartColors = window.EzPro.Charts.colors;

// S-Curve Chart
window.EzPro.Charts.createSCurve = function (canvasId, data) {
    const ctx = document.getElementById(canvasId);
    if (!ctx) return null;

    const chart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: data.labels,
            datasets: [
                {
                    label: 'Planificado (PV)',
                    data: data.planned,
                    borderColor: chartColors.planned,
                    backgroundColor: chartColors.planned + '20',
                    tension: 0.4,
                    fill: false
                },
                {
                    label: 'Real (AC)',
                    data: data.actual,
                    borderColor: chartColors.actual,
                    backgroundColor: chartColors.actual + '20',
                    tension: 0.4,
                    fill: false
                },
                {
                    label: 'Ganado (EV)',
                    data: data.earned,
                    borderColor: chartColors.earned,
                    backgroundColor: chartColors.earned + '20',
                    tension: 0.4,
                    fill: false
                }
            ]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            interaction: {
                mode: 'index',
                intersect: false
            },
            plugins: {
                title: {
                    display: true,
                    text: 'Curva S del Proyecto'
                },
                tooltip: {
                    callbacks: {
                        label: function (context) {
                            let label = context.dataset.label || '';
                            if (label) {
                                label += ': ';
                            }
                            label += EzPro.Utils.formatCurrency(context.parsed.y);
                            return label;
                        }
                    }
                }
            },
            scales: {
                x: {
                    display: true,
                    title: {
                        display: true,
                        text: 'Período'
                    }
                },
                y: {
                    display: true,
                    title: {
                        display: true,
                        text: 'Valor Acumulado'
                    },
                    ticks: {
                        callback: function (value) {
                            return EzPro.Utils.formatCurrency(value);
                        }
                    }
                }
            }
        }
    });

    return chart;
};

// Burndown Chart
window.EzPro.Charts.createBurndown = function (canvasId, data) {
    const ctx = document.getElementById(canvasId);
    if (!ctx) return null;

    const chart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: data.labels,
            datasets: [
                {
                    label: 'Trabajo Ideal',
                    data: data.ideal,
                    borderColor: chartColors.info,
                    borderDash: [5, 5],
                    tension: 0,
                    fill: false,
                    pointRadius: 0
                },
                {
                    label: 'Trabajo Restante',
                    data: data.remaining,
                    borderColor: chartColors.danger,
                    backgroundColor: chartColors.danger + '20',
                    tension: 0.2,
                    fill: true
                }
            ]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                title: {
                    display: true,
                    text: 'Gráfico de Burndown'
                },
                tooltip: {
                    callbacks: {
                        label: function (context) {
                            let label = context.dataset.label || '';
                            if (label) {
                                label += ': ';
                            }
                            label += context.parsed.y + ' horas';
                            return label;
                        }
                    }
                }
            },
            scales: {
                x: {
                    display: true,
                    title: {
                        display: true,
                        text: 'Sprint / Días'
                    }
                },
                y: {
                    display: true,
                    title: {
                        display: true,
                        text: 'Horas Restantes'
                    },
                    beginAtZero: true
                }
            }
        }
    });

    return chart;
};

// Cash Flow Chart
window.EzPro.Charts.createCashFlow = function (canvasId, data) {
    const ctx = document.getElementById(canvasId);
    if (!ctx) return null;

    const chart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: data.labels,
            datasets: [
                {
                    label: 'Ingresos',
                    data: data.income,
                    backgroundColor: chartColors.success,
                    stack: 'stack0'
                },
                {
                    label: 'Egresos',
                    data: data.expenses,
                    backgroundColor: chartColors.danger,
                    stack: 'stack1'
                },
                {
                    label: 'Flujo Neto',
                    data: data.net,
                    type: 'line',
                    borderColor: chartColors.primary,
                    backgroundColor: chartColors.primary + '20',
                    tension: 0.3,
                    fill: false
                }
            ]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                title: {
                    display: true,
                    text: 'Flujo de Caja del Proyecto'
                },
                tooltip: {
                    callbacks: {
                        label: function (context) {
                            let label = context.dataset.label || '';
                            if (label) {
                                label += ': ';
                            }
                            const value = context.parsed.y;
                            label += EzPro.Utils.formatCurrency(Math.abs(value));
                            if (value < 0) {
                                label = '(' + label + ')';
                            }
                            return label;
                        }
                    }
                }
            },
            scales: {
                x: {
                    display: true,
                    title: {
                        display: true,
                        text: 'Período'
                    }
                },
                y: {
                    display: true,
                    title: {
                        display: true,
                        text: 'Monto'
                    },
                    ticks: {
                        callback: function (value) {
                            return EzPro.Utils.formatCurrency(value);
                        }
                    }
                }
            }
        }
    });

    return chart;
};

// Resource Histogram
window.EzPro.Charts.createResourceHistogram = function (canvasId, data) {
    const ctx = document.getElementById(canvasId);
    if (!ctx) return null;

    const chart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: data.labels,
            datasets: data.resources.map((resource, index) => ({
                label: resource.name,
                data: resource.hours,
                backgroundColor: Object.values(chartColors)[index % Object.values(chartColors).length],
                stack: 'stack0'
            }))
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                title: {
                    display: true,
                    text: 'Histograma de Recursos'
                },
                tooltip: {
                    callbacks: {
                        label: function (context) {
                            let label = context.dataset.label || '';
                            if (label) {
                                label += ': ';
                            }
                            label += context.parsed.y + ' horas';
                            return label;
                        }
                    }
                }
            },
            scales: {
                x: {
                    display: true,
                    stacked: true,
                    title: {
                        display: true,
                        text: 'Período'
                    }
                },
                y: {
                    display: true,
                    stacked: true,
                    title: {
                        display: true,
                        text: 'Horas'
                    }
                }
            }
        }
    });

    return chart;
};

// EVM Performance Indices Chart
window.EzPro.Charts.createEVMIndices = function (canvasId, data) {
    const ctx = document.getElementById(canvasId);
    if (!ctx) return null;

    const chart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: data.labels,
            datasets: [
                {
                    label: 'CPI (Índice de Desempeño del Costo)',
                    data: data.cpi,
                    borderColor: chartColors.info,
                    backgroundColor: chartColors.info + '20',
                    tension: 0.3
                },
                {
                    label: 'SPI (Índice de Desempeño del Cronograma)',
                    data: data.spi,
                    borderColor: chartColors.warning,
                    backgroundColor: chartColors.warning + '20',
                    tension: 0.3
                }
            ]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                title: {
                    display: true,
                    text: 'Índices de Desempeño EVM'
                },
                annotation: {
                    annotations: {
                        line1: {
                            type: 'line',
                            yMin: 1,
                            yMax: 1,
                            borderColor: chartColors.success,
                            borderWidth: 2,
                            borderDash: [5, 5],
                            label: {
                                content: 'Objetivo (1.0)',
                                enabled: true,
                                position: 'end'
                            }
                        }
                    }
                }
            },
            scales: {
                x: {
                    display: true,
                    title: {
                        display: true,
                        text: 'Período'
                    }
                },
                y: {
                    display: true,
                    title: {
                        display: true,
                        text: 'Índice'
                    },
                    min: 0,
                    max: 2,
                    ticks: {
                        stepSize: 0.2
                    }
                }
            }
        }
    });

    return chart;
};

// Doughnut Chart for Project Status
window.EzPro.Charts.createProjectStatus = function (canvasId, data) {
    const ctx = document.getElementById(canvasId);
    if (!ctx) return null;

    const chart = new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: ['Planificación', 'Activo', 'En Espera', 'Completado', 'Cancelado'],
            datasets: [{
                data: [data.planning, data.active, data.onHold, data.completed, data.cancelled],
                backgroundColor: [
                    chartColors.planned,
                    chartColors.success,
                    chartColors.warning,
                    chartColors.info,
                    chartColors.danger
                ]
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                title: {
                    display: true,
                    text: 'Estado de Proyectos'
                },
                legend: {
                    position: 'bottom'
                }
            }
        }
    });

    return chart;
};

// Update chart data
window.EzPro.Charts.updateChart = function (chart, newData) {
    if (!chart) return;

    chart.data = newData;
    chart.update();
};

// Destroy chart
window.EzPro.Charts.destroyChart = function (chart) {
    if (chart) {
        chart.destroy();
    }
};

// Export chart as image
window.EzPro.Charts.exportChart = function (chart, filename = 'chart.png') {
    if (!chart) return;

    const url = chart.toBase64Image();
    const link = document.createElement('a');
    link.download = filename;
    link.href = url;
    link.click();
};