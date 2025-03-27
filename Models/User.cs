using System.ComponentModel.DataAnnotations;

public class User
{
    public int Id { get; set; }

    [Required, MaxLength(25)]
    public string login { get; set; }

    [Required, EmailAddress, MaxLength(255)]
    public string Email { get; set; }

    [Required]
    public string PasswordHash { get; set; }

    [Required]
    public string createdAt { get; set; }

    [Required]
    public string updatedAt { get; set; }

    public string disabled { get; set; }

    public string banned { get; set; }


}
