using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    /// <summary>
    /// Represents a piece of Computation that has no Return Value
    /// </summary>
    public partial class Operation
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Exception Error { get; set; }
    }

    /// <summary>
    /// Represents a piece of Computation that has a return Value
    /// </summary>
    /// <typeparam name="T">Return Value Type</typeparam>
    public partial class Operation<T> : Operation
    {
        public T Result { get; set; }
    }
}
