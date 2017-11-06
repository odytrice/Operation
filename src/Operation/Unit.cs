using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public partial class Operation
    {
        /// <summary>
        /// Creates an new Operation by Invoking the Delegate
        /// </summary>
        /// <param name="process">Error Prone Method/Delegate</param>
        /// <returns>Operation</returns>
        [DebuggerHidden]
        public static Operation Create(Action process)
        {
            var operation = new Operation();
            try
            {
                process();
                operation.Succeeded = true;
            }
            catch (Exception ex)
            {
                operation.Catch(ex);
            }
            return operation;
        }

        /// <summary>
        /// Creates an new Operation by Invoking the Delegate
        /// </summary>
        /// <typeparam name="T">Result Type</typeparam>
        /// <param name="process">Error Prone Method/Delegate</param>
        /// <returns>Operation of Result</returns>
        [DebuggerHidden]
        public static Operation<T> Create<T>(Func<T> process)
        {
            var operation = new Operation<T>();
            try
            {
                operation.Result = process();
                operation.Succeeded = true;
            }
            catch (Exception ex)
            {
                operation.Catch(ex);
            }
            return operation;
        }

        /// <summary>
        /// Creates an new Operation by Invoking an Async Delegate
        /// </summary>
        /// <param name="process">Error Prone Async Method/Delegate</param>
        /// <returns>Task of Operation</returns>
        [DebuggerHidden]
        public static async Task<Operation> Run(Func<Task> process)
        {
            var operation = new Operation();
            try
            {
                await process();
                operation.Succeeded = true;
            }
            catch (Exception ex)
            {
                operation.Catch(ex);
            }
            return operation;
        }

        /// <summary>
        /// Creates a new Operation by Invoking an Async Delegate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="process"></param>
        /// <returns></returns>
        [DebuggerHidden]
        public static async Task<Operation<T>> Run<T>(Func<Task<T>> process)
        {
            var operation = new Operation<T>();
            try
            {
                operation.Result = await process();
                operation.Succeeded = true;
            }
            catch (Exception ex)
            {
                operation.Catch(ex);
            }
            return operation;
        }

        public static Operation Success() => Success<object>(null);

        public static Operation<T> Success<T>(T result)
        {
            return new Operation<T>
            {
                Result = result,
                Succeeded = true
            };
        }

        public static Operation Fail(string message) => Fail<object>(message);

        public static Operation<T> Fail<T>(string message)
        {
            var exception = new Exception(message);

            return new Operation<T>(exception)
            {
                Message = message,
                Result = default(T),
                Succeeded = false,
            };
        }
    }

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


        /// <summary>
        /// Implicit Downgrade of 'T Operation to object Operation
        /// </summary>
        /// <param name="operation"></param>
        public static implicit operator Operation(Operation<T> operation)
        {
            return new Operation(operation.GetException())
            {
                Message = operation.Message,
                Result = operation.Result,
                Succeeded = operation.Succeeded
            };
        }
    }

}
