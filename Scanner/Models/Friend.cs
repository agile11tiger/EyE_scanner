using Scanner.Models.Interfaces;
using Scanner.Models.Iterfaces;
using SQLite;
using Xamarin.Forms;

namespace Scanner.Models
{
    [Table("Friends")]
    public class Friend : IDatabaseItem, IClone<Friend>
    {
        //TODO:каждому пользователю будет выдаваться уникальный id, поэтому не надо autoincrement
        [PrimaryKey, Unique]
        public int Id { get; set; } = -1;

        public string Name { get; set; } = "Выберите друга";

        [Ignore]
        public ImageSource Image { get; set; }
        public string Phone { get; set; }

        public Friend Clone()
        {
            return (Friend)MemberwiseClone();
        }
    }
}
