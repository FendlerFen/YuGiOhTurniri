// ============================================
// SIMPLE CLIENT-SIDE VALIDATION
// Only for UX - Server does final validation
// ============================================

/**
 * Simple email validation - just basic checks
 */
function validacijaEmail(email) {
    if (!email || !email.trim()) {
        return { valid: false, error: 'Email je obavezan' };
    }
    const trimmedEmail = email.trim();
    if (!trimmedEmail.includes('@') || !trimmedEmail.includes('.')) {
        return { valid: false, error: 'Email mora biti validnog formata' };
    }
    return { valid: true, error: '' };
}

/**
 * Simple password validation - just length check
 */
function validacijaLozinka(lozinka) {
    return lozinka && lozinka.length >= 6;
}

// ============================================
// HELPER FUNKCIJE ZA PRIKAZ GRE?KE
// ============================================

function prikaziGresku(element, poruka) {
    if (!element) return;

    const existingError = element.parentElement.querySelector('.form-error');
    if (existingError) {
        existingError.remove();
    }

    const errorDiv = document.createElement('div');
    errorDiv.className = 'form-error';
    errorDiv.textContent = poruka;
    errorDiv.style.marginTop = '0.25rem';

    element.parentElement.appendChild(errorDiv);
    element.style.borderColor = '#f56565';
}

function ukloniGresku(element) {
    if (!element) return;

    const existingError = element.parentElement.querySelector('.form-error');
    if (existingError) {
        existingError.remove();
    }
    element.style.borderColor = '#cbd5e0';
}

// ============================================
// SIMPLE CLIENT-SIDE VALIDATION
// Only for UX - Server does final validation
// ============================================

/**
 * Simple email validation - just basic checks
 */
function validacijaEmail(email) {
    if (!email || !email.trim()) {
        return { valid: false, error: 'Email je obavezan' };
    }
    const trimmedEmail = email.trim();
    if (!trimmedEmail.includes('@') || !trimmedEmail.includes('.')) {
        return { valid: false, error: 'Email mora biti validnog formata' };
    }
    return { valid: true, error: '' };
}

/**
 * Simple password validation - just length check
 */
function validacijaLozinka(lozinka) {
    return lozinka && lozinka.length >= 6;
}

// ============================================
// HELPER FUNKCIJE ZA PRIKAZ GRE?KE
// ============================================

function prikaziGresku(element, poruka) {
    if (!element) return;

    const existingError = element.parentElement.querySelector('.form-error');
    if (existingError) {
        existingError.remove();
    }

    const errorDiv = document.createElement('div');
    errorDiv.className = 'form-error';
    errorDiv.textContent = poruka;
    errorDiv.style.marginTop = '0.25rem';

    element.parentElement.appendChild(errorDiv);
    element.style.borderColor = '#f56565';
}

function ukloniGresku(element) {
    if (!element) return;

    const existingError = element.parentElement.querySelector('.form-error');
    if (existingError) {
        existingError.remove();
    }
    element.style.borderColor = '#cbd5e0';
}

// ============================================
// FORM INITIALIZATION
// ============================================

function initializeFormValidation() {
    // Optional: Real-time email validation for better UX
    const emailInputs = document.querySelectorAll('input[type="email"]');
    emailInputs.forEach(input => {
        input.addEventListener('blur', function() {
            if (this.value.length > 0) {
                const validation = validacijaEmail(this.value);
                if (!validation.valid) {
                    prikaziGresku(this, validation.error);
                } else {
                    ukloniGresku(this);
                }
            }
        });
    });

    // Optional: Real-time password validation
    const passwordInputs = document.querySelectorAll('input[name="Lozinka"]');
    passwordInputs.forEach(input => {
        input.addEventListener('blur', function() {
            if (this.value.length > 0 && !validacijaLozinka(this.value)) {
                prikaziGresku(this, 'Lozinka mora imati najmanje 6 karaktera');
            } else if (this.value.length > 0) {
                ukloniGresku(this);
            }
        });
    });
}

// Wait for DOM to be ready
document.addEventListener('DOMContentLoaded', function() {
    initializeFormValidation();
});


// Testiranje - ovo ?e biti vidljivo u console-u
console.log('=== VALIDACIJA.JS U?ITANA ===');

// Wait for DOM to be ready
document.addEventListener('DOMContentLoaded', function() {
    console.log('=== DOM Content Loaded ===');
    initializeFormValidation();
});
