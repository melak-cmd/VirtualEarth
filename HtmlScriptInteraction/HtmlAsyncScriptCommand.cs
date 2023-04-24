using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualEarth.HtmlScriptInteraction
{
  internal abstract class HtmlAsyncScriptCommand<T> : HtmlScriptCommand where T : HtmlAsyncScriptEventArgs
  {
    public HtmlAsyncScriptCommand(HtmlScriptInvoker invoker,
      VEMapEventSink mapEventSink)
      : base(invoker)
    {
      this.mapEventSink = mapEventSink;
    }
    public override void Execute()
    {
      SyncEventHandler();
      InternalExecute();
    }
    protected abstract void SyncEventHandler();

    protected void OnCompleted(HtmlAsyncScriptEventArgs args)
    {
      asyncResult.Result.Error = args.Error;
      asyncResult.Result.ErrorMessage = args.ErrorMessage;
      asyncResult.Complete();
      FireCompleted();
    }
    protected HtmlAsyncScriptResult<T> asyncResult;
    protected VEMapEventSink mapEventSink;
  }
}
