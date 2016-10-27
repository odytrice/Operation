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
        public static IEnumerable<V> SelectMany<T, U, V>(this Operation<T> operation, Func<T, IEnumerable<U>> process, Func<T, U, V> projection)
        {
            if (operation.Succeeded)
            {
                var op2 = process(operation.Result);
                return op2.Select(x => projection(operation.Result, x));
            }
            return new V[0];
        }

        [DebuggerHidden]
        public static IQueryable<V> SelectMany<T, U, V>(this Operation<T> operation, Func<T, IQueryable<U>> process, Func<T, U, V> projection)
        {
            if (operation.Succeeded)
            {
                var op2 = process(operation.Result);
                return op2.Select(x => projection(operation.Result, x));
            }
            return new V[0].AsQueryable();
        }


        [DebuggerHidden]
        public static IEnumerable<Operation<V>> SelectMany<T, U, V>(this IEnumerable<T> operation, Func<T, Operation<U>> process, Func<T, U, V> projection)
        {
            return operation.Select(x => OperationExtensions.Next(process(x), u => projection(x, u)));
        }

        [DebuggerHidden]
        public static Operation<U> Select<T, U>(this Operation<T> operation, Func<T, U> process)
        {
            return OperationExtensions.Next(operation, process);
        }
    }
}
