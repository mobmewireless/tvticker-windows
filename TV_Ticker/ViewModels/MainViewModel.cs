using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;


namespace TV_Ticker
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            this.Showing = new ObservableCollection<ItemViewModel>();
            this.Schedule = new ObservableCollection<ItemViewModel>();
            this.Channels = new ObservableCollection<ChannelViewModel>();
            this.Categories = new ObservableCollection<CategoryViewModel>();
        }

        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        public ObservableCollection<ItemViewModel> Showing { get; private set; }
        public ObservableCollection<ItemViewModel> Schedule { get; private set; }
        public ObservableCollection<ChannelViewModel> Channels { get; private set; }
        public ObservableCollection<CategoryViewModel> Categories { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}