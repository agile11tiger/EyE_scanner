using Ninject;
using Scanner.Views.Authorization;
using Scanner.Views.Scanner;
using Scanner.Views.Scanner.Checks;
using Scanner.Views.Scanner.Friends;
using Scanner.Views.Scanner.QRCodes;
using System;

namespace Scanner
{
    public static class Pages
    {
        public static void Initialize()
        {

        }

        public static readonly Lazy<AuthorizationPage> AuthorizationPage = App.Container.Get<Lazy<AuthorizationPage>>();
        public static readonly SignInPage SignInPage = App.Container.Get<SignInPage>();
        public static readonly SignUpPage SignUpPage = App.Container.Get<SignUpPage>();
        public static readonly Lazy<ForgotPasswordPage> ForgotPasswordPage = App.Container.Get<Lazy<ForgotPasswordPage>>();
        public static readonly ScannerPage ScannerPage = App.Container.Get<ScannerPage>();
        public static readonly FriendsPage FriendsPage = App.Container.Get<FriendsPage>();
        public static readonly WaitingChecksPage WaitingChecksPage = App.Container.Get<WaitingChecksPage>();
        public static readonly ChecksTabbedPage ChecksTabbedPage = App.Container.Get<ChecksTabbedPage>();
        public static readonly ScannerSettingsPage ScannerSettingsPage = App.Container.Get<ScannerSettingsPage>();
        public static readonly CodeGenerationPage CodeGenerationPage = App.Container.Get<CodeGenerationPage>();

        public static ManualScanPage GetManualScanPage() => App.Container.Get<ManualScanPage>();
    }
}
