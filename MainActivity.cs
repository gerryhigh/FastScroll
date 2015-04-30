using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Android.App;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace FastScroll
{
	[Activity(Label = "FastScroll", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : ActionBarActivity
	{
		const int SIZE = 5000;

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			var recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerview);

			ObservableCollection<string> items = new ObservableCollection<string>();
			Random r = new Random();
			for (int i = 0; i < SIZE; i++) {
				items.Add(((char)('A' + r.Next('Z' - 'A'))) + " " + i.ToString());
			}
			items = new ObservableCollection<string>(items.OrderBy(a => a));

			recyclerView.SetAdapter(new LargeAdapter(items));
			recyclerView.SetLayoutManager(new LinearLayoutManager(this, LinearLayoutManager.Vertical, false));
			FastScroller fastScroller = FindViewById<FastScroller>(Resource.Id.fastscroller);
			fastScroller.setRecyclerView(recyclerView);
		}
	}
}
