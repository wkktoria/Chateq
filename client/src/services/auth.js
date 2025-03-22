import axios from "axios";

const baseUrl = "https://localhost:7146/api/Auth";

const login = async (username, password) => {
  const request = axios.post(`${baseUrl}/Login`, { username, password });
  const response = await request;
  return response.data;
};

const register = async (username, password) => {
  const request = axios.put(`${baseUrl}/Register`, { username, password });
  const response = await request;
  return response.data;
};

export default { login, register };
