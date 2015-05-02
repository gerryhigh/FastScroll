using Android.Support.V7.Widget;

namespace FastScroll
{
	public abstract class BaseRecyclerAdapter : RecyclerView.Adapter
	{
		public abstract string GetTextToShowInBubble(int pos);
	}
}

