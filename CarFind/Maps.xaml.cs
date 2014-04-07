#region usings

using System;
using System.Device.Location;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Windows.Devices.Geolocation;
using Windows.Storage;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Maps.Controls;
using Microsoft.Phone.Maps.Services;
using Microsoft.Phone.Maps.Toolkit;
using Microsoft.Phone.Shell;

#endregion

namespace CarFind
{
    public partial class Maps : PhoneApplicationPage
    {
        private readonly MapLayer layer1;
        private GeoCoordinate MyGeoCoordPosition = new GeoCoordinate(0, 0);
        private Geoposition MyGeoPosition;
        private ProgressIndicator pi;



        public Maps()
        {
            
            //setting up the progress indicator
            InitializeComponent();
            pi = new ProgressIndicator { IsIndeterminate = true, IsVisible = false };
            //checking to see if ap settings contains a map layer when page initialized
            if (IsolatedStorageSettings.ApplicationSettings.Contains("MapLayer"))
                layer1 = IsolatedStorageSettings.ApplicationSettings["MapLayer"] as MapLayer;
            else
                layer1 = new MapLayer();
            
            if (layer1 != null)
                MyMap.Layers.Add(layer1);
        }


        //save coordinates after app is closed or naigated from
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            var userSettings = IsolatedStorageSettings.ApplicationSettings;

            //when page closed, add the map layer t user settings
            if (userSettings.Contains("MapLayer"))
                userSettings["MapLayer"] = layer1;
            else
                userSettings.Add("MapLayer", layer1);


            base.OnNavigatedFrom(e);
        }

        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            //asking user for location consent

            //
            if (IsolatedStorageSettings.ApplicationSettings.Contains("LocationConsent"))
            {
                // User has opted in or out of Location
                var locationConsent = (bool)IsolatedStorageSettings.ApplicationSettings["LocationConsent"];
                if (locationConsent) return;
                if (NavigationService.CanGoBack)
                    NavigationService.GoBack();
                return;
            }
            var result =
                MessageBox.Show("This app accesses your phone's location. Is that ok?",
                    "Location",
                    MessageBoxButton.OKCancel);

            IsolatedStorageSettings.ApplicationSettings["LocationConsent"] = result == MessageBoxResult.OK;

        }


        /// Getting Current Coordinates.
        private async Task GetCoordinates()
        {
            // Get the phone's current location.
            var MyGeolocator = new Geolocator { DesiredAccuracyInMeters = 5 };

            try
            {
                //return the geoPosition variable to the calling method
                MyGeoPosition = await MyGeolocator.GetGeopositionAsync();
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
        }


        //Method to draw each pushin to the map
        private void DrawPushPin(GeoCoordinate myGeoPosition, string pushPinName, bool clean = false)
        {
            var pp = new Pushpin { Content = pushPinName };
            var overlay = new MapOverlay { Content = pp };

            //if map isn't clear,then clear
            if (clean)
                layer1.Clear();


            layer1.Add(overlay);
            layer1[layer1.Count - 1].GeoCoordinate = MyGeoCoordPosition;

            MyMap.Center = myGeoPosition;
            MyMap.ZoomLevel = 16;
        }

        private void Pivot_Loaded_1(object sender, RoutedEventArgs e)
        {
        }

        private void deleteBtn_Click(object sender, EventArgs e)
        {
            MessageBoxResult m = MessageBox.Show("This will delete your parking location,are you sure?.",
                "Clear Map",  MessageBoxButton.OKCancel);

            if(m == MessageBoxResult.OK)
            {
            //clear all map pushpins
            layer1.Clear();
            }
            
        }

        private void infoBtn_Click(object sender, EventArgs e)
        {
            //go to location details
            NavigationService.Navigate(new Uri("/LocationDetails.xaml", UriKind.Relative));
        }

        //set location of car and draw pushpin to map
        private async void setLocationBtn_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            //check button sender if it has been selected
            if (btn == null)
                return;
            var pushPinName = "My Car";
            await GetCoordinates();
           
            MessageBox.Show("Processing", "Please Wait...", MessageBoxButton.OK);

            var latitude = MyGeoPosition.Coordinate.Latitude;
            var longitude = MyGeoPosition.Coordinate.Longitude;
            //creates a GeoCoordinate from the lat/lng vars and calls draw pushpin with positon and pin
            //name as params
            MyGeoCoordPosition = new GeoCoordinate(latitude, longitude);
            DrawPushPin(MyGeoCoordPosition, pushPinName, true);
        }

        private async void findCarBtn_Click(object sender, EventArgs e)
        {
            var pushPinName = "Current Location";
            //had to add a messagebox to bridge the gap between retrieving location and drawing.
            //looking in to how to increase the processing time of the operation or optimizing the code further..
            MessageBox.Show("Processing", "Finding car...", MessageBoxButton.OK);

            await GetCoordinates();
            
            var latitude = MyGeoPosition.Coordinate.Latitude;
            var longitude = MyGeoPosition.Coordinate.Longitude;

            //creates a GeoCoordinate from the lat/lng vars and calls draw pushpin with positon and pin
            //name as params
            MyGeoCoordPosition = new GeoCoordinate(latitude, longitude);
            DrawPushPin(MyGeoCoordPosition, pushPinName);
        }

        //developer key for using maps in the app.
        private void MyMap_Loaded(object sender, RoutedEventArgs e)
        {
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.ApplicationId = "fabb9d79-41c9-4ba6-8e10-7c0c44fb29e1";
            Microsoft.Phone.Maps.MapsSettings.ApplicationContext.AuthenticationToken = "a9XC-VfBhKWqg17N0_C5hg";
        }
    }
}