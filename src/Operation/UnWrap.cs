using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace System
{
    public static partial class OperationExtensions
    {
        #region Unwrap
        /// <summary>
        /// Unwraps the Operation if Successful and Throws an Exception if an Operation Fails
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="operation"></param>
        /// <param name="message">Optional Message to Mask Original Error</param>
        /// <returns></returns>
        [DebuggerHidden]
        public static T Unwrap<T>(this Operation<T> operation, string message = null)
        {
            if (operation.Succeeded == false)
                throw (message == null)
                    ? operation.GetException()
                    : new Exception(message, operation.GetException());
            return operation.Result;
        }

        /// <summary>
        /// Unwraps the Operation if Successful and Throws an Exception if an Operation Fails
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="messageExp"></param>
        [DebuggerHidden]
        public static void Unwrap(this Operation operation, Func<string, string> messageExp)
        {
            var message = messageExp(operation.Message);
            Unwrap(operation, message);
        }

        /// <summary>
        /// Unwraps the Operation if Successful and Throws an Exception if an Operation Fails
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="operation"></param>
        /// <param name="messageExp"></param>
        /// <returns></returns>
        [DebuggerHidden]
        public static T Unwrap<T>(this Operation<T> operation, Func<string, string> messageExp)
        {
            var message = messageExp(operation.Message);
            return Unwrap(operation, message);
        }

        #endregion

        #region Throw

        /// <summary>
        /// Unwraps the Operation if Successful and Throws an Exception if an Operation Fails
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="message"></param>
        [DebuggerHidden]
        public static void Throw(this Operation operation, string message = null)
        {
            Unwrap(operation, message);
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
            return Unwrap(operation, message);
        }

        /// <summary>
        /// Unwraps the Operation if Successful and Throws an Exception if an Operation Fails
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="messageExp"></param>
        [DebuggerHidden]
        public static void Throw(this Operation operation, Func<string, string> messageExp)
        {
            Unwrap(operation, messageExp);
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
            return Unwrap(operation, messageExp);
        }
        #endregion
    }
}
