// "Date" chip (GS-REQ-007, 7.2; GS-REQ-008, 8.2.4): a calendar from Monday; the picked day opens "All events"
// for that day. Used by the homepage hero and "All events"; data-url may already hold the other filters.
// With data-range (the "All events" pages) the calendar also picks a from-to range: two clicks pick the range,
// one click and closing the calendar picks that day.
(function () {
    'use strict';

    function loadScript(src) {
        return new Promise(function (resolve, reject) {
            var script = document.createElement('script');
            script.src = src;
            script.onload = resolve;
            script.onerror = reject;
            document.head.appendChild(script);
        });
    }

    // flatpickr (already used on "All events") is loaded on the first click only, to keep the pages light.
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

    function initDateChip(button) {
        var picker = null;
        var isRange = button.hasAttribute('data-range');

        // The other filters in data-url are kept; the days replace any period or dates.
        function openDays(instance, fromDate, toDate) {
            var url = new URL(button.getAttribute('data-url'), window.location.href);
            url.searchParams.set('fromDate', instance.formatDate(fromDate, 'Y-m-d'));
            url.searchParams.set('toDate', instance.formatDate(toDate, 'Y-m-d'));
            window.location.href = url.toString();
        }

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

                // "2026-11-05" or "2026-11-05,2026-11-20"
                var selected = (button.getAttribute('data-selected-date') || '').split(',').filter(Boolean);
                var changed = false;

                picker = window.flatpickr(input, {
                    locale: Object.assign({}, baseLocale, { firstDayOfWeek: 1 }),
                    mode: isRange ? 'range' : 'single',
                    dateFormat: 'Y-m-d',
                    minDate: button.getAttribute('data-min-date'),
                    defaultDate: selected.length > 0 ? selected : null,
                    positionElement: button,
                    disableMobile: true,
                    onChange: function (selectedDates, dateText, instance) {
                        changed = true;
                        if (!isRange && selectedDates.length === 1) {
                            openDays(instance, selectedDates[0], selectedDates[0]);
                        } else if (isRange && selectedDates.length === 2) {
                            openDays(instance, selectedDates[0], selectedDates[1]);
                        }
                    },
                    onClose: function (selectedDates, dateText, instance) {
                        // One day picked in the range calendar, then closed: that day alone.
                        if (isRange && changed && selectedDates.length === 1) {
                            openDays(instance, selectedDates[0], selectedDates[0]);
                        }
                    }
                });
                picker.open();
            }).catch(function (error) {
                console.error('Could not load the calendar', error);
            });
        });
    }

    document.querySelectorAll('[data-gs-date-chip]').forEach(initDateChip);
})();
