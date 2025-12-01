import React from "react";
import { createBrowserRouter, RouterProvider } from "react-router-dom";
import Login from "./components/Login/login";

const router = createBrowserRouter([
  { 
    path: "/", 
    element: <Login />, 
    errorElement: <div>404 Page Not Found</div>
  },

  { 
    path: "/login", 
    element: <Login /> 
  },
]);

function App() {
  return <RouterProvider router={router} />;
}

export default App;
