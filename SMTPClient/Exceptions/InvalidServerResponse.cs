namespace EmailClient.Exceptions;

public class InvalidServerResponse(int errorCode, string message = "") : Exception($"The server responded with invalid code <{errorCode}>.\r\nExtra Info: {message}");
