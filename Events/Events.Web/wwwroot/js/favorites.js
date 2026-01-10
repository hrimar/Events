/**
 * Favorites module for handling event favorite toggle functionality.
 * Provides AJAX operations for adding/removing events from user favorites.
 */

const Favorites = {
    /**
     * Toggles the favorite status of an event for the current user.
     * Makes an async POST request to the API and updates the UI accordingly.
     * 
     * @param {number} eventId - The ID of the event to toggle.
     * @param {HTMLElement} button - The favorite button element to update.
     * @returns {Promise<void>}
     */
    toggleFavorite: async function(eventId, button) {
        try {
            // Disable button during request
            button.disabled = true;
            const wasActive = button.classList.contains('active');

            const response = await fetch(`/api/favorites/toggle/${eventId}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'X-CSRF-TOKEN': this.getCsrfToken()
                }
            });

            if (!response.ok) {
                const error = await response.json();
                this.showMessage(error.message || 'Failed to update favorite', 'error');
                button.disabled = false;
                return;
            }

            const data = await response.json();

            // Update button UI based on response
            if (data.isFavorited) {
                button.classList.add('active');
                button.setAttribute('title', 'Remove from favorites');
                button.innerHTML = '<i class="fas fa-heart"></i>';
            } else {
                button.classList.remove('active');
                button.setAttribute('title', 'Add to favorites');
                button.innerHTML = '<i class="far fa-heart"></i>';
            }

            // Show success message
            this.showMessage(data.message || 'Success', 'success');

        } catch (error) {
            console.error('Error toggling favorite:', error);
            this.showMessage('An error occurred while updating your favorite', 'error');
        } finally {
            button.disabled = false;
        }
    },

    /**
     * Checks if an event is in the current user's favorites.
     * 
     * @param {number} eventId - The ID of the event to check.
     * @returns {Promise<boolean>} - True if favorited, false otherwise.
     */
    isFavorited: async function(eventId) {
        try {
            const response = await fetch(`/api/favorites/check/${eventId}`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                    'X-CSRF-TOKEN': this.getCsrfToken()
                }
            });

            if (!response.ok) {
                return false;
            }

            const data = await response.json();
            return data.isFavorited;
        } catch (error) {
            console.error('Error checking favorite status:', error);
            return false;
        }
    },

    /**
     * Gets the count of favorite events for the current user.
     * 
     * @returns {Promise<number>} - The count of favorite events.
     */
    getFavoriteCount: async function() {
        try {
            const response = await fetch('/api/favorites/count', {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                    'X-CSRF-TOKEN': this.getCsrfToken()
                }
            });

            if (!response.ok) {
                return 0;
            }

            const data = await response.json();
            return data.count;
        } catch (error) {
            console.error('Error getting favorite count:', error);
            return 0;
        }
    },

    /**
     * Extracts CSRF token from the page meta tag or cookie.
     * Required for POST requests in ASP.NET Core.
     * 
     * @returns {string} - The CSRF token.
     */
    getCsrfToken: function() {
        // Try to get from meta tag first
        const metaToken = document.querySelector('meta[name="csrf-token"]');
        if (metaToken) {
            return metaToken.getAttribute('content');
        }

        // Fallback: try to get from cookie
        const name = 'RequestVerificationToken=';
        const decodedCookie = decodeURIComponent(document.cookie);
        const cookieArray = decodedCookie.split(';');
        for (let cookie of cookieArray) {
            cookie = cookie.trim();
            if (cookie.indexOf(name) === 0) {
                return cookie.substring(name.length);
            }
        }

        return '';
    },

    /**
     * Shows a temporary notification message to the user.
     * 
     * @param {string} message - The message to display.
     * @param {string} type - The message type ('success', 'error', 'warning', 'info').
     */
    showMessage: function(message, type = 'info') {
        const alertClass = `alert alert-${
            type === 'success' ? 'success' :
            type === 'error' ? 'danger' :
            type === 'warning' ? 'warning' : 'info'
        }`;

        const alertElement = document.createElement('div');
        alertElement.className = `${alertClass} alert-dismissible fade show`;
        alertElement.style.position = 'fixed';
        alertElement.style.top = '20px';
        alertElement.style.right = '20px';
        alertElement.style.zIndex = '9999';
        alertElement.style.minWidth = '300px';

        alertElement.innerHTML = `
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert" 
                aria-label="Close"></button>
        `;

        document.body.appendChild(alertElement);

        // Auto-dismiss after 4 seconds
        setTimeout(() => {
            alertElement.remove();
        }, 4000);
    }
};

// Export for use in other modules
if (typeof module !== 'undefined' && module.exports) {
    module.exports = Favorites;
}
