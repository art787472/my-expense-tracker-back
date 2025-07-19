using 記帳程式後端.Models;

namespace 記帳程式後端.Dto
{
    public class ImageUploadResponse
    {
        public bool Success { get; set; }
        public string Url { get; set; }
        public string ErrorMessage { get; set; }
        public string StorageKey { get; set; }
        public ImageModel image { get; set; }
    }
}
