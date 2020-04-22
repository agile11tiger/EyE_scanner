using Ninject;
using Ninject.Modules;
using Ninject.Parameters;
using Scanner.Models;
using Scanner.Models.Iterfaces;
using Scanner.Services;
using Scanner.Services.Interfaces;
using Scanner.ViewModels;
using Scanner.ViewModels.Authorization;
using Scanner.ViewModels.Scanner;
using Scanner.ViewModels.Scanner.Checks;
using Scanner.ViewModels.Scanner.QRCodes;
using Scanner.Views;
using Scanner.Views.Authorization;
using Scanner.Views.Scanner;
using Scanner.Views.Scanner.Checks;
using Scanner.Views.Scanner.Friends;
using Scanner.Views.Scanner.QRCodes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ZXing;

namespace Scanner
{
    public class CommonModule : NinjectModule
    {
        //http://www.80levelelf.com/Post?postId=22
        public override void Load()
        {
            //Стараюсь делать все классы в .InSingletonScope() для того, чтобы не тратить время на создания новых.
            #region Pages(указываю ВСЕ СТРАНИЦЫ ЯВНО)
            //Легче определить поведение страниц(одна на всех или нет), так как точкой входа отдельных элементов является страница.
            Bind<AuthorizationPage>().ToSelf();
            Bind<Lazy<AuthorizationPage>>().ToMethod(context => new Lazy<AuthorizationPage>(() => Kernel.Get<AuthorizationPage>()));
            Bind<ForgotPasswordPage>().ToSelf();
            Bind<Lazy<ForgotPasswordPage>>().ToMethod(context => new Lazy<ForgotPasswordPage>(() => Kernel.Get<ForgotPasswordPage>()));
            Bind<SignInPage>().ToSelf();
            Bind<SignUpPage>().ToSelf();
            Bind<ChecksTabbedPage>().ToSelf().InSingletonScope();
            Bind<CheckTabbedPage>().ToSelf();
            Bind<WaitingChecksPage>().ToSelf().InSingletonScope();
            Bind<FriendsPage>().ToSelf().InSingletonScope();
            Bind<FriendPage>().ToSelf();
            Bind<ManualScanPage>().ToSelf();
            Bind<Func<ManualScanPage>>().ToMethod(context => () => Kernel.Get<ManualScanPage>());
            Bind<ScannerPage>().ToSelf().InSingletonScope();
            Bind<ScannerSettingsPage>().ToSelf().InSingletonScope();
            Bind<CodeGenerationPage>().ToSelf().InSingletonScope();
            Bind<MainPage>().ToSelf().InSingletonScope();
            Bind<AppShell>().ToSelf().InSingletonScope();
            #endregion 

            #region ViewModels(указываю только те ViewModel, которые должны быть статические для всех классов)
            //У других же есть точка входа страница и на её уровне уже определяется поведение
            Bind<UserAccountFNSViewModel>().ToSelf().InSingletonScope();
            Bind<ScannerSettingsViewModel>().ToSelf().InSingletonScope();
            Bind<ChecksListsViewModel>().ToSelf().InSingletonScope();
            Bind<WaitingChecksListViewModel>().ToSelf().InSingletonScope();
            Bind<Func<CashQRCode, CashQRCodeViewModel>>()
                .ToMethod(context => (c) => Kernel.Get<CashQRCodeViewModel>(new ConstructorArgument("cashQRCode", c)));
            #endregion

            #region OtherBindings
            Bind<ImageHelper>().ToSelf().InSingletonScope();
            Bind<Func<Sign, Task>>().ToMethod(context => Kernel.Get<UserAccountFNSViewModel>().Update)
                .WhenInjectedInto<SignInViewModel>()
                .InSingletonScope();
            Bind<List<BarcodeFormat>>().ToConstant(barcodeFormats).InSingletonScope();
            Bind<IScannerHelper>().To<ScannerHelper>().InSingletonScope();
            Bind<IPlayer>().To<AudioPlayer>().InSingletonScope();
            #endregion

            SetUpAsyncDataBase();
            Task.Run(() => SetUpDependenciesFromAsyncDatabase().ConfigureAwait(false)).Wait();
        }

        private readonly List<BarcodeFormat> barcodeFormats = new List<BarcodeFormat>()
        {
            BarcodeFormat.QR_CODE,
            //TODO: Какие обрабатывать форматы?
            //BarcodeFormat.AZTEC,
            //BarcodeFormat.DATA_MATRIX
        };

        private const string ASYNC_DATABASE_NAME = "asyncObjects.db";
        private IAsyncDatabase asyncDatabase;

        //private const string DATABASE_NAME = "objects.db";
        //private IDatabase database;
        //private void setUpDataBase()
        //{
        //    var pathToBD = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DATABASE_NAME);
        //    database = new SQLiteDataBase(pathToBD);
        //    App.Container.Bind<IDatabase>().ToConstant(database).InSingletonScope();
        //}

        private void SetUpAsyncDataBase()
        {
            var pathToAsyncBD = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), ASYNC_DATABASE_NAME);
            asyncDatabase = new SQLiteAsyncDataBase(pathToAsyncBD);
            App.Container.Bind<IAsyncDatabase>().ToConstant(asyncDatabase).InSingletonScope();
        }

        public async Task SetUpDependenciesFromAsyncDatabase()
        {
            await SetUpSignDependence();
            await SetUpSettingsDependence();
        }

        private async Task SetUpSignDependence()
        {
            await asyncDatabase.CreateTableAsync<Sign>();
            var sign = await asyncDatabase.GetItemAsync<Sign>();

            if (sign != null)
            {
                App.Container.Bind<Sign>()
                    .ToConstant(sign)
                    .WhenInjectedInto<UserAccountFNSViewModel>()
                    .InSingletonScope();
            }
        }

        private async Task SetUpSettingsDependence()
        {
            await asyncDatabase.CreateTableAsync<ScannerSettings>();
            var scannerSettings = await asyncDatabase.GetItemAsync<ScannerSettings>();

            if (scannerSettings != null)
            {
                scannerSettings.Deserialize();
                App.Container.Bind<ScannerSettings>()
                    .ToConstant(scannerSettings)
                    .WhenInjectedInto<ScannerSettingsViewModel>()
                    .InSingletonScope();
            }
        }
    }
}
