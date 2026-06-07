// Theme
const savedTheme = localStorage.getItem('theme');
if (savedTheme) {
    document.documentElement.setAttribute('data-theme', savedTheme);
} else if (window.matchMedia('(prefers-color-scheme: dark)').matches) {
    document.documentElement.setAttribute('data-theme', 'coffee');
} else {
    document.documentElement.setAttribute('data-theme', 'autumn');
}


// Punch
let punchTimer   = null;
let punchSeconds = 0;
let punchStart   = null;   // records when punch-in was clicked

function setUnit(prefix, val) {
    document.getElementById(prefix + '-t').style.setProperty('--value', Math.floor(val / 10));
    document.getElementById(prefix + '-u').style.setProperty('--value', val % 10);
}

function punchIn() {
    punchStart   = new Date();
    punchSeconds = 0;

    document.getElementById('btn-punch-in').classList.add('hidden');
    document.getElementById('btn-punch-out').classList.remove('hidden');

    punchTimer = setInterval(() => {
        punchSeconds++;
        setUnit('cd-hours',   Math.floor(punchSeconds / 3600));
        setUnit('cd-minutes', Math.floor((punchSeconds % 3600) / 60));
        setUnit('cd-seconds', punchSeconds % 60);
    }, 1000);
}

function punchOut() {
    const punchEnd = new Date();

    clearInterval(punchTimer);
    punchTimer   = null;
    punchSeconds = 0;

    ['cd-hours', 'cd-minutes', 'cd-seconds'].forEach(prefix => setUnit(prefix, 0));

    document.getElementById('btn-punch-out').classList.add('hidden');
    document.getElementById('btn-punch-in').classList.remove('hidden');

    // Submit the hidden form with ISO timestamps — ASP.NET model binding parses these
    document.getElementById('input-start').value = punchStart.toISOString();
    document.getElementById('input-end').value   = punchEnd.toISOString();
    document.getElementById('punch-form').submit();
}

// Event Listeners
document.addEventListener('DOMContentLoaded', () => {
    // theme
    const toggle = document.getElementById('theme-toggle');
    if (toggle) {
        toggle.checked = (savedTheme === 'coffee' ||
            (!savedTheme && window.matchMedia('(prefers-color-scheme: dark)').matches));

        toggle.addEventListener('change', () => {
            const theme = toggle.checked ? 'coffee' : 'autumn';
            localStorage.setItem('theme', theme);
        });
    }
});
