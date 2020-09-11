using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace OldXamarinAdroid
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class SelectionActivity : Activity
    {
        ImageView imageMechan, imageBreak;
        EditText specialityBox;
        Button button;
        ImotorCareWebApi.ImotorCareWebApi api;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SelectionLayout);
            // Create your application here
            imageMechan = FindViewById<ImageView>(Resource.Id.imageMechan);
            imageBreak = FindViewById<ImageView>(Resource.Id.imageBreak);
            specialityBox = FindViewById<EditText>(Resource.Id.specialityText);
            button = FindViewById<Button>(Resource.Id.button);

            api = new ImotorCareWebApi.ImotorCareWebApi();
            
            imageMechan.Click += delegate
            {
                Intent intent = new Intent(this, typeof(MainActivity));
                intent.PutExtra("type", "mechanic");
                StartActivity(intent);
            };
            imageBreak.Click += delegate
            {
                Intent intent = new Intent(this, typeof(MainActivity));
                intent.PutExtra("type", "breakdown");
                StartActivity(intent);
            };
            button.Click += delegate
            {
                if (!string.IsNullOrEmpty(specialityBox.Text))
                {
                    FilterEvent();
                }
                else
                {
                    Android.App.AlertDialog.Builder dialog = new Android.App.AlertDialog.Builder(this);
                    Android.App.AlertDialog alert = dialog.Create();
                    alert.SetTitle("Input Errror");
                    alert.SetMessage("You must place a description");
                    //alert.SetIcon(Resource.Drawable.alert);
                    alert.SetButton("OK", (c, ev) => { alert.Cancel(); });
                    alert.Show();
                }
            };
        }

        private void FilterEvent()
        {

            var textInput = specialityBox.Text.Replace(",", "").Replace(".", "").Split().ToList();
            var list = new List<string>();
            foreach (var set in api.GetDataSet())
            {
                var result = set.WordList.Where(word => textInput.Any(txt => txt == word)).ToList();
                foreach (var item in result)
                {
                    list.Add(item);
                }
            }
            StringBuilder builder = new StringBuilder();
            if (list.Count > 0) { builder.Append("tyres, breakdown"); }
            for (int i = 0; i < list.Count - 1; i++) { builder.Append(list[i] + ", "); }
            builder.Append(list[list.Count - 1]);
            specialityBox.Text = "";
            var intent = new Intent(this, typeof(SearchActivity));
            intent.PutExtra("list", builder.ToString());
            StartActivity(intent);
        }
    }
}