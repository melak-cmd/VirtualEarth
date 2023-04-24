using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualEarth
{
  internal class MapCenterChangedEventArgs : EventArgs
  {
    public double Latitude;
    public double Longitude;
  }
}
