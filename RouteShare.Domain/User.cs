namespace RouteShare.Domain
{
    public abstract class User
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public bool Login(string email, string password)
        {
            return Email == email && Password == password;
        }
    }
}