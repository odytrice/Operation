using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static partial class Extensions
    {
        public static Operation Next(this Operation operation, Func<Operation> process)
        {
            if (operation.Success)
                return process();
            return operation;
        }

        public static Operation<T> Next<T>(this Operation operation, Func<Operation<T>> process)
        {
            if (operation.Success)
                return process();
            return new Operation<T>
            {
                Success = false,
                Result = default(T),
                Message = operation.Message
            };
        }

        public static Operation Next<T>(this Operation<T> operation, Func<T,Operation> process)
        {
            if (operation.Success)
                return process(operation.Result);
            return new Operation
            {
                Message = operation.Message,
                Success = operation.Success
            };
        }

        public static Operation<U> Next<T, U>(this Operation<T> operation, Func<T,Operation<U>> process)
        {
            if (operation.Success)
                return process(operation.Result);
            return new Operation<U>
            {
                Success = false,
                Result = default(U),
                Message = operation.Message
            };
        }
    }
}
