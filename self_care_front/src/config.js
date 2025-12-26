// Конфигурация API
let apiBaseUrl = import.meta.env.VITE_API_BASE_URL || '/api';

// Убеждаемся, что URL заканчивается на /api
if (apiBaseUrl && !apiBaseUrl.endsWith('/api')) {
    // Если URL не заканчивается на /api, добавляем его
    apiBaseUrl = apiBaseUrl.endsWith('/') ? `${apiBaseUrl}api` : `${apiBaseUrl}/api`;
}

export const API_BASE_URL = apiBaseUrl;

// Проверка окружения
export const isDevelopment = import.meta.env.DEV;
export const isProduction = import.meta.env.PROD;

