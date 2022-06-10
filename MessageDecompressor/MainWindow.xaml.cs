
using System.Reactive.Disposables;
using MessageDecompressorWpf.ViewModels;
using ReactiveUI;

namespace MessageDecompressorWpf
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

                this.Bind(
                        ViewModel,
                        viewModel => viewModel.ErrorText,
                        view => view.tb_errors.Text
                        )
                    .DisposeWith(disposableRegistration);

                this.OneWayBind(
                        ViewModel,
                        viewModel => viewModel.SearchResults,
                        view => view.searchResultsListBox.ItemsSource
                        )
                    .DisposeWith(disposableRegistration);

                this.OneWayBind(
                        ViewModel,
                        viewModel => viewModel.ErrorsVisible,
                        view => view.border_errors.Visibility
                        )
                    .DisposeWith(disposableRegistration);
            });
        }
    }
}
