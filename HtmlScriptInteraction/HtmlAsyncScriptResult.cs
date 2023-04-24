using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace VirtualEarth
{
  internal class HtmlAsyncScriptResult<T> : IAsyncResult where T : HtmlAsyncScriptEventArgs
  {
    public HtmlAsyncScriptResult(AsyncCallback callback, object state)
    {
      this.state = state;
      this.callback = callback;
      completedEvent = new ManualResetEvent(false);
    }
    public object AsyncState
    {
      get { return (state); }
    }
    public System.Threading.WaitHandle AsyncWaitHandle
    {
      get { return (completedEvent); }
    }
    public bool CompletedSynchronously
    {
      get { return (false); }
    }
    public bool IsCompleted
    {
      get { return (completedEvent.WaitOne(0)); }
    }
    public void Complete()
    {
      completedEvent.Set();
      callback(this);
    }
    internal void CheckError()
    {
      if (Result.Error)
      {
        throw new ApplicationException(Result.ErrorMessage);
      }
    }

    public T Result { get; set; }

    object state;

    AsyncCallback callback;

    ManualResetEvent completedEvent;
  }
}