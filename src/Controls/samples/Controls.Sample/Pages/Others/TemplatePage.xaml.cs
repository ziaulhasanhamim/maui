using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Layouts;

namespace Maui.Controls.Sample.Pages
{
	[XamlCompilation(XamlCompilationOptions.Skip)]
	public partial class TemplatePage
	{
		public TemplatePage()
		{
			InitializeComponent();
		}
	}

	public class Toolbar : HorizontalStackLayout
	{
		private readonly ObservableCollection<ToolBarItem> toolbarItems;

		public Toolbar()
		{
			toolbarItems = new ObservableCollection<ToolBarItem>();
			toolbarItems.CollectionChanged += OnToolbarItemsChanged;
		}

		public IList<ToolBarItem> ToolBarItems => this.toolbarItems;

		// So, we can hijack the Add here to force it into the alternate collection
		public void Add(ToolBarItem tbi) 
		{
			ToolBarItems.Add(tbi);
		}

		private void OnToolbarItemsChanged(object sender, NotifyCollectionChangedEventArgs eventArgs)
		{
			switch (eventArgs.Action)
			{
				case NotifyCollectionChangedAction.Add:
					var startIndex = eventArgs.NewStartingIndex;

					foreach (ToolBarItem toolbarItem in eventArgs.NewItems)
					{
						var label = new Label
						{
							Text = toolbarItem.Text, 
							FontAttributes = FontAttributes.Bold,
							Margin = new Thickness(20)
						};

						this.Insert(startIndex++, label);
					}
					break;
				case NotifyCollectionChangedAction.Remove:
					foreach (ToolBarItem toolbarItem in eventArgs.OldItems)
					{
						this.RemoveAt(eventArgs.OldStartingIndex);
					}
					break;
				case NotifyCollectionChangedAction.Reset:
					this.Clear();
					break;
			}
		}
	}

	public class ToolBarItem  
	{ 
		public string Text { get; set; }
	}
}