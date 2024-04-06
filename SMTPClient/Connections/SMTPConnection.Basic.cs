using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using EmailClient.Common;
using EmailClient.Exceptions;
using EmailClient.Extensions;
using EmailClient.Settings;

namespace EmailClient.Connections;

internal partial class SMTPConnection : IAsyncDisposable
{
    public bool IsConnected { get; set; }

    private readonly string host;
    private readonly string serverUrl;
    private readonly int serverPort;
    private readonly SMTPTlsModes tlsMode;

    private TcpClient socket = null!;
    private Stream stream = null!;

    public string[] AuthMethods { get; private set; } = null!;

    private static readonly string CRLF = "\r\n";

    public SMTPConnection(string host, string serverUrl, int serverPort, SMTPTlsModes tlsMode)
    {
        this.host = host;
        this.serverUrl = serverUrl;
        this.serverPort = serverPort;
        this.tlsMode = tlsMode;

        IsConnected = false;
    }

    private async Task Send(string command)
    {
        if (!stream.CanRead)
            throw new StreamCannotReadOrWrite("The stream cannot read at this momment");

        if (Config.isDevelopment)
            Console.WriteLine($"Sent: {command.Summarize()}");

        byte[] _command = Encoding.ASCII.GetBytes(command);
        await stream.WriteAsync(_command);
    }

    private async Task<string> Receive()
    {
        if (!stream.CanRead)
            throw new StreamCannotReadOrWrite("The stream cannot write at this momment");

        byte[] buffer = new byte[1024];
        int received = await stream.ReadAsync(buffer);
        string response = Encoding.ASCII.GetString(buffer, 0, received);

        if (Config.isDevelopment)
            Console.WriteLine($"Received: {response.Summarize()}");

        return response;
    }

    private SMTPResponseCode GetCode(string serverResponse)
    {
        string codeStr = serverResponse[0..3];

        if (!codeStr.IsNumeric()) return SMTPResponseCode.ExceptionOccurred;

        try { return (SMTPResponseCode)Convert.ToInt32(codeStr); }
        catch (InvalidCastException) { return SMTPResponseCode.ExceptionOccurred; }
    }

    private void GetAuthMethods(string serverResponse)
    {
        var cutted = serverResponse.Split("\r\n");

        foreach (var line in cutted)
        {
            if (line.Contains("AUTH="))
            {
                AuthMethods = line.Split(' ', '=')[1..];
                return;
            }
        }

        AuthMethods = new[] { "ANONYMOUS" }; // use anonymous by default
    }

    private static bool ValidateServerCertificate(
              object sender,
              X509Certificate certificate,
              X509Chain chain,
              SslPolicyErrors sslPolicyErrors)
    {
        return sslPolicyErrors == SslPolicyErrors.None || Config.trustSslCertificates;
    }

    public async ValueTask DisposeAsync()
    {
        await Send(SMTPCommandBuilder.QUIT());

        var response = await Receive();

        if (!GetCode(response).InValidCodes(SMTPResponseCode.ServiceClosingTransmissionChannel))
            throw new InvalidServerResponse((int)GetCode(response));

        await stream.DisposeAsync();
        socket.Dispose();
    }
}