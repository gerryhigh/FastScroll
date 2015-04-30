using System;

using Android.Views;
using Android.Widget;

using Android.Support.V7.Widget;

namespace FastScroll
{
	public class LargeAdapter : RecyclerView.Adapter
	{
		public class MyView : RecyclerView.ViewHolder, View.IOnClickListener
		{
			public TextView Name 		{ get; set; }

			public MyView(View view) : base(view)
			{
				view.SetOnClickListener(this);
			}

			public void OnClick(View v)
			{
				Console.WriteLine("in onclick:" + this.Name.Text);
			}
		}
			
		public LargeAdapter()
		{
		}

		private System.Collections.ObjectModel.ObservableCollection<string> items;

		public LargeAdapter(System.Collections.ObjectModel.ObservableCollection<string> data)
		{
			this.items = data;
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			View row = LayoutInflater.From(parent.Context).Inflate(Android.Resource.Layout.SimpleListItem1, parent, false);

			TextView name = row.FindViewById<TextView>(Android.Resource.Id.Text1);

			MyView view = new MyView(row){ Name = name };
			return view;
		}

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			MyView hold = holder as MyView;
			hold.Name.Text = items[position];
		}

		public override int ItemCount {
			get {
				return this.items.Count;
			}
		}

		public string GetTextToShowInBubble(int pos)
		{
			return items[pos][0].ToString();
		}
	}
}

