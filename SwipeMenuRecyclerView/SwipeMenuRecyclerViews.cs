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
using Android.Views.Animations;
using Android.Util;
using Android.Support.V4.View;

namespace SwipeMenuRecyclerView
{
    public class SwipeMenuRecyclerViews : RecyclerView
    {
        #region
        public static int TOUCH_STATE_NONE = 0;
        public static int TOUCH_STATE_X = 1;
        public static int TOUCH_STATE_Y = 2;

        public static int DIRECTION_LEFT = 1;
        public static int DIRECTION_RIGHT = -1;
        protected int mDirection = DIRECTION_LEFT; // swipe from right to left by default

        protected float mDownX;
        protected float mDownY;
        protected int mTouchState;
        protected int mTouchPosition;
        protected SwipeMenuLayout mTouchView;
        protected OnSwipeListener mOnSwipeListener;

        protected IInterpolator mCloseInterpolator;
        protected IInterpolator mOpenInterpolator;

        protected LayoutManager mLlm;
        protected ViewConfiguration mViewConfiguration;
        protected long startClickTime;
        protected float dx;
        protected float dy;


        #endregion
        public SwipeMenuRecyclerViews(Context context) : this(context, null)
        {
        }
        public SwipeMenuRecyclerViews(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
            Init();
        }
        public SwipeMenuRecyclerViews(Context context, IAttributeSet attrs) : this(context, attrs, 0)
        {
        }
        protected void Init()
        {
            mTouchState = TOUCH_STATE_NONE;
            mViewConfiguration = ViewConfiguration.Get(Context);
        }
        public void SetCloseInterpolator(IInterpolator interpolator)
        {
            mCloseInterpolator = interpolator;
        }
        public void SetOpenInterpolator(IInterpolator interpolator)
        {
            mOpenInterpolator = interpolator;
        }
        public IInterpolator GetOpenInterpolator()
        {
            return mOpenInterpolator;
        }
        public IInterpolator GetCloseInterpolator()
        {
            return mCloseInterpolator;
        }
        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            if (ev.Action != MotionEventActions.Down && mTouchView == null)
            {
                return base.OnInterceptTouchEvent(ev);
            }

