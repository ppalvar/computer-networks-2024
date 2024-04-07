using EmailClient.Common;
using EmailClient.Connections;
using EmailClient.Exceptions;

namespace EmailClient;

public class SMTPClient
{
    public SMTPCredentials Credentials { get; set; }

    private SMTPConnection? connection = null;
    private readonly string server;
    private readonly int port;
    private readonly string host;
    private readonly bool useTls;

    public SMTPClient(string server, int port, string? hostname = null, bool useTls = true)
    {
        this.server = server;
        this.host = hostname ?? Environment.MachineName;
        this.port = port;
        this.useTls = useTls;

        Credentials = new("no_username", "no_password123*", SMTPAuthMethods.ANONYMOUS);
    }

    public async Task SendEmail(EmailMessage email)
    {
        await Connect();

        await connection!.SendEmail(email);

        await connection!.DisposeAsync();
    }

    private async Task Connect()
    {
        if (useTls)
        {
            try { connection = new(host, server, port, SMTPTlsModes.Implicit); await connection.Connect(); }
            catch
            {
                try { connection = new(host, server, port, SMTPTlsModes.StartTls); await connection.Connect(); }
                catch
                {
                    throw;
                }
            }
        }

        else
        {
            try { connection = new(host, server, port, SMTPTlsModes.StartTls); await connection.Connect(); }
            catch { throw new CannotConnectToServer("The server is not accessible. Try contact client support"); }
        }

        var authMethod = SMTPAuthMethods.FromName(connection.AuthMethods.Last());

        await connection.Authenticate(Credentials);
    }
}