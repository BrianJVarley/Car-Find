using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using CarFind.Resources;
using Microsoft.Phone.Maps;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using System.IO.IsolatedStorage;
using System.Device.Location;
using Microsoft.Phone.Maps.Controls;
using System.Windows.Media.Imaging;



namespace CarFind
{
    public partial class MainPage : PhoneApplicationPage
    {
        Geoposition MyGeoPosition = null;
        
        public MainPage()
        {
            InitializeComponent();
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //asking user for location consent
            if (IsolatedStorageSettings.ApplicationSettings.Contains("LocationConsent"))
            {
                // User has opted in or out of Location
                return;
            }
            else
            {
                MessageBoxResult result =
                    MessageBox.Show("This app accesses your phone's location. Is that ok?",
                    "Location",
                    MessageBoxButton.OKCancel);

                if (result == MessageBoxResult.OK)
                {
                    IsolatedStorageSettings.ApplicationSettings["LocationConsent"] = true;
                }
                else
                {
                    IsolatedStorageSettings.ApplicationSettings["LocationConsent"] = false;
                }

                IsolatedStorageSettings.ApplicationSettings.Save();
            }

            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// Getting Current Coordinate.
        /// </summary>
        private async Task GetCoordinates()
        {
            await Task.Run(async () =>
            {
                // Get the phone's current location.
                Geolocator MyGeolocator = new Geolocator();
                MyGeolocator.DesiredAccuracyInMeters = 1;
              
                try
                {
                    MyGeoPosition = await MyGeolocator.GetGeopositionAsync(TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(10));

                }
                catch (UnauthorizedAccessException)
                {
                    MessageBox.Show("Location is disabled in phone settings or capabilities are not checked.");
                }
                catch (Exception ex)
                {
                    // Something else happened while acquiring the location.
                    MessageBox.Show(ex.Message);
                }
            });
        }

        //use GetCoordinates method to get user location 
        private async void findCarBtn_Click(object sender, RoutedEventArgs e)
        {
            await this.GetCoordinates();

            string pushPinName = "Current Location";
            //NavigationService.Navigate(new Uri("/Maps.xaml", UriKind.Relative));
            this.NavigationService.Navigate(new Uri(string.Format("/Maps.xaml?GeoLat={0}&GeoLong={1}&pName={2}",
                MyGeoPosition.Coordinate.Latitude, MyGeoPosition.Coordinate.Longitude, pushPinName), UriKind.Relative));   
        }

        //clears saved coordinates 
        private void clearLocationBtn_Click(object sender, RoutedEventArgs e)
        {
            //NavigationService.Navigate(new Uri("/Maps.xaml", UriKind.Relative));
            
            Maps map = new Maps();
            map.ClearMapMarkers();  
             
        }

        
        //sets location of parking space using the GetCoordinates method
        //opens map 
        private async void setLocationBtn_Click(object sender, RoutedEventArgs e)
        {
            string pushPinName = "My Car";
            await this.GetCoordinates();
            //NavigationService.Navigate(new Uri("/Maps.xaml", UriKind.Relative));
            this.NavigationService.Navigate(new Uri(string.Format("/Maps.xaml?GeoLat={0}&GeoLong={1}&pName={2}",
                MyGeoPosition.Coordinate.Latitude, MyGeoPosition.Coordinate.Longitude, pushPinName), UriKind.Relative));
        }

        //saves coordinates when navigated from.
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            
            base.OnNavigatedFrom(e);
        }

        //opens locationdetails page
        private void locationDetailsBtn_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/LocationDetails.xaml", UriKind.Relative));

        }
       
    }
}