import { createContext, useContext, useEffect, useMemo, useState } from 'react';
import api, { clearAuthStorage } from '../api/axios';

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  const [user, setUser] = useState(() => {
    const stored = localStorage.getItem('user');
    return stored ? JSON.parse(stored) : null;
  });
  const [loading, setLoading] = useState(true);

  const isAuthenticated = !!user && !!localStorage.getItem('accessToken');

  useEffect(() => {
    const token = localStorage.getItem('accessToken');
    if (token && user) {
      setLoading(false);
      return;
    }
    if (!token) {
      setUser(null);
    }
    setLoading(false);
  }, [user]);

  const login = async (usernameOrEmail, password) => {
    const { data } = await api.post('/auth/login', { usernameOrEmail, password });
    const payload = data.data;

    localStorage.setItem('accessToken', payload.accessToken);
    localStorage.setItem('refreshToken', payload.refreshToken);
    localStorage.setItem('user', JSON.stringify(payload.user));
    setUser(payload.user);

    return payload;
  };

  const logout = () => {
    clearAuthStorage();
    setUser(null);
  };

  const value = useMemo(
    () => ({ user, isAuthenticated, loading, login, logout }),
    [user, isAuthenticated, loading]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within AuthProvider');
  }
  return context;
}
