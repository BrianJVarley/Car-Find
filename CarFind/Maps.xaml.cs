using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.Devices.Geolocation;
using System.Device.Location;
using Microsoft.Phone.Maps.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Threading.Tasks;
using Microsoft.Phone.Maps.Toolkit;
using Microsoft.Phone.Maps.Services;
using System.Collections.ObjectModel;
using Microsoft.Phone.Tasks;
using System.Device.Location;
using System.IO.IsolatedStorage;


namespace CarFind
{
    public partial class Maps : PhoneApplicationPage
    {
        
        ProgressIndicator pi;
        public List<GeoCoordinate> mycoord = new List<GeoCoordinate>();
        RouteQuery MyQuery = null;
        GeocodeQuery Mygeocodequery = null;
        GeoCoordinate MyGeoPosition = new GeoCoordinate(0, 0); 
        
        

        public Maps()
        {
            InitializeComponent();
            pi = new ProgressIndicator();
            pi.IsIndeterminate = true;
            pi.IsVisible = false;

            
            
        }

        //save coordinates after app is closed or naigated from
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (mycoord.Count > 0)
            {
                mycoord.Add(new GeoCoordinate(MyGeoPosition.Latitude, MyGeoPosition.Longitude));

            }
            base.OnNavigatedFrom(e);
        }

        //Receives coordinate data and push pin name from the GetGeoCoordinate() of MainPage
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
           
            if (NavigationContext.QueryString.ContainsKey("GeoLat") && NavigationContext.QueryString.ContainsKey("GeoLong") && NavigationContext.QueryString.ContainsKey("pName"))
            { 
                    var latitude = Convert.ToDouble(NavigationContext.QueryString["GeoLat"]);
                    var longtitude = Convert.ToDouble(NavigationContext.QueryString["GeoLong"]);
                    MyGeoPosition = new GeoCoordinate(latitude, longtitude);
                    var pushPinName = NavigationContext.QueryString["pName"];

                    DrawPushPin(MyGeoPosition, pushPinName);
                    
                    //save the coordinates to a list
                    mycoord.Add(new GeoCoordinate(MyGeoPosition.Latitude, MyGeoPosition.Longitude));

                    if (mycoord.Count == 2)
                    {
                        //call route method when coord list equal to two.
                        GetRoute();
                    }
      
            }

            base.OnNavigatedTo(e);
        }

        //Creates route between coordinate points 
        private void GetRoute()
        {
            
            MyQuery = new RouteQuery();
            MyQuery.TravelMode = TravelMode.Walking;
            MyQuery.Waypoints = mycoord;
            MyQuery.QueryCompleted += MyQuery_QueryCompleted;
            MyQuery.QueryAsync();
        }

        void MyQuery_QueryCompleted(object sender, QueryCompletedEventArgs<Route> e)
        {
            if (e.Error == null)
            {
                Route MyRoute = e.Result;
                MapRoute MyMapRoute = new MapRoute(MyRoute);
                MyMap.AddRoute(MyMapRoute);
                MyQuery.Dispose();
            }
        }

        //Method to draw each pushin to the map
        private void DrawPushPin(GeoCoordinate MyGeoPosition,string pushPinName)
        {
            MapLayer layer1 = new MapLayer();
            Pushpin pushpin1 = new Pushpin();

            pushpin1.GeoCoordinate = MyGeoPosition;
            
            pushpin1.Content = pushPinName;
            

            MapOverlay overlay1 = new MapOverlay();
            overlay1.Content = pushpin1;
            overlay1.GeoCoordinate = MyGeoPosition;
            layer1.Add(overlay1);

            MyMap.Layers.Add(layer1);
            MyMap.Center = MyGeoPosition;
            MyMap.ZoomLevel = 14;
        }

        private void Pivot_Loaded_1(object sender, RoutedEventArgs e)
        {
            
        }

        private void PhoneApplicationPage_Loaded_1(object sender, RoutedEventArgs e)
        {
            
            
        }

        //method to clear all pushpins from map,called from clear location button click.
        public void ClearMapMarkers()
        {
            MyMap.MapElements.Clear();
            mycoord.Clear();
            

        }

       
 
    }
}