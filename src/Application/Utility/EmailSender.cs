using Application.Interfaces.Managers;
using Application.Interfaces.Utility;
using Domain.Entities;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MimeKit;

namespace Application.Utility;

public class EmailSender : IEmailSender
{
    private readonly IUrlHelper _urlHelper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;

    public EmailSender(IAuthenticationManager authenticationManager,
                       IUrlHelper urlHelper,
                       IHttpContextAccessor httpContextAccessor,
                       IConfiguration configuration)
    {
        _urlHelper = urlHelper;
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
    }
    
    public async Task SendEmailConfirmationEmail(User user, string token)
    {
        var confirmationLink = GetEmailConfirmationLink(user, token);
        
        var message = new MimeMessage();
		if (_configuration["LIBRUM_SELFHOSTED"] != "true")
		{
			message.From.Add (new MailboxAddress ("Librum", "noreply@librumreader.com"));
		}
        else
		{
			 var messFrom = _configuration["SMTPMailFrom"];
			 message.From.Add (new MailboxAddress ("Librum", messFrom));
		}
		
		// Legacy - to be removed
		var userName = user.Name.IsNullOrEmpty() ? user.FirstName : user.Name;
        message.To.Add (new MailboxAddress (userName, user.Email));
        message.Subject = "Confirm Your Email";
        
        message.Body = new TextPart ("plain") {
            Text = $"Hello { userName },\n\nThank you for choosing Librum! " + 
                   "We are happy to tell you, that your account has successfully been created. " +
                   "The final step remaining is to confirm it, and you're all set to go.\n" + 
                   $"To confirm your email, please click the link below:\n{confirmationLink}\n\n" +
                   "If you didn't request this email, just ignore it."
        };

        await SendEmail(message);
    }

    public async Task SendPasswordResetEmail(User user, string token)
    {
		// Go to librumreader.com if not self-hosted
		var resetLink = $"https://librumreader.com/resetPassword?email={user.Email}&token={token}";
		
		// if self-hosted, change the resetlink
		if (_configuration["LIBRUM_SELFHOSTED"] == "true")
		{
			var domain = _configuration["CleanUrl"];
			var encodedToken=System.Web.HttpUtility.HtmlEncode(token);
			resetLink = $"{domain}/user/resetPassword?email={user.Email}&token={encodedToken}";
		}
		
        var message = new MimeMessage();
		if (_configuration["LIBRUM_SELFHOSTED"] != "true")
		{
        	message.From.Add (new MailboxAddress ("Librum", "noreply@librumreader.com"));
		}	
		else
		{
			var messFrom = _configuration["SMTPMailFrom"];
			message.From.Add (new MailboxAddress ("Librum",messFrom));
		}
		
		// Legacy - to be removed
		var userName = user.Name.IsNullOrEmpty() ? user.FirstName : user.Name;
        message.To.Add (new MailboxAddress (userName, user.Email));
        message.Subject = "Reset Your Password";
        
        message.Body = new TextPart ("plain") {
            Text = $"Hello { userName },\n\nYou can find the link to reset your password below. " + 
                   "Follow the link and continue the password reset on our website.\n" + 
                   $"{resetLink}\n\n" +
                   "If you didn't request this email, just ignore it."
        };
        
        await SendEmail(message);
    }

    public async Task SendDowngradeWarningEmail(User user)
    {
	    var message = new MimeMessage();
		message.From.Add (new MailboxAddress ("Librum", "noreply@librumreader.com"));
		
		// Legacy - to be removed
		var userName = user.Name.IsNullOrEmpty() ? user.FirstName : user.Name;
		message.To.Add (new MailboxAddress (userName, user.Email));
	    message.Subject = "Your books may be deleted in 7 days! - Take Action";
        
	    message.Body = new TextPart ("plain") {
		    Text = $"Hello { userName },\n\nYou have recently downgraded your Account. " + 
		           "Due to the downgrade your online storage was reduced and your current library " +
		           "size may exceed your new storage limit.\n" + 
		           $"Please reduce your library size if that is the case, otherwise books from your account will automatically be DELETED until " +
		           "your used storage does not exceed the storage your tier provides anymore.\n\n" +
		           "You have 7 days to perform this action. If you think that this is a mistake, or you have any " +
		           "questions, please contact us at: contact@librumreader.com"
	    };

	    await SendEmail(message);
    }

    private string GetEmailConfirmationLink(User user, string token)
    {
        var endpointLink = _urlHelper.Action("ConfirmEmail",
                                             "Authentication",
                                             new
                                             {
                                                 email = user.Email,
                                                 token = token
                                             });
        var serverUri = _httpContextAccessor.HttpContext!.Request.Scheme + "://" +
                        _httpContextAccessor.HttpContext!.Request.Host;
        var confirmationLink = serverUri + endpointLink;

        return confirmationLink;
    }

    
    
    private async Task SendEmail(MimeMessage message)
    {
        using var client = new SmtpClient();
        await client.ConnectAsync(_configuration["SMTPEndpoint"], 465, true);

        await client.AuthenticateAsync(_configuration["SMTPUsername"],
                                       _configuration["SMTPPassword"]);

        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}