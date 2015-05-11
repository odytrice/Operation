using System;
using System.Collections.Generic;
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
        public static Task AsTask(this Operation operation)
        {
            return Task.Factory.StartNew(() =>
            {
                if (operation.Success == false)
                    throw new Exception(operation.Message);
            });
        }
        /// <summary>
        /// Converts an Operation to a Task
        /// </summary>
        /// <typeparam name="T">Return Type of Operation</typeparam>
        /// <param name="operation"></param>
        /// <returns></returns>
        public static Task<T> AsTask<T>(this Operation<T> operation)
        {
            return Task<T>.Factory.StartNew(() =>
            {
                if (operation.Success == false)
                    throw new Exception(operation.Message);
                return operation.Result;
            });
        }

        /// <summary>
        /// Throws an Exception if an Operation Fails
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Operation Throw(this Operation operation, string message = null)
        {
            message = message ?? operation.Message;
            if (operation.Success == false)
                throw new Exception(message);
            return operation;
        }

        public static Operation<T> Throw<T>(this Operation<T> operation, string message = null)
        {
            message = message ?? operation.Message;
            if (operation.Success == false)
                throw new Exception(message);
            return operation;
        }

        public static Operation Throw(this Operation operation, Func<string, string> messageExp)
        {
            var message = messageExp(operation.Message);
            return Throw(operation,message);
        }

        public static Operation<T> Throw<T>(this Operation<T> operation, Func<string,string> messageExp)
        {
            var message = messageExp(operation.Message);
            return Throw(operation, message);
        }
    }
}
