import { BrowserRouter, Routes, Route } from 'react-router-dom';
import Login from './components/Login';
import Register from './components/Register';
import { useEffect } from 'react';
import { loadToken } from './api/authApi';

function App() {
    useEffect(() => {
        loadToken(); // восстанавливаем сессию при загрузке
    }, []);

    return (
        <BrowserRouter>
            <Routes>
                <Route path="/login" element={<Login />} />
                <Route path="/register" element={<Register />} />
                {/* другие маршруты */}
            </Routes>
        </BrowserRouter>
    );
}

export default App;