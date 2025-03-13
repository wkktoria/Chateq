import { useEffect, useRef, useState } from "react";
import {
  Button,
  Container,
  Form,
  InputGroup,
  ListGroup,
} from "react-bootstrap";

// eslint-disable-next-line react/prop-types
function Chat({ messages, onSendMessage, currentChat, onLoadOlderMessages }) {
  const [message, setMessage] = useState("");
  const [chat, setChat] = useState(
    currentChat || { id: "Global", name: "Global" }
  );
  const [page, setPage] = useState(1);
  const [hasInitialLoad, setHasInitialLoad] = useState(false);
  const messagesEndRef = useRef(null);
  const chatMessagesRef = useRef(null);

  useEffect(() => {
    setChat(currentChat);
  }, [currentChat]);

  useEffect(() => {
    // eslint-disable-next-line react/prop-types
    if (!hasInitialLoad && messages.length > 0) {
      scrollToBottom();
      setHasInitialLoad(true);
    }
  }, [messages, hasInitialLoad]);

  useEffect(() => {
    if (hasInitialLoad && !isScrolledToBottom()) {
      scrollToBottom();
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [messages]);

  useEffect(() => {
    const chatContainer = chatMessagesRef.current;

    if (chatContainer) {
      chatContainer.addEventListener("scroll", handleScroll);
    }

    return () => {
      if (chatContainer) {
        chatContainer.addEventListener("scroll", handleScroll);
      }
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [chatMessagesRef, page]);

  const scrollToBottom = () => {
    if (messagesEndRef.current) {
      messagesEndRef.current.scrollIntoView({ behavior: "smooth" });
    }
  };

  const isScrolledToBottom = () => {
    const chatContainer = chatMessagesRef.current;
    return (
      chatContainer &&
      chatContainer.scrollHeight - chatContainer.scrollTop ===
        chatContainer.clientHeight + 50
    );
  };

  const handleScroll = () => {
    if (chatMessagesRef.current.scrollTop === 0) {
      fetchOlderMessages();
    }
  };

  const fetchOlderMessages = async () => {
    const newPage = page + 1;
    const success = await onLoadOlderMessages(chat.id, newPage);

    if (success) {
      setPage(newPage);
    }
  };

  const handleSendMessage = () => {
    scrollToBottom();

    if (!chat || !chat.id) {
      console.error("No chat selected or chat's id is missing");
      return;
    }

    if (message.trim()) {
      onSendMessage(chat.id, message);
      setMessage("");
    }
  };

  const handleKeyPress = (event) => {
    if (event.key === "Enter") {
      handleSendMessage();
    }
  };

  return (
    <Container className="d-flex flex-column align-items-center vh-100">
      <div
        className="w-100 p-3 border bg-light mt-4"
        style={{ maxWidth: "600px" }}
      >
        <div className="text-center">
          <h4>{chat?.name} Chat</h4>
        </div>

        <div
          ref={chatMessagesRef}
          style={{ overflowY: "auto", maxHeight: "70vh" }}
        >
          <ListGroup variant="flush">
            {messages
              // eslint-disable-next-line react/prop-types
              .sort((a, b) => new Date(a.createdAt) - new Date(b.createdAt))
              .map((msg, index) => (
                <ListGroup.Item key={index} className="m-1 rounded">
                  <small className="d-block">{msg.sender}</small>
                  <strong>{msg.message}</strong>
                </ListGroup.Item>
              ))}
            <div ref={messagesEndRef}></div>
          </ListGroup>
        </div>

        <div className="mt-3">
          <InputGroup>
            <Form.Control
              type="text"
              value={message}
              onChange={(event) => setMessage(event.target.value)}
              onKeyPress={handleKeyPress}
              placeholder="Enter your message..."
              className="shadow-none"
            />
            <Button variant="success" onClick={handleSendMessage}>
              Send
            </Button>
          </InputGroup>
        </div>
      </div>
    </Container>
  );
}

export default Chat;
