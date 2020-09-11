using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using OldXamarinAdroid.ImotorCareWebApi;

namespace OldXamarinAdroid
{
    [Activity(Label = "DisplayActivity")]
    public class DisplayActivity : Activity
    {
        ImageView imageView;
        TextView nameText, garageText, textEvent, mRatingscale;
        RatingBar ratingBar;
        EditText commentText;
        Button button2;
        ImotorCareWebApi.ImotorCareWebApi api;
        public string Id { get; set; }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.DisplayPerson);
            // Create your application here
            imageView = FindViewById<ImageView>(Resource.Id.imgid);
            nameText = FindViewById<TextView>(Resource.Id.name);
            garageText = FindViewById<TextView>(Resource.Id.garagename);
            textEvent = FindViewById<TextView>(Resource.Id.linkReview);
            mRatingscale = FindViewById<TextView>(Resource.Id.textView6);
            commentText = FindViewById<EditText>(Resource.Id.editText2);
            button2 = FindViewById<Button>(Resource.Id.button2);
            ratingBar = FindViewById<RatingBar>(Resource.Id.ratingBar);
            api = new ImotorCareWebApi.ImotorCareWebApi();
            Id = Intent.GetStringExtra("uid");
            Load(api.GetPersonelByID(Id));
            button2.Click += RatePersonel;
            textEvent.Click += LoadReviews;
            ratingBar.Click += SetRatingText;
        }
        private void SetRatingText(object sender, EventArgs e)
        {
            switch (ratingBar.Rating)
            {
                case 1: mRatingscale.Text = "Very Bad";break;
                case 2: mRatingscale.Text = "Need some Improvement"; break;
                case 3: mRatingscale.Text = "Good"; break;
                case 4: mRatingscale.Text = "Great"; break;
                case 5: mRatingscale.Text = "Awesome. I loved it"; break;
                default: mRatingscale.Text = ""; break;
            }
        }
        
        private void LoadReviews(object sender, EventArgs e)
        {
            var listview = new ListView(this);
            listview.Adapter = new ReviewsAdapter(api.Personel_Ratings(Id));
            Android.App.AlertDialog.Builder dialog = new Android.App.AlertDialog.Builder(this);
            Android.App.AlertDialog alert = dialog.Create();
            alert.SetTitle("Reviews for " + nameText.Text);
            alert.SetView(listview);
            //alert.SetIcon(Resource.Drawable.alert);
            alert.SetButton("OK", (c, ev) =>
            {
                alert.Cancel();
            });
            alert.SetButton2("CANCEL", (c, ev) => { });
            alert.Show();
        }

        private void RatePersonel(object sender, EventArgs e)
        {
            var numOfStars = ratingBar.Rating;
            var response = api.RatePersonel(Id, commentText.Text, numOfStars.ToString());
            Android.App.AlertDialog.Builder dialog = new Android.App.AlertDialog.Builder(this);
            Android.App.AlertDialog alert = dialog.Create();
            var title = response.ToLower().Contains("success") ? "Success" : "Error";
            alert.SetTitle(title);
            alert.SetMessage(response);
            //alert.SetIcon(Resource.Drawable.alert);
            alert.SetButton("OK", (c, ev) =>
            {
                alert.Cancel();
                Load(api.GetPersonelByID(Id));
            });
            alert.SetButton2("CANCEL", (c, ev) => { });
            alert.Show();
            ratingBar.Rating = 0;
            commentText.Text = string.Empty;
        }

        private void Load(Personel_table personel)
        {
            imageView.SetImageBitmap(Base64ToBitmap(personel.Byte_image));
            nameText.Text = personel.Fname + " " + personel.Lname;
            garageText.Text = personel.Garage_name + "; " + personel.Garage_address;
        }
        public Bitmap Base64ToBitmap(string base64String)
        {
            byte[] imageAsBytes = Base64.Decode(base64String, Base64Flags.Default);
            return BitmapFactory.DecodeByteArray(imageAsBytes, 0, imageAsBytes.Length);
        }
    }
}