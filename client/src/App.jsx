import { useEffect, useState } from "react";
import { HubConnectionBuilder } from "@microsoft/signalr";
import {
  BrowserRouter as Router,
  Route,
  Routes,
  Navigate,
} from "react-router-dom";
import Register from "./pages/Register";
import Login from "./pages/Login";
import { Button, Col, Container, Row } from "react-bootstrap";
import Chat from "./pages/Chat";
import chatService from "./services/chat";

function App() {
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [connection, setConnection] = useState(null);
  const [selectedChat, setSelectedChat] = useState(null);
  const [messages, setMessages] = useState([]);

  useEffect(() => {
    const expiryDate = localStorage.getItem("expiryDate");
    if (expiryDate && new Date(expiryDate) > new Date()) {
      setIsLoggedIn(true);
    }
  }, []);

  useEffect(() => {
    if (isLoggedIn && !connection) {
      const token = localStorage.getItem("token");
      const connect = new HubConnectionBuilder()
        .withUrl(`https://localhost:7146/messageHub?token=${token}`, {
          accessTokenFactory: () => token,
        })
        .withAutomaticReconnect()
        .build();

      setConnection(connect);

      return () => {
        if (connection) {
          connection.stop();
        }
      };
    }
  }, [isLoggedIn, connection]);

  useEffect(() => {
    if (isLoggedIn) {
      const token = localStorage.getItem("token");
      chatService.getPaginatedChat("Global", 1, 20, token).then((data) => {
        const formattedMessages = data.messages.map((msg) => ({
          id: msg.id,
          sender: msg.sender,
          message: msg.messageText,
          createdAt: msg.createdAt,
        }));
        setSelectedChat({ id: data.id, name: data.name });
        setMessages(formattedMessages);
      });
    }
  }, [isLoggedIn]);

  useEffect(() => {
    if (connection) {
      connection
        .start()
        .then(async () => {
          console.log("Connected with SignalR.");
          connection.on("ReceiveMessage", (sender, message) => {
            setMessages((messages) => [...messages, { sender, message }]);
          });
        })
        .catch((error) => console.error("SignalR error:", error));
    }
  }, [connection]);

  const handleLogin = (authData) => {
    localStorage.setItem("token", authData.token);
    localStorage.setItem("expiryDate", authData.expiryDate);
    setIsLoggedIn(true);
  };

  const handleLogout = () => {
    localStorage.removeItem("token");
    localStorage.removeItem("expiryDate");
    setIsLoggedIn(false);
  };

  const onSendMessage = async (chatId, message) => {
    if (connection) {
      try {
        await connection.invoke("SendMessageToChatAsync", chatId, message);
      } catch (error) {
        console.error("Could not send message:", error);
      }
    }
  };

  const onLoadOlderMessages = async (chatName, pageNumber) => {
    const token = localStorage.getItem("token");
    chatService
      .getPaginatedChat(chatName, pageNumber, 20, token)
      .then((data) => {
        const newMessages = data.messages.map((msg) => ({
          id: msg.id,
          sender: msg.sender,
          message: msg.messageText,
          createdAt: msg.createdAt,
        }));

        if (newMessages.length > 0) {
          setMessages((prevMessages) => {
            const prevMessagesIds = new Set(prevMessages.map((msg) => msg.id));
            const filteredNewMessages = newMessages.filter(
              (msg) => !prevMessagesIds.has(msg.id)
            );
            return [...prevMessages, ...filteredNewMessages];
          });
        }

        return newMessages.length === 20;
      })
      .catch(() => {
        return false;
      });
  };

  return (
    <Router>
      {isLoggedIn && (
        <div className="position-absolute top-0 end-0">
          <Button variant="warning" className="m-2" onClick={handleLogout}>
            Logout
          </Button>
        </div>
      )}
      <Routes>
        <Route
          path="/login"
          element={
            isLoggedIn ? (
              <Navigate to="/chat" />
            ) : (
              <Login onLogin={handleLogin} />
            )
          }
        />
        <Route
          path="/register"
          element={isLoggedIn ? <Navigate to="/chat" /> : <Register />}
        />
        <Route
          path="/chat"
          element={
            isLoggedIn ? (
              <Container>
                <Row>
                  <Col>
                    <Chat
                      messages={messages}
                      onSendMessage={onSendMessage}
                      currentChat={selectedChat}
                      onLoadOlderMessages={onLoadOlderMessages}
                    />
                  </Col>
                </Row>
              </Container>
            ) : (
              <Navigate to="/login" />
            )
          }
        />
        <Route path="*" element={<Navigate to="/login" />} />
      </Routes>
    </Router>
  );
}

export default App;
