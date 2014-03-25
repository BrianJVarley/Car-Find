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
    public partial class NoteWrite : PhoneApplicationPage
    {
        public NoteWrite()
        {
            InitializeComponent();
        }

        private void NoteTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
                string filename = this.NavigationContext.QueryString["note"];
                if (!string.IsNullOrEmpty(filename)) 
                { 
                    using (var store = System.IO.IsolatedStorage.IsolatedStorageFile .GetUserStoreForApplication())
                    using (var stream = new IsolatedStorageFileStream(filename, FileMode.Open, FileAccess.ReadWrite, store)) 
                    { 
                        StreamReader reader = new StreamReader(stream);
                        this.NoteTextBox.Text = reader.ReadToEnd(); 
                        this.FilenameTextBox.Text = filename; reader.Close(); 
                    } 
                }
            

            base.OnNavigatedTo(e);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            try { 
                using (var store = IsolatedStorageFile.GetUserStoreForApplication()) 
                using (var stream = new IsolatedStorageFileStream(FilenameTextBox.Text, FileMode.Create, FileAccess.Write, store)) 
                { 
                 StreamWriter writer = new StreamWriter(stream);
                 writer.Write(NoteTextBox.Text); writer.Close(); 
                } 
            } 
            catch (Exception) { 
                MessageBox.Show("Error saving the file"); 
            }
 

        }

        private void ListButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Notes.xaml", UriKind.Relative));

        }
    }
}