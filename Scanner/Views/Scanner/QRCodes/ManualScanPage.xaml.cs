using Scanner.ViewModels.Scanner.QRCodes;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Scanner.Views.Scanner.QRCodes
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ManualScanPage : ContentPage
    {
        CashQRCodeViewModel viewModel;

        public ManualScanPage()
        {
            InitializeComponent();

            fiscalNumberEntry.Completed += (sender, e) => fiscalDocumentEntry.Focus();
            fiscalDocumentEntry.Completed += (sender, e) => fiscalSignDocumentEntry.Focus();
            fiscalSignDocumentEntry.Completed += (sender, e) => dateTimeEntry.Focus();
        }

        protected override void OnAppearing()
        {
            var cashQRCodeVM = new CashQRCodeViewModel();
            cashQRCodeVM.CurrentPage = this;
            BindingContext = viewModel = cashQRCodeVM;
            OnBindingContextChanged();
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            //Так как эта страница переиспользуется нужно убрать видимость PlaceHolder Label-ов
            //И установить видимость самих PlaceHolder-ов
            fiscalNumberLabel.IsVisible = false;
            fiscalNumberEntry.PlaceholderColor = Color.Default;
            fiscalDocumentLabel.IsVisible = false;
            fiscalDocumentEntry.PlaceholderColor = Color.Default;
            fiscalSignDocumentLabel.IsVisible = false;
            fiscalSignDocumentEntry.PlaceholderColor = Color.Default;
            dateTimeLabel.IsVisible = false;
            dateTimeEntry.PlaceholderColor = Color.Default;
            checkAmountLabel.IsVisible = false;
            checkAmountEntry.PlaceholderColor = Color.Default;
            base.OnDisappearing();
        }

        private void FiscalNumberEntry_Focused(object sender, FocusEventArgs e)
        {
            fiscalNumberEntry.PlaceholderColor = Color.Transparent;
            fiscalNumberLabel.IsVisible = true;
        }

        private void FiscalNumberEntry_Unfocused(object sender, FocusEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(fiscalNumberEntry.Text))
            {
                fiscalNumberLabel.IsVisible = false;
                fiscalNumberEntry.PlaceholderColor = Color.Default;
            }
        }

        private void FiscalDocumentEntry_Focused(object sender, FocusEventArgs e)
        {
            fiscalDocumentEntry.PlaceholderColor = Color.Transparent;
            fiscalDocumentLabel.IsVisible = true;
        }

        private void FiscalDocumentEntry_Unfocused(object sender, FocusEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(fiscalDocumentEntry.Text))
            {
                fiscalDocumentLabel.IsVisible = false;
                fiscalDocumentEntry.PlaceholderColor = Color.Default;
            }
        }

        private void FiscalSignDocumentEntry_Focused(object sender, FocusEventArgs e)
        {
            fiscalSignDocumentEntry.PlaceholderColor = Color.Transparent;
            fiscalSignDocumentLabel.IsVisible = true;
        }

        private void FiscalSignDocumentEntry_Unfocused(object sender, FocusEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(fiscalSignDocumentEntry.Text))
            {
                fiscalSignDocumentLabel.IsVisible = false;
                fiscalSignDocumentEntry.PlaceholderColor = Color.Default;
            }
        }

        #region Одно целое
        private void DateTimeEntry_Focused(object sender, FocusEventArgs e)
        {
            date.Focus();
        }

        private void DateTimeEntry_Unfocused(object sender, FocusEventArgs e)
        {

        }

        private void Date_DateSelected(object sender, DateChangedEventArgs e)
        {
        }

        private void Date_Focused(object sender, FocusEventArgs e)
        {

        }

        private void Date_Unfocused(object sender, FocusEventArgs e)
        {
            time.Focus();
        }

        private void Time_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == TimePicker.TimeProperty.PropertyName)
            {
                viewModel.DateTime = date.Date.AddTicks(time.Time.Ticks);

                if (string.IsNullOrWhiteSpace(dateTimeEntry.Text))
                {
                    dateTimeLabel.IsVisible = false;
                    dateTimeEntry.PlaceholderColor = Color.Default;
                }

                checkAmountEntry.Focus();
            }
        }

        private void Time_Focused(object sender, FocusEventArgs e)
        {
            dateTimeEntry.PlaceholderColor = Color.Transparent;
            dateTimeLabel.IsVisible = true;
        }

        private void Time_Unfocused(object sender, FocusEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(dateTimeEntry.Text))
            {
                dateTimeLabel.IsVisible = false;
                dateTimeEntry.PlaceholderColor = Color.Default;
            }
        }
        #endregion

        private void CheckAmountEntry_Focused(object sender, FocusEventArgs e)
        {
            checkAmountEntry.PlaceholderColor = Color.Transparent;
            checkAmountLabel.IsVisible = true;
        }

        private void CheckAmountEntry_Unfocused(object sender, FocusEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(checkAmountEntry.Text))
            {
                checkAmountLabel.IsVisible = false;
                checkAmountEntry.PlaceholderColor = Color.Default;
            }
        }
    }
}