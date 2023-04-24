using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace VirtualEarth
{
  [ComVisible(true)]
  [Guid("0A52F9EB-6519-47ce-B598-53A63F6EE220")]
  [InterfaceType(ComInterfaceType.InterfaceIsDual)] 
  public interface IVEMapEvents
  {
    void JSMapEventError(string error);
    void JSMapEventControlReadyHandler();
    void JSMapEventLatLonHandler(double lat, double lon);
    void JSMapEventZoomHandler(float zoom);
    void JSMapEventMapStyleHandler(string mode);
    void JSMapEventFoundPostCode(bool error, string errorMessage,
      double lat, double lon);
    void JSMapEventFoundDistance(bool error, string errorMessage,
      double distance, double time);
  }

  [ComVisible(true)]
  [Guid("F690152C-43E8-43da-9D82-730E444A61D7")]
  [ClassInterface(ClassInterfaceType.None)]
  public class VEMapEventSink : IVEMapEvents
  {
    internal VEMapEventSink()
    {
    }
    internal event EventHandler<MapErrorEventArgs> Error;
    internal event EventHandler<MapCenterChangedEventArgs> LatLonChanged;
    internal event EventHandler<MapZoomLevelChangedEventArgs> ZoomLevelChanged;
    internal event EventHandler<MapStyleChangedEventArgs> MapStyleChanged;
    internal event EventHandler ControlReady;
    internal event EventHandler<PostCodeAsyncScriptEventArgs> PostCodeFound;
    internal event EventHandler<DistanceAsyncScriptEventArgs> DistanceFound;

    public void JSMapEventError(string error)
    {
      if (Error != null)
      {
        Error(this, new MapErrorEventArgs() { Error = error });
      }
    }
    public void JSMapEventControlReadyHandler()
    {
      if (ControlReady != null)
      {
        ControlReady(this, null);
      }
    }
    public void JSMapEventLatLonHandler(double lat, double lon)
    {
      if (LatLonChanged != null)
      {
        LatLonChanged(this, new MapCenterChangedEventArgs()
        {
          Latitude = lat,
          Longitude = lon
        });
      }
    }
    public void JSMapEventZoomHandler(float zoomLevel)
    {
      if (ZoomLevelChanged != null)
      {
        ZoomLevelChanged(this, new MapZoomLevelChangedEventArgs() { ZoomLevel = zoomLevel });
      }
    }
    public void JSMapEventMapStyleHandler(string mapStyle)
    {
      if (MapStyleChanged != null)
      {
        MapStyleChanged(this, new MapStyleChangedEventArgs() { 
          MapStyle = Conversion.EnumFromVEMapStyle(mapStyle) });
      }
    }
    public void JSMapEventFoundPostCode(bool error, string errorMessage, 
      double lat, double lon)
    {
      if (PostCodeFound != null)
      {
        PostCodeFound(this, new PostCodeAsyncScriptEventArgs()
        {
          Error = error,
          ErrorMessage = errorMessage,
          Result = new PostCodeLookupResult() { Latitude = lat, Longitude = lon }
        });
      }
    }
    public void JSMapEventFoundDistance(bool error, string errorMessage,
      double distance, double time)
    {
      if (DistanceFound != null)
      {
        DistanceFound(this, new DistanceAsyncScriptEventArgs()
        {
          Error = error,
          ErrorMessage = errorMessage,
          Result = new DistanceLookupResult()
          {
            Distance = distance,
            Time = time
          }
        });
      }
    }
  }
}
