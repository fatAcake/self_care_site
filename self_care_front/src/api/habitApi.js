import axios from 'axios';
import { loadToken } from './authApi';

const API_BASE_URL = '/api';


const ensureToken = () => {
    loadToken();
};


axios.interceptors.request.use(
    (config) => {
        ensureToken();
        return config;
    },
    (error) => {
        return Promise.reject(error);
    }
);

axios.interceptors.response.use(
    (response) => response,
    (error) => {
        if (error.response?.status === 401) {
            localStorage.removeItem('token');
            localStorage.removeItem('user');
            window.location.href = '/auth';
        }
        return Promise.reject(error);
    }
);


export const getAllHabits = async () => {
    const res = await axios.get(`${API_BASE_URL}/habit/all`);
    return res.data;
};


export const getUserHabits = async (date = null) => {
    const params = date ? { date: date.toISOString() } : {};
    const res = await axios.get(`${API_BASE_URL}/habit/user-habits`, { params });
    return res.data;
};


export const getProgress = async (date = null) => {
    const params = date ? { date: date.toISOString() } : {};
    const res = await axios.get(`${API_BASE_URL}/habit/progress`, { params });
    return res.data;
};


export const addHabitToUser = async (habitId, executeDate) => {
    const res = await axios.post(`${API_BASE_URL}/habit/add-to-user`, {
        habitId,
        executeDate: executeDate.toISOString()
    });
    return res.data;
};


export const createHabit = async (name, description = null) => {
    const res = await axios.post(`${API_BASE_URL}/habit/create`, {
        name,
        description
    });
    return res.data;
};


export const toggleHabit = async (userHabitId) => {
    const res = await axios.patch(`${API_BASE_URL}/habit/toggle/${userHabitId}`);
    return res.data;
};


export const deleteUserHabit = async (userHabitId) => {
    await axios.delete(`${API_BASE_URL}/habit/user-habits/${userHabitId}`);
};

