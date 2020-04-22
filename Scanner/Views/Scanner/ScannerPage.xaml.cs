using Scanner.ViewModels.Scanner;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Scanner.Views.Scanner
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScannerPage : ContentPage
    {
        private readonly ScannerViewModel viewModel;

        public ScannerPage(ScannerViewModel scannerVM)
        {
            InitializeComponent();
            scannerVM.CurrentPage = this;
            scannerVM.CashQRCodeVM.CurrentPage = this;

            //scannerVM.Corner = corner;   //TODO: убрать
            //scannerVM.Scanner = scanner;  // убрать

            scannerVM.TorchImage = ImageSource.FromResource("Scanner.Resources.Images.Scanner.torch_off.png");
            scannerVM.InitialOutlineQRCodeCommand.Execute(null);

            BindingContext = viewModel = scannerVM;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            viewModel.ScannerSwitchCommand.Execute(true);
            viewModel.RunAnimationCommand.Execute(null);
        }

        protected override void OnDisappearing()
        {
            viewModel.TurnTorchCommand.Execute(false);
            viewModel.ScannerSwitchCommand.Execute(false);
            this.AbortAnimation("SimpleAnimation");
            base.OnDisappearing();
        }
    }
}