import { useNavigate } from 'react-router-dom';
import Header from './Header';
import './style/AppList.css';

export default function AppList() {
    const navigate = useNavigate();

    const apps = [
        {
            id: 'breathing',
            title: 'Дыхательная практика',
            description: 'Техники дыхания для расслабления и концентрации',
            icon: '🫁',
            color: '#7c3aed'
        },
        {
            id: 'meditation',
            title: 'Медитация',
            description: 'Скоро будет доступно',
            icon: '',
            color: '#9f6bff',
            comingSoon: true
        }
    ];

    const handleAppClick = (appId) => {
        if (apps.find(a => a.id === appId)?.comingSoon) {
            alert('Это приложение скоро будет доступно');
            return;
        }
        navigate(`/app/${appId}`);
    };

    return (
        <div className="app-list-page">
            <div className="app-list-content">
                <header className="app-list-header">
                    <h1>Приложения</h1>
                    <p>Выберите приложение для работы</p>
                </header>

                <div className="apps-grid">
                    {apps.map((app) => (
                        <div
                            key={app.id}
                            className={`app-card ${app.comingSoon ? 'coming-soon' : ''}`}
                            onClick={() => handleAppClick(app.id)}
                            style={{ '--app-color': app.color }}
                        >
                            <div className="app-icon" style={{ backgroundColor: app.color + '20' }}>
                                {app.icon}
                            </div>
                            <h3 className="app-title">{app.title}</h3>
                            <p className="app-description">{app.description}</p>
                            {app.comingSoon && (
                                <span className="coming-soon-badge">Скоро</span>
                            )}
                        </div>
                    ))}
                </div>
            </div>
            <Header />
        </div>
    );
}

