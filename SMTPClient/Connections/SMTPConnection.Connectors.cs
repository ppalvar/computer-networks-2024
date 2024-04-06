using System.Net.Security;
using System.Net.Sockets;
using EmailClient.Common;
using EmailClient.Exceptions;
using EmailClient.Extensions;

namespace EmailClient.Connections;

internal partial class SMTPConnection
{
    public async Task Connect()
    {
        string response;

        if (this.tlsMode == SMTPTlsModes.StartTls)
        {
            try { socket = new TcpClient(serverUrl, serverPort); }
            catch { throw new HandshakeFailed(); }

            stream = socket.GetStream();

            response = await Receive();
            if (!GetCode(response).InValidCodes(SMTPResponseCode.ServiceReady))
                throw new InvalidServerResponse((int)GetCode(response));

            await Send(SMTPCommandBuilder.STARTTLS());

            response = await Receive();

            if (!GetCode(response).InValidCodes(SMTPResponseCode.ServiceReady))
                throw new InvalidServerResponse((int)GetCode(response));

            var _stream = new SslStream(socket.GetStream(), false, ValidateServerCertificate!);

            await _stream.AuthenticateAsClientAsync(host);

            stream = _stream;
        }

        else if (this.tlsMode == SMTPTlsModes.Implicit)
        {
            try { socket = new TcpClient(serverUrl, serverPort); }
            catch { throw new HandshakeFailed(); }

            var _stream = new SslStream(socket.GetStream(), false, ValidateServerCertificate!);

            await _stream.AuthenticateAsClientAsync(host);

            stream = _stream;

            response = await Receive();
            if (!GetCode(response).InValidCodes(SMTPResponseCode.ServiceReady))
                throw new InvalidServerResponse((int)GetCode(response));
        }
        else if (this.tlsMode == SMTPTlsModes.None)
        {
            try { socket = new TcpClient(serverUrl, serverPort); }
            catch { throw new HandshakeFailed(); }

            stream = socket.GetStream();

            response = await Receive();
            if (!GetCode(response).InValidCodes(SMTPResponseCode.ServiceReady))
                throw new InvalidServerResponse((int)GetCode(response));
        }

        await Send(SMTPCommandBuilder.EHLO(host));
        response = await Receive();

        if (!GetCode(response).InValidCodes(SMTPResponseCode.OK))
            throw new InvalidServerResponse((int)GetCode(response));

        GetAuthMethods(response);

        IsConnected = true;
    }

    public async Task TestConnection()
    {
        if (!IsConnected) throw new ConnectionNotStablished("No connection yet.");

        await Send(SMTPCommandBuilder.NOOP());
        var response = await Receive();

        if (!GetCode(response).InValidCodes(SMTPResponseCode.OK))
        {
            IsConnected = false;
            throw new ConnectionNotStablished("Connection died at some point, try reestart it.");
        }
    }
}