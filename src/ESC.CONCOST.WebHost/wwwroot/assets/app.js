//Update url
function update_url(newUrl) {
    history.pushState({}, '', newUrl);
}


const menuToggle = document.getElementById("menu-toggle");
if (menuToggle) {
    menuToggle.addEventListener("click", e => {
        e.preventDefault();
        document.body.classList.toggle("sb-sidenav-toggled");
    });
}


window.initOrgChartPanning = function(containerId) {
    const container = document.getElementById(containerId);
    if (!container) return;

    let isDown = false;
    let startX;
    let startY;
    let scrollLeft;
    let scrollTop;
    const interactiveSelector = 'input, textarea, select, button, a, label, [contenteditable="true"]';

    const stopDragging = () => {
        isDown = false;
        container.style.cursor = 'grab';
    };

    const hasActiveTextSelection = () => {
        const selection = window.getSelection();
        return !!selection && !selection.isCollapsed;
    };

    container.addEventListener('mousedown', (e) => {
        if (e.button !== 0) return;
        if (e.target.closest(interactiveSelector)) return;

        isDown = true;
        startX = e.pageX - container.offsetLeft;
        startY = e.pageY - container.offsetTop;
        scrollLeft = container.scrollLeft;
        scrollTop = container.scrollTop;
        container.style.cursor = 'grabbing';
    });

    container.addEventListener('mouseleave', () => {
        if (!isDown) {
            container.style.cursor = 'grab';
        }
    });

    container.addEventListener('mouseup', stopDragging);
    document.addEventListener('mouseup', stopDragging);
    window.addEventListener('blur', stopDragging);
    document.addEventListener('dragstart', stopDragging);
    document.addEventListener('selectionchange', () => {
        if (isDown && hasActiveTextSelection()) {
            stopDragging();
        }
    });

    container.addEventListener('mousemove', (e) => {
        if (!isDown) return;

        if ((e.buttons & 1) !== 1 || hasActiveTextSelection()) {
            stopDragging();
            return;
        }

        e.preventDefault();
        const x = e.pageX - container.offsetLeft;
        const y = e.pageY - container.offsetTop;
        const walkX = (x - startX) * 1.5; // Adjust drag speed
        const walkY = (y - startY) * 1.5;
        container.scrollLeft = scrollLeft - walkX;
        container.scrollTop = scrollTop - walkY;
    });
}


window.initGaugeChart = function (canvasId, success, delay, fail, pending, centerValue, centerText) {
    const el = document.getElementById(canvasId);
    if (!el) return;
    if (typeof Chart === 'undefined') {
        console.error('Chart.js not loaded');
        return;
    }
    const ctx = el.getContext('2d');
    new Chart(ctx, {
        type: 'doughnut',
        data: {
            datasets: [{
                data: [success, delay, fail, pending],
                backgroundColor: ['#198754', '#fd7e14', '#dc3545', '#adb5bd'],
                borderWidth: 0,
                cutout: '75%',
                circumference: 180,
                rotation: 270,
            }]
        },
        options: {
            aspectRatio: 1.5,
            plugins: {
                legend: { display: false },
                tooltip: { enabled: true }
            }
        }
    });
};

window.initDonutChart = function (canvasId, approved, pending) {
    const el = document.getElementById(canvasId);
    if (!el) return;
    if (typeof Chart === 'undefined') {
        console.error('Chart.js not loaded');
        return;
    }
    const ctx = el.getContext('2d');
    new Chart(ctx, {
        type: 'doughnut',
        data: {
            datasets: [{
                data: [approved, pending],
                backgroundColor: ['#198754', 'rgb(30, 41, 59)'],
                borderWidth: 0,
                cutout: '75%'
            }]
        },
        options: {
            plugins: {
                legend: { display: false },
                tooltip: { enabled: true }
            }
        }
    });
};
