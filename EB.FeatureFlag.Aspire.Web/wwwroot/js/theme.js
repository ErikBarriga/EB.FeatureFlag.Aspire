window.themeManager = {
    get: function () {
        return localStorage.getItem('theme') || 'light';
    },
    set: function (theme) {
        localStorage.setItem('theme', theme);
        document.documentElement.setAttribute('data-bs-theme', theme);
    }
};
