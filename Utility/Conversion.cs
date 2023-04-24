using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VirtualEarth
{
  internal static class Conversion
  {
    static Conversion()
    {
      mapStyles = new Dictionary<string,MapStyle>();
      mapStyles.Add("a", MapStyle.Aerial);
      mapStyles.Add("r", MapStyle.Road);
      mapStyles.Add("h", MapStyle.Hybrid);
    }
    public static string VEMapStyleFromEnum(MapStyle mapStyle)
    {
      return (mapStyles.Where(kvp => kvp.Value == mapStyle).Select(kvp => kvp.Key).Single());
    }
    public static MapStyle EnumFromVEMapStyle(string mapStyle)
    {
      return (mapStyles[mapStyle]);
    }
    static Dictionary<string, MapStyle> mapStyles;
  }
}
