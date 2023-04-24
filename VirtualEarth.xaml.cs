using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Net.NetworkInformation;
using VirtualEarth.Network;
using System.ComponentModel;
using VirtualEarth.PostCodes;
using VirtualEarth.Distances;

namespace VirtualEarth
{
    public partial class VirtualEarth : UserControl, INotifyPropertyChanged
  {
    #region public 

    #region Events

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion // Events

    #region ctor
    public VirtualEarth()
    {
      InitializeComponent();

      // TODO: How to figure out design mode?
      //if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
      //{           
      InitialiseSizeChangedHandler();

      InitialiseNetworkStatusChangedHandler();

      InitialiseWebBrowserHelpers();

      InitialiseForNetworkStatus();

      //}
    }
    #endregion

    #region DP Registration

    public static DependencyProperty LatitudeProperty =
      DependencyProperty.Register("Latitude", typeof(double), typeof(VirtualEarth),
        new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, 
          OnLatitudeDpChanged));

    public static DependencyProperty LongitudeProperty =
      DependencyProperty.Register("Longitude", typeof(double), typeof(VirtualEarth),
        new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
          OnLongitudeDpChanged));

    public static DependencyProperty ZoomLevelProperty =
      DependencyProperty.Register("ZoomLevel", typeof(int), typeof(VirtualEarth),
        new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
          OnZoomLevelDpChanged));

    public static DependencyProperty MapStyleProperty =
      DependencyProperty.Register("MapStyle", typeof(MapStyle), typeof(VirtualEarth),
        new FrameworkPropertyMetadata(MapStyle.Hybrid, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, 
          OnMapStyleDpChanged));

    public static DependencyProperty PushPinsProperty =
      DependencyProperty.Register("PushPins", typeof(ObservableCollection<PushPin>), typeof(VirtualEarth),
        new FrameworkPropertyMetadata(OnPushPinsDpChanged));

    #endregion // DP Registration

    #region DPs

    public double Latitude
    {
      get
      {
        return ((double)GetValue(LatitudeProperty));
      }
      set
      {
        SetValue(LatitudeProperty, value);
      }
    }
    public double Longitude
    {
      get
      {
        return ((double)GetValue(LongitudeProperty));
      }
      set
      {
        SetValue(LongitudeProperty, value);
      }
    }
    public int ZoomLevel
    {
      get
      {
        return ((int)GetValue(ZoomLevelProperty));
      }
      set
      {
        SetValue(ZoomLevelProperty, value);
      }
    }
    public MapStyle MapStyle
    {
      get
      {
        return ((MapStyle)GetValue(MapStyleProperty));
      }
      set
      {
        SetValue(MapStyleProperty, value);
      }
    }
    public ObservableCollection<PushPin> PushPins
    {
      get
      {
        return ((ObservableCollection<PushPin>)GetValue(PushPinsProperty));
      }
      set
      {
        SetValue(PushPinsProperty, value);
      }
    }
    #endregion // DPs

    #region Regular Properties

    public bool IsOnline 
    {
      get
      {
        return (NetworkMonitor.IsAvailable);
      }
    }

    #endregion 

    #region PostCodes

    public IAsyncResult BeginLookupPostCode(string postCode, AsyncCallback callback,
      object asyncState)
    {
      HtmlAsyncScriptResult<PostCodeAsyncScriptEventArgs> asyncResult = 
        new HtmlAsyncScriptResult<PostCodeAsyncScriptEventArgs>(callback, asyncState);

      HtmlPostCodeAsyncScriptCommand command = new HtmlPostCodeAsyncScriptCommand(scriptInvoker, mapEventSink, asyncResult)
      {
        Function = ScriptFunction.LookupPostCode,
        Arguments = new object[] { postCode }
      };
      commandQueue.Enqueue(command);

      return (asyncResult);
    }
    public PostCodeLookupResult EndLookupPostCode(IAsyncResult result)
    {
      HtmlAsyncScriptResult<PostCodeAsyncScriptEventArgs> asyncResult =
        (HtmlAsyncScriptResult<PostCodeAsyncScriptEventArgs>)result;

      asyncResult.CheckError();

      return (asyncResult.Result.Result);
    }

    #endregion // PostCodes

    #region Distances

    public IAsyncResult BeginFindDistance(PushPin pin1, PushPin pin2,
      AsyncCallback callback, object asyncState)
    {
      HtmlAsyncScriptResult<DistanceAsyncScriptEventArgs> asyncResult =
        new HtmlAsyncScriptResult<DistanceAsyncScriptEventArgs>(callback, asyncState);

      HtmlDistanceAsyncScriptCommand command = new HtmlDistanceAsyncScriptCommand(
        scriptInvoker, mapEventSink, asyncResult)
        {
          Function = ScriptFunction.CalculateDistance,
          Arguments = new object[] { pin1.Id, pin2.Id }
        };

      commandQueue.Enqueue(command);

      return (asyncResult);
    }
    public DistanceLookupResult EndFindDistance(IAsyncResult result)
    {
      HtmlAsyncScriptResult<DistanceAsyncScriptEventArgs> asyncResult =
        (HtmlAsyncScriptResult<DistanceAsyncScriptEventArgs>)result;

      asyncResult.CheckError();

      return (asyncResult.Result.Result);
    }

    #endregion // Distances

    #endregion // Public

    #region private

    #region PushPins

    void AddPushPin(PushPin pin)
    {
      pin.ScriptInvoker = scriptInvoker;
      pin.CommandQueue = commandQueue;
      commandQueue.Enqueue(new HtmlScriptCommand(scriptInvoker)
      {
        Function = ScriptFunction.AddPushPin,
        Arguments = new object[] { pin.Id, pin.Latitude, pin.Longitude, pin.Text }
      });
    }
    void RemovePushPin(PushPin pin)
    {
      commandQueue.Enqueue(new HtmlScriptCommand(scriptInvoker)
      {
        Function = ScriptFunction.RemovePushPin,
        Arguments = new object[] { pin != null ? pin.Id : -1 }
      });
      if (pin != null)
      {
        pin.ScriptInvoker = null;
        pin.CommandQueue = null;
      }
    }
    void OnPushPinsChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if ((e.Action == NotifyCollectionChangedAction.Add) ||
        (e.Action == NotifyCollectionChangedAction.Replace))
      {
        foreach (PushPin pin in e.NewItems)
        {
          AddPushPin(pin);
        }
      }
      if ((e.Action == NotifyCollectionChangedAction.Remove) || 
        (e.Action == NotifyCollectionChangedAction.Replace))
      {
        foreach (PushPin pin in e.OldItems)
        {
          RemovePushPin(pin);
        }
      }
      if (e.Action == NotifyCollectionChangedAction.Reset)
      {
        RemovePushPin(null);
      }
    }

    #endregion // PushPins

    #region Initialisation routines

    void InitialiseSizeChangedHandler()
    {
      this.SizeChanged += OnControlSizeChanged;
    }
    void InitialiseNetworkStatusChangedHandler()
    {
      NetworkMonitor.AvailabilityChanged += OnNetworkAvailableChanged;
    }
    void InitialiseWebBrowserHelpers()
    {
      scriptInvoker = new HtmlScriptInvoker(webBrowser);

      mapEventSink = new VEMapEventSink();
      mapEventSink.LatLonChanged += OnControlLatLonChanged;
      mapEventSink.ZoomLevelChanged += OnControlZoomLevelChanged;
      mapEventSink.MapStyleChanged += OnControlMapStyleChanged;

      webBrowser.ObjectForScripting = mapEventSink;

      commandQueue = new HtmlScriptCommandQueue(mapEventSink);   

      PushPins = new ObservableCollection<PushPin>();
    }
    void InitialiseForNetworkStatus()
    {
      if (NetworkMonitor.IsAvailable)
      {
        webBrowser.Visibility = Visibility.Visible;                
        webBrowser.NavigateToStream(HtmlLoader.LoadEmbeddedHtml());
      }
      else
      {
        webBrowser.Visibility = Visibility.Collapsed;

        // Call these to cause us to queue up commands which will put the
        // map back in the state that it is currently in when the network
        // comes back up.
        SetZoomLevel();
        SetMapStyle(); 
        SetCenter();               

        if ((PushPins != null) && (PushPins.Count > 0))
        {
          foreach (PushPin p in PushPins)
          {
            AddPushPin(p);
          }
        }
      }
    }
    #endregion 

    #region DP Change Handlers & Instance Methods

    void SetCenter()
    {
      if (!controlGeneratedEvent)
      {
        commandQueue.Enqueue(new HtmlScriptCommand(scriptInvoker)
        {
          Function = ScriptFunction.SetCenter,
          Arguments = new object[] { Latitude, Longitude }
        });
      }
    }
    void SetZoomLevel()
    {
      if ((!controlGeneratedEvent) && (ZoomLevel >= Constants.MinZoomLevel) &&
        (ZoomLevel <= Constants.MaxZoomLevel))
      {
        commandQueue.Enqueue(new HtmlScriptCommand(scriptInvoker)
        {
          Function = ScriptFunction.SetZoomLevel,
          Arguments = new object[] { (double)ZoomLevel }
        });
      }
    }
    void SetMapStyle()
    {
      if (!controlGeneratedEvent)
      {
        commandQueue.Enqueue(new HtmlScriptCommand(scriptInvoker)
        {
          Function = ScriptFunction.SetMapStyle,
          Arguments = new object[] { Conversion.VEMapStyleFromEnum(MapStyle) }
        });
      }
    }
    void SetPushPins(ObservableCollection<PushPin> oldCollection)
    {
      if (oldCollection != null)
      {
        oldCollection.Clear();
        oldCollection.CollectionChanged -= OnPushPinsChanged;
      }
      if (PushPins != null)
      {
        PushPins.CollectionChanged += OnPushPinsChanged;

        foreach (PushPin pin in PushPins)
        {
          AddPushPin(pin);
        }
      }
    }
    static void OnLatitudeDpChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      VirtualEarth ve = (VirtualEarth)sender;
      ve.SetCenter();
    }
    static void OnLongitudeDpChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      VirtualEarth ve = (VirtualEarth)sender;
      ve.SetCenter();
    }
    static void OnZoomLevelDpChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      VirtualEarth ve = (VirtualEarth)sender;
      ve.SetZoomLevel();
    }
    static void OnMapStyleDpChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      VirtualEarth ve = (VirtualEarth)sender;
      ve.SetMapStyle();
    }
    static void OnPushPinsDpChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      VirtualEarth ve = (VirtualEarth)sender;
      ve.SetPushPins((ObservableCollection<PushPin>)args.OldValue);
    }
    #endregion 

    #region Map Control Callbacks

    void OnControlMapStyleChanged(object sender, MapStyleChangedEventArgs e)
    {
      controlGeneratedEvent = true;
      MapStyle = e.MapStyle;
      controlGeneratedEvent = false;
    }
    void OnControlZoomLevelChanged(object sender, MapZoomLevelChangedEventArgs e)
    {
      controlGeneratedEvent = true;
      SetValue(ZoomLevelProperty, (int)e.ZoomLevel);
      controlGeneratedEvent = false;
    }
    void OnControlLatLonChanged(object sender, MapCenterChangedEventArgs e)
    {
      controlGeneratedEvent = true;
      Latitude = e.Latitude;
      Longitude = e.Longitude;
      controlGeneratedEvent = false;
    }
    #endregion // Map Callbacks

    #region Network Status Change Handler

    void OnNetworkAvailableChangedUIThread()
    {
      InitialiseForNetworkStatus();

      FirePropertyChanged("IsOnline");
    }
    void OnNetworkAvailableChanged(object sender, NetworkAvailabilityEventArgs e)
    {
      Dispatcher.Invoke(new Action(OnNetworkAvailableChangedUIThread));
    }
 
    #endregion 

    #region Resize Handler

    void OnControlSizeChanged(object sender, SizeChangedEventArgs e)
    {
      commandQueue.Enqueue(new HtmlScriptCommand(scriptInvoker)
      {
        Function = ScriptFunction.Resize
      });
    }
    #endregion // Resize Handler

    #region Property Changed

    void FirePropertyChanged(string property)
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs(property));
      }
    }

    #endregion

    HtmlScriptInvoker scriptInvoker;
    VEMapEventSink mapEventSink;
    HtmlScriptCommandQueue commandQueue;
    bool controlGeneratedEvent;    
    #endregion // private    
  }
}