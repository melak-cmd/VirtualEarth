using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VirtualEarth.Network;
using System.Net.NetworkInformation;

namespace VirtualEarth
{
  internal class HtmlScriptCommandQueue
  {
    public HtmlScriptCommandQueue(VEMapEventSink mapEventSink)
    {
      commands = new Queue<HtmlScriptCommand>();

      NetworkMonitor.AvailabilityChanged += OnNetworkStatusChanged;

      mapEventSink.ControlReady += OnControlReady;
    }
    void OnNetworkStatusChanged(object sender,
      NetworkAvailabilityEventArgs e)
    {
      if (e.IsAvailable)
      {
        controlAvailable = false;
      }
    }
    void OnControlReady(object sender, EventArgs e)
    {
      controlAvailable = true;
      ExecuteNext();
    }
    public void Enqueue(HtmlScriptCommand command)
    {
      commands.Enqueue(command);
      ExecuteNext();
    }
    void ExecuteNext()
    {
      if (!executing && controlAvailable && (commands.Count > 0) && NetworkMonitor.IsAvailable)
      {
        executing = true;
        HtmlScriptCommand command = commands.Dequeue();
        command.Completed += OnCommandCompleted;
        command.Execute();
      }
    }
    void OnCommandCompleted(object sender, EventArgs args)
    {
      HtmlScriptCommand command = (HtmlScriptCommand)sender;
      command.Completed -= OnCommandCompleted;
      executing = false;
      ExecuteNext();
    }
    Queue<HtmlScriptCommand> commands;
    bool controlAvailable;
    bool executing;
  }
}