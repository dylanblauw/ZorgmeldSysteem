using System.ComponentModel.DataAnnotations;

namespace ZorgmeldSysteem.Domain.Entities
{
    public class Company
    {
        [Key]
        public int CompanyID { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Phonenumber { get; set; } = string.Empty;

        // Adres opgesplitst
        [MaxLength(100)]
        public string Street { get; set; } = string.Empty;  // Straatnaam

        [MaxLength(10)]
        public string HouseNumber { get; set; } = string.Empty;  // Huisnummer

        [MaxLength(10)]
        public string HouseNumberAddition { get; set; } = string.Empty;  // Toevoeging (bijv. "A", "bis")

        [MaxLength(10)]
        public string PostalCode { get; set; } = string.Empty;  // Postcode

        [MaxLength(100)]
        public string City { get; set; } = string.Empty;  // Woonplaats

        [MaxLength(100)]
        public string Province { get; set; } = string.Empty;  // Provincie

        [MaxLength(100)]
        public string Country { get; set; } = string.Empty;  // Land

        [MaxLength(100)]
        public string Contact { get; set; } = string.Empty;

        public bool IsExternal { get; set; } = false;

        // Audit fields
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? ChangedOn { get; set; }
        public string ChangedBy { get; set; } = string.Empty;

        // Navigation properties
        public List<Ticket> Tickets { get; set; } = new();
        public List<Mechanic> Mechanics { get; set; } = new();
        public List<Objects> Objects { get; set; } = new();
        public List<UserCompany> UserCompanies { get; set; } = new();

    }
}