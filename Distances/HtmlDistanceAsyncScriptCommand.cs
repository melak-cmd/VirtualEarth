using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VirtualEarth.HtmlScriptInteraction;

namespace VirtualEarth.Distances
{
  internal class HtmlDistanceAsyncScriptCommand : HtmlAsyncScriptCommand<DistanceAsyncScriptEventArgs>
  {
    public HtmlDistanceAsyncScriptCommand(HtmlScriptInvoker invoker, VEMapEventSink mapEventSink,
      HtmlAsyncScriptResult<DistanceAsyncScriptEventArgs> asyncResult) :
      base(invoker, mapEventSink)
    {
      this.asyncResult = asyncResult;
    }
    protected override void SyncEventHandler()
    {
      this.mapEventSink.DistanceFound += OnDistanceFound;
    }
    void OnDistanceFound(object sender, DistanceAsyncScriptEventArgs e)
    {
      this.mapEventSink.DistanceFound -= OnDistanceFound;
      asyncResult.Result = e;
      base.OnCompleted(e);
    }
  }
}
