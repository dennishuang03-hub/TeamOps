// src/components/Login/loginUtils.js

export function validateUsername(value) { 
  if (!value) return { isValid: false, message: "Username is required" };
  if (value.length < 3)
    return { isValid: false, message: "Username must be at least 3 characters" };
  return { isValid: true, message: "" };
}

export function validatePassword(value) {
  if (!value) return { isValid: false, message: "Password is required" };
  if (value.length < 0)
    return { isValid: false, message: "Password must be at least 1 characters" };
  return { isValid: true, message: "" };
}
