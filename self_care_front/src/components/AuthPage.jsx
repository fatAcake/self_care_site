import { useState, useEffect } from 'react';
import { login, register, setAuthToken } from '../api/authApi';
import { useNavigate } from 'react-router-dom';
import './style/AuthPage.css';

export default function AuthPage() {
    const [isLogin, setIsLogin] = useState(true); 
    const [form, setForm] = useState({
        email: '',
        password: '',
        nickname: '',
        timezone: 'UTC' 
    });
    const navigate = useNavigate();


    useEffect(() => {
        const token = localStorage.getItem('token');
        const user = localStorage.getItem('user');
        if (token && user) {
            navigate('/dashboard');
        }
    }, [navigate]);

    const handleChange = (e) => {
        setForm({ ...form, [e.target.name]: e.target.value });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            if (isLogin) {
                const data = await login({
                    email: form.email,
                    password: form.password
                });
                setAuthToken(data.access_token);
                localStorage.setItem('user', JSON.stringify({
                    user_id: data.user_id,
                    nickname: data.nickname,
                    email: data.email,
                    timezone: data.timezone
                }));
                navigate('/dashboard');
            } else {
                await register({
                    nickname: form.nickname,
                    email: form.email,
                    password: form.password,
                    timezone: form.timezone
                });
                alert('Регистрация успешна! Пожалуйста, войдите.');
                setIsLogin(true); 
            }
        } catch (err) {
            alert(err.response?.data?.message || err.response?.data || 'Произошла ошибка');
        }
    };

    return (
        <div className="auth-container">
            <div className="auth-header">
                <button
                    type="button"
                    className={`tab ${isLogin ? 'active' : ''}`}
                    onClick={() => setIsLogin(true)}
                >
                    Вход
                </button>
                <button
                    type="button"
                    className={`tab ${!isLogin ? 'active' : ''}`}
                    onClick={() => setIsLogin(false)}
                >
                    Регистрация
                </button>
            </div>

            <form onSubmit={handleSubmit} className="auth-form">
                <div className="input-group">
                    <span className="icon">✉️</span>
                    <input
                        name="email"
                        type="email"
                        placeholder="Почта..."
                        value={form.email}
                        onChange={handleChange}
                        required
                    />
                </div>

                <div className="input-group">
                    <span className="icon">🔒</span>
                    <input
                        name="password"
                        type="password"
                        placeholder="Пароль..."
                        value={form.password}
                        onChange={handleChange}
                        required
                    />
                </div>

                {!isLogin && (
                    <div className="input-group">
                        <span className="icon">👤</span>
                        <input
                            name="nickname"
                            placeholder="Имя пользователя..."
                            value={form.nickname}
                            onChange={handleChange}
                            required
                        />
                    </div>
                )}

                <button type="submit" className="submit-btn">
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
    );
}