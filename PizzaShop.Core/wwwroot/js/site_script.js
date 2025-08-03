let droppingBox = document.querySelector(".droppingBox");
let openeye = document.querySelector(".openeye");
let openeye1 = document.querySelector(".openeye1");
let openeye2 = document.querySelector(".openeye2");
let openeye3 = document.querySelector(".openeye3");

let closeeye = document.querySelector(".closeeye");
let closeeye1 = document.querySelector(".closeeye1");
let closeeye2 = document.querySelector(".closeeye2");
let closeeye3 = document.querySelector(".closeeye3");

let exampleInputPassword1 = document.querySelector("#exampleInputPassword1");
let exampleInputPassword2 = document.querySelector("#exampleInputPassword2");
let exampleInputPassword3 = document.querySelector("#exampleInputPassword3");
let exampleInputPassword4 = document.querySelector("#exampleInputPassword4");

let count = false;
let count1 = false;
let count2 = false;
let count3 = false;


function togglePassword(eyeOpen, eyeClose, input, countState, countName) {
    countState = !countState;
    console.log(`clicked ${countName}`, countState);
    
    if (countState) {
        eyeOpen.classList.add("active");
        eyeOpen.classList.remove("inactive");
        eyeClose.classList.remove("active");
        eyeClose.classList.add("inactive");
        input.type = "text";
    } else {
        eyeOpen.classList.add("inactive");
        eyeOpen.classList.remove("active");
        eyeClose.classList.remove("inactive");
        eyeClose.classList.add("active");
        input.type = "password";
    }
    
    return countState;
}

// Usage
function eyeActive() {
    count = togglePassword(openeye, closeeye, exampleInputPassword1, count, "count");
}

function eyeActive1() {
    count1 = togglePassword(openeye1, closeeye1, exampleInputPassword2, count1, "count1");
}

function eyeActive2() {
    count2 = togglePassword(openeye2, closeeye2, exampleInputPassword3, count2, "count2");
}

function eyeActive3() {
    count3 = togglePassword(openeye3, closeeye3, exampleInputPassword4, count3, "count3");
}


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

    // Function to set active sidebar item
    function setActiveSidebarItem() {
        var activeLink = window.location.href.split('/')[window.location.href.split('/').length - 1].toLowerCase();
        var $sidebarItems = $('.ActiveAside');

        // Remove active class from all items
        $sidebarItems.removeClass('active');

        // Map data-value to corresponding URL paths
        var pathMap = {
            'UserDashboard': ['UserDashboard'],
            'UserTable': ['UserTable', 'Index'],
            'Role': ['Role'],
            'Menu': ['Menu', 'Index'],
            'section': ['section', 'Index'],
            'Tax': ['Tax', 'Index'],
            'Order': ['Order', 'Index'],
            'Customer': ['Customer', 'Index'],
            'Events': ['Events', 'Index'] // Added for Events
        };

        // Find matching data-value for current path
        $.each(pathMap, function (dataValue, paths) {
            if (paths.some(path => activeLink.includes(path.toLowerCase()))) {
                
                $sidebarItems.filter(`[data-value="${dataValue}"]`).addClass('active');
            }
        });
    }

    // Set active item on page load
    setActiveSidebarItem();

    // Handle click on sidebar items
    $('.ActiveAside').on('click', function () {
        $('.ActiveAside').removeClass('active'); // Remove active from all
        $(this).addClass('active'); // Add active to clicked item
    });
});
