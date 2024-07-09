using System;

namespace WebApiMacuco.Models
{
    public class UserRecord
    {
        public int UserId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Document { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Observation { get; set; } = string.Empty;
        public string Base64 { get; set; } = string.Empty;
        public string UserCode { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

}
