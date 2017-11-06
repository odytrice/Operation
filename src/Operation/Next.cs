using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static partial class OperationExtensions
    {
        #region Regular Bind
        [DebuggerHidden]
        public static Operation Next(this Operation operation, Action process)
        {
            return operation.Map(process);
        }
        [DebuggerHidden]
        public static Operation<T> Next<T>(this Operation operation, Func<T> process)
        {
            return operation.Map(process);
        }
        [DebuggerHidden]
        public static Operation Next<T>(this Operation<T> operation, Action<T> process)
        {
            return operation.Map(process);
        }
        [DebuggerHidden]
        public static Operation<U> Next<T, U>(this Operation<T> operation, Func<T, U> process)
        {
            return operation.Map(process);
        }
        #endregion

        #region Criminal Bind
        [DebuggerHidden]
        public static Operation Next(this Operation operation, Func<Operation> process)
        {
            return operation.Bind(process);
        }

        [DebuggerHidden]
        public static Operation<T> Next<T>(this Operation operation, Func<Operation<T>> process)
        {
            return operation.Bind(process);
        }

        [DebuggerHidden]
        public static Operation Next<T>(this Operation<T> operation, Func<T, Operation> process)
        {
            return operation.Bind(process);
        }

        [DebuggerHidden]
        public static Operation<U> Next<T, U>(this Operation<T> operation, Func<T, Operation<U>> process)
        {
            return operation.Bind(process);
        }
        #endregion
    }
}
