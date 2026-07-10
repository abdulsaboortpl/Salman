import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../api/axios';
import { useAuth } from '../context/AuthContext';
import LoadingSpinner from '../components/LoadingSpinner';

/**
 * Protected dashboard showing user profile and registered users list.
 */
export default function Dashboard() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const [profile, setProfile] = useState(null);
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    const fetchData = async () => {
      try {
        const [profileRes, usersRes] = await Promise.all([
          api.get('/auth/profile'),
          api.get('/users'),
        ]);
        setProfile(profileRes.data.data);
        setUsers(usersRes.data.data || []);
      } catch (err) {
        setError(err.response?.data?.message || 'Failed to load dashboard data.');
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, []);

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  if (loading) {
    return <LoadingSpinner fullPage message="Loading dashboard..." />;
  }

  const displayUser = profile || user;

  return (
    <div className="page-container align-items-start">
      <div className="container py-3">
        <div className="d-flex justify-content-between align-items-center mb-4">
          <h2 className="text-white mb-0">Dashboard</h2>
          <button className="btn btn-outline-light" onClick={handleLogout}>
            Logout
          </button>
        </div>

        {error && (
          <div className="alert alert-danger" role="alert">
            {error}
          </div>
        )}

        <div className="row g-4">
          <div className="col-lg-4">
            <div className="card dashboard-card h-100">
              <div className="card-body">
                <h5 className="card-title">Your Profile</h5>
                {displayUser ? (
                  <ul className="list-group list-group-flush">
                    <li className="list-group-item px-0">
                      <strong>Name:</strong> {displayUser.fullName}
                    </li>
                    <li className="list-group-item px-0">
                      <strong>Email:</strong> {displayUser.email}
                    </li>
                    <li className="list-group-item px-0">
                      <strong>Username:</strong> {displayUser.username}
                    </li>
                    <li className="list-group-item px-0">
                      <strong>Role:</strong>{' '}
                      <span className="badge bg-primary">{displayUser.role}</span>
                    </li>
                  </ul>
                ) : (
                  <p className="text-muted mb-0">No profile data available.</p>
                )}
              </div>
            </div>
          </div>

          <div className="col-lg-8">
            <div className="card dashboard-card h-100">
              <div className="card-body">
                <h5 className="card-title">Registered Users</h5>
                <div className="table-responsive">
                  <table className="table table-hover align-middle">
                    <thead>
                      <tr>
                        <th>Name</th>
                        <th>Email</th>
                        <th>Username</th>
                        <th>Role</th>
                        <th>Status</th>
                      </tr>
                    </thead>
                    <tbody>
                      {users.length === 0 ? (
                        <tr>
                          <td colSpan="5" className="text-muted text-center">
                            No users found.
                          </td>
                        </tr>
                      ) : (
                        users.map((u) => (
                          <tr key={u.id}>
                            <td>{u.fullName}</td>
                            <td>{u.email}</td>
                            <td>{u.username}</td>
                            <td>
                              <span
                                className={`badge ${
                                  u.role === 'Admin' ? 'bg-danger' : 'bg-secondary'
                                }`}
                              >
                                {u.role}
                              </span>
                            </td>
                            <td>
                              <span
                                className={`badge ${
                                  u.isActive ? 'bg-success' : 'bg-warning'
                                }`}
                              >
                                {u.isActive ? 'Active' : 'Inactive'}
                              </span>
                            </td>
                          </tr>
                        ))
                      )}
                    </tbody>
                  </table>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
