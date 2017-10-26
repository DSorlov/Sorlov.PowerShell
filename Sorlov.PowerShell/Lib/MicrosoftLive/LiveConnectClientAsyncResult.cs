using System;
using System.Threading;

namespace Mjollnir
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
    class LiveConnectClientAsyncResult<T> : IAsyncResult
    {
        public LiveConnectClientAsyncResult(object state, SynchronizationContext syncContext)
        {
            this.AsyncState = state;
            this.SynchronizationContext = syncContext;
            this.CompletedSynchronously = true;
        }

        object syncRoot = new object();

        bool isCancellationRequested;

        public object AsyncState { get; private set; }

        public SynchronizationContext SynchronizationContext { get; private set; }

        public T Result { get; set; }

        public bool CompletedSynchronously { get; internal set; }

        bool _IsCompleted;

        public bool IsCompleted
        {
            get
            {
                lock (this.syncRoot)
                {
                    return this._IsCompleted;
                }
            }
            internal set
            {
                lock (this.syncRoot)
                {
                    this._IsCompleted = value;

                    if (value)
                    {
                        if (this._AsyncWaitHandle != null)
                        {
                            this._AsyncWaitHandle.Set();
                        }
                    }
                }
            }
        }

        ManualResetEvent _AsyncWaitHandle;

        public WaitHandle AsyncWaitHandle
        {
            get
            {
                lock (this.syncRoot)
                {
                    if (this._AsyncWaitHandle == null)
                    {
                        this._AsyncWaitHandle = new ManualResetEvent(this._IsCompleted);
                    }
                }

                return this._AsyncWaitHandle;
            }
        }

        public void Sync(Action action)
        {
            ThrowArgumentException.IfNull(action, "action");

            if (this.SynchronizationContext == null)
            {
                try
                {
                    action();
                }
                catch (Exception x)
                {
                    System.Diagnostics.Debug.WriteLine("LiveConnectClientAsyncResult<T>.Sync(): " + x);
                }
            }
            else
            {
                this.SynchronizationContext.Post(userState => action(), this.AsyncState);
            }
        }

        public void Cancel()
        {
            this.isCancellationRequested = true;
        }

        public void ThrowIfCancellationRequested()
        {
            if (this.isCancellationRequested)
            {
                throw new OperationCanceledException();
            }
        }
    }
}
