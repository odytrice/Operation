using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static partial class OperationExtensions
    {
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
    }
}
