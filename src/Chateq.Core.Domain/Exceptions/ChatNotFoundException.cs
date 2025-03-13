namespace Chateq.Core.Domain.Exceptions;

public class ChatNotFoundException(string chatName) : Exception($"Could not find chat with name: {chatName}");