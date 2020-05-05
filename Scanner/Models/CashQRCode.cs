using Scanner.Models.Interfaces;
using Scanner.Models.Iterfaces;
using SQLite;
using System;
using System.Globalization;

namespace Scanner.Models
{
    /// <summary>
    /// Класс, хранящий информацию о кассовом QR-коде
    /// </summary>
    [Table("CashQRCodes")]
    public class CashQRCode : ICode, IDatabaseItem, IClone<CashQRCode>
    {
        private static readonly string[] separators = new string[] { "t=", "&s=", "&fn=", "&i=", "&fp=", "&n=" };

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; } = "CashQRCode";
        public string CodeInfo { get; set; }
        public DateTime? DateTime { get; set; }
        public double? TotalSum { get; set; }
        public string FiscalNumber { get; set; }
        public string FiscalDocument { get; set; }
        public string FiscalSignDocument { get; set; }
        public string TypeCashCheck { get; set; } = "1";

        public bool TryParseCode(string qrCode)
        {
            var data = qrCode.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            if (data.Length != 6)
                throw new FormatException($"Этот QR-Code: \"{qrCode}\", имеет неверный формат");

            CodeInfo = qrCode;
            DateTime = System.DateTime.ParseExact(data[0], "yyyyMMddTHHmm", CultureInfo.InvariantCulture);
            TotalSum = double.Parse(data[1], CultureInfo.InvariantCulture);
            FiscalNumber = data[2];
            FiscalDocument = data[3];
            FiscalSignDocument = data[4];
            TypeCashCheck = data[5];
            return true;
        }

        public CashQRCode Clone()
        {
            return (CashQRCode)MemberwiseClone();
        }
    }
}
