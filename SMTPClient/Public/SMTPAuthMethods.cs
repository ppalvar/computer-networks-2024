using Ardalis.SmartEnum;

namespace EmailClient;

public sealed class SMTPAuthMethods : SmartEnum<SMTPAuthMethods>
{
    public SMTPAuthMethods(string name, int value) : base(name, value) { }

    public static readonly SMTPAuthMethods ANONYMOUS = new("ANONYMOUS", 0);
    public static readonly SMTPAuthMethods LOGIN = new("LOGIN", 1);
    public static readonly SMTPAuthMethods PLAIN = new("PLAIN", 2);
    // public static readonly SMTPAuthMethods CRAM_MD5 = new("CRAM-MD5", 3);

}