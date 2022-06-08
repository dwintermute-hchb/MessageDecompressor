using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageDecompressor.Library
{
    public readonly record struct DecompressedItem(string messageId, string messageJson);
}
