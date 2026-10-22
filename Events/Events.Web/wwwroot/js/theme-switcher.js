// Theme toggle: switches between the light and dark theme and remembers the choice in localStorage.
// The initial theme (saved choice, else system preference) is applied by the inline script in the
// <head> of _Layout.cshtml, before the page renders, so there is no flash of the wrong theme.
class ThemeManager {
    constructor(toggleButton) {
        this.toggleButton = toggleButton;
        this.toggleButton.addEventListener('click', () => this.toggle());
        this.updateToggleLabel();
    }

    get currentTheme() {
        return document.documentElement.getAttribute('data-theme') === 'dark' ? 'dark' : 'light';
    }

    toggle() {
        const nextTheme = this.currentTheme === 'dark' ? 'light' : 'dark';

        try {
            localStorage.setItem('selectedTheme', nextTheme);
        } catch {
            // Storage can be unavailable (private mode, blocked site data) - the theme still switches for this page.
        }

        document.documentElement.setAttribute('data-theme', nextTheme);
        this.updateToggleLabel();
    }

    // The label names the theme the button switches to (localized texts come from data-label-* attributes).
    updateToggleLabel() {
        const label = this.currentTheme === 'dark'
            ? this.toggleButton.dataset.labelLight
            : this.toggleButton.dataset.labelDark;

        this.toggleButton.setAttribute('aria-label', label);
        this.toggleButton.setAttribute('title', label);
    }
}

document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('.theme-toggle').forEach(button => new ThemeManager(button));
});
