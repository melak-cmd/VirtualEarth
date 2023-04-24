using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.NetworkInformation;

namespace VirtualEarth.Network
{
  internal static class NetworkMonitor
  {
    public static event EventHandler<NetworkAvailabilityEventArgs> AvailabilityChanged;

    static NetworkMonitor()
    {
      NetworkChange.NetworkAvailabilityChanged += OnAvailabilityChanged;
      networkAvailable = NetworkInterface.GetIsNetworkAvailable();
    }
    static void OnAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
    {
      networkAvailable = e.IsAvailable;

      if (AvailabilityChanged != null)
      {
        AvailabilityChanged(null, e);
      }
    }
    public static bool IsAvailable
    {
      get
      {
        return (networkAvailable);
      }
    }
    static bool networkAvailable;
  }
}
