import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { useEffect } from 'react';
import { loadToken } from './api/authApi';
import AuthPage from './components/AuthPage';
import Dashboard from './components/Dashboard';
import Articles from './components/Articles';
import AppPage from './components/AppPage';
import BreathingPractice from './components/BreathingPractice';
import ProtectedRoute from './components/ProtectedRoute';

function App() {
    useEffect(() => {
        loadToken();
    }, []);

    return (
        <BrowserRouter>
            <Routes>
                <Route path="/auth" element={<AuthPage />} />
                <Route 
                    path="/dashboard" 
                    element={
                        <ProtectedRoute>
                            <Dashboard />
                        </ProtectedRoute>
                    } 
                />
                <Route 
                    path="/articles" 
                    element={
                        <ProtectedRoute>
                            <Articles />
                        </ProtectedRoute>
                    } 
                />
                <Route 
                    path="/app" 
                    element={
                        <ProtectedRoute>
                            <AppPage />
                        </ProtectedRoute>
                    } 
                />
                <Route 
                    path="/app/breathing" 
                    element={
                        <ProtectedRoute>
                            <BreathingPractice />
                        </ProtectedRoute>
                    } 
                />
                <Route path="/" element={<Navigate to="/dashboard" replace />} />
            </Routes>
        </BrowserRouter>
    );
}

export default App;
