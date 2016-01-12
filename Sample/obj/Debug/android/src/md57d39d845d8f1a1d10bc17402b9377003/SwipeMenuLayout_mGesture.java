package md57d39d845d8f1a1d10bc17402b9377003;


public class SwipeMenuLayout_mGesture
	extends android.view.GestureDetector.SimpleOnGestureListener
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onDown:(Landroid/view/MotionEvent;)Z:GetOnDown_Landroid_view_MotionEvent_Handler\n" +
			"n_onFling:(Landroid/view/MotionEvent;Landroid/view/MotionEvent;FF)Z:GetOnFling_Landroid_view_MotionEvent_Landroid_view_MotionEvent_FFHandler\n" +
			"";
		mono.android.Runtime.register ("SwipeMenuRecyclerView.SwipeMenuLayout+mGesture, SwipeMenuRecyclerView, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", SwipeMenuLayout_mGesture.class, __md_methods);
	}


	public SwipeMenuLayout_mGesture () throws java.lang.Throwable
	{
		super ();
		if (getClass () == SwipeMenuLayout_mGesture.class)
			mono.android.TypeManager.Activate ("SwipeMenuRecyclerView.SwipeMenuLayout+mGesture, SwipeMenuRecyclerView, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public boolean onDown (android.view.MotionEvent p0)
	{
		return n_onDown (p0);
	}

	private native boolean n_onDown (android.view.MotionEvent p0);


	public boolean onFling (android.view.MotionEvent p0, android.view.MotionEvent p1, float p2, float p3)
	{
		return n_onFling (p0, p1, p2, p3);
	}

	private native boolean n_onFling (android.view.MotionEvent p0, android.view.MotionEvent p1, float p2, float p3);

	java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
