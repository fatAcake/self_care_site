import { Navigate, useLocation } from 'react-router-dom';
import { useEffect, useState } from 'react';
import { loadToken } from '../api/authApi';

export default function ProtectedRoute({ children }) {
    const [isAuthenticated, setIsAuthenticated] = useState(null);
    const location = useLocation();

    useEffect(() => {
      
        const token = localStorage.getItem('token');
        const user = localStorage.getItem('user');
        
        if (token && user) {
            loadToken(); 
            setIsAuthenticated(true);
        } else {
            setIsAuthenticated(false);
        }
    }, []);

    
    if (isAuthenticated === null) {
        return <div style={{ 
            display: 'flex', 
            justifyContent: 'center', 
            alignItems: 'center', 
            height: '100vh',
            color: 'white',
            fontSize: '18px'
        }}>Загрузка...</div>;
    }


    if (!isAuthenticated) {
        // Сохраняем текущий путь для редиректа после авторизации
        return <Navigate to="/auth" state={{ from: location.pathname }} replace />;
    }

    return children;
}

