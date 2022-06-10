using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ReactiveUI;

namespace MessageDecompressorWpf
{
    /// <summary>
    /// Interaction logic for DecompressedMessageView.xaml
    /// </summary>
    public partial class DecompressedMessageView
    {
        public DecompressedMessageView()
        {
            InitializeComponent();

            this.WhenActivated(disposableRegistration =>
            {
                this.OneWayBind(
                    ViewModel,
                    viewModel => viewModel.Id,
                    view => view.tb_messageId.Text
                    )
                .DisposeWith(disposableRegistration);

                this.OneWayBind(
                    ViewModel,
                    viewModel => viewModel.Json,
                    view => view.tb_json.Text
                    )
                .DisposeWith(disposableRegistration);
            });
        }
    }
}
