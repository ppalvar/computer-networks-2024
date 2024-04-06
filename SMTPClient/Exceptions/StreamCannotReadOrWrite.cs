namespace EmailClient.Exceptions;

public class StreamCannotReadOrWrite(string message = "") : Exception(message);
