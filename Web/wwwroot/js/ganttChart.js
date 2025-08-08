window.ganttChart = {
    charts: {},

    initialize: function (config, dotNetHelper) {
        const { containerId, data, options, callbacks } = config;
        
        // Create SVG container
        const container = document.getElementById(containerId);
        if (!container) return;

        const svg = d3.select(container)
            .append('svg')
            .attr('width', '100%')
            .attr('height', '100%');

        // Store chart instance
        this.charts[containerId] = {
            svg,
            data,
            options,
            dotNetHelper,
            callbacks
        };

        this.render(containerId);
    },

    render: function (containerId) {
        const chart = this.charts[containerId];
        if (!chart) return;

        const { svg, data, options } = chart;
        const container = document.getElementById(containerId);
        const width = container.clientWidth;
        const height = container.clientHeight;

        // Clear previous content
        svg.selectAll('*').remove();

        // Calculate dimensions
        const margin = { top: options.headerHeight, right: 20, bottom: 20, left: 200 };
        const chartWidth = width - margin.left - margin.right;
        const chartHeight = height - margin.top - margin.bottom;

        // Create scales
        const xScale = d3.scaleTime()
            .domain([new Date(data.startDate), new Date(data.endDate)])
            .range([0, chartWidth]);

        const yScale = d3.scaleBand()
            .domain(data.tasks.map(t => t.id))
            .range([0, chartHeight])
            .padding(0.1);

        // Create main group
        const g = svg.append('g')
            .attr('transform', `translate(${margin.left},${margin.top})`);

        // Draw grid
        if (options.showGrid !== false) {
            this.drawGrid(g, xScale, yScale, chartWidth, chartHeight);
        }

        // Draw tasks
        this.drawTasks(g, data.tasks, xScale, yScale, chart);

        // Draw dependencies
        if (options.showDependencies && data.tasks.some(t => t.dependencies.length > 0)) {
            this.drawDependencies(g, data.tasks, xScale, yScale);
        }

        // Draw milestones
        if (options.showMilestones && data.milestones.length > 0) {
            this.drawMilestones(g, data.milestones, xScale, yScale);
        }

        // Draw axes
        this.drawAxes(g, xScale, yScale, chartWidth, chartHeight, options);

        // Draw today line
        if (options.showToday) {
            this.drawTodayLine(g, xScale, chartHeight);
        }
    },

    drawGrid: function (g, xScale, yScale, width, height) {
        const xAxis = d3.axisTop(xScale)
            .tickSize(-height)
            .tickFormat('');

        g.append('g')
            .attr('class', 'gantt-grid')
            .call(xAxis)
            .selectAll('line')
            .style('stroke', '#e0e0e0')
            .style('stroke-dasharray', '2,2');
    },

    drawTasks: function (g, tasks, xScale, yScale, chart) {
        const { options, dotNetHelper, callbacks } = chart;

        const taskGroups = g.selectAll('.gantt-task-group')
            .data(tasks)
            .enter()
            .append('g')
            .attr('class', 'gantt-task-group');

        // Draw task bars
        taskGroups.append('rect')
            .attr('class', d => `gantt-task ${d.isCritical ? 'critical' : ''}`)
            .attr('x', d => xScale(new Date(d.startDate)))
            .attr('y', d => yScale(d.id))
            .attr('width', d => {
                const start = xScale(new Date(d.startDate));
                const end = xScale(new Date(d.endDate));
                return Math.max(0, end - start);
            })
            .attr('height', yScale.bandwidth())
            .attr('fill', d => d.color || '#0066CC')
            .attr('rx', 4)
            .attr('ry', 4)
            .style('cursor', 'pointer')
            .on('click', function (event, d) {
                if (callbacks.onTaskClick && dotNetHelper) {
                    dotNetHelper.invokeMethodAsync(callbacks.onTaskClick, d.id);
                }
            });

        // Draw progress bars
        if (options.showProgress) {
            taskGroups.append('rect')
                .attr('class', 'gantt-task-progress')
                .attr('x', d => xScale(new Date(d.startDate)))
                .attr('y', d => yScale(d.id))
                .attr('width', d => {
                    const start = xScale(new Date(d.startDate));
                    const end = xScale(new Date(d.endDate));
                    const width = Math.max(0, end - start);
                    return width * (d.progress / 100);
                })
                .attr('height', yScale.bandwidth())
                .attr('fill', d => d.color || '#0066CC')
                .attr('opacity', 0.3)
                .attr('rx', 4)
                .attr('ry', 4)
                .style('pointer-events', 'none');
        }

        // Draw task labels
        taskGroups.append('text')
            .attr('x', -10)
            .attr('y', d => yScale(d.id) + yScale.bandwidth() / 2)
            .attr('text-anchor', 'end')
            .attr('dominant-baseline', 'middle')
            .style('font-size', '12px')
            .text(d => d.name);

        // Add tooltips
        this.addTooltips(taskGroups, tasks);
    },

    drawDependencies: function (g, tasks, xScale, yScale) {
        const taskMap = new Map(tasks.map(t => [t.id, t]));

        tasks.forEach(task => {
            task.dependencies.forEach(depId => {
                const predecessor = taskMap.get(depId);
                if (predecessor) {
                    const path = this.calculateDependencyPath(
                        predecessor, task, xScale, yScale
                    );

                    g.append('path')
                        .attr('class', 'gantt-dependency')
                        .attr('d', path)
                        .attr('fill', 'none')
                        .attr('stroke', '#999')
                        .attr('stroke-width', 2)
                        .attr('marker-end', 'url(#arrow)');
                }
            });
        });

        // Add arrow marker
        const defs = g.append('defs');
        defs.append('marker')
            .attr('id', 'arrow')
            .attr('viewBox', '0 -5 10 10')
            .attr('refX', 10)
            .attr('refY', 0)
            .attr('markerWidth', 6)
            .attr('markerHeight', 6)
            .attr('orient', 'auto')
            .append('path')
            .attr('d', 'M0,-5L10,0L0,5')
            .attr('fill', '#999');
    },

    calculateDependencyPath: function (from, to, xScale, yScale) {
        const fromEnd = xScale(new Date(from.endDate));
        const fromY = yScale(from.id) + yScale.bandwidth() / 2;
        const toStart = xScale(new Date(to.startDate));
        const toY = yScale(to.id) + yScale.bandwidth() / 2;

        const midX = (fromEnd + toStart) / 2;

        return `M${fromEnd},${fromY} L${midX},${fromY} L${midX},${toY} L${toStart},${toY}`;
    },

    drawMilestones: function (g, milestones, xScale, yScale) {
        const milestoneGroups = g.selectAll('.gantt-milestone')
            .data(milestones)
            .enter()
            .append('g')
            .attr('class', 'gantt-milestone');

        milestoneGroups.append('polygon')
            .attr('points', d => {
                const x = xScale(new Date(d.date));
                const y = 10;
                const size = 8;
                return `${x},${y - size} ${x + size},${y} ${x},${y + size} ${x - size},${y}`;
            })
            .attr('fill', d => d.color || '#FFA500')
            .style('cursor', 'pointer');

        milestoneGroups.append('text')
            .attr('x', d => xScale(new Date(d.date)))
            .attr('y', 30)
            .attr('text-anchor', 'middle')
            .style('font-size', '10px')
            .text(d => d.name);
    },

    drawAxes: function (g, xScale, yScale, width, height, options) {
        const xAxis = d3.axisBottom(xScale)
            .tickFormat(d3.timeFormat(options.dateFormat || '%b %d'));

        g.append('g')
            .attr('class', 'gantt-axis')
            .attr('transform', `translate(0,${height})`)
            .call(xAxis);
    },

    drawTodayLine: function (g, xScale, height) {
        const today = new Date();
        const x = xScale(today);

        g.append('line')
            .attr('class', 'gantt-today-line')
            .attr('x1', x)
            .attr('y1', 0)
            .attr('x2', x)
            .attr('y2', height)
            .style('stroke', '#ff0000')
            .style('stroke-width', 2)
            .style('stroke-dasharray', '5,5');

        g.append('text')
            .attr('x', x)
            .attr('y', -5)
            .attr('text-anchor', 'middle')
            .style('font-size', '10px')
            .style('fill', '#ff0000')
            .text('Today');
    },

    addTooltips: function (selection, data) {
        const tooltip = d3.select('body').append('div')
            .attr('class', 'gantt-tooltip')
            .style('opacity', 0);

        selection
            .on('mouseover', function (event, d) {
                tooltip.transition()
                    .duration(200)
                    .style('opacity', .9);
                
                const content = `
                    <strong>${d.name}</strong><br/>
                    Start: ${new Date(d.startDate).toLocaleDateString()}<br/>
                    End: ${new Date(d.endDate).toLocaleDateString()}<br/>
                    Progress: ${d.progress}%<br/>
                    ${d.assignedTo ? `Assigned to: ${d.assignedTo}` : ''}
                `;
                
                tooltip.html(content)
                    .style('left', (event.pageX + 10) + 'px')
                    .style('top', (event.pageY - 28) + 'px');
            })
            .on('mouseout', function () {
                tooltip.transition()
                    .duration(500)
                    .style('opacity', 0);
            });
    },

    changeViewMode: function (containerId, viewMode) {
        const chart = this.charts[containerId];
        if (chart) {
            chart.options.viewMode = viewMode;
            this.render(containerId);
        }
    },

    zoomIn: function (containerId) {
        // Implement zoom functionality
        console.log('Zoom in');
    },

    zoomOut: function (containerId) {
        // Implement zoom functionality
        console.log('Zoom out');
    },

    resetZoom: function (containerId) {
        // Implement zoom reset
        console.log('Reset zoom');
    },

    export: function (containerId, format) {
        const chart = this.charts[containerId];
        if (!chart) return;

        // Implement export functionality
        console.log(`Export as ${format}`);
    },

    update: function (containerId, data) {
        const chart = this.charts[containerId];
        if (chart) {
            chart.data = data;
            this.render(containerId);
        }
    }
};