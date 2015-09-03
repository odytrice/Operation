using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class LinqSugar
    {
        [DebuggerHidden]
        public static Operation<V> SelectMany<T, U, V>(this Operation<T> operation, Func<T, Operation<U>> process, Func<T, U, V> projection)
        {
            if (operation.Succeeded)
            {
                var op2 = process(operation.Result);
                if (op2.Succeeded)
                {
                    return Operation.Create(() => projection(operation.Result, op2.Result));
                }
                else
                {
                    return new Operation<V>
                    {
                        Succeeded = false,
                        Error = op2.Error,
                        Message = op2.Message,
                        Result = default(V)
                    };
                }
            }
            return new Operation<V>
            {
                Succeeded = false,
                Result = default(V),
                Message = operation.Message,
                Error = operation.Error
            };
        }
    }
}
