using EyE.Droid.Dependancies;
using Ninject.Modules;
using Scanner.Services.Interfaces;

namespace EyE.Droid
{
    public class AndroidModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IZxingImageHelper>().To<ZxingImageHelper>().InSingletonScope();
        }
    }
}