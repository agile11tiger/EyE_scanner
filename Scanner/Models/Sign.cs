using Ninject;
using Scanner.Services.Interfaces;
using SQLite;
using System.Threading.Tasks;
using VerificationCheck.Core.Interfaces;

namespace Scanner.Models
{
    /// <summary>
    /// Класс, хранящий информацию о авторизации пользователя
    /// </summary>
    [Table("Sign")]
    public class Sign: IDatabaseItem
    {
        public const string PathUserImageDefault = "Scanner.Resources.Images.user.png";

        [PrimaryKey, Unique]
        public int Id { get; set; }
        public bool IsAuthorization { get; set; }
        public string FailMessage { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public string PathToUserImage { get; set; } = PathUserImageDefault;
    }
}
