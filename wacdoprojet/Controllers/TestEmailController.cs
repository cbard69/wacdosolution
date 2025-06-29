

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace wacdoproject.Controllers
{
    public class TestEmailController : Controller
    {
        private readonly IEmailSender _emailSender;

        public TestEmailController(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        [HttpGet("/testemail")]
        public async Task<IActionResult> Send()
        {
            await _emailSender.SendEmailAsync("christian.bardon@gmail.com", "Test email", "<p>Ceci est un test</p>");
            return Content("Email tenté !");
        }
    }
}
