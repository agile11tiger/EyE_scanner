
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Net.Mobile.Forms;

namespace Scanner.Views.Scanner
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CodeGenerationPage : ContentPage
    {
        public CodeGenerationPage()
        {
            InitializeComponent();
        }

        public void Handle_Clicked(object sender, System.EventArgs e)
        {
            var barcode = new ZXingBarcodeImageView
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
            barcode.BarcodeFormat = ZXing.BarcodeFormat.QR_CODE;
            barcode.BarcodeOptions.Height = 410;
            barcode.BarcodeOptions.Width = 410;
            barcode.BarcodeValue = "2800100028014";
            code = barcode;
        }
    }
}