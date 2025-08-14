document.addEventListener('DOMContentLoaded', () => {
    const loginbtn = document.querySelector('#login');
    const loginForm = document.querySelector('.login-form');
    const registerbtn = document.querySelector('#register');
    const registerForm = document.querySelector('.register-form');

    loginbtn.addEventListener('click', () => {
        loginbtn.style.backgroundColor = '#21264D';
        registerbtn.style.backgroundColor = 'rgba(255, 255, 255, 0.2)';
        loginForm.style.left = '50%';
        registerForm.style.left = '-50%';

        loginForm.style.opacity = '1';
        registerForm.style.opacity = '0';
        document.querySelector('.col-1-login').style.borderRadius = '0 30% 20% 0';
    });

    registerbtn.addEventListener('click', () => {
        loginbtn.style.backgroundColor = 'rgba(255, 255, 255, 0.2)';
        registerbtn.style.backgroundColor = '#21264D';
        loginForm.style.left = '-150%';
        registerForm.style.left = '50%';

        loginForm.style.opacity = '0';
        registerForm.style.opacity = '1';
        document.querySelector('.col-1-login').style.borderRadius = '0 20% 30% 0';
    });
});
