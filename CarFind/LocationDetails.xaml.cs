using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using System.IO;

namespace CarFind
{
    public partial class LocationDetails : PhoneApplicationPage
    {
        public LocationDetails()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

        if(NavigationContext.QueryString == null)
        {
            //QueryString is null
        }
        else if (NavigationContext.QueryString.ContainsKey("note"))
        {
            _openSavedFile(NavigationContext.QueryString["note"]);
        }
        else
        {
            //QueryString does not contain a "note" parameter and QueryString is not null
        }


 	        base.OnNavigatedTo(e);
        }
        
      
        private void _openSavedFile(string filename)
        {
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            using (var stream = new IsolatedStorageFileStream(filename, FileMode.Open, FileAccess.ReadWrite, store))
            {
                StreamReader reader = new StreamReader(stream);
                this.NoteTextBox.Text = reader.ReadToEnd();
                this.FilenameTextBox.Text = filename;
                reader.Close();
            }

        }
        
         
         
     

        //saves location details to isolated storage
        private void SaveButton_Click(object sender, EventArgs e)
        {
            try
            {
                using (var store = IsolatedStorageFile.GetUserStoreForApplication())
                using (var stream = new IsolatedStorageFileStream(FilenameTextBox.Text, FileMode.Create, FileAccess.Write, store))
                {
                    StreamWriter writer = new StreamWriter(stream);
                    writer.Write(NoteTextBox.Text); writer.Close();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Error saving the file");
            }
        }

        //Show list of saved location details on click
        private void ListButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/LocationDetailsList.xaml", UriKind.Relative));
        }

        private void NoteTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}