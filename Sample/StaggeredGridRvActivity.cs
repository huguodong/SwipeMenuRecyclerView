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
using Android.Support.V4.Widget;
using SwipeMenuRecyclerView;
using Android.Support.V7.Widget;
using Android.Views.Animations;

namespace Sample
{
    [Activity(Label = "StaggeredGridRvActivity")]
    public class StaggeredGridRvActivity : Activity, SwipeRefreshLayout.IOnRefreshListener
    {
        private static Context mContext;
        private List<User> users;
        private static AppAdapter mAdapter;
        private static SwipeMenuRecyclerViews mRecyclerView;
        private SwipeRefreshLayout swipeRefreshLayout;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_list);
            mContext = this;
            users = GetUsers();
            swipeRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.swipeRefreshLayout);
            swipeRefreshLayout.SetOnRefreshListener(this);

            mRecyclerView = FindViewById<SwipeMenuRecyclerViews>(Resource.Id.listView);
            mRecyclerView.SetLayoutManager(new StaggeredGridLayoutManager(2, StaggeredGridLayoutManager.Vertical));
            mRecyclerView.AddItemDecoration(new StaggeredSpaceItemDecoration(15, 0, 15, 45));
            mRecyclerView.SetOpenInterpolator(new BounceInterpolator());
            mRecyclerView.SetCloseInterpolator(new BounceInterpolator());
            mAdapter = new AppAdapter(this, users);
            mRecyclerView.SetAdapter(mAdapter);
        }
        private List<User> GetUsers()
        {
            List<User> userList = new List<User>();
            Random random = new Random();
            for (int i = 0; i < 100; i++)
            {
                User user = new User();
                user.userId = i + 1000;
                user.userName = "Pobi " + (i + 1);
                int num = random.Next(4);
                if (num == 0)
                {
                    user.photoRes = Resource.Drawable.one;
                }
                else if (num == 1)
                {
                    user.photoRes = Resource.Drawable.two;
                }
                else if (num == 2)
                {
                    user.photoRes = Resource.Drawable.three;
                }
                else if (num == 3)
                {
                    user.photoRes = Resource.Drawable.four;
                }
                userList.Add(user);
            }
            return userList;
        }
        public class AppAdapter : RecyclerView.Adapter
        {
            private const int VIEW_TYPE_ENABLE = 0;
            private const int VIEW_TYPE_DISABLE = 1;
            List<User> users;
            public AppAdapter(Context context, List<User> users)
            {
                this.users = users;
            }
            public override long GetItemId(int position)
            {
                return position;
            }
            public override int ItemCount
            {
                get
                {
                    return users.Count;
                }
            }
            public override int GetItemViewType(int position)
            {
                User user = users[position];
                if (user.userId % 2 == 0)
                {
                    return VIEW_TYPE_DISABLE;
                }
                else {
                    return VIEW_TYPE_ENABLE;
                }
            }
            public Boolean SwipeEnableByViewType(int viewType)
            {
                if (viewType == VIEW_TYPE_ENABLE) return true;
                else if (viewType == VIEW_TYPE_DISABLE) return false;
                else return true; // default
            }

            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                User user = users[position];
                MyViewHolder myViewHolder = (MyViewHolder)holder;
                SwipeMenuLayout itemView = (SwipeMenuLayout)myViewHolder.ItemView.JavaCast<SwipeMenuLayout>();
                itemView.Click += delegate
                {
                    Toast.MakeText(mContext, "Hi " + user.userName, ToastLength.Short).Show();
                };
                myViewHolder.btOpen.Click += delegate { Toast.MakeText(mContext, "Open " + user.userName, ToastLength.Short).Show(); };
                myViewHolder.btDelete.Click += delegate
                {
                    try
                    {
                        users.Remove(users[holder.AdapterPosition]);
                        mAdapter.NotifyItemRemoved(holder.AdapterPosition);
                    }
                    catch
                    {
                        //有的时候会执行多次点击事件导致报错
                    }

                };
                myViewHolder.tvName.Text = user.userName;
                myViewHolder.ivPhoto.SetImageResource(user.photoRes);
                /**
                 * optional
                 */
                itemView.SetOpenInterpolator(mRecyclerView.GetOpenInterpolator());
                itemView.SetCloseInterpolator(mRecyclerView.GetCloseInterpolator());
            }

            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                View itemView = LayoutInflater.From(mContext).Inflate(Resource.Layout.item_staggered, parent, false);
                return new MyViewHolder(itemView);
            }
        }
        public class MyViewHolder : RecyclerView.ViewHolder
        {
            public TextView tvName;
            public ImageView ivPhoto;
            public View btGood;
            public View btOpen;
            public View btDelete;
            public MyViewHolder(View itemView) : base(itemView)
            {
                tvName = itemView.FindViewById<TextView>(Resource.Id.tvName);
                ivPhoto = itemView.FindViewById<ImageView>(Resource.Id.ivPhoto);
                btOpen = itemView.FindViewById(Resource.Id.btOpen);
                btDelete = itemView.FindViewById(Resource.Id.btDelete);
            }
        }
        public void OnRefresh()
        {
            Toast.MakeText(mContext, "Refresh success", ToastLength.Short).Show();
            swipeRefreshLayout.Refreshing = false;
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;

            if (id == Resource.Id.action_left)
            {
                mRecyclerView.SetSwipeDirection(SwipeMenuRecyclerViews.DIRECTION_LEFT);
                return true;
            }
            if (id == Resource.Id.action_right)
            {
                mRecyclerView.SetSwipeDirection(SwipeMenuRecyclerViews.DIRECTION_RIGHT);
                return true;
            }
            return base.OnOptionsItemSelected(item);
        }
        public class User
        {
            public int userId;
            public String userName;
            public int photoRes;
        }
    }
}