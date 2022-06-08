using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MessageDecompressor.Library;
using ReactiveUI;

namespace MessageDecompressor.ViewModels
{
    public class MainAppViewModel : ReactiveObject
    {
        private string _connectionString;
        private string _queueName;
        private CancellationTokenSource _activeToken = null;

        public string ConnectionString
        {
            get => _connectionString;
            set => this.RaiseAndSetIfChanged(ref _connectionString, value);
        }

        public string QueueName
        {
            get => _queueName;
            set => this.RaiseAndSetIfChanged(ref _queueName, value);
        }

        private readonly ObservableAsPropertyHelper<string> _searchResults;

        public string SearchResults => _searchResults.Value;

        public MainAppViewModel()
        {
            var queueObs = this.ObservableForProperty(vm => vm.QueueName)
                .Select(v => v.Value)
                .Where(v => !string.IsNullOrEmpty(v));

            var connObs = this.ObservableForProperty(vm => vm.ConnectionString)
                .Select(v => v.Value)
                .Where(v => !string.IsNullOrEmpty(v));

            _searchResults = queueObs.CombineLatest(connObs, (queueName, connString) => (queueName: queueName, connectionString: connString))
                .Throttle(TimeSpan.FromMilliseconds(800))
                .Where(v => !string.IsNullOrWhiteSpace(v.queueName) && !string.IsNullOrWhiteSpace(v.connectionString))
                .SelectMany(v => GetMessagesAsync(v.connectionString, v.queueName))
                .Select(MessagesAsString)
                .Catch(Observable.Return<string>("(error)"))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Do(x => Debug.WriteLine(x))
                .ToProperty(this, x => x.SearchResults);
        }

        private async Task<IEnumerable<DecompressedItem>> GetMessagesAsync(string connectionString, string queueName)
        {
            if (_activeToken != null && !_activeToken.IsCancellationRequested)
            {
                _activeToken.Cancel();
            }

            _activeToken = new CancellationTokenSource();

            var messages = await MessageDecompressorLib.GetDecompressedMessages(connectionString, queueName, _activeToken.Token).ConfigureAwait(false);

            return messages;
        }

        private string MessagesAsString(IEnumerable<DecompressedItem> messages)
        {
            var sb = new StringBuilder();

            foreach (var message in messages)
            {
                sb.Append($"Message ID = {message.messageId}\n\n{message.messageJson}\n\n");
            }

            return sb.ToString();
        }
    }
}
