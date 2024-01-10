using Application.Common.DTOs.Tags;
using Application.Common.DTOs.Users;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace Application.Interfaces.Services;

public interface IUserService
{
    public Task<UserOutDto> GetUserAsync(string email);
    public Task DeleteUserAsync(string email);
    public Task PatchUserAsync(string email,
                               JsonPatchDocument<UserForUpdateDto> patchDoc,
                               ControllerBase controllerBase);
    public Task ChangeProfilePicture(string email, MultipartReader reader);
    public Task<Stream> GetProfilePicture(string email);
    public Task DeleteProfilePicture(string email);
    public Task ChangePasswordAsync(string email, string newPassword);
    public Task ChangePasswordWithTokenAsync(string email, string token,
                                             string newPassword);
    public Task ForgotPassword(string email);
    public Task AddCustomerIdToUser(string email, string customerId);
    public Task AddTierToUser(string customerId, string productId);
    public Task ResetUserToFreeTier(string customerId);
}