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
    class SearchAdapter : BaseAdapter<Personel_rating_view>
    {
        private Personel_rating_view[] personell;

        public SearchAdapter(Personel_rating_view[] personell)
        {
            this.personell = personell;
        }

        public override Personel_rating_view this[int position] => personell[position];

        public override int Count => personell.Length;

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView;

            if (view == null)
            {
                view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.SearchRow, parent, false);

                var photo = view.FindViewById<ImageView>(Resource.Id.imageView);
                var name = view.FindViewById<TextView>(Resource.Id.name);
                var garage = view.FindViewById<TextView>(Resource.Id.garage);
                var trash = view.FindViewById<ImageButton>(Resource.Id.trash);
                var rate = view.FindViewById<RatingBar>(Resource.Id.setRating);

                trash.Focusable = false;
                trash.FocusableInTouchMode = false;
                trash.Clickable = true;

                view.Tag = new ViewHolderSearch()
                {
                    Photo = photo,
                    Name = name,
                    Garage = garage,
                    Trash = trash,
                    Rate = rate
                };
            }

            var holder = (ViewHolderSearch)view.Tag;
            var imageManager = new ImageManager_();
            holder.Photo.SetImageBitmap(imageManager.Base64ToBitmap(personell[position].Byte_image));
            holder.Name.Text = personell[position].Personel_name;
            holder.Garage.Text = personell[position].Garage_details;
            holder.Rate.Rating = float.Parse(personell[position].TotalStars1);

            holder.Trash.Click += delegate
            {
                var intent = new Intent(Intent.ActionCall);
                intent.SetData(Android.Net.Uri.Parse("tel:" + personell[position].Contact));
                parent.Context.StartActivity(intent);
            };

            return view;
        }
    }
    public class ImageManager_
    {
        public Bitmap Base64ToBitmap(string base64String)
        {
            byte[] imageAsBytes = Base64.Decode(base64String, Base64Flags.Default);
            return BitmapFactory.DecodeByteArray(imageAsBytes, 0, imageAsBytes.Length);
        }
    }
    public class ViewHolderSearch : Java.Lang.Object
    {
        public ImageView Photo { get; set; }
        public TextView Name { get; set; }
        public TextView Garage { get; set; }
        public RatingBar Rate { get; set; }
        public ImageButton Trash { get; set; }
    }
}