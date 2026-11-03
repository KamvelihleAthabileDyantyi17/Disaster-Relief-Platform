using System.ComponentModel.DataAnnotations;

namespace DisasterRelief.Models
{
    // A news update posted by an employee and shown on the home page
    public class ReliefUpdate
    {
        public int Id { get; set; }

        [Required, StringLength(120)]
        public string Title { get; set; } = "";

        [Required, StringLength(2000)]
        public string Message { get; set; } = "";

        public string? PostedBy { get; set; }

        public DateTime DatePosted { get; set; } = DateTime.Now;
    }
}