import Header from './Header';
import './style/Articles.css';

const articles = [
    {
        id: 1,
        title: 'Статья о тренировках',
    },
    {
        id: 2,
        title: 'Статья о медитации',
    },
    {
        id: 3,
        title: 'Статья о планировании времени',
    },
];

export default function Articles() {
    return (
        <div className="articles-page">
            <div className="articles-content">
                <h1 className="articles-title">Полезные материалы</h1>

                <div className="articles-list">
                    {articles.map((article) => (
                        <button key={article.id} className="article-card">
                            <div className="article-image-placeholder" />
                            <div className="article-footer">
                                <span className="article-title">{article.title}</span>
                                <span className="article-arrow">›</span>
                            </div>
                        </button>
                    ))}
                </div>
            </div>
            <Header />
        </div>
    );
}

