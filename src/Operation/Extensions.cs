using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
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

        /// <summary>
        /// Unwraps the Operation if Successful and Throws an Exception if an Operation Fails
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="message"></param>
        [DebuggerHidden]
        public static void Throw(this Operation operation, string message = null)
        {
            message = message ?? operation.Message;
            if (operation.Succeeded == false)
                throw (message == null)
                    ? operation.GetException()
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


        /// <summary>
        /// Unwraps the Operation if Successful and Throws an Exception if an Operation Fails
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="message"></param>
        [DebuggerHidden]
        public static void Unwrap(this Operation operation, string message = null)
        {
            message = message ?? operation.Message;
            if (operation.Succeeded == false)
                throw (message == null)
                    ? operation.GetException()
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
        public static T Unwrap<T>(this Operation<T> operation, string message = null)
        {
            message = message ?? operation.Message;
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
        public static T Unwrap<T>(this Operation<T> operation, Func<string, string> messageExp)
        {
            var message = messageExp(operation.Message);
            return Throw(operation, message);
        }



        public static Operation<T> Fold<T>(this IEnumerable<Operation<T>> operations, Func<T, T, T> accumulate)
        {
            using (IEnumerator<Operation<T>> e = operations.GetEnumerator())
            {

                //Make Sure Sequence isn't empty
                if (!e.MoveNext())
                {
                    var ex = new InvalidOperationException("Sequence contains no Elements");
                    return new Operation<T>(ex)
                    {
                        Message = ex.Message,
                    };
                }

                List<Operation<T>> badOperations = new List<Operation<T>>();

                //Process First Element
                var result = default(T);

                if (e.Current.Succeeded)
                {
                    result = e.Current.Result;
                }
                else
                {
                    badOperations.Add(e.Current);
                }

                //Process the Rest
                while (e.MoveNext()) 
                {
                    if (e.Current.Succeeded)
                    {
                        result = accumulate(result, e.Current.Result);
                    }
                    else
                    {
                        badOperations.Add(e.Current);
                    }
                }


                //If there are any bad operations
                if (badOperations.Any())
                {
                    string messages = null;
                    for (int i = 0; i < badOperations.Count; i++)
                    {
                        //Get Innermost Exception
                        var badOperation = badOperations[i];

                        //If this this the First Exception then Assign it Directly
                        if (i == 0)
                        {
                            messages = badOperation.Message;
                        }
                        else
                        {
                            //Othewise Concat the Message
                            messages = messages + ", " + badOperation.Message;
                        }
                    }

                    //Aggregate Exceptions
                    var aggregate = new AggregateException(badOperations.Select(o => o.GetException()));

                    return new Operation<T>(aggregate)
                    {
                        Message = messages,
                        Succeeded = false
                    };
                }
                else
                {
                    return new Operation<T>()
                    {
                        Succeeded = true,
                        Result = result
                    };
                }
            }
        }


        //public static Operation Fold(this IEnumerable<Operation> operations)
        //{
        //    using (var e = operations.GetEnumerator())
        //    {
        //        if (!e.MoveNext())
        //        {
        //            var ex = new InvalidOperationException("Sequence contains no Elements");
        //            return new Operation(ex)
        //            {
        //                Message = ex.Message,
        //            };
        //        }

        //        List<Operation> badOperations = new List<Operation>();
        //        do
        //        {
        //            //If the Current Operation did not succeed, add it
        //            if (!e.Current.Succeeded)
        //            {
        //                badOperations.Add(e.Current);
        //            };
        //        }
        //        while (e.MoveNext());


        //        //If there were any bad operations
        //        if (badOperations.Any())
        //        {
        //            return new Operation
        //            {
        //                Message = badOperations.Select(o => o.Message).Aggregate((ag, m) => ag + ", " + m),
        //                Succeeded = false
        //            };
        //        }
        //        else
        //        {
        //            return new Operation()
        //            {
        //                Succeeded = true,
        //            };
        //        }
        //    }
        //}
    }
}
