using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace System
{

    public static partial class OperationExtensions
    {
        public static OperationAwaiter<T> GetAwaiter<T>(this Operation<T> operation)
        {
            return new OperationAwaiter<T>(operation);
        }

        public static OperationAwaiter GetAwaiter(this Operation operation)
        {
            return new OperationAwaiter(operation);
        }

    }

    public class OperationAwaiter : INotifyCompletion
    {
        private Operation _operation;

        public OperationAwaiter(Operation operation)
        {
            _operation = operation;
        }
        public bool IsCompleted => true;

        public void OnCompleted(Action continuation)
        {
            continuation();
        }

        public void GetResult()
        {
            _operation.Unwrap();
        }
    }


    public class OperationAwaiter<T> : INotifyCompletion
    {
        private Operation<T> _operation;

        public OperationAwaiter(Operation<T> operation)
        {
            _operation = operation;
        }
        public bool IsCompleted => true;

        public void OnCompleted(Action continuation)
        {
            continuation();
        }

        public T GetResult()
        {
            return _operation.Unwrap();
        }
    }
}