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
    }

    /// <summary>
    /// Extensions
    /// </summary>
    public static partial class OperationExtensions
    {
        /// <summary>
        /// Converts an Operation to a Task
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        [DebuggerHidden]
        public static Task AsTask(this Operation operation)
        {
            return Task.Factory.StartNew(() =>
            {
                if (operation.Succeeded == false)
                    throw operation.GetException();
            });
        }
        /// <summary>
        /// Converts an Operation to a Task
        /// </summary>
        /// <typeparam name="T">Return Type of Operation</typeparam>
        /// <param name="operation"></param>
        /// <returns></returns>
        [DebuggerHidden]
        public static Task<T> AsTask<T>(this Operation<T> operation)
        {
            return Task<T>.Factory.StartNew(() =>
            {
                if (operation.Succeeded == false)
                    throw operation.GetException();
                return operation.Result;
            });
        }
    }
}
