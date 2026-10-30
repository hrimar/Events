// Period view (GS-REQ-008, 8.2.8): "More N events on Friday" loads all the other events of the day from
// Events/PeriodDay and appends them to the day's list, without leaving the page or moving the scroll position.
// With more than 200 events in the period, the days after the third come without cards (8.2.11); each loads its
// section from Events/PeriodDaySection when it comes near on scroll.
(function () {
    'use strict';

    // Start loading a deferred day this far before it reaches the screen, so its cards are usually there in time.
    var lazyMargin = '800px 0px';

    function fetchHtml(url) {
        return fetch(url, { headers: { 'X-Requested-With': 'XMLHttpRequest' } })
            .then(function (response) {
                if (!response.ok) {
                    var error = new Error('HTTP ' + response.status);
                    error.status = response.status;
                    throw error;
                }
                return response.text();
            })
            .then(function (html) {
                var template = document.createElement('template');
                template.innerHTML = html.trim();
                return template.content;
            });
    }

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

            fetchHtml(button.getAttribute('data-url'))
                .then(function (content) {
                    var newItems = Array.prototype.slice.call(content.children);
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

    // placeholder: the day's "See all for the day" block, which holds the URL of the day section.
    function loadDeferredDay(placeholder) {
        var section = placeholder.closest('section');
        fetchHtml(placeholder.getAttribute('data-gs-period-lazy'))
            .then(function (content) {
                var newSection = content.firstElementChild;
                if (!newSection) {
                    throw new Error('Unexpected response');
                }

                // A keyboard user on the "See all for the day" link continues from the day's first card.
                var hadFocus = section.contains(document.activeElement);
                section.replaceWith(newSection);
                newSection.querySelectorAll('[data-gs-period-more]').forEach(initMoreButton);

                var firstLink = hadFocus ? newSection.querySelector('.gs-event-grid a') : null;
                if (firstLink) {
                    firstLink.focus({ preventScroll: true });
                }
            })
            .catch(function (error) {
                // The day has no events any more (they were removed after the page was built).
                if (error.status === 404) {
                    section.remove();
                    return;
                }

                // The link to the day's own page stays, with a short notice.
                console.error('Could not load the events of the day', error);
                var errorText = section.querySelector('[role="alert"]');
                if (errorText) {
                    errorText.hidden = false;
                }
            });
    }

    function initDeferredDays() {
        var placeholders = Array.prototype.slice.call(document.querySelectorAll('[data-gs-period-lazy]'));
        if (placeholders.length === 0) {
            return;
        }

        if (!('IntersectionObserver' in window)) {
            placeholders.forEach(loadDeferredDay);
            return;
        }

        var observer = new IntersectionObserver(function (entries) {
            entries.forEach(function (entry) {
                if (entry.isIntersecting) {
                    observer.unobserve(entry.target);
                    loadDeferredDay(entry.target);
                }
            });
        }, { rootMargin: lazyMargin });

        placeholders.forEach(function (placeholder) { observer.observe(placeholder); });
    }

    document.querySelectorAll('[data-gs-period-more]').forEach(initMoreButton);
    initDeferredDays();
})();
