import axios from 'axios';

// Base URL for the ASP.NET Core API (configure via .env if needed)
const API_BASE_URL = import.meta.env.VITE_API_URL || 'https://localhost:7001/api';

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Attach JWT from localStorage to every request
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('accessToken');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Handle 401 responses: try refresh token, else redirect to login
api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;

      const refreshToken = localStorage.getItem('refreshToken');
      if (refreshToken) {
        try {
          const { data } = await axios.post(`${API_BASE_URL}/auth/refresh`, {
            refreshToken,
          });

          const payload = data.data;
          localStorage.setItem('accessToken', payload.accessToken);
          localStorage.setItem('refreshToken', payload.refreshToken);
          localStorage.setItem('user', JSON.stringify(payload.user));

          originalRequest.headers.Authorization = `Bearer ${payload.accessToken}`;
          return api(originalRequest);
        } catch {
          clearAuthStorage();
          window.location.href = '/login';
        }
      } else {
        clearAuthStorage();
        if (!window.location.pathname.includes('/login')) {
          window.location.href = '/login';
        }
      }
    }

    return Promise.reject(error);
  }
);

export function clearAuthStorage() {
  localStorage.removeItem('accessToken');
  localStorage.removeItem('refreshToken');
  localStorage.removeItem('user');
}

export default api;
