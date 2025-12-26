import { useState } from 'react';
import { register } from '../api/authApi';
import { useNavigate } from 'react-router-dom';

export default function Register() {
    const [form, setForm] = useState({ nickname: '', email: '', password: '', timezone: 'UTC' });
    const navigate = useNavigate();

    const handleChange = (e) => {
        setForm({ ...form, [e.target.name]: e.target.value });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            await register(form);
            alert('Registration successful! Please log in.');
            navigate('/login');
        } catch (err) {
            alert(err.response?.data || 'Registration failed');
        }
    };

    return (
        <form onSubmit={handleSubmit}>
            <input name="nickname" placeholder="Nickname" onChange={handleChange} required />
            <input name="email" type="email" placeholder="Email" onChange={handleChange} required />
            <input name="password" type="password" placeholder="Password" onChange={handleChange} required />
            <button type="submit">Register</button>
        </form>
    );
}