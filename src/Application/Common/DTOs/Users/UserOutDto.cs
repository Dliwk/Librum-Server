using Application.Common.DTOs.Tags;

namespace Application.Common.DTOs.Users;

public class UserOutDto
{
    // Legacy - to be removed
    public string FirstName { get; set; }
    
    // Legacy - to be removed
    public string LastName { get; set; }
    
    public string Name { get; set; }

    public string Email { get; set; }
    
    public string Role { get; set; }
    
    public string ProductId { get; set; }
    
    public string CustomerId { get; set; }

    public DateTime AccountCreation { get; set; }
    
    public long UsedBookStorage { get; set; }
    
    public long BookStorageLimit { get; set; }
    
    public DateTime ProfilePictureLastUpdated { get; set; }
    
    public bool HasProfilePicture { get; set; }

    public ICollection<TagOutDto> Tags { get; set; } = new List<TagOutDto>();
}
