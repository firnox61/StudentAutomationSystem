(function () {
    const root = document.documentElement;
    const btn = document.getElementById('themeToggle');
    const label = document.getElementById('themeLabel');

    // tercih yükle
    const saved = localStorage.getItem('ustatakip-theme');
    if (saved === 'dark' || saved === 'light') {
        root.setAttribute('data-theme', saved);
    } else {
        // sistem tercihi
        const prefersDark = window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches;
        root.setAttribute('data-theme', prefersDark ? 'dark' : 'light');
    }

    function refreshLabel() {
        label && (label.textContent = root.getAttribute('data-theme') === 'dark' ? 'Koyu' : 'Açık');
    }
    refreshLabel();

    btn && btn.addEventListener('click', function () {
        const next = root.getAttribute('data-theme') === 'dark' ? 'light' : 'dark';
        root.setAttribute('data-theme', next);
        localStorage.setItem('ustatakip-theme', next);
        refreshLabel();
    });
})();