            int action = (int)ev.Action;
            switch (action & MotionEventCompat.ActionMask)
            {
                case (int)MotionEventActions.Down:
                    dx = 0.0f; // reset
                    dy = 0.0f; // reset
                    startClickTime = Java.Lang.JavaSystem.CurrentTimeMillis(); // reset
                    int oldPos = mTouchPosition;
                    mDownX = ev.GetX();
                    mDownY = ev.GetY();
                    mTouchState = TOUCH_STATE_NONE;
                    mTouchPosition = GetChildAdapterPosition(FindChildViewUnder((int)ev.GetX(), (int)ev.GetY()));
                    if (mTouchPosition == oldPos && mTouchView != null
                            && mTouchView.IsOpen())
                    {
                        mTouchState = TOUCH_STATE_X;
                        mTouchView.OnSwipe(ev);
                    }
                    // find the touched child view
                    View view = null;
                    ViewHolder vh = FindViewHolderForAdapterPosition(mTouchPosition);
                    if (vh != null)
                    {
                        view = vh.ItemView;
                    }
                    // is not touched the opened menu view, so we intercept this touch event
                    if (mTouchPosition != oldPos && mTouchView != null && mTouchView.IsOpen())
                    {
                        mTouchView.SmoothCloseMenu();
                        mTouchView = null;
                        // try to cancel the touch event
                        MotionEvent cancelEvent = MotionEvent.Obtain(ev);
                        cancelEvent.Action = MotionEventActions.Cancel;
                        base.OnTouchEvent(cancelEvent);
                        return true;
                    }
                    if (view is SwipeMenuLayout)
                    {
                        mTouchView = (SwipeMenuLayout)view;
                        mTouchView.SetSwipeDirection(mDirection);
                    }
                    if (mTouchView != null)
                    {
                        mTouchView.OnSwipe(ev);
                    }
                    break;
                case (int)MotionEventActions.Move:
                    dy = Math.Abs((ev.GetY() - mDownY));
                    dx = Math.Abs((ev.GetX() - mDownX));
                    if (mTouchState == TOUCH_STATE_X && mTouchView.IsSwipeEnable())
                    {
                        mTouchView.OnSwipe(ev);
                        ev.Action = MotionEventActions.Cancel;
                        base.OnTouchEvent(ev);
                    }
                    else if (mTouchState == TOUCH_STATE_NONE && mTouchView.IsSwipeEnable())
                    {
                        if (Math.Abs(dy) > mViewConfiguration.ScaledTouchSlop)
                        {
                            mTouchState = TOUCH_STATE_Y;
                        }
                        else if (dx > mViewConfiguration.ScaledTouchSlop)
                        {
                            mTouchState = TOUCH_STATE_X;
                            if (mOnSwipeListener != null)
                            {
                                mOnSwipeListener.onSwipeStart(mTouchPosition);
                            }
                        }
                    }
                    break;
                case (int)MotionEventActions.Up:
                    Boolean isCloseOnUpEvent = false;
                    if (mTouchState == TOUCH_STATE_X && mTouchView.IsSwipeEnable())
                    {
                        isCloseOnUpEvent = !mTouchView.OnSwipe(ev);
                        if (mOnSwipeListener != null)
                        {
                            mOnSwipeListener.onSwipeEnd(mTouchPosition);
                        }
                        if (!mTouchView.IsOpen())
                        {
                            mTouchPosition = -1;
                            mTouchView = null;
                        }
                        ev.Action = MotionEventActions.Cancel;
                        base.OnTouchEvent(ev);
                    }
                    long clickDuration = Java.Lang.JavaSystem.CurrentTimeMillis() - startClickTime;
                    Boolean isOutDuration = clickDuration > ViewConfiguration.LongPressTimeout;
                    Boolean isOutX = dx > mViewConfiguration.ScaledTouchSlop;
                    Boolean isOutY = dy > mViewConfiguration.ScaledTouchSlop;
                    // long pressed or scaled touch, we just intercept up touch event
                    if (isOutDuration || isOutX || isOutY)
                    {
                        return true;
                    }
                    else {
                        float eX = ev.GetX();
                        float eY = ev.GetY();
                        View upView = FindChildViewUnder(eX, eY);
                        if (upView is SwipeMenuLayout)
                        {
                            SwipeMenuLayout smView = (SwipeMenuLayout)upView;
                            int x = (int)eX - smView.Left;
                            int y = (int)eY - smView.Top;
                            View menuView = smView.GetMenuView();
                            float translationX = ViewCompat.GetTranslationX(menuView);
                            float translationY = ViewCompat.GetTranslationY(menuView);
                            // intercept the up event when touched on the contentView of the opened SwipeMenuLayout
                            if (!(x >= menuView.Left + translationX &&
                                    x <= menuView.Right + translationX &&
                                    y >= menuView.Top + translationY &&
                                    y <= menuView.Bottom + translationY) &&
                                    isCloseOnUpEvent)
                            {
                                return true;
                            }
                        }
                    }
                    break;
                case (int)MotionEventActions.Cancel:
                    if (mTouchView != null && mTouchView.IsSwipeEnable())
                    {
                        // when event has canceled, we just consider as up event
                        ev.Action = MotionEventActions.Up;
                        mTouchView.OnSwipe(ev);
                    }
                    break;
            }
            return base.OnInterceptTouchEvent(ev);
        }
        public void SmoothOpenMenu(int position)
        {
            View view = mLlm.FindViewByPosition(position);
            if (view is SwipeMenuLayout) {
                mTouchPosition = position;
                // close pre opened swipe menu
                if (mTouchView != null && mTouchView.IsOpen())
                {
                    mTouchView.SmoothCloseMenu();
                }
                mTouchView = (SwipeMenuLayout)view;
                mTouchView.SetSwipeDirection(mDirection);
                mTouchView.SmoothOpenMenu();
            }
        }

        /**
    * close the opened menu manually
    */
        public void SmoothCloseMenu()
        {
            // close the opened swipe menu
            if (mTouchView != null && mTouchView.IsOpen())
            {
                mTouchView.SmoothCloseMenu();
            }
        }
        public void SetOnSwipeListener(OnSwipeListener onSwipeListener)
        {
            this.mOnSwipeListener = onSwipeListener;
        }

        /**
         * get current touched view
         * @return touched view, maybe null
         */
        public SwipeMenuLayout GetTouchView()
        {
            return mTouchView;
        }


        /**
         * set the swipe direction
         * @param direction swipe direction (left or right)
         */
        public void SetSwipeDirection(int direction)
        {
            mDirection = direction;
        }

        public override void SetLayoutManager(LayoutManager layout)
        {
            base.SetLayoutManager(layout);
            mLlm = layout;
        }
    }
}