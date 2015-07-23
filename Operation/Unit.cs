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
        [DebuggerHidden]
        private void Catch(Exception ex)
        {
            this.Error = ex;
            while (ex.InnerException != null) ex = ex.InnerException;
            this.Succeeded = false;
            this.Message = ex.Message;
        } 
    }

}
