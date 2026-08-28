using System.ComponentModel.DataAnnotations;

namespace DisasterRelief.Models
{
    public class Volunteer
    {
        [Key]
        public int VolunteerID { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required, EmailAddress]
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string SpecializedSkills { get; set; }
        public bool IsAvailable { get; set; } = true;
    }
}