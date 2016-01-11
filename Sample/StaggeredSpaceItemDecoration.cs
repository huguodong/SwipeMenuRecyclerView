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
    public class StaggeredSpaceItemDecoration : RecyclerView.ItemDecoration
    {
        private int left;
        private int top;
        private int right;
        private int bottom;

        public StaggeredSpaceItemDecoration(int left, int top, int right, int bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }
        public override void GetItemOffsets(Rect outRect, View view, RecyclerView parent, RecyclerView.State state)
        {
            outRect.Left = left;
            outRect.Top = top;
            outRect.Right = right;
            outRect.Bottom = bottom;
        }
    }
}