using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Resources;
using System.IO;

namespace VirtualEarth
{
  internal static class HtmlLoader
  {
    public static Stream LoadEmbeddedHtml()
    {
      return (
        Assembly.GetExecutingAssembly().GetManifestResourceStream(htmlResource));
    }
    static readonly string htmlResource = "WpfVirtualEarth.VirtualEarth.htm";
  }
}
