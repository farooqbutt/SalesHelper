document.addEventListener('DOMContentLoaded', function () {
    var offcanvasButtons = document.querySelectorAll('[data-toggle="offcanvas"]');
    var offcanvasComponents = document.querySelectorAll('.offcanvas');
    var backdrop = document.querySelector('.offcanvas-backdrop');

    function openOffcanvas(target) {
        target.classList.add('open');
        backdrop.classList.add('show');
    }

    function closeOffcanvas(target) {
        target.classList.remove('open');
        backdrop.classList.remove('show');
    }

    offcanvasButtons.forEach(function (button) {
        button.addEventListener('click', function () {
            var target = document.querySelector(button.getAttribute('data-target'));
            if (target) {
                openOffcanvas(target);
            }
        });
    });

    offcanvasComponents.forEach(function (offcanvas) {
        var closeButton = offcanvas.querySelector('[data-dismiss="offcanvas"]');
        closeButton.addEventListener('click', function () {
            closeOffcanvas(offcanvas);
        });

        document.addEventListener('click', function (event) {
            if (!offcanvas.contains(event.target) && !event.target.matches('[data-toggle="offcanvas"]')) {
                closeOffcanvas(offcanvas);
            }
        });
    });

    backdrop.addEventListener('click', function () {
        offcanvasComponents.forEach(function (offcanvas) {
            closeOffcanvas(offcanvas);
        });
    });

    // Expose functions to global scope
    window.openOffcanvas = function (selector) {
        var target = document.querySelector(selector);
        if (target) {
            openOffcanvas(target);
        }
    };

    window.closeOffcanvas = function (selector) {
        var target = document.querySelector(selector);
        if (target) {
            closeOffcanvas(target);
        }
    };
});
