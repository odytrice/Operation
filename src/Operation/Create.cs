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
        /// Creates a Successful Operation
        /// </summary>
        /// <returns></returns>
        [DebuggerHidden]
        public static Operation Success() => Success<object>(null);

        /// <summary>
        /// Creates a Successful Operation
        /// </summary>
        /// <typeparam name="T">Result Type</typeparam>
        /// <param name="result">Result</param>
        /// <returns></returns>
        [DebuggerHidden]
        public static Operation<T> Success<T>(T result)
        {
            return new Operation<T>
            {
                Result = result,
                Succeeded = true
            };
        }

        /// <summary>
        /// Creates a Failed Operation
        /// </summary>
        /// <param name="message">Error Message</param>
        /// <returns></returns>
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
}
