using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace VirtualEarth
{
  internal enum ScriptFunction
  {
    SetCenter,
    SetZoomLevel,
    SetMapStyle,
    LookupPostCode,
    AddPushPin,
    RemovePushPin,
    CalculateDistance,
    Resize
  }
  internal class HtmlScriptInvoker
  {
    public HtmlScriptInvoker(WebBrowser browserControl)
    {
      this.browserControl = browserControl;
    }
    public void InvokeScript(ScriptFunction function, params object[] parameters)
    {
      string enumName = Enum.GetName(typeof(ScriptFunction), function);

      browserControl.InvokeScript(enumName, parameters);
    }
    WebBrowser browserControl;
  }
}
