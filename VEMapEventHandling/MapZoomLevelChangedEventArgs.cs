using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualEarth
{
  internal class MapZoomLevelChangedEventArgs : EventArgs
  {
    public double ZoomLevel { get; set; }
  }
}
