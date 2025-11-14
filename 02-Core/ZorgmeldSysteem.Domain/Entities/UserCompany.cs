using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZorgmeldSysteem.Domain.Entities
{
    public class UserCompany
    {
        [Key]
        public int UserCompanyID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        public int CompanyID { get; set; }

        public int? AddedByUserID { get; set; }
        public DateTime AddedOn { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;

        // Navigation Properties
        [ForeignKey("UserID")]
        public User User { get; set; } = null!;

        [ForeignKey("CompanyID")]
        public Company Company { get; set; } = null!;

        [ForeignKey("AddedByUserID")]
        public User? AddedBy { get; set; }
    }
}