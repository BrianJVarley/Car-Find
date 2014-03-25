using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace CarFind
{
    public partial class LocationDetailsList : PhoneApplicationPage
    {
        public LocationDetailsList()
        {
            InitializeComponent();
        }

        //Loads files from isolated storage to list.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            using (var store = System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForApplication())
            {
                this.NotesListBox.ItemsSource = store.GetFileNames();
            }

            base.OnNavigatedTo(e);
        }

        //selects saved location detail based on slected index in list
        private void NotesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
                NavigationService.Navigate(new Uri(string.Format("/LocationDetails.xaml?note={0}", e.AddedItems[0]), UriKind.Relative));  
             
        }
    }
}