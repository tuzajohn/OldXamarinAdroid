using System;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using System.Collections.Generic;
using Android.Util;
using Android.Locations;
using System.Linq;
using Android.Runtime;
using Android.Content.PM;
using Android;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Content;
using System.Threading.Tasks;
using System.Text;
using System.Timers;
using System.Threading;

[assembly: UsesFeature(Android.Content.PM.PackageManager.FeatureTelephony)]
[assembly: UsesPermission(Manifest.Permission.CallPhone)]
namespace OldXamarinAdroid
{
	[Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = false)]
	public class MainActivity : AppCompatActivity, ILocationListener
    {
        ListView listView;
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        Location currentLocation;
        LocationManager locationManager;
        string locationProvider;
        ImotorCareWebApi.ImotorCareWebApi imotorCare;
        ProgressBar progressBar;
        int startStatus = 0, endStatus = 100;
        SeekBar seekBar;

        PersonelAdatper personel;
        protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);            
			SetContentView(Resource.Layout.activity_main);
			Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
			FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;
            seekBar = FindViewById<SeekBar>(Resource.Id.seekbar);
            imotorCare = new ImotorCareWebApi.ImotorCareWebApi();
            listView = FindViewById<ListView>(Resource.Id.listView);
            progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);
            progressBar.IncrementProgressBy(10);
            listView.ItemClick += ListEvent;
            GetMyCurrentLocation();
            seekBar.ProgressChanged += seekbarAction;
        }

        private void seekbarAction(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            progressBar.Visibility = ViewStates.Visible;
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
                        var type = Intent.GetStringExtra("type");
                        var personell = imotorCare.GetAllFromTypeWithDistance(Longitude,
                            Latitude, type, (e.Progress * 1000));
                        personel = new PersonelAdatper(personell);
                        RunOnUiThread(() => { listView.Adapter = personel; progressBar.Visibility = ViewStates.Gone; });
                    }
                })).Start();
            }
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
                        var type = Intent.GetStringExtra("type");
                        var personell = imotorCare.GetAllFromTypeWithDistance(Longitude,
                            Latitude, type, 60000);
                        personel = new PersonelAdatper(personell);
                        RunOnUiThread(()=>{ listView.Adapter = personel; progressBar.Visibility = ViewStates.Gone; });
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
                Latitude = location.Latitude.ToString();
                Longitude = location.Longitude.ToString();
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
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            return id == Resource.Id.action_settings ? true : base.OnOptionsItemSelected(item);
        }
        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View) sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
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
                    //var personell = imotorCare.GetAllFromTypeWithDistance(Longitude,
                    //Latitude, "mechanic", 500);

                    //listView.Adapter = new PersonelAdatper(personell);
                }
            }
            catch (Exception) { Toast.MakeText(this, "Address not found", ToastLength.Long).Show(); }
        }
        public void OnProviderDisabled(string provider) { }
        public void OnProviderEnabled(string provider) { }
        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras) { }
    }
}

