using Ninject;
using Ninject.Modules;
using VerificationCheck.Core;

namespace VerificationCheck
{
    public class VerificationCheckModule : NinjectModule
    {
        public static StandardKernel Container { get; set; }

        public override void Load()
        {
            Bind<FNS>().ToSelf().InSingletonScope();
        }
    }
}
