using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualEarth
{
  internal class PostCodeAsyncScriptEventArgs : HtmlAsyncScriptEventArgs
  {
    public PostCodeLookupResult Result { get; set; }
  }
}
