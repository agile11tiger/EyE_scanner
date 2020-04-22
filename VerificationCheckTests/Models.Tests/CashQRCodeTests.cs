using NUnit.Framework;
using Scanner.Models;
using System;
using System.Globalization;
using System.Threading;

namespace VerificationCheckTests
{
    public class Tests
    {
        private const string goodQRCode = "t=20191108T111700&s=2438.30&fn=9284000100222080&i=0000043560&fp=3019256497&n=1";
        private CashQRCode cashQRCode;

        [OneTimeSetUp]
        public void Setup()
        {
            cashQRCode = new CashQRCode();
            cashQRCode.ParseQRCode(goodQRCode);
        }
        [Test]
        public void A()
        {
        //    var path = new Do().Play();
        //    new QRCode.Droid.Dependancies.Audio().PlayAudioFile(path);
        }
        [Test]
        public void Test9()
        {
            //    var a = FNS.CheckAsync(
            //        cashQRCode.FiscalNumber, 
            //        cashQRCode.FiscalDocument,
            //        cashQRCode.FiscalSignDocument,
            //        cashQRCode.DateTime,
            //        Convert.ToDecimal(cashQRCode.Sum));

            //var b = FNS.ReceiveAsync(
            //    cashQRCode.FiscalNumber,
            //    cashQRCode.FiscalDocument,
            //    cashQRCode.FiscalSignDocument,
            //    "+79000630379",
            //    "233964"
            //    ); ;
            while (true)
            {
                Thread.Sleep(1000);
            }
        }

        [Test]
        public void Test1()
        {
            Assert.AreEqual(cashQRCode.QRCodeInfo, goodQRCode);
        }

        [Test]
        public void Test2()
        {
            var dateTime = DateTime.ParseExact("20191108T111700", "yyyyMMddTHHmmss", CultureInfo.InvariantCulture);
            Assert.AreEqual(cashQRCode.DateTime, dateTime);
        }

        [Test]
        public void Test3()
        {
            Assert.AreEqual(cashQRCode.CheckAmount.Value, 2438, 30);
        }

        [Test]
        public void Test4()
        {
            Assert.AreEqual(cashQRCode.FiscalNumber, "9284000100222080");
        }

        [Test]
        public void Test5()
        {
            Assert.AreEqual(cashQRCode.FiscalDocument, "0000043560");
        }
        [Test]
        public void Test6()
        {
            Assert.AreEqual(cashQRCode.FiscalSignDocument, "3019256497");
        }

        [Test]
        public void Test7()
        {
            Assert.AreEqual(cashQRCode.TypeCashCheck, "1");
        }
    }
}