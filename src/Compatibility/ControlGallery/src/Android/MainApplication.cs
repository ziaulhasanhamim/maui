using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Microsoft.Maui.Controls.DualScreen;

namespace Microsoft.Maui.Controls.Compatibility.ControlGallery.Android
{
	//You can specify additional application information in this attribute
    [Application]
    public class MainApplication : MauiApplication<Startup>, global::Android.App.Application.IActivityLifecycleCallbacks
    {
		internal static Context ActivityContext { get; private set; }

		public MainApplication(IntPtr handle, JniHandleOwnership transer)
          :base(handle, transer)
        {
        }

        public override void OnCreate()
		{
#if NET6_0_OR_GREATER
			// TODO: https://github.com/dotnet/runtime/issues/51274
			Java.Lang.JavaSystem.LoadLibrary("System.Security.Cryptography.Native.OpenSsl");

#endif
            base.OnCreate();
            RegisterActivityLifecycleCallbacks(this);
            //A great place to initialize Xamarin.Insights and Dependency Services!
        }

        public override void OnTerminate()
        {
            base.OnTerminate();
            UnregisterActivityLifecycleCallbacks(this);
        }

        public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
        {
			ActivityContext = activity;
        }

        public void OnActivityDestroyed(Activity activity)
		{
			ActivityContext = activity;
		}

        public void OnActivityPaused(Activity activity)
		{
			ActivityContext = activity;
		}

        public void OnActivityResumed(Activity activity)
        {
			ActivityContext = activity;
		}

        public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
		{
			ActivityContext = activity;
		}

        public void OnActivityStarted(Activity activity)
        {
			ActivityContext = activity;
		}

        public void OnActivityStopped(Activity activity)
		{
			ActivityContext = activity;
		}
    }
}