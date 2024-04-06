namespace EmailClient.Common;

internal static class SMTPCommandBuilder
{
    private static readonly string CRLF = "\r\n";

    /// <summary>
    /// Initiates the SMTP session conversation
    /// </summary>
    public static string EHLO(string url = "localhost") => $"EHLO {url}{CRLF}";
    /// <summary>
    /// Initiates a mail transfer
    /// </summary>
    public static string MAIL_FROM(string emailAddr) => $"MAIL FROM:<{emailAddr}>{CRLF}";
    /// <summary>
    /// Specifies the recipient
    /// </summary>
    public static string RCPT_TO(string emailAddr) => $"RCPT TO: <{emailAddr}>{CRLF}";
    /// <summary>
    /// The client asks the server for permission to transfer the mail data
    /// </summary>
    public static string DATA() => $"DATA{CRLF}";
    /// <summary>
    /// Used only to check whether the server can respond
    /// </summary>
    public static string NOOP() => $"NOOP{CRLF}";
    /// <summary>
    /// The client requests a list of commands the server supports
    /// </summary>
    public static string HELP() => $"HELP{CRLF}";
    /// <summary>
    /// Verify whether a mailbox in the argument exists on the local host
    /// </summary>
    public static string VRFY(string username) => $"VRFY {username}{CRLF}";
    /// <summary>
    /// Verify whether a mailing list in the argument exists on the local host
    /// </summary>
    public static string EXPN(params string[] mailList) => $"EXPN {mailList}{CRLF}";
    /// <summary>
    /// Resets the SMTP connection to the initial state
    /// </summary>
    public static string RSET() => $"RSET{CRLF}";
    /// <summary>
    /// Send the request to terminate the SMTP session
    /// </summary>
    public static string QUIT() => $"QUIT{CRLF}";
    /// <summary>
    /// Start a TLS handshake for a secure SMTP session
    /// </summary>
    public static string STARTTLS() => $"STARTTLS{CRLF}";
    /// <summary>
    /// Authenticate the client to the server
    /// </summary>
    public static string AUTH(SMTPAuthMethods loginOption)
    {
        return $"AUTH {loginOption.Name}{CRLF}";
    }
    /// <summary>
    /// Reverse the connection between the local and external SMTP servers
    /// </summary>
    public static string ATRN(string srverA, string srverB) => $"ATRN {srverA}, {srverB}{CRLF}";
    /// <summary>
    /// Submit mail contents. It can be an alternative to the DATA command
    /// </summary>
    public static string BDAT(uint length, string lastChunk = "LAST") => $"BDAT {length} {lastChunk}{CRLF}";
    /// <summary>
    /// Request to start SMTP queue processing of a specified server host
    /// </summary>
    public static string ETRN(string host) => $"ETRN {host}{CRLF}";
}