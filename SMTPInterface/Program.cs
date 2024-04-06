using EmailClient;

Console.WriteLine("Server configuration");

Console.Write("Server (default <localhost>): "); var server = Console.ReadLine();
Console.Write("Port (default <25>): "); var port = Convert.ToInt32(Console.ReadLine());

var client = new SMTPClient(server, port);

Console.WriteLine("\n Credentials (blank for anonymous auth)");

Console.Write("Username: "); var username = Console.ReadLine().EmptyOrNull();
Console.Write("Password: "); var password = Console.ReadLine().EmptyOrNull();

client.Credentials = new(username ?? "-", password ?? "-", username == null ? SMTPAuthMethods.ANONYMOUS : SMTPAuthMethods.LOGIN);

Console.WriteLine("Configuration completed");

while (true)
{
    Console.Clear();

    Console.Write("Mail from: "); string from = Console.ReadLine()!;
    Console.Write("Mail to: "); string to = Console.ReadLine()!;
    Console.Write("Subject: "); string subject = Console.ReadLine()!;
    Console.Write("Body: "); string body = Console.ReadLine()!;
    Console.Write("Attached files, separed by ';', blank for no attachments: ");
    string[] fileNames = Console.ReadLine().EmptyOrNull().TrySplit() ?? Array.Empty<string>();

    var files = fileNames.Select(f => new FileAttachment(f));

    var email = new EmailMessage(from, to, subject, body);

    foreach (var file in files) email.Attachments.Add(file);

    Console.Write("Are you sure you want to send this e-mail(y/n)?: "); char confirm = Console.ReadKey().KeyChar;

    if (confirm == 'Y' || confirm == 'y')
    {
        Console.Write("Sending...");

        try { await client.SendEmail(email); Console.WriteLine("Done!"); }
        catch (Exception e) { Console.WriteLine($"Some error occured: {e.Message}"); }
    }

    Console.Write("Do you want to send another e-mail(y\n)?: "); confirm = Console.ReadKey().KeyChar;
    if (confirm == 'N' || confirm == 'n')
    {
        Console.WriteLine("Goodbye!");
        break;
    }
}