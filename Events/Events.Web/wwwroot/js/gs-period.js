// Period view (GS-REQ-008, 8.2.8): "More N events on Friday" loads all the other events of the day from
// Events/PeriodDay and appends them to the day's list, without leaving the page or moving the scroll position.
(function () {
    'use strict';

    function initMoreButton(root) {
        var button = root.querySelector('button[data-url]');
        var errorText = root.querySelector('[role="alert"]');
        var list = button ? document.getElementById(button.getAttribute('aria-controls')) : null;
        if (!button || !list) {
            return;
        }

        button.addEventListener('click', function () {
            button.disabled = true;
            list.setAttribute('aria-busy', 'true');

            fetch(button.getAttribute('data-url'), { headers: { 'X-Requested-With': 'XMLHttpRequest' } })
                .then(function (response) {
                    if (!response.ok) {
                        throw new Error('HTTP ' + response.status);
                    }
                    return response.text();
                })
                .then(function (html) {
                    var template = document.createElement('template');
                    template.innerHTML = html.trim();
                    var newItems = Array.prototype.slice.call(template.content.children);
                    newItems.forEach(function (item) { list.appendChild(item); });

                    // The new cards come after the ones already seen, so the page does not jump; keyboard users
                    // continue from the first new card instead of the removed button.
                    var firstLink = newItems.length > 0 ? newItems[0].querySelector('a') : null;
                    if (firstLink) {
                        firstLink.focus({ preventScroll: true });
                    }

                    root.remove();
                })
                .catch(function (error) {
                    console.error('Could not load the other events of the day', error);
                    button.disabled = false;
                    if (errorText) {
                        errorText.hidden = false;
                    }
                })
                .finally(function () {
                    list.removeAttribute('aria-busy');
                });
        });
    }

    document.querySelectorAll('[data-gs-period-more]').forEach(initMoreButton);
})();
