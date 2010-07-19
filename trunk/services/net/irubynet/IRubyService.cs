using System;
using System.Collections.Generic;
using System.Text;

namespace Nohros.Ruby
{
    /// <summary>
    /// Defines a way to execute code from a .NET assembly library.
    /// </summary>
    public interface IRubyService
    {
        /// <summary>
        /// Gets the name of the service.
        /// </summary>
        string Name { get ; }

        /// <summary>
        /// Starts the service.
        /// </summary>
        void Run();
    }
}
