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
    public partial class Operation: Operation<object>
    {

        public Operation()
        {

        }

        public Operation(Exception ex): base(ex)
        {

        }
    }

    /// <summary>
    /// Represents a piece of Computation that has a return Value
    /// </summary>
    /// <typeparam name="T">Return Value Type</typeparam>
    public partial class Operation<T>
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public T Result { get; set; }

        protected Exception _exception;
        public Exception GetException() => _exception;

        public Operation()
        {

        }

        public Operation(Exception ex)
        {
            _exception = ex;
        }
    }
}
