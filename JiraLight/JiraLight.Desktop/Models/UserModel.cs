namespace JiraLight.Desktop.Models
{
    public class UserModel
    {
        private static int _nextId = 1;

        public UserModel()
        {
            Id = _nextId++;
        }

        public int Id { get; set; }
        public string Username { get; set; } // логин
        public string Password { get; set; } // пароль (пока в открытом виде, но лучше хэш)
        public string Name { get; set; }
        public bool? IsAdmin { get; set; }
    }
}
