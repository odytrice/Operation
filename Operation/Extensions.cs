using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static partial class Extensions
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
                if (operation.Success == false)
                    throw operation.Error;
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
                if (operation.Success == false)
                    throw operation.Error;
                return operation.Result;
            });
        }

        /// <summary>
        /// Unwraps the Operation if Successful and Throws an Exception if an Operation Fails
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="message"></param>
        [DebuggerHidden]
        public static void Throw(this Operation operation, string message = null)
        {
            message = message ?? operation.Message;
            if (operation.Success == false)
                throw (message == null)
                    ? operation.Error
                    : new Exception(message);
        }
        /// <summary>
        /// Unwraps the Operation if Successful and Throws an Exception if an Operation Fails
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="operation"></param>
        /// <param name="message">Optional Message to Mask Original Error</param>
        /// <returns></returns>
        [DebuggerHidden]
        public static T Throw<T>(this Operation<T> operation, string message = null)
        {
            message = message ?? operation.Message;
            if (operation.Success == false)
                throw (message == null)
                    ? operation.Error
                    : new Exception(message, operation.Error);
            return operation.Result;
        }

        /// <summary>
        /// Unwraps the Operation if Successful and Throws an Exception if an Operation Fails
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="messageExp"></param>
        [DebuggerHidden]
        public static void Throw(this Operation operation, Func<string, string> messageExp)
        {
            var message = messageExp(operation.Message);
            Throw(operation, message);
        }

        /// <summary>
        /// Unwraps the Operation if Successful and Throws an Exception if an Operation Fails
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="operation"></param>
        /// <param name="messageExp"></param>
        /// <returns></returns>
        [DebuggerHidden]
        public static T Throw<T>(this Operation<T> operation, Func<string, string> messageExp)
        {
            var message = messageExp(operation.Message);
            return Throw(operation, message);
        }
    }
}
