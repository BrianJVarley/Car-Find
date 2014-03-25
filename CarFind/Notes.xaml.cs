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
    public partial class Notes : PhoneApplicationPage
    {
        public Notes()
        {
            InitializeComponent();
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            using (var store = System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForApplication())
            { 
                this.NotesListBox.ItemsSource = store.GetFileNames();
            }

            base.OnNavigatedTo(e);
        }

        private void NotesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
             if (e.AddedItems.Count > 0)
                 NavigationService.Navigate(new Uri( string.Format("/NoteWrite.xaml?note={0}", e.AddedItems[0]), UriKind.Relative)); 

        }



    }
}