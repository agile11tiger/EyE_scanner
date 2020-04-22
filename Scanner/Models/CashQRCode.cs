using Scanner.Models.Iterfaces;
using SQLite;
using System;
using System.Globalization;
using VerificationCheck.Core.Interfaces;

namespace Scanner.Models
{
    /// <summary>
    /// Класс, хранящий информацию о кассовом QR-коде
    /// </summary>
    [Table("CashQRCodes")]
    public class CashQRCode : ICode, IDatabaseItem
    {
        public CashQRCode()
        {
            Name = "CashQRCode";
            TypeCashCheck = "1";
        }

        private static readonly string[] separators = new string[] { "t=", "&s=", "&fn=", "&i=", "&fp=", "&n=" };

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; }
        public string CodeInfo { get; set; }
        public DateTime? DateTime { get; set; }
        public double? CheckAmount { get; set; }
        public string FiscalNumber { get; set; }
        public string FiscalDocument { get; set; }
        public string FiscalSignDocument { get; set; }
        public string TypeCashCheck { get; set; }


        public void ParseCode(
            DateTime dateTime,
            double checkAmount,
            string fiscalNumber,
            string fiscalDocument,
            string fiscalSignDocument,
            string typeCashCheck = "1")
        {
            DateTime = dateTime;
            CheckAmount = checkAmount;
            FiscalNumber = fiscalNumber;
            FiscalDocument = fiscalDocument;
            FiscalSignDocument = fiscalSignDocument;
            TypeCashCheck = typeCashCheck;

            //checkAmout проблема с точкой и запятой
            CodeInfo = $"t=20191108T111700&s={checkAmount}&fn={fiscalNumber}&i={fiscalDocument}&fp={fiscalSignDocument}&n={typeCashCheck}";
        }

        public bool TryParseCode(string qrCode)
        {
            try
            {
                //t=20191108T111700&s=2438.30&fn=9284000100222080&i=0000043560&fp=3019256497&n=1
                var data = qrCode.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                if (data.Length != 6)
                    throw new FormatException($"Этот QR-Code: \"{qrCode}\", имеет неверный формат");

                CodeInfo = qrCode;
                DateTime = System.DateTime.ParseExact(data[0], "yyyyMMddTHHmmss", CultureInfo.InvariantCulture);
                CheckAmount = double.Parse(data[1].Replace('.', ','));
                FiscalNumber = data[2];
                FiscalDocument = data[3];
                FiscalSignDocument = data[4];
                TypeCashCheck = data[5];
            }
            catch { return false; }

            return true;
            //var startDate = QRCodeInfo.IndexOf("t=") + 2;
            //var endDate = QRCodeInfo.IndexOf("&s=");
            //var date = QRCodeInfo.Substring(startDate, endDate - startDate);
            //DateTime = DateTime.ParseExact(date, "yyyyMMddTHHmmss", CultureInfo.InvariantCulture);

            //var startSum = QRCodeInfo.IndexOf("&s=") + 3;
            //var endSum = QRCodeInfo.IndexOf("&fn=");
            //var strSum = QRCodeInfo.Substring(startSum, endSum - startSum).Replace('.', ',');
            //Sum = double.Parse(strSum);

            //var startFN = QRCodeInfo.IndexOf("&fn=") + 4;
            //var endFN = QRCodeInfo.IndexOf("&i=");
            //FiscalNumber = QRCodeInfo.Substring(startFN, endFN - startFN);

            //var startFD = QRCodeInfo.IndexOf("&i=") + 3;
            //var endFD = QRCodeInfo.IndexOf("&fp=");
            //FiscalDocument = QRCodeInfo.Substring(startFD, endFD - startFD);

            //var startFS = QRCodeInfo.IndexOf("&fp=") + 4;
            //var endFS = QRCodeInfo.IndexOf("&n=");
            //FiscalSignDocument = QRCodeInfo.Substring(startFS, endFS - startFS);

            //var startTCR = QRCodeInfo.IndexOf("&n=") + 3;
            //var endTCR = QRCodeInfo.Length;
            //TypeCashReceipt = QRCodeInfo.Substring(startTCR, endTCR - startTCR);
        }
    }
}
