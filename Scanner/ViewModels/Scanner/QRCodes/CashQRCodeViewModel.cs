using Scanner.Models;
using Scanner.Models.Interfaces;
using Scanner.ViewModels.Scanner.Checks;
using System;
using System.Threading.Tasks;
using VerifyReceiptSDK;
using VerifyReceiptSDK.Results;

namespace Scanner.ViewModels.Scanner.QRCodes
{
    /// <summary>
    /// Класс, взаимодействующий с кассовым QR-кодом 
    /// </summary>
    public class CashQRCodeViewModel : CodeViewModel<WaitingChecksListViewModel, ChecksListsViewModel>, IEquatable<CashQRCodeViewModel>, IPartialClone<CashQRCodeViewModel>
    {
        public CashQRCodeViewModel(
            CashQRCode cashQRCode,
            WaitingChecksListViewModel waitingChecksListVM,
            ChecksListsViewModel checksListsVM,
            UserAccountFNSViewModel userAccountFNS)
            : base(
            cashQRCode,
            waitingChecksListVM,
            checksListsVM)
        {
            CashQRCode = cashQRCode;
            UserAccountFNS = userAccountFNS;
        }

        public CashQRCode CashQRCode { get; private set; }
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

        public double? TotalSum
        {
            get => CashQRCode.TotalSum;
            set
            {
                if (CashQRCode.TotalSum != value)
                {
                    CashQRCode.TotalSum = value;
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

        protected override async Task ProcessQRCode()
        {
            if (await TryProcessCode())
            {
                await Navigation.PushAsync(Pages.ChecksTabbedPage);
            }
            else
            {
                await RequestList.AddCommand.ExecuteAsync(this);
                await Navigation.PushAsync(Pages.WaitingChecksPage);
            }
        }

        public async Task<bool> TryProcessCode()
        {
            var checkResult = await TryGetCheckResult();

            if (checkResult == null)
                return false;

            if (checkResult.IsSuccess)
            {
                var check = new Check(checkResult.Document.Receipt);
                var commonCheckVM = new CheckViewModel(check);
                await ResultList.Add(commonCheckVM, PageTitles.COMMON_CHECKS);
                await RequestList.RemoveCommand.ExecuteAsync(this);
                return true;
            }
            else
            {
                FailMessage = checkResult.Message;
                return false;
            }
        }

        private async Task<ReceiptResult> TryGetCheckResult()
        {
            if (!UserAccountFNS.IsAuthorization)
                return null;

            try
            {
                var verifyResult = await FNS.VerifyAsync(
                   FiscalNumber,
                   FiscalDocument,
                   FiscalSignDocument,
                   DateTime.GetValueOrDefault(),
                   TotalSum.GetValueOrDefault()
                   );

                if (!verifyResult.ReceiptExists)
                {
                    FailMessage = verifyResult.Message;
                    return null;
                }

                return await FNS.ReceiveAsync(
                    FiscalNumber,
                    FiscalDocument,
                    FiscalSignDocument,
                    UserAccountFNS.GetClearPhone(),
                    UserAccountFNS.Password);
            }
            catch(ArgumentException e)
            {
                FailMessage = e.Message;
                throw;
            }
        }

        public bool Equals(CashQRCodeViewModel other)
        {
            return CashQRCode.Id == other.CashQRCode.Id;
        }

        public CashQRCodeViewModel PartialClone()
        {
            var clone = (CashQRCodeViewModel)MemberwiseClone();
            clone.CashQRCode = CashQRCode.Clone();
            return clone;
        }
    }
}
