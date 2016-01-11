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
using Android.Support.V4.View;
using Android.Graphics;
using Android.Support.V4.Widget;
using Android.Util;
using Android.Content.Res;
using Android.Views.Animations;

namespace SwipeMenuRecyclerView
{
    public class SwipeMenuLayout : FrameLayout
    {
        #region
        private static int STATE_CLOSE = 0;
        private static int STATE_OPEN = 1;
        private static Boolean OVER_API_11 = Build.VERSION.SdkInt >= BuildVersionCodes.Honeycomb;
        private int mSwipeDirection;
        private View mContentView;
        private View mMenuView;
        private int mDownX;
        private int state = STATE_CLOSE;
        private GestureDetectorCompat mGestureDetector;
        private Android.Views.GestureDetector.IOnGestureListener mGestureListener;
        private static Boolean isFling;
        private ScrollerCompat mOpenScroller;
        private ScrollerCompat mCloseScroller;
        private int mBaseX;
        private IInterpolator mCloseInterpolator;
        private IInterpolator mOpenInterpolator;
        private static ViewConfiguration mViewConfiguration;
        private Boolean swipeEnable = true;
        private int animDuration;
        #endregion

        public SwipeMenuLayout(Context context) : this(context, null)
        {
        }
        public SwipeMenuLayout(Context context, IAttributeSet attrs) : this(context, attrs, 0)
        {
        }
        public SwipeMenuLayout(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        {
            TypedArray a = context.ObtainStyledAttributes(attrs, Resource.Styleable.SwipeMenu, 0, defStyleAttr);
            animDuration = a.GetInteger(Resource.Styleable.SwipeMenu_anim_duration, 500);
            a.Recycle();
        }

        protected override void OnFinishInflate()
        {
            base.OnFinishInflate();
            Clickable = true;
            mContentView = FindViewById(Resource.Id.smContentView);
            if (mContentView == null)
            {
                throw new Java.Lang.IllegalArgumentException("not find contentView by id smContentView");
            }
            mMenuView = FindViewById(Resource.Id.smMenuView);
            if (mMenuView == null)
            {
                throw new Java.Lang.IllegalArgumentException("not find menuView by id smMenuView");
            }
            mViewConfiguration = ViewConfiguration.Get(Context);
            Init();
        }

        public void SetSwipeDirection(int swipeDirection)
        {
            mSwipeDirection = swipeDirection;
        }
        public void Init()
        {
            mGestureListener = new mGesture();
            mGestureDetector = new GestureDetectorCompat(Context, mGestureListener);
            mCloseScroller = ScrollerCompat.Create(Context);
            mOpenScroller = ScrollerCompat.Create(Context);
        }
        public void SetCloseInterpolator(IInterpolator closeInterpolator)
        {
            mCloseInterpolator = closeInterpolator;
            if (mCloseInterpolator != null)
            {
                mCloseScroller = ScrollerCompat.Create(Context, mCloseInterpolator);
            }
        }
        public void SetOpenInterpolator(IInterpolator openInterpolator)
        {
            mOpenInterpolator = openInterpolator;
            if (mOpenInterpolator != null)
            {
                mOpenScroller = ScrollerCompat.Create(Context, mOpenInterpolator);
            }
        }

        public Boolean OnSwipe(MotionEvent e)
        {
            mGestureDetector.OnTouchEvent(e);
            switch (e.Action)
            {
                case MotionEventActions.Down:
                    mDownX = (int)e.GetX();
                    isFling = false;
                    break;
                case MotionEventActions.Move:

                    int dis = (int)(mDownX - e.GetX());
                    if (state == STATE_OPEN)
                    {
                        dis += mMenuView.Width * mSwipeDirection;
                    }
                    Swipe(dis);
                    break;
                case MotionEventActions.Up:
                    if ((isFling || Math.Abs(mDownX - e.GetX()) > (mMenuView.Width / 3)) &&
                            Java.Lang.Math.Signum(mDownX - e.GetX()) == mSwipeDirection)
                    {
                        SmoothOpenMenu();
                    }
                    else {
                        SmoothCloseMenu();
                        return false;
                    }
                    break;
            }
            return true;
        }
        public Boolean IsOpen()
        {
            return state == STATE_OPEN;
        }

        private void Swipe(int dis)
        {
            if (Java.Lang.Math.Signum(dis) != mSwipeDirection)
            {
                dis = 0;
            }
            else if (Math.Abs(dis) > mMenuView.Width)
            {
                dis = mMenuView.Width * mSwipeDirection;
                state = STATE_OPEN;
            }

            LayoutParams lp = (LayoutParams)mContentView.LayoutParameters;
            int lGap = PaddingLeft + lp.LeftMargin;
            mContentView.Layout(lGap - dis,
                    mContentView.Top,
                    lGap + (OVER_API_11 ? mContentView.MeasuredWidthAndState : mContentView.MeasuredWidth) - dis,
                    mContentView.Bottom);

            if (mSwipeDirection == SwipeMenuRecyclerViews.DIRECTION_LEFT)
            {
                mMenuView.Layout(MeasuredWidth - dis, mMenuView.Top,
                        MeasuredWidth + (OVER_API_11 ? mMenuView.MeasuredWidthAndState : mMenuView.MeasuredWidth) - dis,
                        mMenuView.Bottom);
            }
            else {
                mMenuView.Layout(-(OVER_API_11 ? mMenuView.MeasuredWidthAndState : mMenuView.MeasuredWidth) - dis, mMenuView.Top,
                        -dis, mMenuView.Bottom);
            }
        }

        public override void ComputeScroll()
        {
            if (state == STATE_OPEN)
            {
                if (mOpenScroller.ComputeScrollOffset())
                {
                    Swipe(mOpenScroller.CurrX * mSwipeDirection);
                    PostInvalidate();
                }
            }
            else {
                if (mCloseScroller.ComputeScrollOffset())
                {
                    Swipe((mBaseX - mCloseScroller.CurrX) * mSwipeDirection);
                    PostInvalidate();
                }
            }
        }
        public void SmoothCloseMenu()
        {
            CloseOpenedMenu();
        }
        public void CloseOpenedMenu()
        {
            state = STATE_CLOSE;
            if (mSwipeDirection == SwipeMenuRecyclerViews.DIRECTION_LEFT)
            {
                mBaseX = -mContentView.Left;
                mCloseScroller.StartScroll(0, 0, mMenuView.Width, 0, animDuration);
            }
            else {
                mBaseX = mMenuView.Right;
                mCloseScroller.StartScroll(0, 0, mMenuView.Width, 0, animDuration);
            }
            PostInvalidate();
        }
        public void SmoothOpenMenu()
        {
            state = STATE_OPEN;
            if (mSwipeDirection == SwipeMenuRecyclerViews.DIRECTION_LEFT)
            {
                mOpenScroller.StartScroll(-mContentView.Left, 0, mMenuView.Width, 0, animDuration);
            }
            else {
                mOpenScroller.StartScroll(mContentView.Left, 0, mMenuView.Width, 0, animDuration);
            }
            PostInvalidate();
        }
        public void CloseMenu()
        {
            if (mCloseScroller.ComputeScrollOffset())
            {
                mCloseScroller.AbortAnimation();
            }
            if (state == STATE_OPEN)
            {
                state = STATE_CLOSE;
                Swipe(0);
            }
        }

        public void OpenMenu()
        {
            if (state == STATE_CLOSE)
            {
                state = STATE_OPEN;
                Swipe(mMenuView.Width * mSwipeDirection);
            }
        }
        public View GetMenuView()
        {
            return mMenuView;
        }
        public View GetContentView()
        {
            return mContentView;
        }

        protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
        {
            FrameLayout.LayoutParams lp = (LayoutParams)mContentView.LayoutParameters;
            int lGap = PaddingLeft + lp.LeftMargin;
            int tGap = PaddingTop + lp.TopMargin;
            mContentView.Layout(lGap,
                    tGap,
                    lGap + (OVER_API_11 ? mContentView.MeasuredWidthAndState : mContentView.MeasuredWidth),
                    tGap + (OVER_API_11 ? mContentView.MeasuredHeightAndState : mContentView.MeasuredHeight));


            lp = (LayoutParams)mMenuView.LayoutParameters;
            tGap = PaddingTop + lp.TopMargin;
            if (mSwipeDirection == SwipeMenuRecyclerViews.DIRECTION_LEFT)
            {
                mMenuView.Layout(MeasuredWidth, tGap,
                        MeasuredWidth + (OVER_API_11 ? mMenuView.MeasuredWidthAndState : mMenuView.MeasuredWidth),
                        tGap + mMenuView.MeasuredHeightAndState);
            }
            else {
                mMenuView.Layout(-(OVER_API_11 ? mMenuView.MeasuredWidthAndState : mMenuView.MeasuredWidth), tGap,
                        0, tGap + mMenuView.MeasuredHeightAndState);
            }
        }
        public void SetSwipeEnable(Boolean swipeEnable)
        {
            this.swipeEnable = swipeEnable;
        }

        public Boolean IsSwipeEnable()
        {
            return swipeEnable;
        }
        public class mGesture : GestureDetector.SimpleOnGestureListener
        {
            public override bool OnDown(MotionEvent e)
            {
                isFling = false;
                return true;
            }
            public override bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
            {
                if (velocityX > mViewConfiguration.ScaledMinimumFlingVelocity || velocityY > mViewConfiguration.ScaledMinimumFlingVelocity)
                    isFling = true;
                return isFling;
            }


        }
    }
}