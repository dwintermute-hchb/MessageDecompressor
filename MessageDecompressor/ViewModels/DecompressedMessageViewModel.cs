using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;
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
    public class DecompressedMessageViewModel : ReactiveObject
    {
        private string _json;
        private string _id;

        public string Json
        {
            get => _json;
            set => this.RaiseAndSetIfChanged(ref _json, value);
        }

        public string Id
        {
            get => _id;
            set => this.RaiseAndSetIfChanged(ref _id, value);
        }
    }
}
