import { useState } from 'react';
import { Navigate, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import LoadingSpinner from '../components/LoadingSpinner';

/**
 * Login page with form validation and API integration.
 */
export default function Login() {
  const { login, isAuthenticated, loading: authLoading } = useAuth();
  const navigate = useNavigate();

  const [form, setForm] = useState({ usernameOrEmail: '', password: '' });
  const [errors, setErrors] = useState({});
  const [apiError, setApiError] = useState('');
  const [loading, setLoading] = useState(false);

  if (authLoading) {
    return <LoadingSpinner fullPage message="Loading..." />;
  }

  if (isAuthenticated) {
    return <Navigate to="/dashboard" replace />;
  }

  const validate = () => {
    const nextErrors = {};
    if (!form.usernameOrEmail.trim()) {
      nextErrors.usernameOrEmail = 'Username or email is required.';
    }
    if (!form.password) {
      nextErrors.password = 'Password is required.';
    } else if (form.password.length < 6) {
      nextErrors.password = 'Password must be at least 6 characters.';
    }
    setErrors(nextErrors);
    return Object.keys(nextErrors).length === 0;
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setForm((prev) => ({ ...prev, [name]: value }));
    setErrors((prev) => ({ ...prev, [name]: '' }));
    setApiError('');
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!validate()) return;

    setLoading(true);
    setApiError('');

    try {
      await login(form.usernameOrEmail.trim(), form.password);
      navigate('/dashboard');
    } catch (err) {
      const message =
        err.response?.data?.message ||
        err.response?.data?.title ||
        'Login failed. Please check your credentials.';
      setApiError(message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="page-container">
      <div className="container">
        <div className="card auth-card p-4">
          <div className="card-body">
            <h2 className="text-center mb-1">Welcome Back</h2>
            <p className="text-center text-muted mb-4">Sign in to your account</p>

            {apiError && (
              <div className="alert alert-danger py-2" role="alert">
                {apiError}
              </div>
            )}

            <form onSubmit={handleSubmit} noValidate>
              <div className="mb-3">
                <label htmlFor="usernameOrEmail" className="form-label">
                  Username or Email
                </label>
                <input
                  type="text"
                  className={`form-control ${errors.usernameOrEmail ? 'is-invalid' : ''}`}
                  id="usernameOrEmail"
                  name="usernameOrEmail"
                  value={form.usernameOrEmail}
                  onChange={handleChange}
                  placeholder="admin or admin@example.com"
                  autoComplete="username"
                />
                {errors.usernameOrEmail && (
                  <div className="invalid-feedback">{errors.usernameOrEmail}</div>
                )}
              </div>

              <div className="mb-4">
                <label htmlFor="password" className="form-label">
                  Password
                </label>
                <input
                  type="password"
                  className={`form-control ${errors.password ? 'is-invalid' : ''}`}
                  id="password"
                  name="password"
                  value={form.password}
                  onChange={handleChange}
                  placeholder="Enter your password"
                  autoComplete="current-password"
                />
                {errors.password && (
                  <div className="invalid-feedback">{errors.password}</div>
                )}
              </div>

              <button
                type="submit"
                className="btn btn-primary w-100"
                disabled={loading}
              >
                {loading ? (
                  <>
                    <span
                      className="spinner-border spinner-border-sm me-2"
                      role="status"
                      aria-hidden="true"
                    />
                    Signing in...
                  </>
                ) : (
                  'Login'
                )}
              </button>
            </form>

            <p className="text-center text-muted mt-4 mb-0 small">
              Sample user: <strong>admin</strong> / <strong>Admin@123</strong>
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}
