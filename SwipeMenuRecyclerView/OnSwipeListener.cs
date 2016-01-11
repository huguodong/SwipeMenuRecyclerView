using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SwipeMenuRecyclerView
{
    public interface OnSwipeListener
    {
        void onSwipeStart(int position);
        void onSwipeEnd(int position);
    }
}
