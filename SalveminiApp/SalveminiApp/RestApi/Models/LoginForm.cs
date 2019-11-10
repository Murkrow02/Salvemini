using System;
namespace SalveminiApp.RestApi.Models
{
    public class LoginForm
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class changeBlock
    {
        public string vecchiaPassword { get; set; }
        public string nuovaPassword { get; set; }
    }
}
