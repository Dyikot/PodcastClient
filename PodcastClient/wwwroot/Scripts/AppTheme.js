function SetTheme(theme) {
    document.documentElement.setAttribute("data-theme", theme);
}

function SetThemeFromLocalStorage() {
    const theme = localStorage.getItem("theme") || "light";
    SetTheme(theme);
}

SetThemeFromLocalStorage();