using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace 記帳程式後端.Models
{
    public class ImageModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string StorageKey { get; set; }
        public string StorageProvider {  get; set; }
        public string  url { get; set; }
    }
}
