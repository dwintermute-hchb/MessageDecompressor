using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MessageDecompressorWpf.Library;
using MessageDecompressorWpf.Wpf.Config;
using Microsoft.Extensions.Configuration;
using ReactiveUI;
using Splat;

namespace MessageDecompressorWpf.ViewModels
{
    public class MainAppViewModel : ReactiveObject
    {
        private string _connectionString = string.Empty;
        private string _queueName = string.Empty;
        private CancellationTokenSource _activeToken = null;
        private BehaviorSubject<string> _errorTextObs = new BehaviorSubject<string>(String.Empty);

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

        private readonly ObservableAsPropertyHelper<string> _errorText;
        public string ErrorText => _errorText.Value;

        private readonly ObservableAsPropertyHelper<IEnumerable<DecompressedMessageViewModel>> _searchResults;
        public IEnumerable<DecompressedMessageViewModel> SearchResults => _searchResults.Value;

        private readonly ObservableAsPropertyHelper<bool> _errorsVisible;
        public bool ErrorsVisible => _errorsVisible.Value;

        public MainAppViewModel()
        {
            var queueObs = this.ObservableForProperty(vm => vm.QueueName)
                .Select(v => v.Value)
                .Where(v => !string.IsNullOrEmpty(v));

            var connObs = this.ObservableForProperty(vm => vm.ConnectionString)
                .Select(v => v.Value)
                .Where(v => !string.IsNullOrEmpty(v));

            _searchResults = queueObs
                .CombineLatest(connObs, (queueName, connString) => (queueName: queueName, connectionString: connString))
                .Throttle(TimeSpan.FromMilliseconds(800))
                .Where(v => !string.IsNullOrWhiteSpace(v.queueName) && !string.IsNullOrWhiteSpace(v.connectionString))
                .SelectMany(v => GetMessagesAsync(v.connectionString, v.queueName))
                .Select(items =>
                    items.Select(b => new DecompressedMessageViewModel()
                    {
                        Id = b.messageId,
                        Json = b.messageJson,
                    }))
                .Catch(Observable.Return<IEnumerable<DecompressedMessageViewModel>>(new List<DecompressedMessageViewModel>()))
                .ObserveOn(RxApp.MainThreadScheduler)
                .ToProperty(this, x => x.SearchResults);

            _errorText = _errorTextObs
                .ObserveOn(RxApp.MainThreadScheduler)
                .ToProperty(this, x => x.ErrorText);

            _errorsVisible = this.ObservableForProperty(vm => vm.ErrorText)
                .Select(text => !string.IsNullOrWhiteSpace(text.Value))
                .ObserveOn(RxApp.MainThreadScheduler)
                .ToProperty(this, x => x.ErrorsVisible);

            var config = Splat.Locator.Current.GetService<IConfiguration>();

            var queueName = config.GetValue<string>(Config.ConfigKeys.QueueName);
            var connectionString = config.GetValue<string>(Config.ConfigKeys.ServiceBusConnectionString);

            if (queueName != null)
            {
                this.QueueName = queueName;
            }
            if (connectionString != null)
            {
                this.ConnectionString = connectionString;
            }

        }

        private async Task<IEnumerable<DecompressedItem>> GetMessagesAsync(string connectionString, string queueName)
        {
            this._errorTextObs.OnNext(string.Empty);

            if (_activeToken != null && !_activeToken.IsCancellationRequested)
            {
                _activeToken.Cancel();
            }

            _activeToken = new CancellationTokenSource();

            try
            {
                var messages = await MessageDecompressorLib.GetDecompressedMessages(connectionString, queueName, _activeToken.Token).ConfigureAwait(false);
                return messages;
            }
            catch (Exception ex)
            {
                this._errorTextObs.OnNext(StringifyException(ex));
                return new List<DecompressedItem>();
            }
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

        private string StringifyException(Exception ex) => $"{ex.Message}\n\n{ex.StackTrace}";
    }
}
