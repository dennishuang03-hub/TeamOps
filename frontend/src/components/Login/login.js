import React, { useState } from "react";
import "./style.css";
import { validateUsername, validatePassword } from "./loginUtils";

export function Login({ onLoginSuccess }) { 
  const [username, setUsername] = useState(""); 
  const [password, setPassword] = useState("");
  const [success, setSuccess] = useState(false);
  const [errors, setErrors] = useState({ username: "", password: "" }); 

  const handleSubmit = async (e) => {
    e.preventDefault();

    const usernameValidation = validateUsername(username);
    const passwordValidation = validatePassword(password);

    if (!usernameValidation.isValid || !passwordValidation.isValid) {
        setErrors({
        username: usernameValidation.message,
        password: passwordValidation.message,
        });
        return;
    }

    try {
        const response = await fetch("http://localhost:5226/api/login/auth", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ username, password }),
        });

        const data = await response.json();

        if (response.ok && data.message === "Login successful") {
        setSuccess(true);
        localStorage.setItem("username", data.username);

        // Notify App.js that login succeeded
        if (typeof onLoginSuccess === "function") {
            onLoginSuccess();
        }
        } else {
        setErrors({ username: "", password: data.message });
        }
    } catch (error) {
        console.error("Error:", error);
        setErrors({ username: "", password: "Server error. Please try again later." });
    }
};


  if (success) {
    return (
      <div className="login-container">
        <div className="success-message show">
          <div className="success-icon">✓</div>
          <h3>Login Successful!</h3>
          <p>Redirecting to your dashboard...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="login-container">
      <div className="login-card">
        <div className="login-header">
          <h2>Sign In</h2>
          <p>Enter your credentials to access your account</p>
        </div>

        <form className="login-form" onSubmit={handleSubmit}>
          <div className="form-group">
            <div className="input-wrapper">
              <input
                type="text"
                id="username"
                value={username}
                onChange={(e) => setUsername(e.target.value)}
                required
              />
              <label htmlFor="username">Username</label>
            </div>
            {errors.username && (
              <span className="error-message">{errors.username}</span>
            )}
          </div>

          <div className="form-group">
            <div className="input-wrapper password-wrapper">
              <input
                type="password"
                id="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                required
              />
              <label htmlFor="password">Password</label>
            </div>
            {errors.password && (
              <span className="error-message">{errors.password}</span>
            )}
          </div>

          <button type="submit" className="login-btn">
            Sign In
          </button>
        </form>

        <div className="signup-link">
          <p>
            Don't have an account? <a href="#">Create one</a>
          </p>
        </div>
      </div>
    </div>
  );
}

export default Login;
