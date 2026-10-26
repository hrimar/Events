// Desktop homepage hero: featured carousel (GS-REQ-007, 7.4) and the "Date" chip (GS-REQ-007, 7.2).
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
    document.querySelectorAll('[data-gs-date-chip]').forEach(initDateChip);
})();
