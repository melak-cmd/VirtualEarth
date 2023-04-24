using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualEarth
{
  internal class HtmlScriptCommand
  {
    public HtmlScriptCommand(HtmlScriptInvoker invoker)
    {
      this.invoker = invoker;
    }
    public ScriptFunction Function { get; set; }

    public object[] Arguments { get; set; }

    protected void InternalExecute()
    {
      invoker.InvokeScript(Function, Arguments);
    }
    public virtual void Execute()
    {
      InternalExecute();
      FireCompleted();
    }
    protected void FireCompleted()
    {
      if (Completed != null)
      {
        Completed(this, null);
      }
    }
    public event EventHandler Completed;
    HtmlScriptInvoker invoker;
  }
}