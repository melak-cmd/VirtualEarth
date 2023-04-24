using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualEarth
{
  internal class HtmlAsyncScriptEventArgs : EventArgs
  {
    public bool Error { get; set; }
    public string ErrorMessage { get; set; }
  }
}
