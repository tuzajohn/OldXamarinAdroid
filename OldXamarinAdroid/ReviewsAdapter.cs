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
using OldXamarinAdroid.ImotorCareWebApi;

namespace OldXamarinAdroid
{
    public class ReviewsAdapter : BaseAdapter<Rating_table>
    {
        Context context;
        private Rating_table[] rating_table;
        
        public ReviewsAdapter(Rating_table[] rating_table)
        {
            this.rating_table = rating_table;
        }
        public override Rating_table this[int position] => rating_table[position];

        public override int Count => rating_table.Length;
        public override Java.Lang.Object GetItem(int position)
        {
            return position;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView;

            if (view == null)
            {
                view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.ReviewRowLayout, parent, false);
                var stars = view.FindViewById<TextView>(Resource.Id.ratevslue);
                var comment = view.FindViewById<TextView>(Resource.Id.reviewcomment);
                var rating = view.FindViewById<RatingBar>(Resource.Id.ratingBar2);
                view.Tag = new ReviewsAdapterViewHolder
                {
                    NumberOfStars = stars,
                    Comment = comment,
                    Rating = rating
                };
            }
            var holder = (ReviewsAdapterViewHolder)view.Tag;
            holder.NumberOfStars.Text = rating_table[position].Stars;
            holder.Comment.Text = rating_table[position].Comment;
            holder.Rating.Rating = float.Parse(rating_table[position].Stars);

            return view;
        }

    }

    public class ReviewsAdapterViewHolder : Java.Lang.Object
    {
        public TextView Comment { get; set; }
        public TextView NumberOfStars { get; set; }
        public RatingBar Rating { get; set; }
    }
}