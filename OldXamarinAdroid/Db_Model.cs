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
    public class Personel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string Garage { get; set; }
        public string Speciality { get; set; }
        public string Contact { get; set; }
        public string Type { get; set; }
        public string ImageByte { get; set; }

        public Personel()
        {

        }
    }
}