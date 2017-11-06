using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace System
{
    public static partial class OperationExtensions
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
                    return new Operation<V>(op2.GetException())
                    {
                        Succeeded = false,
                        Message = op2.Message,
                        Result = default(V)
                    };
                }
            }
            return new Operation<V>(operation.GetException())
            {
                Succeeded = false,
                Result = default(V),
                Message = operation.Message,
            };
        }

        [DebuggerHidden]
        public static Operation<U> Select<T, U>(this Operation<T> operation, Func<T, U> process)
        {
            return Map(operation, process);
        }

        // Linq with IEnumerable
        [DebuggerHidden]
        public static Operation<IEnumerable<V>> SelectMany<T, U, V>(this Operation<T> operation, Func<T, IEnumerable<U>> process, Func<T, U, V> projection)
        {
            if (operation.Succeeded)
            {
                var op2 = Operation.Create(() => process(operation.Result));
                return op2.Next((enumerable) => enumerable.Select(x => projection(operation.Result, x)));
            }
            return new Operation<IEnumerable<V>>(operation.GetException())
            {
                Succeeded = false,
                Result = default(IEnumerable<V>),
                Message = operation.Message
            };
        }

        [DebuggerHidden]
        public static IEnumerable<Operation<V>> SelectMany<T, U, V>(this IEnumerable<T> items, Func<T, Operation<U>> process, Func<T, U, V> projection)
        {
            return items.Select(x => Map(process(x), u => projection(x, u)));
        }
    }
}
