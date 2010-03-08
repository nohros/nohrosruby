using System;
using System.Collections.Generic;
using System.Text;

namespace Nohros.Ruby
{
    internal sealed class Const
    {
        /* The default buffer length */
        public const int RUBY_BUFFER_SIZE = 4096;

        /* The name of the logger used by the log4net */
        public const string RUBY_LOGGER_NAME = "RubyLogger";
    }
}
