// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Markup of a Lucide icon from the sprite, the client-side twin of the <gs-icon> tag helper (GS-REQ-006, 6.4).
// The versioned sprite URL comes from the data-icon-sprite attribute on <html> (_Layout.cshtml).
// name and extraClasses are fixed strings from our own scripts, never user input.
function gsIcon(name, extraClasses) {
    const sprite = document.documentElement.dataset.iconSprite || '/icons/gs-icons.svg';
    const classes = extraClasses ? `gs-icon ${extraClasses}` : 'gs-icon';
    return `<svg class="${classes}" aria-hidden="true" focusable="false"><use href="${sprite}#${name}"></use></svg>`;
}
