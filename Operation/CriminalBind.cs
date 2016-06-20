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
        [DebuggerHidden]
        public static Operation Next(this Operation operation, Func<Operation> process)
        {
            if (operation.Succeeded)
                return process();
            return operation;
        }

        [DebuggerHidden]
        public static Operation<T> Next<T>(this Operation operation, Func<Operation<T>> process)
        {
            if (operation.Succeeded)
                return process();
            return new Operation<T>(operation.GetException())
            {
                Succeeded = false,
                Result = default(T),
                Message = operation.Message
            };
        }

        [DebuggerHidden]
        public static Operation Next<T>(this Operation<T> operation, Func<T,Operation> process)
        {
            if (operation.Succeeded)
                return process(operation.Result);
            return operation;
        }

        [DebuggerHidden]
        public static Operation<U> Next<T, U>(this Operation<T> operation, Func<T,Operation<U>> process)
        {
            if (operation.Succeeded)
                return process(operation.Result);
            return new Operation<U>(operation.GetException())
            {
                Succeeded = false,
                Result = default(U),
                Message = operation.Message
            };
        }
    }
}
