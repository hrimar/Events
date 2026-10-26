// Desktop homepage: hero featured carousel (GS-REQ-007, 7.4), "Date" chip (GS-REQ-007, 7.2)
// card rows with arrows (GS-REQ-007, 7.5) and the "This weekend" tabs and filters (GS-REQ-007, 7.6).
(function () {
    'use strict';

    // Landscape images fill the frame (cover); portrait posters stay whole on the blurred copy (GS-REQ-006, 6.5).
    function markOrientation(img) {
        function apply() {
            if (img.naturalWidth > 0 && img.naturalWidth >= img.naturalHeight) {
                img.closest('.gs-hero-media').classList.add('is-landscape');
            }
        }

        if (img.complete) {
            apply();
        } else {
            img.addEventListener('load', apply, { once: true });
        }
    }

    // Starts loading the images of a slide that is about to be shown.
    function preloadSlide(slide) {
        slide.querySelectorAll('img[loading="lazy"]').forEach(function (img) {
            img.loading = 'eager';
        });
    }

    // Auto-advances every data-interval ms; pauses on hover, keyboard focus, hidden tab and reduced motion.
    function initCarousel(root) {
        root.querySelectorAll('[data-gs-orientation]').forEach(markOrientation);

        var slides = Array.prototype.slice.call(root.querySelectorAll('[data-gs-slide]'));
        var dots = Array.prototype.slice.call(root.querySelectorAll('[data-gs-dot]'));
        var liveRegion = root.querySelector('[data-gs-slides]');
        var prevButton = root.querySelector('[data-gs-prev]');
        var nextButton = root.querySelector('[data-gs-next]');

        // Without controls (single or fallback slide) there is nothing to rotate.
        if (slides.length < 2 || !nextButton) {
            return;
        }

        var interval = parseInt(root.getAttribute('data-interval'), 10) || 6000;
        var reducedMotion = window.matchMedia('(prefers-reduced-motion: reduce)');
        var current = 0;
        var timer = null;
        var isHovered = false;
        var hasFocus = false;

        function show(index) {
            var next = (index + slides.length) % slides.length;
            if (next === current) {
                return;
            }

            slides[current].hidden = true;
            slides[next].hidden = false;
            dots.forEach(function (dot, i) {
                dot.setAttribute('aria-current', i === next ? 'true' : 'false');
            });
            current = next;
            preloadSlide(slides[(current + 1) % slides.length]);
        }

        function canRotate() {
            return !isHovered && !hasFocus && !document.hidden && !reducedMotion.matches;
        }

        // Restarts the timer; while paused, slide changes are announced to screen readers.
        function schedule() {
            window.clearTimeout(timer);
            var rotating = canRotate();
            liveRegion.setAttribute('aria-live', rotating ? 'off' : 'polite');
            if (rotating) {
                timer = window.setTimeout(function () {
                    show(current + 1);
                    schedule();
                }, interval);
            }
        }

        prevButton.addEventListener('click', function () { show(current - 1); schedule(); });
        nextButton.addEventListener('click', function () { show(current + 1); schedule(); });
        dots.forEach(function (dot, i) {
            dot.addEventListener('click', function () { show(i); schedule(); });
        });

        root.addEventListener('mouseenter', function () { isHovered = true; schedule(); });
        root.addEventListener('mouseleave', function () { isHovered = false; schedule(); });
        root.addEventListener('focusin', function () { hasFocus = true; schedule(); });
        root.addEventListener('focusout', function (e) {
            if (!root.contains(e.relatedTarget)) {
                hasFocus = false;
                schedule();
            }
        });
        document.addEventListener('visibilitychange', schedule);
        if (reducedMotion.addEventListener) {
            reducedMotion.addEventListener('change', schedule);
        }

        // The second slide loads after the page, so the first paint only waits for the first image.
        if (document.readyState === 'complete') {
            preloadSlide(slides[1]);
        } else {
            window.addEventListener('load', function () { preloadSlide(slides[1]); }, { once: true });
        }

        schedule();
    }

    // Card row ("Tonight", GS-REQ-007, 7.5.3): the arrows scroll by the visible width (5, 4 or 3 cards)
    // and are disabled at the start and the end. Native scrolling keeps keyboard focus and touch working.
    function initCardRow(root) {
        var track = root.querySelector('[data-gs-row-track]');
        var prevButton = root.querySelector('[data-gs-row-prev]');
        var nextButton = root.querySelector('[data-gs-row-next]');
        if (!track || !prevButton || !nextButton) {
            return;
        }

        var reducedMotion = window.matchMedia('(prefers-reduced-motion: reduce)');

        function updateButtons() {
            var maxScroll = track.scrollWidth - track.clientWidth;
            prevButton.disabled = track.scrollLeft <= 1;
            nextButton.disabled = track.scrollLeft >= maxScroll - 1;
        }

        function scrollByPage(direction) {
            var gap = parseFloat(window.getComputedStyle(track).columnGap) || 0;
            track.scrollBy({
                left: direction * (track.clientWidth + gap),
                behavior: reducedMotion.matches ? 'auto' : 'smooth'
            });
        }

        prevButton.addEventListener('click', function () { scrollByPage(-1); });
        nextButton.addEventListener('click', function () { scrollByPage(1); });
        track.addEventListener('scroll', updateButtons, { passive: true });
        window.addEventListener('resize', updateButtons);
        updateButtons();
    }

    // "This weekend" (GS-REQ-007, 7.6): day tabs (role="tablist", arrow keys) and filter chips reload the grid in
    // place from Home/WeekendEvents; "Show more" appends the next 8 cards and moves the focus to the first new one.
    function initWeekend(root) {
        var url = root.getAttribute('data-url');
        var panel = root.querySelector('[data-gs-weekend-panel]');
        var moreButton = root.querySelector('[data-gs-weekend-more]');
        var errorText = root.querySelector('[data-gs-weekend-error]');
        var tabs = Array.prototype.slice.call(root.querySelectorAll('[role="tab"]'));
        var chips = Array.prototype.slice.call(root.querySelectorAll('[data-filter]'));
        if (!url || !panel || tabs.length === 0) {
            return;
        }

        var state = { day: tabs[0].getAttribute('data-day'), filter: 'all' };
        var requestId = 0;

        function currentGrid() {
            return panel.querySelector('[data-gs-weekend-grid]');
        }

        function updateMoreButton(grid) {
            if (moreButton) {
                moreButton.hidden = !grid || grid.getAttribute('data-has-more') !== 'true';
            }
        }

        function load(append) {
            var grid = currentGrid();
            var skip = append && grid ? grid.querySelectorAll('li').length : 0;
            var id = ++requestId;
            var query = '?day=' + encodeURIComponent(state.day) + '&filter=' + encodeURIComponent(state.filter) + '&skip=' + skip;

            panel.setAttribute('aria-busy', 'true');
            if (moreButton) {
                moreButton.disabled = true;
            }

            return fetch(url + query, { headers: { 'X-Requested-With': 'XMLHttpRequest' } })
                .then(function (response) {
                    if (!response.ok) {
                        throw new Error('HTTP ' + response.status);
                    }
                    return response.text();
                })
                .then(function (html) {
                    // A newer tab or chip click won: drop this answer.
                    if (id !== requestId) {
                        return;
                    }

                    var template = document.createElement('template');
                    template.innerHTML = html.trim();
                    var newGrid = template.content.querySelector('[data-gs-weekend-grid]');
                    if (!newGrid) {
                        throw new Error('Unexpected response');
                    }

                    if (append && grid && grid.tagName === 'UL') {
                        var newItems = Array.prototype.slice.call(newGrid.children);
                        newItems.forEach(function (item) { grid.appendChild(item); });
                        grid.setAttribute('data-has-more', newGrid.getAttribute('data-has-more'));
                        updateMoreButton(grid);

                        var firstLink = newItems.length > 0 ? newItems[0].querySelector('a') : null;
                        if (firstLink) {
                            firstLink.focus();
                        }
                    } else {
                        if (grid) {
                            grid.replaceWith(newGrid);
                        } else {
                            panel.appendChild(newGrid);
                        }
                        updateMoreButton(newGrid);
                    }

                    if (errorText) {
                        errorText.hidden = true;
                    }
                })
                .catch(function (error) {
                    if (id === requestId && errorText) {
                        errorText.hidden = false;
                    }
                    console.error('Could not load weekend events', error);
                })
                .then(function () {
                    if (id === requestId) {
                        panel.removeAttribute('aria-busy');
                        if (moreButton) {
                            moreButton.disabled = false;
                        }
                    }
                });
        }

        function selectTab(tab, moveFocus) {
            tabs.forEach(function (other) {
                var isSelected = other === tab;
                other.setAttribute('aria-selected', isSelected ? 'true' : 'false');
                other.tabIndex = isSelected ? 0 : -1;
            });
            panel.setAttribute('aria-labelledby', tab.id);
            if (moveFocus) {
                tab.focus();
            }

            var day = tab.getAttribute('data-day');
            if (day !== state.day) {
                state.day = day;
                load(false);
            }
        }

        tabs.forEach(function (tab, index) {
            tab.addEventListener('click', function () { selectTab(tab, false); });

            // Arrow keys, Home and End move between the tabs (WAI-ARIA tabs pattern).
            tab.addEventListener('keydown', function (e) {
                var target = null;
                if (e.key === 'ArrowRight') {
                    target = tabs[(index + 1) % tabs.length];
                } else if (e.key === 'ArrowLeft') {
                    target = tabs[(index - 1 + tabs.length) % tabs.length];
                } else if (e.key === 'Home') {
                    target = tabs[0];
                } else if (e.key === 'End') {
                    target = tabs[tabs.length - 1];
                }

                if (target) {
                    e.preventDefault();
                    selectTab(target, true);
                }
            });
        });

        chips.forEach(function (chip) {
            chip.addEventListener('click', function () {
                var filter = chip.getAttribute('data-filter');
                if (filter === state.filter) {
                    return;
                }

                chips.forEach(function (other) {
                    other.setAttribute('aria-pressed', other === chip ? 'true' : 'false');
                });
                state.filter = filter;
                load(false);
            });
        });

        if (moreButton) {
            moreButton.addEventListener('click', function () { load(true); });
        }
    }

    function loadScript(src) {
        return new Promise(function (resolve, reject) {
            var script = document.createElement('script');
            script.src = src;
            script.onload = resolve;
            script.onerror = reject;
            document.head.appendChild(script);
        });
    }

    // flatpickr (already used on "All events") is loaded on the first click only, to keep the homepage light.
    function loadFlatpickr(assetsBase, locale) {
        if (window.flatpickr) {
            return Promise.resolve();
        }

        var link = document.createElement('link');
        link.rel = 'stylesheet';
        link.href = assetsBase + '/flatpickr.min.css';
        document.head.appendChild(link);

        return loadScript(assetsBase + '/flatpickr.min.js').then(function () {
            return locale === 'bg' ? loadScript(assetsBase + '/l10n/bg.js') : null;
        });
    }

    // "Date" chip: a calendar from Monday; the picked day opens "All events" for that day.
    function initDateChip(button) {
        var picker = null;

        button.addEventListener('click', function () {
            if (picker) {
                picker.open();
                return;
            }

            var locale = button.getAttribute('data-locale');
            loadFlatpickr(button.getAttribute('data-assets'), locale).then(function () {
                var input = document.createElement('input');
                input.type = 'text';
                input.className = 'gs-date-chip-input';
                input.tabIndex = -1;
                input.setAttribute('aria-hidden', 'true');
                button.insertAdjacentElement('afterend', input);

                var baseLocale = locale === 'bg' ? window.flatpickr.l10ns.bg : window.flatpickr.l10ns.default;
                picker = window.flatpickr(input, {
                    locale: Object.assign({}, baseLocale, { firstDayOfWeek: 1 }),
                    dateFormat: 'Y-m-d',
                    minDate: button.getAttribute('data-min-date'),
                    positionElement: button,
                    disableMobile: true,
                    onChange: function (selectedDates, dateText) {
                        if (dateText) {
                            window.location.href = button.getAttribute('data-url') +
                                '?fromDate=' + encodeURIComponent(dateText) + '&toDate=' + encodeURIComponent(dateText);
                        }
                    }
                });
                picker.open();
            }).catch(function (error) {
                console.error('Could not load the calendar', error);
            });
        });
    }

    document.querySelectorAll('[data-gs-carousel]').forEach(initCarousel);
    document.querySelectorAll('[data-gs-card-row]').forEach(initCardRow);
    document.querySelectorAll('[data-gs-weekend]').forEach(initWeekend);
    document.querySelectorAll('[data-gs-date-chip]').forEach(initDateChip);
})();
