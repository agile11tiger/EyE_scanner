using SQLite;

namespace Scanner.Models
{
    /// <summary>
    /// Класс, хранящий информацию о авторизации пользователя
    /// </summary>
    public class Sign
    {
        public const string PathUserImageDefault = "Scanner.Resources.Images.user.png";

        [Unique]
        public bool IsAuthorization { get; set; }
        [Unique]
        public string FailMessage { get; set; }
        [Unique]
        public string Name { get; set; }
        [Unique]
        public string Email { get; set; }
        [Unique]
        public string Phone { get; set; }
        [Unique]
        public string Password { get; set; }
        [Unique]
        public string PathToUserImage { get; set; } = PathUserImageDefault;
    }
}
