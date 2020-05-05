using Scanner.Models.Iterfaces;
using SQLite;
using System.Runtime.Serialization;

namespace Scanner.Models
{
    /// <summary>
    /// Класс, хранящий информацию о авторизации пользователя
    /// </summary>
    [Table("Sign")]
    public class Sign : IDatabaseItem
    {
        [PrimaryKey, Unique]
        public int Id { get; set; }
        public bool IsAuthorization { get; set; }
        public string FailMessage { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "email")]
        public string Email { get; set; }
        [DataMember(Name = "phone")]
        public string Phone { get; set; }
        [DataMember(Name = "passowrd")]
        public string Password { get; set; }
        public string PathToUserImage { get; set; } = ImagePaths.User;
    }
}
