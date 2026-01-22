/* ============================================
   NOTES SYSTEM ADMIN - JAVASCRIPT
   Theme Toggle & Admin Functionality
   ============================================ */

// Wait for DOM to be fully loaded
document.addEventListener('DOMContentLoaded', function () {

    // Initialize theme
    initTheme();

    // Initialize animations
    initAnimations();

    // Initialize toast notifications
    initToasts();

    // Initialize form validation
    initFormValidation();

});

/* ============================================
   THEME TOGGLE FUNCTIONALITY
   ============================================ */
function initTheme() {
    const themeToggle = document.getElementById('themeToggle');
    const themeIcon = document.getElementById('themeIcon');
    const html = document.documentElement;

    // Check for saved theme preference
    const savedTheme = localStorage.getItem('admin-theme') || 'light';
    html.setAttribute('data-theme', savedTheme);
    updateThemeIcon(savedTheme);

    // Theme toggle click handler
    if (themeToggle) {
        themeToggle.addEventListener('click', function () {
            const currentTheme = html.getAttribute('data-theme');
            const newTheme = currentTheme === 'light' ? 'dark' : 'light';

            // Add transition class
            html.classList.add('theme-transition');

            // Update theme
            html.setAttribute('data-theme', newTheme);
            localStorage.setItem('admin-theme', newTheme);

            // Update icon
            updateThemeIcon(newTheme);

            // Remove transition class
            setTimeout(function () {
                html.classList.remove('theme-transition');
            }, 300);
        });
    }
}

function updateThemeIcon(theme) {
    const themeIcon = document.getElementById('themeIcon');
    if (themeIcon) {
        if (theme === 'dark') {
            themeIcon.classList.remove('bi-moon-fill');
            themeIcon.classList.add('bi-sun-fill');
        } else {
            themeIcon.classList.remove('bi-sun-fill');
            themeIcon.classList.add('bi-moon-fill');
        }
    }
}

/* ============================================
   SCROLL ANIMATIONS
   ============================================ */
function initAnimations() {
    // Add scroll animation to elements
    const animateElements = document.querySelectorAll('.stat-card, .dashboard-card, .action-card');

    const observer = new IntersectionObserver(function (entries) {
        entries.forEach(function (entry) {
            if (entry.isIntersecting) {
                entry.target.classList.add('animate-fade-in');
                observer.unobserve(entry.target);
            }
        });
    }, {
        threshold: 0.1
    });

    animateElements.forEach(function (element) {
        observer.observe(element);
    });
}

/* ============================================
   TOAST NOTIFICATIONS
   ============================================ */
function initToasts() {
    // Auto hide toasts after 5 seconds
    const toasts = document.querySelectorAll('.toast');

    toasts.forEach(function (toast) {
        // Add animation class
        toast.classList.add('animate-fade-in');

        // Auto hide after 5 seconds
        setTimeout(function () {
            toast.classList.remove('show');
            setTimeout(function () {
                toast.remove();
            }, 300);
        }, 5000);
    });
}

/* ============================================
   FORM VALIDATION
   ============================================ */
function initFormValidation() {
    // Add focus effects to form inputs
    const formInputs = document.querySelectorAll('.form-control');

    formInputs.forEach(function (input) {
        // Focus effect
        input.addEventListener('focus', function () {
            this.closest('.form-floating')?.classList.add('focused');
        });

        // Blur effect
        input.addEventListener('blur', function () {
            this.closest('.form-floating')?.classList.remove('focused');
        });

        // Input validation feedback
        input.addEventListener('input', function () {
            if (this.value.length > 0) {
                this.classList.add('has-value');
            } else {
                this.classList.remove('has-value');
            }
        });
    });
}

/* ============================================
   UTILITY FUNCTIONS
   ============================================ */

// Show loading spinner on button
function showButtonLoader(button, text) {
    button.disabled = true;
    button.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>' + text;
}

// Hide loading spinner on button
function hideButtonLoader(button, text) {
    button.disabled = false;
    button.innerHTML = text;
}

// Show alert message
function showAlert(type, message) {
    const alertHtml = '<div class="alert alert-' + type + ' alert-dismissible fade show" role="alert">' +
        '<i class="bi bi-' + (type === 'success' ? 'check-circle-fill' : 'exclamation-triangle-fill') + ' me-2"></i>' +
        message +
        '<button type="button" class="btn-close" data-bs-dismiss="alert"></button>' +
        '</div>';

    const container = document.querySelector('.main-content');
    if (container) {
        container.insertAdjacentHTML('afterbegin', alertHtml);

        // Auto remove after 5 seconds
        setTimeout(function () {
            const alert = container.querySelector('.alert');
            if (alert) {
                alert.remove();
            }
        }, 5000);
    }
}

// Format number with commas
function formatNumber(num) {
    return num.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}

// Format date
function formatDate(date) {
    const options = { year: 'numeric', month: 'long', day: 'numeric' };
    return new Date(date).toLocaleDateString('en-US', options);
}

/* ============================================
   NAVBAR SCROLL EFFECT
   ============================================ */
window.addEventListener('scroll', function () {
    const header = document.querySelector('.main-header');
    if (header) {
        if (window.scrollY > 50) {
            header.classList.add('scrolled');
        } else {
            header.classList.remove('scrolled');
        }
    }
});

/* ============================================
   SIDEBAR TOGGLE (For future use)
   ============================================ */
function toggleSidebar() {
    const sidebar = document.querySelector('.admin-sidebar');
    const mainContent = document.querySelector('.main-content');

    if (sidebar) {
        sidebar.classList.toggle('collapsed');
        mainContent.classList.toggle('expanded');
    }
}
