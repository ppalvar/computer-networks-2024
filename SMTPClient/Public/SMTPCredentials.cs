using EmailClient.Common;

namespace EmailClient;

public sealed class SMTPCredentials(string UserName, string Password, SMTPAuthMethods? AuthMethod = null)
{
    public string UserName { get; } = UserName;
    public string Password { get; } = Password;
    public SMTPAuthMethods AuthMethod { get; } = AuthMethod ?? SMTPAuthMethods.LOGIN;
}