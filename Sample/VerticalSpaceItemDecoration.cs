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
using Android.Support.V7.Widget;
using Android.Graphics;

namespace Sample
{
    public class VerticalSpaceItemDecoration : RecyclerView.ItemDecoration
    {
        private  int mVerticalSpaceHeight;
        public VerticalSpaceItemDecoration(int mVerticalSpaceHeight)
        {
            this.mVerticalSpaceHeight = mVerticalSpaceHeight;
        }
        public override void GetItemOffsets(Rect outRect, View view, RecyclerView parent, RecyclerView.State state)
        {
            outRect.Bottom = mVerticalSpaceHeight;
        }
    }
}