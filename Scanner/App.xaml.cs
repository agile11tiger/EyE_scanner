using Ninject;
using Ninject.Modules;
using Scanner.ViewModels.Scanner.Checks;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Scanner
{
    public partial class App : Application
    {
        //TODO: Replace with *.azurewebsites.net url after deploying backend to Azure
        //To debug on Android emulators run the web backend against .NET Core not IIS
        //If using other emulators besides stock Google images you may need to adjust the IP address
        public static string AzureBackendUrl =
            DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5000" : "http://localhost:5000";

        public static StandardKernel Container { get; set; }

        public App(NinjectModule phoneModule)
        {
            InitializeComponent();
            AppDomain.CurrentDomain.UnhandledException += ProcessException;
            var settings = new NinjectSettings() { LoadExtensions = false };
            var commonModule = new CommonModule();
            Container = new StandardKernel(settings, phoneModule);
            Container.Load(commonModule);

            CreateInAdvance();
            MainPage = Container.Get<AppShell>();
        }

        private void CreateInAdvance()
        {
            #region чтобы не было циклической зависимости
            Pages.Initialize();
            Container.Get<WaitingChecksListViewModel>().CallInitializeListFromDatabase().Wait();
            Container.Get<ChecksListsViewModel>();
            #endregion
        }

        private void ProcessException(object sender, UnhandledExceptionEventArgs args)
        {
            //TODO: Добавить глобальный обработчик ошибок
            //Console.WriteLine((args.ExceptionObject as Exception).StackTrace);
            Environment.Exit(1);
        }

        protected override void OnStart()
        {
            //TODO: Handle when your app starts
        }

        protected override void OnSleep()
        {
            //TODO: Handle when your app sleeps
        }

        protected override void OnResume()
        {
            //TODO: Handle when your app resumes
        }
    }
}
