using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualEarth
{
  class DistanceAsyncScriptEventArgs : HtmlAsyncScriptEventArgs
  {
    public DistanceLookupResult Result { get; set; }
  }
}

