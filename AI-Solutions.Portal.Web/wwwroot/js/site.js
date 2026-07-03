// Navbar scroll effect
(function () {
    const navbar = document.querySelector('.ai-navbar');
    if (!navbar) return;

    function updateNavbar() {
        if (window.scrollY > 20) {
            navbar.classList.add('scrolled');
        } else {
            navbar.classList.remove('scrolled');
        }
    }

    window.addEventListener('scroll', updateNavbar);
    updateNavbar();
})();
