using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualEarth
{
  internal class MapStyleChangedEventArgs : EventArgs 
  {
    public MapStyle MapStyle { get; set; }
  }
}
