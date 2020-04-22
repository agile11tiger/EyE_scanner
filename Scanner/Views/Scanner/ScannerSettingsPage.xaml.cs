using Scanner.ViewModels.Scanner;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Scanner.Views.Scanner
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScannerSettingsPage : ContentPage
    {
        private readonly ScannerSettingsViewModel viewModel;

        public ScannerSettingsPage(ScannerSettingsViewModel viewModel)
        {
            InitializeComponent();
            viewModel.CurrentPage = this;
            BindingContext = this.viewModel = viewModel;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            viewModel.OnDisappearingCommand.Execute(null);
        }

        private void InitialDelayBeforeAnalyzingFrames_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            if (initialDelayBeforeAnalyzingFramesLabel != null)
                initialDelayBeforeAnalyzingFramesLabel.Text = string.Format("Выбрано: {0:F1}", e.NewValue);
        }

        private void DelayBetweenAnalyzingFrames_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            if (delayBetweenAnalyzingFramesLabel != null)
                delayBetweenAnalyzingFramesLabel.Text = string.Format("Выбрано: {0:F1}", e.NewValue);
        }

        private void DelayBetweenContinuousScans_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            if (delayBetweenContinuousScansLabel != null)
                delayBetweenContinuousScansLabel.Text = string.Format("Выбрано: {0:F1}", e.NewValue);
        }
    }
}