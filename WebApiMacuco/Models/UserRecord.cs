using System;

namespace WebApiMacuco.Models
{
  public class UserRecord
  {
    public int UserId { get; set; }
    public string Description { get; set; }
    public string Document { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string Observation { get; set; }
    public string Base64 { get; set; }
    public string UserCode { get; set; }
    public DateTime BirthDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
  }

  public class AuthenticationRequest
  {
    public string Username { get; set; }
    public string Password { get; set; }
  }

  public class AuthenticationResponse
  {
    public string Token { get; set; }
    public DateTime Expiration { get; set; }
  }
}
