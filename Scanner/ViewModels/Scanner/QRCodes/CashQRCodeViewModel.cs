using Newtonsoft.Json;
using Ninject;
using Scanner.Models;
using Scanner.Models.Iterfaces;
using Scanner.ViewModels.Scanner.Checks;
using Scanner.Views.Scanner.Checks;
using SQLite;
using System;
using System.Linq;
using System.Threading.Tasks;
using VerificationCheck.Core;
using VerificationCheck.Core.Interfaces;

namespace Scanner.ViewModels.Scanner.QRCodes
{
    /// <summary>
    /// Класс, взаимодействующий с кассовым QR-кодом 
    /// </summary>
    public class CashQRCodeViewModel : CodeViewModel<CashQRCodeViewModel, FriendsChecksViewModel>, ISerializable, IDBItem
    {
        public CashQRCodeViewModel() : base(
            App.Container.Get<ICode>("CashQRCode"),
            App.Container.Get<WaitingChecksListViewModel>(),
            App.Container.Get<ChecksListsViewModel>())
        {
            fns = App.Container.Get<FNS>();
            cashQRCode = Code as CashQRCode;
            UserAccountFNS = App.Container.Get<UserAccountFNSViewModel>();
        }

        private FNS fns;
        private CashQRCode cashQRCode;
        public string CashQRCodeJson { get; set; }
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        [Ignore]
        public UserAccountFNSViewModel UserAccountFNS { get; set; }

        [Ignore]
        public string FiscalNumber
        {
            get => cashQRCode.FiscalNumber;
            set
            {
                if (cashQRCode.FiscalNumber != value)
                {
                    cashQRCode.FiscalNumber = value;
                    OnPropertyChanged();
                }
            }
        }

        [Ignore]
        public string FiscalDocument
        {
            get => cashQRCode.FiscalDocument;
            set
            {
                if (cashQRCode.FiscalDocument != value)
                {
                    cashQRCode.FiscalDocument = value;
                    OnPropertyChanged();
                }
            }
        }

        [Ignore]
        public string FiscalSignDocument
        {
            get => cashQRCode.FiscalSignDocument;
            set
            {
                if (cashQRCode.FiscalSignDocument != value)
                {
                    cashQRCode.FiscalSignDocument = value;
                    OnPropertyChanged();
                }
            }
        }

        [Ignore]
        public DateTime? DateTime
        {
            get => cashQRCode.DateTime;
            set
            {
                if (cashQRCode.DateTime != value)
                {
                    cashQRCode.DateTime = value;
                    OnPropertyChanged();
                }
            }
        }

        [Ignore]
        public double? CheckAmount
        {
            get => cashQRCode.CheckAmount;
            set
            {
                if (cashQRCode.CheckAmount != value)
                {
                    cashQRCode.CheckAmount = value;
                    OnPropertyChanged();
                }
            }
        }

        [Ignore]
        public string TypeCashCheck
        {
            get => cashQRCode.TypeCashCheck;
            set
            {
                if (cashQRCode.TypeCashCheck != value)
                {
                    cashQRCode.TypeCashCheck = value;
                    OnPropertyChanged();
                }
            }
        }

        public void Serialize()
        {
            CashQRCodeJson = JsonConvert.SerializeObject(cashQRCode, ISerializable.JsonSettings);
        }

        public void Deserialize()
        {
            cashQRCode = JsonConvert.DeserializeObject<CashQRCode>(CashQRCodeJson, ISerializable.JsonSettings);
            Code = cashQRCode;
        }

        protected override async Task ProcessCode()
        {
            var checksWaitingPage = App.Container.Get<WaitingChecksPage>();
            await RequestList.AddCommand.ExecuteAsync(this);
            await Navigation.PushAsync(checksWaitingPage);

            //var verifyResult = await fns.VerifyAsync(
            //    FiscalNumber,
            //    FiscalDocument,
            //    FiscalSignDocument,
            //    DateTime.GetValueOrDefault(),
            //    CheckAmount.GetValueOrDefault()
            //    );        


            //if (!verifyResult.CheckExists)
            //    return false;
            //var checkResult = await fns.ReceiveAsync(FiscalNumber, FiscalDocument, FiscalSignDocument, UserAccountFNS.Phone, UserAccountFNS.Password);

            //if (checkResult.IsSuccess)
            //{
            //    var checksPage = App.Container.Get<ChecksPage>();
            //    var checkVM = new FriendsChecksViewModel(checkResult.Document.Check);
            //    await ResultList.AddCommand.ExecuteAsync(checkVM);
            //    await RequestList.RemoveCommand.ExecuteAsync(this);
            //    await Navigation.PushAsync(checksPage).ConfigureAwait(false);
            //}
            //else
            //{
            //    var checksWaitingPage = App.Container.Get<ChecksWaitingPage>();
            //    FailMessage = verifyResult.Message;
            //    await RequestList.AddCommand.ExecuteAsync(this);
            //    await Navigation.PushAsync(checksWaitingPage).ConfigureAwait(false);
            //}
        }

        /// <summary>
        /// Вызывается только внутри другой команды
        /// </summary>
        public async Task<bool> TryProcessCode()
        {
            var verifyResult = await fns.VerifyAsync(
                FiscalNumber,
                FiscalDocument,
                FiscalSignDocument,
                DateTime.GetValueOrDefault(),
                CheckAmount.GetValueOrDefault()
                );

            if (!verifyResult.CheckExists)
                return false;

            var checkResult = await fns.ReceiveAsync(FiscalNumber, FiscalDocument, FiscalSignDocument, UserAccountFNS.Phone, UserAccountFNS.Password);

            if (checkResult.IsSuccess)
            {
                var checkVM = new FriendsChecksViewModel(checkResult.Document.Check);
                await ResultLists.Checks.FirstOrDefault(c => c.Title == "Чеки")?.AddCommand.ExecuteAsync(checkVM);
                await RequestList.RemoveCommand.ExecuteAsync(this).ConfigureAwait(false);
                return true;
            }

            return false;
        }
    }
}
