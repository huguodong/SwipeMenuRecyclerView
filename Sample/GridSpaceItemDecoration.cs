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
    public class GridSpaceItemDecoration : RecyclerView.ItemDecoration
    {
        private int mVerticalSpaceHeight;
        private int mHorizontalSpaceHeight;

        public GridSpaceItemDecoration(int verticalSpaceHeight, int horizontalSpaceHeight)
        {
            this.mVerticalSpaceHeight = verticalSpaceHeight;
            this.mHorizontalSpaceHeight = horizontalSpaceHeight;
        }
        public override void GetItemOffsets(Rect outRect, View view, RecyclerView parent, RecyclerView.State state)
        {
            if (parent.GetChildAdapterPosition(view) % 2 == 1) outRect.Left = mHorizontalSpaceHeight;
            outRect.Bottom = mVerticalSpaceHeight;
        }
    }
}