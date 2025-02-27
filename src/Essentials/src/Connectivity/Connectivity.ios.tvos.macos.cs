#if !(MACCATALYST || MACOS)
using CoreTelephony;
#endif
using System;
using System.Collections.Generic;

namespace Microsoft.Maui.Essentials.Implementations
{
	public partial class ConnectivityImplementation : IConnectivity
	{
#if !(MACCATALYST || MACOS)
		// TODO: Use NWPathMonitor on > iOS 12
#pragma warning disable BI1234
		static readonly Lazy<CTCellularData> cellularData = new Lazy<CTCellularData>(() => new CTCellularData());

		internal static CTCellularData CellularData => cellularData.Value;
#pragma warning restore BI1234
#endif

		static ReachabilityListener listener;

		public void StartListeners()
		{
			listener = new ReachabilityListener();
			listener.ReachabilityChanged += Connectivity.OnConnectivityChanged;
		}

		public void StopListeners()
		{
			if (listener == null)
				return;

			listener.ReachabilityChanged -= Connectivity.OnConnectivityChanged;
			listener.Dispose();
			listener = null;
		}

		public NetworkAccess NetworkAccess
		{
			get
			{
				var restricted = false;
#if !(MACCATALYST || MACOS)
				// TODO: Use NWPathMonitor on > iOS 12
#pragma warning disable BI1234
				restricted = CellularData.RestrictedState == CTCellularDataRestrictedState.Restricted;
#pragma warning restore BI1234
#endif
				var internetStatus = Reachability.InternetConnectionStatus();
				if ((internetStatus == NetworkStatus.ReachableViaCarrierDataNetwork && !restricted) || internetStatus == NetworkStatus.ReachableViaWiFiNetwork)
					return NetworkAccess.Internet;

				var remoteHostStatus = Reachability.RemoteHostStatus();
				if ((remoteHostStatus == NetworkStatus.ReachableViaCarrierDataNetwork && !restricted) || remoteHostStatus == NetworkStatus.ReachableViaWiFiNetwork)
					return NetworkAccess.Internet;

				return NetworkAccess.None;
			}
		}

		public IEnumerable<ConnectionProfile> ConnectionProfiles
		{
			get
			{
				var statuses = Reachability.GetActiveConnectionType();
				foreach (var status in statuses)
				{
					switch (status)
					{
						case NetworkStatus.ReachableViaCarrierDataNetwork:
							yield return ConnectionProfile.Cellular;
							break;
						case NetworkStatus.ReachableViaWiFiNetwork:
							yield return ConnectionProfile.WiFi;
							break;
						default:
							yield return ConnectionProfile.Unknown;
							break;
					}
				}
			}
		}
	}
}
