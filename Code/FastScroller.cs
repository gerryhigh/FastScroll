using System;

using Android.Content;
using Android.Views;
using Android.Widget;
using Android.Util;
using Android.Animation;

using Android.Support.V7.Widget;

namespace FastScroll
{
	public class FastScroller : LinearLayout
	{
		private static int BUBBLE_ANIMATION_DURATION=100;
		private static int TRACK_SNAP_RANGE=5;

		private TextView bubble;
		private View handle;
		private RecyclerView recyclerView;
		private MyScrollListener scrollListener;
		private int height;
		private ObjectAnimator currentAnimator = null;
			
		public FastScroller(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
		{
			init(context);
		}
		public FastScroller(Context context) : base(context)
		{
			init(context);
		}
		public FastScroller(Context context, IAttributeSet attrs) : base(context, attrs)
		{
			init(context);
		}

		private void init(Context context)
		{
			scrollListener = new MyScrollListener(this);

			Orientation = Orientation.Horizontal;
			SetClipChildren(false);
			LayoutInflater inflater = LayoutInflater.FromContext(context);
			inflater.Inflate(Resource.Layout.FastScroller, this, true);

			bubble = FindViewById<TextView>(Resource.Id.fastscroller_bubble);
			handle = FindViewById<View>(Resource.Id.fastscroller_handle);
			bubble.Visibility = ViewStates.Invisible;
		}

		protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
		{
			base.OnSizeChanged(w, h, oldw, oldh);
			height = h;
		}

		public override bool OnTouchEvent(MotionEvent e)
		{
			var action = e.Action;
			switch (action) {
			case MotionEventActions.Down:
				if (e.GetX() < handle.GetX())
					return false;
				if (currentAnimator != null)
					currentAnimator.Cancel();
				if (bubble.Visibility == ViewStates.Invisible)
					showBubble();
				handle.Selected = true;
				setPosition(e.GetY());
				setRecyclerViewPosition(e.GetY());
				return true;

			case MotionEventActions.Move:
				setPosition(e.GetY());
				setRecyclerViewPosition(e.GetY());
				return true;

			case MotionEventActions.Up:
			case MotionEventActions.Cancel:
				handle.Selected = false;
				hideBubble();
				return true;
			}

			return base.OnTouchEvent(e);
		}

		public void SetRecyclerView(RecyclerView rv)
		{
			this.recyclerView = rv;
			this.recyclerView.SetOnScrollListener(scrollListener);
		}

		private void setRecyclerViewPosition(float y)
		{
			if (recyclerView != null) {
				var itemCount = recyclerView.GetAdapter().ItemCount;
				float proportion;
				if ((int)handle.GetY() == 0)
					proportion = 0f;
				else if (handle.GetY() + handle.Height >= height - TRACK_SNAP_RANGE)
					proportion = 1f;
				else
					proportion = y / (float)height;

				int targetPos = getValueInRange(0, itemCount - 1, (int)(proportion * (float)itemCount));
				recyclerView.ScrollToPosition(targetPos);

				var adapter = recyclerView.GetAdapter() as BaseRecyclerAdapter;
				bubble.Text = adapter.GetTextToShowInBubble(targetPos);
			}
		}

		private int getValueInRange(int min,int max,int value)
		{
			int minimum=Math.Max(min,value);
			return Math.Min(minimum,max);
		}

		private void setPosition(float y)
		{
			int bubbleHeight=bubble.Height;
			int handleHeight=handle.Height;
			handle.SetY(getValueInRange(0,height-handleHeight,(int)(y-handleHeight/2)));
			bubble.SetY(getValueInRange(0,height-bubbleHeight-handleHeight/2,(int)(y-bubbleHeight)));
		}

		private void showBubble()
		{
			bubble.Visibility = ViewStates.Visible;
			if(currentAnimator!=null)
				currentAnimator.Cancel();
			currentAnimator = (ObjectAnimator)ObjectAnimator.OfFloat(bubble, "alpha", 0f, 1f).SetDuration(BUBBLE_ANIMATION_DURATION);
			currentAnimator.Start();
		}

		private void hideBubble()
		{
			if(currentAnimator != null)
				currentAnimator.Cancel();
			currentAnimator = (ObjectAnimator)ObjectAnimator.OfFloat(bubble,"alpha",1f,0f).SetDuration(BUBBLE_ANIMATION_DURATION);
			currentAnimator.AddListener(new MyListener(this));
			currentAnimator.Start();
		}

		internal class MyListener : AnimatorListenerAdapter
		{
			private FastScroller scroll;

			public MyListener(FastScroller scroller)
			{
				this.scroll = scroller;
			}

			public override void OnAnimationEnd(Animator animation)
			{
				base.OnAnimationEnd(animation);
				scroll.bubble.Visibility = ViewStates.Invisible;
				scroll.currentAnimator = null;
			}
			public override void OnAnimationCancel(Animator animation)
			{
				base.OnAnimationCancel(animation);
				scroll.bubble.Visibility = ViewStates.Invisible;
				scroll.currentAnimator = null;
			}
		}

		internal class MyScrollListener : RecyclerView.OnScrollListener
		{
			private readonly FastScroller scroll;

			public MyScrollListener(FastScroller scroller)
			{
				this.scroll = scroller;
			}

			public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
			{
				View firstVisibleView 	= recyclerView.GetChildAt(0);
				int firstVisiblePosition = recyclerView.GetChildPosition(firstVisibleView);
				int visibleRange 		= recyclerView.ChildCount;
				int lastVisiblePosition = firstVisiblePosition + visibleRange;
				int itemCount 			= recyclerView.GetAdapter().ItemCount;
				int position;

				if(firstVisiblePosition==0)
					position=0;
				else if(lastVisiblePosition==itemCount-1)
					position = itemCount-1;
				else
					position = firstVisiblePosition;
				
				float proportion=(float)position/(float)itemCount;
				this.scroll.setPosition(scroll.height*proportion);
			}
		}
	}
}

