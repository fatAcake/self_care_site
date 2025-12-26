import { useState } from 'react';
import { login, setAuthToken } from '../api/authApi';
import { useNavigate } from 'react-router-dom';

export default function Login() {
    const [form, setForm] = useState({ email: '', password: '' });
    const navigate = useNavigate();

    const handleChange = (e) => {
        setForm({ ...form, [e.target.name]: e.target.value });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            const data = await login(form);
            setAuthToken(data.access_token);
            localStorage.setItem('user', JSON.stringify({
                user_id: data.user_id,
                nickname: data.nickname,
                email: data.email,
                timezone: data.timezone
            }));
            navigate('/dashboard'); 
        } catch (err) {
            alert('Login failed: ' + (err.response?.data || 'Unknown error'));
        }
    };

    return (
        <form onSubmit={handleSubmit}>
            <input name="email" type="email" placeholder="Email" onChange={handleChange} required />
            <input name="password" type="password" placeholder="Password" onChange={handleChange} required />
            <button type="submit">Login</button>
        </form>
    );
}