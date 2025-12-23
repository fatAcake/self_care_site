import { useNavigate, useLocation } from 'react-router-dom';
import { setAuthToken } from '../api/authApi';
import './Header.css';

export default function Header() {
    const navigate = useNavigate();
    const location = useLocation();
    
    const user = JSON.parse(localStorage.getItem('user') || '{}');

    const handleLogout = () => {
        setAuthToken(null);
        localStorage.removeItem('token');
        localStorage.removeItem('user');
        navigate('/auth');
    };

    const isActive = (path) => {
        return location.pathname === path;
    };

    return (
        <header className="app-header">
            <div className="header-content">
                <nav className="header-nav">
                    <button
                        className={`nav-icon-button ${isActive('/dashboard') ? 'active' : ''}`}
                        onClick={() => navigate('/dashboard')}
                        aria-label="Главная"
                    >
                        <span className="icon-square" />
                    </button>
                    <button
                        className={`nav-icon-button ${isActive('/articles') ? 'active' : ''}`}
                        onClick={() => navigate('/articles')}
                        aria-label="Статьи"
                    >
                        <span className="icon-book" />
                    </button>
                    <button
                        className={`nav-icon-button ${isActive('/app') ? 'active' : ''}`}
                        onClick={() => navigate('/app')}
                        aria-label="Приложение"
                    >
                        <span className="icon-grid" />
                    </button>
                </nav>

                <button className="logout-chip" onClick={handleLogout}>
                    {user.nickname || 'Пользователь'} · Выйти
                </button>
            </div>
        </header>
    );
}

