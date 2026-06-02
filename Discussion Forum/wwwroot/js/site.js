// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

function applyTheme(theme) {
  const root = document.documentElement;
  const body = document.body;
  const button = document.getElementById('themeToggleBtn');
  if (!root || !body || !button) {
    return;
  }

  const icon = button.querySelector('i');
  const isLight = theme === 'light';

  root.dataset.theme = theme;
  body.dataset.theme = theme;

  if (isLight) {
    root.classList.add('light-theme');
    body.classList.add('light-theme');
    root.classList.remove('dark-theme');
    body.classList.remove('dark-theme');
    icon.className = 'bi bi-sun-fill';
    button.setAttribute('aria-label', 'Switch to dark theme');
    button.title = 'Switch to dark theme';
  } else {
    root.classList.remove('light-theme');
    body.classList.remove('light-theme');
    root.classList.add('dark-theme');
    body.classList.add('dark-theme');
    icon.className = 'bi bi-moon-stars-fill';
    button.setAttribute('aria-label', 'Switch to light theme');
    button.title = 'Switch to light theme';
  }
}

function initThemeToggle() {
  const button = document.getElementById('themeToggleBtn');
  if (!button) {
    return;
  }

  const savedTheme = localStorage.getItem('theme');
  const defaultTheme = window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
  const theme = savedTheme || defaultTheme;
  applyTheme(theme);

  button.addEventListener('click', () => {
    const nextTheme = document.documentElement.classList.contains('light-theme') ? 'dark' : 'light';
    applyTheme(nextTheme);
    localStorage.setItem('theme', nextTheme);
  });
}

document.addEventListener('DOMContentLoaded', function () {
  initThemeToggle();
});
