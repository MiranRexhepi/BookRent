import React, { useEffect, useState } from "react";
import { getDashboardStats, getDailyRentalStats } from "../Services/dashboardService";
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from "recharts";
import "bootstrap/dist/css/bootstrap.min.css";

export default function Dashboard() {
  const [stats, setStats] = useState(null);
  const [dailyStats, setDailyStats] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const loadDashboardData = async () => {
      try {
        setLoading(true);
        const [statsData, dailyStatsData] = await Promise.all([
          getDashboardStats(),
          getDailyRentalStats()
        ]);
        setStats(statsData);
        setDailyStats(dailyStatsData);
        setError(null);
      } catch (err) {
        console.error("Failed to load dashboard data:", err);
        setError("Failed to load dashboard data. Please try again.");
      } finally {
        setLoading(false);
      }
    };

    loadDashboardData();
  }, []);

  if (loading) {
    return (
      <div className="container mt-4">
        <div className="text-center">
          <div className="spinner-border" role="status">
            <span className="visually-hidden">Loading...</span>
          </div>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="container mt-4">
        <div className="alert alert-danger" role="alert">
          {error}
        </div>
      </div>
    );
  }

  const formatDate = (dateString) => {
    const date = new Date(dateString);
    return date.toLocaleDateString("en-US", { month: "short", day: "numeric" });
  };

  const chartData = dailyStats.map((item) => ({
    date: formatDate(item.date || item.Date),
    Rents: item.rentsCount || item.RentsCount || 0,
    Returns: item.returnsCount || item.ReturnsCount || 0,
  }));

  return (
    <div className="container-fluid mt-4">
      <h1 className="mb-4 fw-bold">Dashboard</h1>

      {/* Statistics Cards */}
      <div className="row g-3 mb-4">
        <div className="col-md-4 col-sm-6">
          <div className="card h-100">
            <div className="card-body">
              <h5 className="card-title text-primary fw-bold">Active Users This Month</h5>
              <h2 className="card-text fw-bold">{stats?.activeUsersThisMonth || 0}</h2>
              <p className="text-muted mb-0 fw-semibold">Users who rented this month</p>
            </div>
          </div>
        </div>

        <div className="col-md-4 col-sm-6">
          <div className="card h-100">
            <div className="card-body">
              <h5 className="card-title text-info fw-bold">Rented Books at the Moment</h5>
              <h2 className="card-text fw-bold">{stats?.rentedBooksAtMoment || 0}</h2>
              <p className="text-muted mb-0 fw-semibold">Currently rented or overdue</p>
            </div>
          </div>
        </div>

        <div className="col-md-4 col-sm-6">
          <div className="card h-100">
            <div className="card-body">
              <h5 className="card-title text-danger fw-bold">Overdue Books</h5>
              <h2 className="card-text fw-bold">{stats?.overdueBooks || 0}</h2>
              <p className="text-muted mb-0 fw-semibold">Books past return date</p>
            </div>
          </div>
        </div>

        <div className="col-md-4 col-sm-6">
          <div className="card h-100">
            <div className="card-body">
              <h5 className="card-title text-warning fw-bold">Passive Users</h5>
              <h2 className="card-text fw-bold">{stats?.passiveUsers || 0}</h2>
              <p className="text-muted mb-0 fw-semibold">No rentals in last 30 days</p>
            </div>
          </div>
        </div>

        <div className="col-md-4 col-sm-6">
          <div className="card h-100">
            <div className="card-body">
              <h5 className="card-title text-success fw-bold">Total Books Added This Month</h5>
              <h2 className="card-text fw-bold">{stats?.totalBooksAddedThisMonth || 0}</h2>
              <p className="text-muted mb-0 fw-semibold">All non-deleted books</p>
            </div>
          </div>
        </div>

        <div className="col-md-4 col-sm-6">
          <div className="card h-100">
            <div className="card-body">
              <h5 className="card-title text-success fw-bold">Total Books Available</h5>
              <h2 className="card-text fw-bold">{stats?.totalBooksAvailable || 0}</h2>
              <p className="text-muted mb-0 fw-semibold">Available for rent</p>
            </div>
          </div>
        </div>
      </div>

      {/* Chart */}
      <div className="row mb-4">
        <div className="col-12">
          <div className="card">
            <div className="card-header">
              <h5 className="mb-0 fw-bold">Daily Rents and Returns (Last 30 Days)</h5>
            </div>
            <div className="card-body">
              <ResponsiveContainer width="100%" height={400}>
                <BarChart data={chartData}>
                  <CartesianGrid strokeDasharray="3 3" />
                  <XAxis 
                    dataKey="date" 
                    angle={-45}
                    textAnchor="end"
                    height={100}
                    style={{ fontWeight: 'bold' }}
                  />
                  <YAxis style={{ fontWeight: 'bold' }} />
                  <Tooltip contentStyle={{ fontWeight: 'bold' }} />
                  <Legend wrapperStyle={{ fontWeight: 'bold' }} />
                  <Bar dataKey="Rents" fill="#0d6efd" />
                  <Bar dataKey="Returns" fill="#198754" />
                </BarChart>
              </ResponsiveContainer>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

