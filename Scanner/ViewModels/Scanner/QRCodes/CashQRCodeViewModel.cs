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
    public class CashQRCodeViewModel : CodeViewModel<CashQRCodeViewModel, FriendsChecksViewModel>
    {
        public CashQRCodeViewModel(
            CashQRCode cashQRCode, 
            WaitingChecksListViewModel waitingChecksListVM, 
            ChecksListsViewModel checksListsVM,
            FNS fns,
            UserAccountFNSViewModel userAccountFNS,
            ChecksTabbedPage checksTabbedPage,
            WaitingChecksPage waitingChecksPage)
            : base(
            cashQRCode,
            waitingChecksListVM,
            checksListsVM)
        {
            this.fns = fns;
            this.waitingChecksPage = waitingChecksPage;
            this.checksTabbedPage = checksTabbedPage;
            CashQRCode = cashQRCode;
            UserAccountFNS = userAccountFNS;
        }

        private readonly FNS fns;
        private readonly ChecksTabbedPage checksTabbedPage;
        private readonly WaitingChecksPage waitingChecksPage;
        public CashQRCode CashQRCode { get; }
        public UserAccountFNSViewModel UserAccountFNS { get; }

        public string FiscalNumber
        {
            get => CashQRCode.FiscalNumber;
            set
            {
                if (CashQRCode.FiscalNumber != value)
                {
                    CashQRCode.FiscalNumber = value;
                    OnPropertyChanged();
                }
            }
        }

        public string FiscalDocument
        {
            get => CashQRCode.FiscalDocument;
            set
            {
                if (CashQRCode.FiscalDocument != value)
                {
                    CashQRCode.FiscalDocument = value;
                    OnPropertyChanged();
                }
            }
        }

        public string FiscalSignDocument
        {
            get => CashQRCode.FiscalSignDocument;
            set
            {
                if (CashQRCode.FiscalSignDocument != value)
                {
                    CashQRCode.FiscalSignDocument = value;
                    OnPropertyChanged();
                }
            }
        }

        public DateTime? DateTime
        {
            get => CashQRCode.DateTime;
            set
            {
                if (CashQRCode.DateTime != value)
                {
                    CashQRCode.DateTime = value;
                    OnPropertyChanged();
                }
            }
        }

        public double? CheckAmount
        {
            get => CashQRCode.CheckAmount;
            set
            {
                if (CashQRCode.CheckAmount != value)
                {
                    CashQRCode.CheckAmount = value;
                    OnPropertyChanged();
                }
            }
        }

        public string TypeCashCheck
        {
            get => CashQRCode.TypeCashCheck;
            set
            {
                if (CashQRCode.TypeCashCheck != value)
                {
                    CashQRCode.TypeCashCheck = value;
                    OnPropertyChanged();
                }
            }
        }

        protected override async Task ProcessCode()
        {
            await RequestList.AddCommand.ExecuteAsync(this);
            await Navigation.PushAsync(waitingChecksPage);

            //var verifyResult = await fns.VerifyAsync(
            //    FiscalNumber,
            //    FiscalDocument,
            //    FiscalSignDocument,
            //    DateTime.GetValueOrDefault(),
            //    CheckAmount.GetValueOrDefault()
            //    );


            //if (!verifyResult.CheckExists)
            //    return;
            //var checkResult = await fns.ReceiveAsync(FiscalNumber, FiscalDocument, FiscalSignDocument, UserAccountFNS.Phone, UserAccountFNS.Password);

            //if (checkResult.IsSuccess)
            //{
            //    var checkVM = new FriendsChecksViewModel(checkResult.Document.Check);
            //    await ResultLists.AddToCommonChecks(checkVM);
            //    await RequestList.RemoveCommand.ExecuteAsync(this);
            //    await Navigation.PushAsync(checksTabbedPage).ConfigureAwait(false);
            //}
            //else
            //{
            //    FailMessage = verifyResult.Message;
            //    await RequestList.AddCommand.ExecuteAsync(this);
            //    await Navigation.PushAsync(waitingChecksPage).ConfigureAwait(false);
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
