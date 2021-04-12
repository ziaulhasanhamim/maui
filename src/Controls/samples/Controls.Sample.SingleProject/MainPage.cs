using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Extensions.DependencyInjection;

namespace Maui.Controls.Sample.SingleProject
{
	public class MainPage : ContentPage, IPage
	{
		public MainPage()
		{
			System.Console.WriteLine("asdasdasd-mainpage");
			System.Diagnostics.Debug.WriteLine("eiloneilon-mainpage");


			var serviceCollection = new ServiceCollection();
			serviceCollection.AddBlazorWebView();
			//serviceCollection.AddSingleton<AppState>(_appState);

			var bwv = new BlazorWebView
			{
				BackgroundColor = Color.Orange,
				Services = serviceCollection.BuildServiceProvider(),
				HeightRequest = 400,
				MinimumHeightRequest = 400,
				HostPage = @"wwwroot/index.html",
			};
			bwv.RootComponents.Add(new RootComponent { Selector = "#app", ComponentType = typeof(Main) });
			Content = bwv;


			// Content = new Label
			// {
			// 	Text = "Hello, .NET MAUI Single Project!",
			// 	BackgroundColor = Color.White
			// };
		}

		public IView View { get => (IView)Content; set => Content = (View)value; }
	}
}