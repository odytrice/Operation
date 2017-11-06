using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace System
{
    public partial class Operation
    {
        /// <summary>
        /// Creates an new Operation by Invoking the Delegate
        /// </summary>
        /// <typeparam name="T">Result Type</typeparam>
        /// <param name="process">Error Prone Method/Delegate</param>
        /// <returns>Operation of Result</returns>
        [DebuggerHidden]
        public static Operation CreateBind(Func<Operation> process)
        {
            try
            {
                return process();
            }
            catch (Exception ex)
            {
                var operation = new Operation();
                operation.Catch(ex);
                return operation;
            }
        }

        /// <summary>
        /// Creates an new Operation by Invoking the Delegate
        /// </summary>
        /// <typeparam name="T">Result Type</typeparam>
        /// <param name="process">Error Prone Method/Delegate</param>
        /// <returns>Operation of Result</returns>
        [DebuggerHidden]
        public static Operation<T> CreateBind<T>(Func<Operation<T>> process)
        {
            try
            {
                return process();
            }
            catch (Exception ex)
            {
                var operation = new Operation<T>();
                operation.Catch(ex);
                return operation;
            }
        }
    }

    public static partial class OperationExtensions
    {
        [DebuggerHidden]
        public static Operation Bind(this Operation operation, Func<Operation> process)
        {
            if (operation.Succeeded)
                return process();
            return operation;
        }

        [DebuggerHidden]
        public static Operation<T> Bind<T>(this Operation operation, Func<Operation<T>> process)
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
        public static Operation Bind<T>(this Operation<T> operation, Func<T, Operation> process)
        {
            if (operation.Succeeded)
                return process(operation.Result);
            return operation;
        }

        [DebuggerHidden]
        public static Operation<U> Bind<T, U>(this Operation<T> operation, Func<T, Operation<U>> process)
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
