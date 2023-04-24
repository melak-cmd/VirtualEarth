using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Markup;
using System.Collections.Specialized;
using System.ComponentModel;

namespace VirtualEarth
{
  [ContentProperty("PushPins")]
  public class PushPinCollection : INotifyCollectionChanged
  {
    public event NotifyCollectionChangedEventHandler CollectionChanged;

    public PushPinCollection()
    {
      pins = new ObservableCollection<PushPin>();
      pins.CollectionChanged += OnCollectionChanged;
    } 
    public ObservableCollection<PushPin> PushPins
    {
      get
      {
        return (pins);
      }
    }
    void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (CollectionChanged != null)
      {
        CollectionChanged(this, e);
      }
    }   
    ObservableCollection<PushPin> pins;    
  }
}