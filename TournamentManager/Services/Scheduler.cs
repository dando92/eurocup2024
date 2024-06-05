using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using TournamentManager.Contexts;
using TournamentManager.DbModels;

namespace TournamentManager.Services
{
    public class ExecutionToken
    {
        ManualResetEvent _handle = new ManualResetEvent(false);
        Action<ExecutionToken> _action;
        object _result;
        public ExecutionToken(Action<ExecutionToken> action)
        {
            _action = action;
        }

        public void Execute()
        {
            _action.Invoke(this);
        }

        public void SetResult<T>(T obj)
        {
            _result = obj;
        }

        public void Complete()
        {
            _handle.Set();
        }

        public void OnError(Exception ex)
        {
            SetResult(ex);
            _handle.Set();
        }
        public void Wait()
        {
            _handle.WaitOne();
        }
        public T WaitResult<T>()
        {
            _handle.WaitOne();

            if (_result == null)
                return default(T);
            else
                return (T)_result;
        }
    }
    public class Scheduler
    {
        Thread t;
        private object _actionQueueLocker = new object();
        private AutoResetEvent _actionEnqueuedHandle = new AutoResetEvent(initialState: false);
        Queue<ExecutionToken> _queue;
        CancellationTokenSource _cts;
        private bool _completeQueueOnShutdown;
        private bool _shuttingDown = false;
        private readonly ManualResetEvent _idleEvt = new ManualResetEvent(true);
        public int Length
        {
            get
            {
                lock (_actionQueueLocker)
                {
                    return _queue.Count;
                }
            }
        }

        public ManualResetEvent IdleEvt { get { return _idleEvt; } }

        public ExecutionToken Schedule(Action<ExecutionToken> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            ExecutionToken token;
            
            lock (_actionQueueLocker)
            {
                token = new ExecutionToken(action);
                _queue.Enqueue(token);
            }

            _actionEnqueuedHandle.Set();

            return token;
        }

        private void ProcessCurrentQueue(CancellationToken ct)
        {
            ExecutionToken action = null;

            while (Length > 0 && !ct.IsCancellationRequested)
            {
                lock (_actionQueueLocker)
                {
                    action = _queue.Dequeue();

                }


                try
                {
                    action.Execute();
                    action.Complete();
                }
                catch (Exception ex)
                {
                    action.OnError(ex);
                }

                lock (_actionQueueLocker)
                {
                    //Multiple schedule can happen while dequeueing, at each schedule the event is set:
                    //avoid doing useless outer loop because the event is set but all the scheduled actions
                    //have been consumed. Lock allows a concurrent Schedule() to set the event again.
                    if (Length <= 0)
                        _actionEnqueuedHandle.Reset();
                }
            }
        }

        private void RunOnCurrentThread()
        {
            var ct = _cts.Token;

            while (!ct.IsCancellationRequested)
            {
                // Process all actions of the queue until it's empty or cancellation is requested:
                ProcessCurrentQueue(ct);

                lock (_actionQueueLocker)
                {
                    if (Length == 0)
                        _idleEvt.Set();
                }

                // The queue is now empty.
                if (_shuttingDown)
                    return; // Do not wait for new additions.

                // Wait stop request or the addition of an action:
                WaitHandle.WaitAny(new WaitHandle[] { ct.WaitHandle, _actionEnqueuedHandle });
            }
        }

        public Scheduler()
        {
            _queue = new Queue<ExecutionToken>();
            _cts = new CancellationTokenSource();

            lock (_actionQueueLocker)
            {
                if (t != null)
                {
                    throw new InvalidOperationException("ActionQueueScheduler already started.");
                }

                _completeQueueOnShutdown = false;
                t = new Thread(RunOnCurrentThread)
                {
                    IsBackground = true
                };
                t.Start();
            }
        }

        public void Stop()
        {
            _shuttingDown = true;

            _cts.Cancel();
            if (t != null)
                t.Join();

            _queue.Clear();
        }
    }

}
