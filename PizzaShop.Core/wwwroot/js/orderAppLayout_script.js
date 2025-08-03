
$(document).ready(function () {
    // Toggle custom user dropdown visibility
    $('.user-dropdown-btn').on('click', function (e) {
        e.preventDefault();
        var $menu = $(this).siblings('.user-dropdown-menu');
        $menu.toggle();
    });

    // Close dropdown when clicking outside
    $(document).on('click', function (e) {
        var $dropdown = $('.custom-user-dropdown');
        if (!$dropdown.is(e.target) && $dropdown.has(e.target).length === 0) {
            $('.user-dropdown-menu').hide();
        }
    });

    // Set active class based on current URL on page load
    function setActiveLink() {
        var currentPath = window.location.pathname.toLowerCase();
        console.log("Current Path:", currentPath); // Debug current URL

        $('.orderAppLink').each(function () {
            var linkPath = $(this).attr('href');
            if (!linkPath) {
                console.log("No href found for:", this); // Debug missing href
                return;
            }
            linkPath = linkPath.toLowerCase();
            console.log("Link Path:", linkPath); // Debug each link’s href

            if (currentPath === linkPath) {
                console.log("Match found, adding active to:", linkPath);
                $(this).addClass('active');
            } else {
                $(this).removeClass('active');
            }
        });
    }

    // Call on page load
    setActiveLink();
});
