// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.TestFx.STAExtensions
{
    using Microsoft.TestFx.STAExtensions.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    internal class STAThreadManager<T> : IThreadManager<T>
    {
        private Thread staThread;

        private AutoResetEvent actionAvailableWaithHandle = new AutoResetEvent(false);
        private AutoResetEvent runCompletedAvailableWaithHandle = new AutoResetEvent(false);

        private Func<T> functionToExecuteOnThread;

        private TaskCompletionSource<T> taskCompletionSource;

        private object lockObject = new object();

        public T Execute(Func<T> functionToExecuteOnThread)
        {
            lock (lockObject)
            {
                // Ensure thread initialized
                EnsureThreadInitialized();

                // Initialize Thread-specific vars
                this.taskCompletionSource = new TaskCompletionSource<T>();
                this.functionToExecuteOnThread = functionToExecuteOnThread;

                // Send signal to sta thread to execute above function
                this.actionAvailableWaithHandle.Set();

                // Wait for result
                var task = this.taskCompletionSource.Task;
                try
                {
                    task.Wait();
                    return task.Result;
                }
                catch(AggregateException ex)
                {
                    if (ex.InnerException != null)
                    {
                        throw ex.InnerException;
                    }
                    else throw;
                }
            }
        }

        ~STAThreadManager()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            if (staThread != null && staThread.IsAlive)
            {
                this.runCompletedAvailableWaithHandle.Set();
                // TODO: Better way to cleanup
                //if(!staThread.Join(10))
                //{
                //    staThread.Abort();
                //    staThread.Join();
                //}
                this.staThread = null;
            }
        }

        private void EnsureThreadInitialized()
        {
            if (this.staThread == null)
            {
                this.staThread = new Thread(ThreadLoop);
                this.staThread.IsBackground = true;
                this.staThread.SetApartmentState(ApartmentState.STA);
                this.staThread.Name = "testfxSTAExThread";
                this.staThread.Start();
            }
        }

        private void ThreadLoop()
        {
            var waitForActions = true;
            while (waitForActions)
            {
                var waitResult = WaitHandle.WaitAny(new WaitHandle[2] { actionAvailableWaithHandle, runCompletedAvailableWaithHandle });
                if (waitResult == 0)
                {
                    if (functionToExecuteOnThread != null)
                    {
                        try
                        {
                            this.taskCompletionSource?.SetResult(functionToExecuteOnThread.Invoke());
                        }
                        catch (Exception ex)
                        {
                            this.taskCompletionSource?.SetException(ex);
                        }
                    }
                }
                else
                {
                    waitForActions = false;
                }
            }
        }
    }
}
