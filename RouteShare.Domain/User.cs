namespace RouteShare.Domain
{
    public abstract class User
    {
        public string Email { get; set; }
        protected string Password { get; set; }

        public bool Login(string email, string password)
        {
            return true;
        }
    }
}