// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

const chartOptions = {
    chart: {
        type: 'area',
        height: 180,
        toolbar: { show: false },
        zoom: { enable: false }
    },
    colors: ['#3498db'],
    series: [{ name: 'Grade', data: [0, 5, 7, 8, 9, 9, 7] }],
    dataLabels: { enable: false },
    stroke: { width: 5, curve: 'smooth' },
    fill: {
        type: 'gradient',
        gradient: {
            shadeIntensity: 1,
            opacityFrom: 0.7,
            opacityTo: 0,
            stops: [0, 90, 100]
        }
    },
    xaxis: {
        categories: ['June', 'July', 'August', 'September', 'October', 'November', 'December'],
        axisBorder: { show: false },
        labels: { style: { colors: '#a7a7a7', fontFamily: 'system-ui' } }
    },
    yaxis: { show: false },
    grid: { borderColor: 'rgba(0,0,0,0)', padding: { top: -30, bottom: -8, left: 12, right: 12 } },
    tooltip: {
        disable: true,
        y: { formatter: value => '${value}K' },
        style: { fontFamily: 'system-ui' }
    },
    markers: { show: false }
};

const chart = new ApexCharts(document.querySelector('.chart-area'), chartOptions);
chart.render();