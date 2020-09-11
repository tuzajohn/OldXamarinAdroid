using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.IO;
using Java.Lang;
using OldXamarinAdroid.ImotorCareWebApi;

namespace OldXamarinAdroid
{
    public class PersonelAdatper : BaseAdapter<Personel_table>
    {
        private Personel_table[] personel_table;
        
        public PersonelAdatper(Personel_table[] personel_table)
        {
            this.personel_table = personel_table;
        }

        public override Personel_table this[int position] => personel_table[position];

        public override int Count => personel_table.Length;

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView;

            if (view == null)
            {
                view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.PersonelLayout, parent, false);

                var photo = view.FindViewById<ImageView>(Resource.Id.imageView);
                var name = view.FindViewById<TextView>(Resource.Id.name);
                var garage = view.FindViewById<TextView>(Resource.Id.garage);
                var trash = view.FindViewById<ImageButton>(Resource.Id.trash);
                var rate = view.FindViewById<RatingBar>(Resource.Id.setRating);
                
                trash.Focusable = false;
                trash.FocusableInTouchMode = false;
                trash.Clickable = true;

                view.Tag = new ViewHolder() {
                    Photo = photo,
                    Name = name,
                    Garage = garage,
                    Trash = trash,
                    Rate = rate};
            }

            var holder = (ViewHolder)view.Tag;
            var imageManager = new ImageManager();
            holder.Photo.SetImageBitmap(imageManager.Base64ToBitmap(personel_table[position].Byte_image));
            holder.Name.Text = personel_table[position].Fname + " " + personel_table[position].Lname;
            holder.Garage.Text = personel_table[position].Garage_name + ": " + personel_table[position].Garage_address;
            holder.Rate.Rating = 4;

            holder.Trash.Click += delegate
            {
                var intent = new Intent(Intent.ActionCall);
                intent.SetData(Android.Net.Uri.Parse("tel:" + personel_table[position].Contact));
                parent.Context.StartActivity(intent);
            };
            
            return view;
        }
    }
    public class ImageManager
    {
        static Dictionary<string, Drawable> cache = new Dictionary<string, Drawable>();

        public static Drawable Get(Context context, string url)
        {
            if (!cache.ContainsKey(url))
            {
                var drawable = Drawable.CreateFromStream(context.Assets.Open(url), null);

                cache.Add(url, drawable);
            }

            return cache[url];
        }
        public Bitmap Base64ToBitmap(string base64String)
        {
            byte[] imageAsBytes = Base64.Decode(base64String, Base64Flags.Default);
            return BitmapFactory.DecodeByteArray(imageAsBytes, 0, imageAsBytes.Length);
        }
        //public string BitmapToBase64(Bitmap bitmap)
        //{
        //    ByteArrayOutputStream byteArrayOutputStream = new ByteArrayOutputStream();
        //    bitmap.Compress(Bitmap.CompressFormat.Png, 100, byteArrayOutputStream);
        //    byte[] byteArray = byteArrayOutputStream.ToByteArray();
        //    return Base64.EncodeToString(byteArray, Base64Flags.Default);
        //}
    }
    public class ViewHolder : Java.Lang.Object
    {
        public ImageView Photo { get; set; }
        public TextView Name { get; set; }
        public TextView Garage { get; set; }
        public RatingBar Rate { get; set; }
        public ImageButton Trash { get; set; }
    }
}