import Header from './Header';
import './AppPage.css';

export default function AppPage() {
    return (
        <div className="app-page">
            <div className="app-content">
                <h1>Приложение</h1>
                <p>Информация о приложении будет здесь</p>
            </div>
            <Header />
        </div>
    );
}

