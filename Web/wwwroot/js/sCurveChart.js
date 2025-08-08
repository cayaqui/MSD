window.sCurveChart = {
    charts: {},

    initialize: function (config, dotNetHelper) {
        const { containerId, data, options, callbacks } = config;
        
        // Use Chart.js for S-Curve visualization
        const canvas = document.createElement('canvas');
        const container = document.getElementById(containerId);
        if (!container) return;
        
        container.appendChild(canvas);
        
        const ctx = canvas.getContext('2d');
        
        // Configure Chart.js
        const chartConfig = {
            type: options.chartType || 'line',
            data: this.prepareChartData(data, options),
            options: this.getChartOptions(options, callbacks, dotNetHelper)
        };

        const chart = new Chart(ctx, chartConfig);

        // Store chart instance
        this.charts[containerId] = {
            chart,
            data,
            options,
            dotNetHelper,
            callbacks
        };
    },

    prepareChartData: function (data, options) {
        const datasets = data.series.map(series => {
            const dataset = {
                label: series.name,
                data: series.data.map(point => ({
                    x: point.x,
                    y: point.y
                })),
                borderColor: series.color,
                backgroundColor: series.fill ? this.hexToRgba(series.color, 0.2) : 'transparent',
                borderWidth: series.lineWidth || 2,
                fill: series.fill || false,
                tension: 0.1,
                pointRadius: series.showMarkers ? 4 : 0,
                pointHoverRadius: 6
            };

            // Apply line style
            if (series.lineStyle === 'dashed') {
                dataset.borderDash = [5, 5];
            } else if (series.lineStyle === 'dotted') {
                dataset.borderDash = [2, 2];
            }

            // Handle different chart types
            if (series.type === 'bar' || options.chartType === 'bar') {
                dataset.type = 'bar';
                dataset.backgroundColor = series.color;
            } else if (series.type === 'area') {
                dataset.fill = true;
            }

            return dataset;
        });

        return { datasets };
    },

    getChartOptions: function (options, callbacks, dotNetHelper) {
        return {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                title: {
                    display: false
                },
                legend: {
                    display: false // We're handling legend in Blazor
                },
                tooltip: {
                    enabled: options.showTooltips !== false,
                    mode: 'point',
                    intersect: false,
                    callbacks: {
                        label: function (context) {
                            let label = context.dataset.label || '';
                            if (label) {
                                label += ': ';
                            }
                            
                            const value = context.parsed.y;
                            if (options.valueFormat === 'currency') {
                                label += new Intl.NumberFormat('en-US', {
                                    style: 'currency',
                                    currency: 'USD'
                                }).format(value);
                            } else if (options.valueFormat === 'percentage') {
                                label += value.toFixed(1) + '%';
                            } else {
                                label += value.toLocaleString();
                            }
                            
                            return label;
                        }
                    }
                },
                datalabels: {
                    display: options.showDataLabels || false,
                    align: 'top',
                    formatter: (value, context) => {
                        if (options.valueFormat === 'currency') {
                            return '$' + value.toLocaleString();
                        } else if (options.valueFormat === 'percentage') {
                            return value.toFixed(1) + '%';
                        }
                        return value.toLocaleString();
                    }
                }
            },
            scales: {
                x: {
                    type: 'time',
                    time: {
                        unit: options.timeUnit || 'month',
                        displayFormats: {
                            day: 'MMM dd',
                            week: 'MMM dd',
                            month: 'MMM yyyy',
                            quarter: 'Q yyyy',
                            year: 'yyyy'
                        }
                    },
                    title: {
                        display: true,
                        text: options.xAxisLabel || 'Time'
                    },
                    grid: {
                        display: options.showGrid !== false
                    }
                },
                y: {
                    title: {
                        display: true,
                        text: options.yAxisLabel || 'Value'
                    },
                    grid: {
                        display: options.showGrid !== false
                    },
                    ticks: {
                        callback: function (value) {
                            if (options.valueFormat === 'currency') {
                                return '$' + value.toLocaleString();
                            } else if (options.valueFormat === 'percentage') {
                                return value + '%';
                            }
                            return value.toLocaleString();
                        }
                    }
                }
            },
            interaction: {
                mode: 'index',
                intersect: false
            },
            onClick: function (event, elements) {
                if (elements.length > 0 && callbacks.onDataPointClick && dotNetHelper) {
                    const element = elements[0];
                    const datasetIndex = element.datasetIndex;
                    const index = element.index;
                    const dataset = this.data.datasets[datasetIndex];
                    const dataPoint = dataset.data[index];
                    
                    dotNetHelper.invokeMethodAsync(
                        callbacks.onDataPointClick,
                        dataset.label,
                        dataPoint.x,
                        dataPoint.y
                    );
                }
            }
        };
    },

    update: function (containerId, data) {
        const chartInstance = this.charts[containerId];
        if (!chartInstance) return;

        const { chart, options } = chartInstance;
        
        // Update chart data
        chart.data = this.prepareChartData(data, options);
        chart.update();
    },

    changeChartType: function (containerId, type) {
        const chartInstance = this.charts[containerId];
        if (!chartInstance) return;

        const { chart } = chartInstance;
        chart.config.type = type;
        chart.update();
    },

    export: function (containerId, format) {
        const chartInstance = this.charts[containerId];
        if (!chartInstance) return;

        const { chart } = chartInstance;
        
        if (format === 'png') {
            const url = chart.toBase64Image();
            const link = document.createElement('a');
            link.download = 'scurve-chart.png';
            link.href = url;
            link.click();
        }
    },

    hexToRgba: function (hex, alpha) {
        const r = parseInt(hex.slice(1, 3), 16);
        const g = parseInt(hex.slice(3, 5), 16);
        const b = parseInt(hex.slice(5, 7), 16);
        return `rgba(${r}, ${g}, ${b}, ${alpha})`;
    },

    destroy: function (containerId) {
        const chartInstance = this.charts[containerId];
        if (chartInstance && chartInstance.chart) {
            chartInstance.chart.destroy();
            delete this.charts[containerId];
        }
    }
};