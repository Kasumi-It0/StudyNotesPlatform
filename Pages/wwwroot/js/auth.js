const API_BASE_URL = '/api/auth';
const STORAGE_TOKEN_KEY = 'accessToken';
const STORAGE_USER_KEY = 'userData';

const saveToStorage = (key, value) => localStorage.setItem(key, JSON.stringify(value));
const getFromStorage = (key) => {
    const data = localStorage.getItem(key);
    return data ? JSON.parse(data) : null;
};
const removeFromStorage = (key) => localStorage.removeItem(key);

const showMessage = (text) => {
    const messageDiv = document.getElementById('message');
    if (messageDiv) {
        messageDiv.textContent = text;
        setTimeout(() => {
            messageDiv.textContent = '';
        }, 5000);
    }
};

const handleAuthResponse = async (response, successCallback) => {
    const data = await response.json();

    if (response.ok) {
        saveToStorage(STORAGE_TOKEN_KEY, data.token);
        saveToStorage(STORAGE_USER_KEY, {
            fullName: data.fullName,
            email: data.email,
            university: data.university
        });
        successCallback(data);
    } else {
        showMessage(data.message || 'Ошибка');
    }
};

const checkAuth = () => {
    const token = getFromStorage(STORAGE_TOKEN_KEY);
    const user = getFromStorage(STORAGE_USER_KEY);

    if (!token || !user) {
        if (window.location.pathname === '/' || window.location.pathname === '/Index') {
            window.location.href = '/Login';
        }
        return null;
    }

    if (window.location.pathname.includes('/Login') || window.location.pathname.includes('/Register')) {
        window.location.href = '/';
    }

    return { token, user };
};

const initRegisterForm = () => {
    const form = document.getElementById('registerForm');
    if (!form) return;

    form.addEventListener('submit', async (e) => {
        e.preventDefault();

        const formData = {
            fullName: document.getElementById('fullName').value.trim(),
            email: document.getElementById('email').value.trim(),
            password: document.getElementById('password').value,
            university: document.getElementById('university').value
        };

        if (!formData.fullName || !formData.email || !formData.password || !formData.university) {
            showMessage('Заполните все поля');
            return;
        }

        if (formData.password.length < 6) {
            showMessage('Пароль должен быть не менее 6 символов');
            return;
        }

        try {
            const response = await fetch(`${API_BASE_URL}/register`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(formData)
            });

            await handleAuthResponse(response, () => {
                window.location.href = '/';
            });
        } catch (error) {
            showMessage('Ошибка сети. Попробуйте позже.');
        }
    });
};

const initLoginForm = () => {
    const form = document.getElementById('loginForm');
    if (!form) return;

    form.addEventListener('submit', async (e) => {
        e.preventDefault();

        const formData = {
            email: document.getElementById('email').value.trim(),
            password: document.getElementById('password').value
        };

        if (!formData.email || !formData.password) {
            showMessage('Заполните все поля');
            return;
        }

        try {
            const response = await fetch(`${API_BASE_URL}/login`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(formData)
            });

            await handleAuthResponse(response, () => {
                window.location.href = '/';
            });
        } catch (error) {
            showMessage('Ошибка сети. Попробуйте позже.');
        }
    });
};

const initDashboard = () => {
    const userInfoDiv = document.getElementById('userInfo');
    const logoutBtn = document.getElementById('logoutBtn');

    const user = getFromStorage(STORAGE_USER_KEY);

    if (userInfoDiv && user) {
        userInfoDiv.innerHTML = `
            <strong>${user.fullName}</strong><br>
            ${user.email}<br>
            🎓 ${user.university}
        `;
    }

    if (logoutBtn) {
        logoutBtn.addEventListener('click', () => {
            removeFromStorage(STORAGE_TOKEN_KEY);
            removeFromStorage(STORAGE_USER_KEY);
            window.location.href = '/Login';
        });
    }
};

document.addEventListener('DOMContentLoaded', () => {
    checkAuth();
    initRegisterForm();
    initLoginForm();
    initDashboard();
});