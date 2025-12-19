// src/AuthPage.js
import React, { useState } from 'react';
import './AuthPage.css'; // Мы создадим этот CSS файл ниже

const AuthPage = () => {
    const [isLogin, setIsLogin] = useState(true); // true - Войти, false - Регистрация
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [username, setUsername] = useState('');

    const handleSwitch = () => {
        setIsLogin(!isLogin);
    };

    const handleSubmit = (e) => {
        e.preventDefault();
        // Здесь должна быть логика отправки формы на сервер
        console.log('Form submitted:', { email, password, username });
        // После успешной регистрации/входа, вы должны перенаправить пользователя на главную страницу
    };

    return (
        <div className="auth-page">
            <div className="auth-container">
                <div className="auth-tabs">
                    <button
                        className={`tab-button ${isLogin ? 'active' : ''}`}
                        onClick={() => setIsLogin(true)}
                    >
                        Войти
                    </button>
                    <button
                        className={`tab-button ${!isLogin ? 'active' : ''}`}
                        onClick={() => setIsLogin(false)}
                    >
                        Регистрация
                    </button>
                </div>

                <form className="auth-form" onSubmit={handleSubmit}>
                    <div className="input-group">
                        <span className="icon">✉️</span>
                        <input
                            type="email"
                            placeholder="Почта..."
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            required
                        />
                    </div>

                    <div className="input-group">
                        <span className="icon">🔒</span>
                        <input
                            type="password"
                            placeholder="Пароль..."
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            required
                        />
                    </div>

                    {!isLogin && (
                        <div className="input-group">
                            <span className="icon">👤</span>
                            <input
                                type="text"
                                placeholder="Имя пользователя..."
                                value={username}
                                onChange={(e) => setUsername(e.target.value)}
                                required
                            />
                        </div>
                    )}

                    <button type="submit" className="submit-button">
                        {isLogin ? 'Войти' : 'Регистрация'}
                    </button>
                </form>

                <div className="social-login">
                    <p>Или войдите через</p>
                    <div className="social-buttons">
                        <button className="social-btn yandex">Я</button>
                        <button className="social-btn google">G</button>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default AuthPage;