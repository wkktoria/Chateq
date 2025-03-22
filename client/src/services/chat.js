import axios from "axios";

const baseUrl = "https://localhost:7146/api/Chat";

const getPaginatedChat = async (chatName, pageNumber, pageSize, token) => {
  const request = axios.post(
    `${baseUrl}/GetPaginatedChat?chatName=${chatName}&pageNumber=${pageNumber}&pageSize=${pageSize}`,
    {},
    {
      headers: {
        Authorization: `Bearer ${token}`,
      },
    }
  );
  const response = await request;
  return response.data;
};

export default { getPaginatedChat };
