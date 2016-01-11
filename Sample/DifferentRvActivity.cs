using System;
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
using System.Collections.Generic;

namespace Sample
{
    [Activity(Label = "DifferentRvActivity")]
    public class DifferentRvActivity : Activity, SwipeRefreshLayout.IOnRefreshListener
    {
        private static Context mContext;
        private List<User> users;
        private static AppAdapter mAdapter;
        private SwipeMenuRecyclerViews mRecyclerView;
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
            mRecyclerView.SetLayoutManager(new LinearLayoutManager(this));
            mRecyclerView.SetOpenInterpolator(new BounceInterpolator());
            mRecyclerView.SetCloseInterpolator(new BounceInterpolator());
            mAdapter = new AppAdapter(this, users);
            mRecyclerView.SetAdapter(mAdapter);
        }
        private List<User> GetUsers()
        {
            List<User> userList = new List<User>();
            for (int i = 0; i < 100; i++)
            {
                User user = new User();
                user.userId = i + 1000;
                user.userName = "Pobi " + (i + 1);
                userList.Add(user);
            }
            return userList;
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
        public class AppAdapter : RecyclerView.Adapter
        {
            public const int VIEW_TYPE_SIMPLE = 0;
            public const int VIEW_TYPE_DIFFERENT = 1;

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
                    return VIEW_TYPE_SIMPLE;
                }
                else {
                    return VIEW_TYPE_DIFFERENT;
                }
            }
            public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
            {
                User user = users[position];
                int viewType = GetItemViewType(position);
                switch (viewType)
                {
                    case VIEW_TYPE_SIMPLE:
                        FillSimpleView(holder, user);
                        break;
                    case VIEW_TYPE_DIFFERENT:
                        var a = holder as DifferentViewHolder;
                        FillDifferentView(a, user);
                        break;
                }


            }

            public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
            {
                switch (viewType)
                {
                    case VIEW_TYPE_SIMPLE:
                        View simpleView = LayoutInflater.From(mContext).Inflate(Resource.Layout.item_simple, parent, false);
                        return new NormalViewHolder(simpleView);
                    case VIEW_TYPE_DIFFERENT:
                        View differentView = LayoutInflater.From(mContext).Inflate(Resource.Layout.item_different, parent, false);
                        return new DifferentViewHolder(differentView);
                }

                return null;
            }

            private void FillDifferentView(DifferentViewHolder vh, User user)
            {
                DifferentViewHolder differentViewHolder = vh;
                SwipeMenuLayout itemView = (SwipeMenuLayout)differentViewHolder.ItemView.JavaCast<SwipeMenuLayout>();
                differentViewHolder.tvName.Text = user.userName;
                itemView.Click += delegate
                  {
                      Toast.MakeText(mContext, "Hi " + user.userName, ToastLength.Short).Show();
                  };
                differentViewHolder.btGood.Click += delegate
                { Toast.MakeText(differentViewHolder.ItemView.Context, "Good ", ToastLength.Short).Show(); };
                differentViewHolder.btFavorite.Click += delegate
                  { Toast.MakeText(differentViewHolder.ItemView.Context, "Good " + user.userName, ToastLength.Short).Show(); };
            }

            private void FillSimpleView(RecyclerView.ViewHolder vh, User user)
            {
                NormalViewHolder normalViewHolder = (NormalViewHolder)vh.JavaCast<NormalViewHolder>(); ;
                SwipeMenuLayout itemView = (SwipeMenuLayout)normalViewHolder.ItemView.JavaCast<SwipeMenuLayout>();
                itemView.Click += delegate { Toast.MakeText(mContext, "Hi " + user.userName, ToastLength.Short).Show(); };
                normalViewHolder.btGood.Click += delegate
                {
                    Toast.MakeText(normalViewHolder.ItemView.Context, "Good ", ToastLength.Short).Show();
                };
                normalViewHolder.btOpen.Click += delegate
                {
                    Toast.MakeText(mContext, "Open " + user.userName, ToastLength.Short).Show();
                };
                normalViewHolder.btDelete.Click += delegate
                {
                    try
                    {
                
                        users.RemoveAt(vh.AdapterPosition);
                        mAdapter.NotifyItemRemoved(vh.AdapterPosition);
                    }
                    catch
                    {
                        //有的时候会执行多次点击事件导致报错
                    }

                };
                normalViewHolder.tvName.Text = user.userName;
            }


            private void BtDelete_Click(object sender, EventArgs e)
            {
                throw new NotImplementedException();
            }
        }
        public class User
        {
            public int userId;
            public String userName;
        }
        public class NormalViewHolder : RecyclerView.ViewHolder
        {
            public TextView tvName;
            public TextView tvSwipeEnable;
            public View btGood;
            public View btOpen;
            public TextView btDelete;
            public NormalViewHolder(View itemView) : base(itemView)
            {
                tvName = itemView.FindViewById<TextView>(Resource.Id.tvName);
                tvSwipeEnable = itemView.FindViewById<TextView>(Resource.Id.tvSwipeEnable);
                btGood = itemView.FindViewById(Resource.Id.btGood);
                btOpen = itemView.FindViewById(Resource.Id.btOpen);
                btDelete = itemView.FindViewById<TextView>(Resource.Id.btDelete);
            }
        }

        public class DifferentViewHolder : RecyclerView.ViewHolder
        {
            public TextView tvName;
            public View btGood;
            public View btFavorite;
            public DifferentViewHolder(View itemView) : base(itemView)
            {
                tvName = (TextView)itemView.FindViewById(Resource.Id.tvName);
                btGood = itemView.FindViewById(Resource.Id.btGood);
                btFavorite = itemView.FindViewById(Resource.Id.btFavorite);
            }
        }
    }
}