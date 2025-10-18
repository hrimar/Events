// Search autocomplete functionality
$('#search').autocomplete({
    source: '/Events/Search',
    minLength: 2,
    select: function(event, ui) {
        window.location.href = '/Events/Details/' + ui.item.id;
    }
});