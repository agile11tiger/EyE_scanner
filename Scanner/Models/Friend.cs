using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using VerificationCheck.Core.Interfaces;
using Xamarin.Forms;

namespace Scanner.Models
{
    [Table("Friends")]
    public class Friend : IDatabaseItem
    {
        //каждому пользователю будет выдаваться уникальный id, поэтому не надо autoincrement
        //TODO решить проблему с unique(не работает)
        [PrimaryKey, Unique]
        public int Id { get; set; } = -1;

        public string Name { get; set; } = "Выберите друга";

        [Ignore]
        public ImageSource Image { get; set; }
        public string Phone { get; set; }
    }
}
