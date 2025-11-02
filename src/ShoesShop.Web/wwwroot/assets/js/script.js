document.addEventListener('DOMContentLoaded', () => {
    // Toggle between Login and Register forms
    const loginBtns = document.querySelectorAll('#login, [data-text="Sign in"]');
    const registerBtns = document.querySelectorAll('#register, [data-text="Sign Up"]');
    const loginForm = document.querySelector('.login-form');
    const registerForm = document.querySelector('.register-form');
    const col1 = document.querySelector('.col-1-login');

    loginBtns.forEach(btn => {
        btn.addEventListener('click', () => {
            document.querySelector('#login').style.backgroundColor = '#21264D';
            document.querySelector('#register').style.backgroundColor = 'rgba(255, 255, 255, 0.2)';

            loginForm.style.left = '50%';
            registerForm.style.left = '-50%';
            loginForm.style.opacity = '1';
            registerForm.style.opacity = '0';

            col1.style.borderRadius = '0 30% 20% 0';
        });
    });

    registerBtns.forEach(btn => {
        btn.addEventListener('click', () => {
            document.querySelector('#login').style.backgroundColor = 'rgba(255, 255, 255, 0.2)';
            document.querySelector('#register').style.backgroundColor = '#21264D';

            loginForm.style.left = '-150%';
            registerForm.style.left = '50%';
            loginForm.style.opacity = '0';
            registerForm.style.opacity = '1';

            col1.style.borderRadius = '0 20% 30% 0';
        });
    });

    // Calendar icon for date input
    const dateInput = document.getElementById('dob');
    const calendarIcon = document.getElementById('calendarIcon');
    if (calendarIcon && dateInput) {
        calendarIcon.addEventListener('click', () => {
            if (dateInput.showPicker) {
                dateInput.showPicker(); 
            } else {
                dateInput.focus();
            }
        });
    }

    // Custom file input for avatar upload
    const fileInput = document.querySelector('.custom-file-upload input[type="file"]');
    const fileText = document.getElementById('file-text');

    if (fileInput && fileText) {
        fileInput.addEventListener('change', () => {
            if (fileInput.files.length > 0) {
                fileText.textContent = fileInput.files[0].name; // hiện tên file
            } else {
                fileText.textContent = 'Avatar'; // trở về chữ mặc định
            }
        });
    }
});
