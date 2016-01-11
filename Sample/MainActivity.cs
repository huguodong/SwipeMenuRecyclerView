using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
namespace Sample
{
    [Activity(Label = "Sample", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity, View.IOnClickListener
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

        }
        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.button1:
                    StartActivity(new Intent(this, typeof(SimpleRvActivity)));
                    break;
                case Resource.Id.button2:
                    StartActivity(new Intent(this, typeof(DifferentRvActivity)));
                    break;
                case Resource.Id.button3:
                    StartActivity(new Intent(this, typeof(GridRvActivity)));
                    break;
                case Resource.Id.button4:
                    StartActivity(new Intent(this, typeof(StaggeredGridRvActivity)));
                    break;
            }
        }
    }
}

