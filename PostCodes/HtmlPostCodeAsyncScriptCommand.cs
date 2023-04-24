using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VirtualEarth.HtmlScriptInteraction;

namespace VirtualEarth.PostCodes
{
  internal class HtmlPostCodeAsyncScriptCommand : HtmlAsyncScriptCommand<PostCodeAsyncScriptEventArgs>
  {
    public HtmlPostCodeAsyncScriptCommand(HtmlScriptInvoker invoker, VEMapEventSink mapEventSink,
      HtmlAsyncScriptResult<PostCodeAsyncScriptEventArgs> asyncResult) :
      base(invoker, mapEventSink)
    {
      this.asyncResult = asyncResult;
    }
    protected override void SyncEventHandler()
    {
      this.mapEventSink.PostCodeFound += OnPostCodeFound;
    }
    void OnPostCodeFound(object sender, PostCodeAsyncScriptEventArgs e)
    {
      this.mapEventSink.PostCodeFound -= OnPostCodeFound;
      asyncResult.Result = e;
      base.OnCompleted(e);
    }
  }

}
