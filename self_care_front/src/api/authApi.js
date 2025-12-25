import axios from 'axios';
import { API_BASE_URL } from '../config'; 

export const register = async (userData) => {
    const res = await axios.post(`${API_BASE_URL}/auth/register`, userData);
    return res.data;
};

export const login = async (credentials) => {
    const res = await axios.post(`${API_BASE_URL}/auth/login`, credentials);
    return res.data;
};


export const setAuthToken = (token) => {
    if (token) {
        axios.defaults.headers.common['Authorization'] = `Bearer ${token}`;
        localStorage.setItem('token', token);
    } else {
        delete axios.defaults.headers.common['Authorization'];
        localStorage.removeItem('token');
    }
};

export const loadToken = () => {
    const token = localStorage.getItem('token');
    if (token) setAuthToken(token);
};