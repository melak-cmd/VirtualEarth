using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace VirtualEarth
{
  public class PushPin
  {
    public PushPin()
    {
      instanceId = ++globalId;
    }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Text { get; set; }
    public int Id
    {
      get
      {
        return (instanceId);
      }
    }
    public void CenterInMap()
    {
      CommandQueue.Enqueue(new HtmlScriptCommand(ScriptInvoker)
      {
        Function = ScriptFunction.SetCenter,
        Arguments = new object[] { this.Latitude, this.Longitude }
      });
    }
    internal HtmlScriptCommandQueue CommandQueue { get; set; }
    internal HtmlScriptInvoker ScriptInvoker { get; set; }
    int instanceId;
    static int globalId = -1;
  }
}
