using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using System.Reflection;
using System.Windows;

namespace MessageDecompressorWpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Splat.Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetCallingAssembly());

            this.InitDependencyInjection();
        }

        private void InitDependencyInjection()
        {
            var config = new ConfigurationBuilder()
                .AddUserSecrets<App>()
                .Build();

            Splat.Locator.CurrentMutable.Register(() => config, typeof(IConfiguration));
        }
    }
}
