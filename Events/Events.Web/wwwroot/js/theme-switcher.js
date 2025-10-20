// Simplified Theme Management System
class ThemeManager {
    constructor() {
        this.themes = {
            light: { name: 'Light Theme', icon: 'fas fa-sun' },
            dark: { name: 'Dark Theme', icon: 'fas fa-moon' }
        };
        
        this.init();
    }

    init() {
        this.loadSavedTheme();
        this.bindEvents();
        this.updateThemeDisplay();
    }

    loadSavedTheme() {
        // Try to get saved theme, fallback to system preference, then light
        const savedTheme = localStorage.getItem('selectedTheme');
        
        if (savedTheme && this.themes[savedTheme]) {
            this.applyTheme(savedTheme);
        } else {
            // Smart default - check system preference once
            const prefersDark = window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches;
            const defaultTheme = prefersDark ? 'dark' : 'light';
            this.applyTheme(defaultTheme);
        }
    }

    bindEvents() {
        // Theme switcher clicks
        document.addEventListener('click', (e) => {
            if (e.target.closest('.theme-option')) {
                e.preventDefault();
                const themeId = e.target.closest('.theme-option').dataset.theme;
                this.switchTheme(themeId);
            }
        });
    }

    switchTheme(themeId) {
        localStorage.setItem('selectedTheme', themeId);
        this.applyTheme(themeId);
        this.updateThemeDisplay();
        this.showToast(`Switched to ${this.themes[themeId]?.name || themeId}`, 'success');
    }

    applyTheme(themeId) {
        document.documentElement.setAttribute('data-theme', themeId);
    }

    updateThemeDisplay() {
        const currentTheme = localStorage.getItem('selectedTheme') || 'light';
        const themeNameElement = document.getElementById('current-theme-name');
        
        if (themeNameElement) {
            themeNameElement.textContent = this.themes[currentTheme]?.name || 'Light Theme';
        }

        // Update active state in dropdown
        document.querySelectorAll('.theme-option').forEach(option => {
            const isActive = option.dataset.theme === currentTheme;
            option.classList.toggle('active', isActive);
        });
    }

    showToast(message, type = 'info') {
        const toast = document.createElement('div');
        toast.className = `position-fixed top-0 end-0 m-3 alert alert-${type === 'success' ? 'success' : 'info'} alert-dismissible fade show`;
        toast.style.zIndex = '9999';
        toast.innerHTML = `
            <i class="fas fa-${type === 'success' ? 'check' : 'info-circle'} me-2"></i>
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        `;
        
        document.body.appendChild(toast);
        
        setTimeout(() => {
            if (document.body.contains(toast)) {
                toast.classList.remove('show');
                setTimeout(() => {
                    if (document.body.contains(toast)) {
                        document.body.removeChild(toast);
                    }
                }, 150);
            }
        }, 3000);
    }
}

// Initialize theme manager when DOM is ready
document.addEventListener('DOMContentLoaded', () => {
    new ThemeManager();
    
    // Initialize theme immediately to prevent flash
    const savedTheme = localStorage.getItem('selectedTheme');
    
    if (savedTheme) {
        document.documentElement.setAttribute('data-theme', savedTheme);
    } else {
        // Smart default based on system preference
        const prefersDark = window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches;
        document.documentElement.setAttribute('data-theme', prefersDark ? 'dark' : 'light');
    }
});