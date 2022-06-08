using System;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Azure.Messaging.ServiceBus;
using MessageDecompressor.ViewModels;
using MessagePack;
using MessagePack.ImmutableCollection;
using MessagePack.Resolvers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReactiveUI;

namespace MessageDecompressor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            ViewModel = new MainAppViewModel();

            this.WhenActivated(disposableRegistration =>
            {
                this.Bind(
                        ViewModel,
                        viewModel => viewModel.ConnectionString,
                        view => view.tb_connectionString.Text
                        )
                    .DisposeWith(disposableRegistration);

                this.Bind(
                        ViewModel,
                        viewModel => viewModel.QueueName,
                        view => view.tb_queueName.Text
                        )
                    .DisposeWith(disposableRegistration);

                this.OneWayBind(
                    ViewModel,
                    viewModel => viewModel.SearchResults,
                    view => view.tb_decompressed
                    )
                .DisposeWith(disposableRegistration);
            });
        }
    }
}
