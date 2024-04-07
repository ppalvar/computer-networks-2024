namespace EmailClient;

public class EmailMessage(string From, string To, string Subject, string Body)
{
    public string From { get; } = From;
    public string To { get; } = To;
    public string Subject { get; } = Subject;
    public string Body { get; } = Body;

    public List<FileAttachment> Attachments { get; set; } = new List<FileAttachment>();
}