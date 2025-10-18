$(document).ready(function() {
    // Search autocomplete functionality
    $('#search').autocomplete({
        source: '/Events/Search',
        minLength: 2,
        select: function(event, ui) {
            window.location.href = '/Events/Details/' + ui.item.id;
        }
    });
    
    // Add loading states
    function showLoading() {
        $('.events-container').addClass('position-relative');
        $('.events-container').append('<div class="loading-overlay"><div class="loading-spinner"></div></div>');
    }
    
    function hideLoading() {
        $('.loading-overlay').remove();
    }
    
    // Auto-submit form when filters change
    $('#category, #free, #fromDate, #toDate').change(function() {
        showLoading();
        $('#search-form').submit();
    });
    
    // View toggle functionality
    $('#grid-view').click(function() {
        $('.events-grid').removeClass('events-list-view');
        $(this).addClass('active').siblings().removeClass('active');
        localStorage.setItem('eventsView', 'grid');
    });
    
    $('#list-view').click(function() {
        $('.events-grid').addClass('events-list-view');
        $(this).addClass('active').siblings().removeClass('active');
        localStorage.setItem('eventsView', 'list');
    });
    
    // Restore saved view preference
    var savedView = localStorage.getItem('eventsView');
    if (savedView === 'list') {
        $('#list-view').click();
    }
    
    // Enhanced search with debouncing
    let searchTimeout;
    $('#search').on('input', function() {
        clearTimeout(searchTimeout);
        const searchTerm = $(this).val();
        
        if (searchTerm.length >= 2) {
            searchTimeout = setTimeout(function() {
                // Could trigger instant search here
                console.log('Searching for:', searchTerm);
            }, 500);
        }
    });
});