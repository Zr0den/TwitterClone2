namespace Helpers
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserTag { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }
    public class UserCreateDto
    {
        public string Name { get; set; }
        public string UserTag { get; set; }
        public string Email { get; set; }
        public string Password { get; set; } // You may add further password handling later
    }

    public class UserUpdateDto
    {
        public string Name { get; set; }
        public string UserTag { get; set; }
        public string Email { get; set; }
    }

    public class UserProfileDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserTag { get; set; }
        public string Email { get; set; }
    }
}
