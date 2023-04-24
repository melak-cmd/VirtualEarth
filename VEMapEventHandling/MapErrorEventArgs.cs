using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualEarth
{
  internal class MapErrorEventArgs : EventArgs 
  {
    public string Error { get; set; }
  }
}
