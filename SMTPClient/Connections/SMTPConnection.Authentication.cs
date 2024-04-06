using EmailClient.Common;
using EmailClient.Exceptions;
using EmailClient.Extensions;

namespace EmailClient.Connections;

internal partial class SMTPConnection
{
    public async Task Authenticate(SMTPCredentials credentials) =>
        await Authenticate(credentials.UserName, credentials.Password, credentials.AuthMethod);
    public async Task Authenticate(string username, string password, SMTPAuthMethods authMethod)
    {
        await TestConnection(); // if dies here, there is no connection

        if (authMethod == SMTPAuthMethods.ANONYMOUS) return;

        var command = SMTPCommandBuilder.AUTH(authMethod);
        await Send(command);

        var response = await Receive();
        if (!GetCode(response).InValidCodes(SMTPResponseCode.RequestedSecurityMechanismAccepted))
            throw new InvalidServerResponse((int)GetCode(response));

        if (authMethod == SMTPAuthMethods.LOGIN)
        {
            var encoded_username = username.ToBase64();
            await Send(encoded_username + CRLF);

            response = await Receive();
            if (!GetCode(response).InValidCodes(SMTPResponseCode.RequestedSecurityMechanismAccepted))
                throw new InvalidServerResponse((int)GetCode(response));

            var encoded_password = password.ToBase64();
            await Send(encoded_password + CRLF);

            response = await Receive();
            if (!GetCode(response).InValidCodes(SMTPResponseCode.AuthenticationSuccessful))
                throw new InvalidServerResponse((int)GetCode(response));
        }
        else if (authMethod == SMTPAuthMethods.PLAIN)
        {
            var passCode = $"\0{username}\0{password}".ToBase64();
            await Send(passCode + CRLF);

            response = await Receive();
            if (!GetCode(response).InValidCodes(SMTPResponseCode.AuthenticationSuccessful))
                throw new InvalidServerResponse((int)GetCode(response));
        }
        // else if (authMethod == SMTPAuthMethods.CRAM_MD5)
        // {
        //     var challenge = response.Substring(4).FromBase64();
        //     var hexDigits = challenge.ToMd5Hash(password).ToBase16().ToLower();
        //     var passCode = $"{username} {challenge}".ToBase64();

        //     Console.WriteLine(challenge);

        //     await Send(passCode + CRLF);

        //     response = await Receive();
        //     if (!GetCode(response).InValidCodes(SMTPResponseCode.AuthenticationSuccessful))
        //         throw new InvalidServerResponse((int)GetCode(response));
        // }
    }
}