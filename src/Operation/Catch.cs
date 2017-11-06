using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace System
{
    public partial class Operation<T>
    {
        [DebuggerHidden]
        internal void Catch(Exception ex)
        {
            _exception = ex;
            while (ex.InnerException != null) ex = ex.InnerException;
            Succeeded = false;
            Message = ex.Message;
        }
    }

    public static partial class OperationExtensions
    {
        public static Operation<T> Catch<T>(this Operation<T> operation, Action<Operation<T>> func)
        {
            if (!operation.Succeeded)
            {
                func(operation);
            }
            return operation;
        }
    }
}
