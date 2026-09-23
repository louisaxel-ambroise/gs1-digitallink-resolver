function getTheme() {
    return localStorage.getItem('theme')
        ? localStorage.getItem('theme')
        : window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
}

function setTheme(theme) {
    document.documentElement.setAttribute('data-theme', theme);
    localStorage.setItem('theme', theme);
    updateThemeToggle(theme);
}

function updateThemeToggle(theme) {
    document.querySelector('.light-icon').style.display = theme === 'dark' ? 'none' : 'inline';
    document.querySelector('.dark-icon').style.display = theme === 'dark' ? 'inline' : 'none';
}

document.addEventListener('DOMContentLoaded', () => {
    setTheme(getTheme());
    document.getElementById('themeToggle').addEventListener('click', function(e){ setTheme(getTheme() === 'dark' ? 'light' : 'dark'); e.preventDefault();});
});