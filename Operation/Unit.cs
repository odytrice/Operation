using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public partial class Operation
    {
        public static Operation Create(Action process)
        {
            var operation = new Operation();
            try
            {
                process();
                operation.Success = true;
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) ex = ex.InnerException;
                operation.Success = false;
                operation.Message = ex.Message;
            }
            return operation;
        }

        public static Operation<T> Create<T>(Func<T> process)
        {
            var operation = new Operation<T>();
            try
            {
                operation.Result = process();
                operation.Success = true;
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) ex = ex.InnerException;
                operation.Success = false;
                operation.Message = ex.Message;
            }
            return operation;
        }

        public static async Task<Operation> Run(Func<Task> process)
        {
            var operation = new Operation();
            try
            {
                await process();
                operation.Success = true;
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) ex = ex.InnerException;
                operation.Success = false;
                operation.Message = ex.Message;
            }
            return operation;
        }

        public static async Task<Operation<T>> Run<T>(Func<Task<T>> process)
        {
            var operation = new Operation<T>();
            try
            {
                operation.Result = await process();
                operation.Success = true;
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null) ex = ex.InnerException;
                operation.Success = false;
                operation.Message = ex.Message;
            }
            return operation;
        }
    }

}
