
let apiBaseUrl = import.meta.env.VITE_API_BASE_URL || '/api';


if (apiBaseUrl && !apiBaseUrl.endsWith('/api')) {
    apiBaseUrl = apiBaseUrl.endsWith('/') ? `${apiBaseUrl}api` : `${apiBaseUrl}/api`;
}

export const API_BASE_URL = apiBaseUrl;


export const isDevelopment = import.meta.env.DEV;
export const isProduction = import.meta.env.PROD;


if (isDevelopment) {
    console.log('API Base URL:', API_BASE_URL);
}

