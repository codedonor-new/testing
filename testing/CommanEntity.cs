using System.ComponentModel.DataAnnotations.Schema;

namespace testing
{
    public class CommanEntity
    {
        public bool IsActive { get; set; } = true;

        [ForeignKey("Created")]
        public int? CreatedId { get; set; }
        public int? UpdatedId { get; set; }
        public DateTime CreatedDate { get; set; } 
        public DateTime? UpdatedDate { get; set; }
    }
}
