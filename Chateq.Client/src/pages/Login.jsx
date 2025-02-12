import { useState } from "react";
import { Alert, Button, Container, Form } from "react-bootstrap";
import { Link, useNavigate } from "react-router-dom";

// eslint-disable-next-line react/prop-types
function Login({ onLogin }) {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [errorMessage, setErrorMessage] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const navigate = useNavigate();

  const handleLogin = async (event) => {
    event.preventDefault();
    setIsLoading(true);
    setErrorMessage("");

    try {
      const response = await fetch("http://localhost:5201/api/Auth/Login", {
        method: "POST",
        headers: {
          Accept: "*/*",
          "Content-Type": "application/json",
        },
        body: JSON.stringify({ username, password }),
      });

      if (response.ok) {
        const authData = await response.json();
        onLogin(authData);
        navigate("/chat");
      } else {
        const error = await response.json();
        setErrorMessage(
          error.message || "The login attempt was not successful."
        );
      }
    } catch (error) {
      setErrorMessage(`There was a problem during the login: ${error.message}`);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <Container className="d-flex align-items-center justify-content-center vh-100">
      <div className="w-100" style={{ maxWidth: "400px" }}>
        <h2 className="text-center mb-4">Login</h2>
        {errorMessage && <Alert variant="danger">{errorMessage}</Alert>}

        <Form onSubmit={handleLogin}>
          <Form.Group className="mb-3" controlId="formUsername">
            <Form.Label>Username</Form.Label>
            <Form.Control
              type="text"
              placeholder="Enter username"
              value={username}
              onChange={(event) => setUsername(event.target.value)}
              required
            />
          </Form.Group>

          <Form.Group className="mb-3" controlId="formPassword">
            <Form.Label>Password</Form.Label>
            <Form.Control
              type="password"
              placeholder="Enter password"
              value={password}
              onChange={(event) => setPassword(event.target.value)}
              required
            />
          </Form.Group>

          <Button
            variant="primary"
            type="submit"
            className="w-100"
            disabled={isLoading}
          >
            {isLoading ? "Loading..." : "Log In"}
          </Button>
        </Form>

        <div className="mt-3 text-center">
          <p>
            Do not have an account? <Link to="/register">Register here</Link>.
          </p>
        </div>
      </div>
    </Container>
  );
}

export default Login;
