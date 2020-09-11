using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace OldXamarinAdroid
{
    [Activity(Label = "SearchActivity")]
    public class SearchActivity : Activity, ILocationListener
    {
        ListView listView;
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        Location currentLocation;
        LocationManager locationManager;
        string locationProvider;
        ImotorCareWebApi.ImotorCareWebApi imotorCare;
        SearchAdapter personel;
        ProgressBar progressBar;
        int startStatus = 0, endStatus = 100;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SearchLayout);
            // Create your application here
            
            imotorCare = new ImotorCareWebApi.ImotorCareWebApi();
            listView = FindViewById<ListView>(Resource.Id.listViewSearch);
            progressBar = FindViewById<ProgressBar>(Resource.Id.progressBarSearch);
            listView.ItemClick += ListEvent;
            GetMyCurrentLocation();
        }
        private void ListEvent(object sender, AdapterView.ItemClickEventArgs e)
        {
            var item = personel[e.Position];
            Intent intent = new Intent(this, typeof(DisplayActivity));
            intent.PutExtra("uid", item.Id);
            StartActivity(intent);
        }

        private void DisplayPersonels()
        {
            if (!string.IsNullOrEmpty(Longitude) || !string.IsNullOrEmpty(Latitude))
            {
                progressBar.Max = 100;
                progressBar.Progress = 100;
                progressBar.SecondaryProgress = 100;

                new Thread(new ThreadStart(delegate
                {
                    while (personel is null)
                    {
                        startStatus += 5;
                        endStatus -= 5;
                        progressBar.Progress = endStatus;
                        var list = Intent.GetStringExtra("list");
                        var personell = imotorCare.SearchForSpeciality(
                            list,
                            Longitude,
                            Latitude, 
                            6000);
                        personel = new SearchAdapter(personell);
                        RunOnUiThread(() => { listView.Adapter = personel; progressBar.Visibility = ViewStates.Gone; });
                    }
                })).Start();
            }
        }
        void GetLastLocationFromDevice()
        {
            var criteria = new Criteria { PowerRequirement = Power.Medium };

            var bestProvider = locationManager.GetBestProvider(criteria, true);
            var location = locationManager.GetLastKnownLocation(bestProvider);

            if (location != null)
            {
                var th = location.Latitude;
                var t = location.Longitude;
                DisplayPersonels();
            }
            else
            {
                Thread.Sleep(100);
            }
        }
        private void GetMyCurrentLocation()
        {
            locationManager = (LocationManager)GetSystemService(LocationService);
            Criteria criteria = new Criteria
            {
                Accuracy = Accuracy.Coarse,
                PowerRequirement = Power.Medium
            };
            IList<string> accpeptableLocationProviders = locationManager.GetProviders(criteria, true);
            locationProvider = accpeptableLocationProviders.Any() ? accpeptableLocationProviders.First() : string.Empty;
            GetLastLocationFromDevice();
        }
        protected override void OnResume()
        {
            base.OnResume();
            locationManager.RequestLocationUpdates(locationProvider, 0, 0, this);
        }
        protected override void OnPause()
        {
            base.OnPause();
            locationManager.RemoveUpdates(this);
        }
        public void OnLocationChanged(Location location)
        {
            try
            {
                currentLocation = location;
                if (currentLocation is null) { Toast.MakeText(this, "No location found", ToastLength.Long).Show(); }
                else
                {
                    Latitude = currentLocation.Latitude.ToString();
                    Longitude = currentLocation.Longitude.ToString();
                    DisplayPersonels();
                }
            }
            catch (Exception) { Toast.MakeText(this, "Address not found", ToastLength.Long).Show(); }
        }

        public void OnProviderDisabled(string provider) { }

        public void OnProviderEnabled(string provider) { }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras) { }
    }
}