import axios from 'axios';
import { loadToken } from './authApi';
import { API_BASE_URL } from '../config';


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

export const createSession = async (payload) => {
    const res = await axios.post(`${API_BASE_URL}/sessions`, payload);
    return res.data;
};

