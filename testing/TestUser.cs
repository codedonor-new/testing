namespace testing
{
    public class TestUser
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ReferenceCode { get; set; }
        public long? IMEI { get; set; }
        public long? UDID { get; set; }
    }

    public class FileModel
    {
        public List<FileItem> Files { get; set; }
    }

    public class FileItem
    {
        public string DocumentId { get; set; }
        public IFormFile File { get; set; }
    }
}
