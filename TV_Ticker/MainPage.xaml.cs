using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Scheduler;

namespace TV_Ticker
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        public void Notifier_Test()
        {
            Reminder reminder = new Reminder(Guid.NewGuid().ToString());
            // NOTE: setting the Title property is supported for reminders
            // in contrast to alarms where setting the Title property is not supported
            reminder.Title = "Some Title";
            reminder.Content = "Some Content";

            //NOTE: the value of BeginTime must be after the current time
            //set the BeginTime time property in order to specify when the reminder should be shown
            reminder.BeginTime = DateTime.Now.AddSeconds(10.0);

            // NOTE: ExpirationTime must be after BeginTime
            // the value of the ExpirationTime property specifies when the schedule of the reminder expires
            // very useful for recurring reminders, ex:
            // show reminder every day at 5PM but stop after 10 days from now
            reminder.ExpirationTime = reminder.BeginTime.AddSeconds(5.0);
            reminder.RecurrenceType = RecurrenceInterval.None;

            // NOTE: another difference from alerts
            // you can set a navigation uri that is passed to the application when it is launched from the reminder
            //reminder.NavigationUri = navigationUri;
            ScheduledActionService.Add(reminder);
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Test Code Only!
            JsonData myJsonData = new JsonData();
            myJsonData.jsonrpc = "2.0";
            myJsonData.id = "channels_list";
            myJsonData.method = "channels";
            myJsonData.Params = new List<string>() { "1234", "8765554ce9cb7cc57b5d99d658e018ca"};
            JsonWorker.makeJsonRequest("http://api.tvticker.in/service", myJsonData);
            myJsonData.jsonrpc = "2.0";
            myJsonData.id = "categories_list";
            myJsonData.method = "categories";
            myJsonData.Params = new List<string>() { "1234", "8765554ce9cb7cc57b5d99d658e018ca" };
            JsonWorker.makeJsonRequest("http://api.tvticker.in/service", myJsonData);
            myJsonData.jsonrpc = "2.0";
            myJsonData.id = "programs_list";
            myJsonData.method = "programs_for_current_frame";
            string date = DateTime.UtcNow.Hour.ToString() + ":00";
            myJsonData.Params = new List<string>() { "1234", "8765554ce9cb7cc57b5d99d658e018ca", date, "now" };
            JsonWorker.makeJsonRequest("http://api.tvticker.in/service", myJsonData);
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //How to Navigate between pages.
            //NavigationService.Navigate(new Uri("/Page1.xaml", UriKind.Relative));
        }


        private void PhoneApplicationPage_GotFocus(object sender, RoutedEventArgs e)
        {
            Showing.SelectedIndex = -1;
            Schedule.SelectedIndex = -1;
        }
    }
}