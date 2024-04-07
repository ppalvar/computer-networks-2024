namespace EmailClient;

public class FileAttachment
{
    public string FileName { get; private set; }
    public byte[] FileContent { get { return File.ReadAllBytes(filePath); } }
    public string FileBase64 { get { return Convert.ToBase64String(FileContent); } }

    private readonly string filePath;

    public FileAttachment(string filePath)
    {
        this.filePath = filePath;
        this.FileName = Path.GetFileName(filePath);
    }
}