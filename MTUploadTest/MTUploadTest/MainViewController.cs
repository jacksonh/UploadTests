using System;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.ObjCRuntime;

namespace MTUploadTest
{
	public partial class MainViewController : UIViewController
	{
		private NSUrlConnection url_connection;
		
		public MainViewController () : base ("MainViewController", null)
		{
			// Custom initialization
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			// Perform any additional setup after loading the view, typically from a nib.
		}
		
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
		}
		
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}
		
		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();
			
			// Clear any references to subviews of the main view in order to
			// allow the Garbage Collector to collect them sooner.
			//
			// e.g. myOutlet.Dispose (); myOutlet = null;
			
			ReleaseDesignerOutlets ();
		}
		
		partial void OnStartUploadClicked (NSObject sender)
		{
			InvokeOnMainThread (delegate {
				
				Console.WriteLine ("{0}:  Creating delegate", DateTime.Now);
				var condelegate = new NativeUrlDelegate ((body) => {

				}, (reason) => {

				});
				
				Console.WriteLine ("current mode: '{0}'", NSRunLoop.NSRunLoopCommonModes);
				
				var buffer = new byte [5 * 1024];
				var data = NSData.FromArray (buffer);
				var request = CreateDataPostRequest ("https://ec.jacksonh.org:9000/", data);
				
				url_connection = new NSUrlConnection (request, condelegate, false);
				url_connection.Schedule (NSRunLoop.Current, NSRunLoop.NSRunLoopCommonModes);
				url_connection.Start ();
				Console.WriteLine ("{0}: started request", DateTime.Now);
			});
		}
		
		
		private NSMutableUrlRequest CreateDataPostRequest (string url, NSData data)
		{
			NSUrl nsurl = NSUrl.FromString (url);
			
			if (nsurl == null)
				throw new Exception ("Invalid upload URL, could not create NSUrl from: '" + url + "'.");
			
			NSMutableUrlRequest request = new NSMutableUrlRequest (nsurl);
			
			request.Body = data;
			request.HttpMethod = "POST";
			
			request.Headers = new NSDictionary ();
			request.Headers.SetValueForKey (NSObject.FromObject (data.Length.ToString ()), new NSString ("Content-Length"));

			
			return request;
		}
		
		private class NativeUrlDelegate : NSUrlConnectionDelegate {
			
		    private Action<string> success_callback;
		    private Action<string> failure_callback;
		    private NSMutableData data;
			private int status_code;

		
		    public NativeUrlDelegate (Action<string> success, Action<string> failure) {
		        success_callback = success;
		        failure_callback = failure;
				
		        data = new NSMutableData();
		    }
	
		    public override void ReceivedData (NSUrlConnection connection, NSData d)
		    {
		        data.AppendData (d);
		    }
		
			public override void ReceivedResponse (NSUrlConnection connection, NSUrlResponse response)
			{
				var http_response = response as NSHttpUrlResponse;
				if (http_response == null) {
					Console.WriteLine ("Received non HTTP url response: '{0}'", response);
					status_code = -1;
					return;
				}
				
				if (OnResponse != null)
					OnResponse (this, EventArgs.Empty);
				
				status_code = http_response.StatusCode;
				Console.WriteLine ("Status code of result:   '{0}'", status_code);
			}
			
		    public override void FailedWithError (NSUrlConnection connection, NSError error)
		    {				
				Console.WriteLine ("failed with error:  '{0}'", error.LocalizedDescription);
		        if (failure_callback != null)
		            failure_callback (error.LocalizedDescription);
		    }
		
		    public override void FinishedLoading (NSUrlConnection connection)
		    {
				if (status_code != 200) {
					failure_callback (String.Format ("Did not receive a 200 HTTP status code, received '{0}'", status_code));
					return;
				}

		        success_callback (data.ToString ());
		    }
			
			public override bool CanAuthenticateAgainstProtectionSpace (NSUrlConnection connection, NSUrlProtectionSpace protectionSpace)
			{
				Console.WriteLine ("UrlConnection Attempting to authenticate against protection space:  '{0}'", protectionSpace.AuthenticationMethod);
				return protectionSpace.AuthenticationMethod == "NSURLAuthenticationMethodServerTrust";
			}
			
			public override void ReceivedAuthenticationChallenge (NSUrlConnection connection, NSUrlAuthenticationChallenge challenge)
			{
				Console.WriteLine ("UrlConnection received authentication method:  '{0}'", challenge.ProtectionSpace.AuthenticationMethod);
				// challenge.Sender.ContinueWithoutCredentialForAuthenticationChallenge (challenge);
				if (challenge.ProtectionSpace.AuthenticationMethod=="NSURLAuthenticationMethodServerTrust")
					challenge.Sender.UseCredentials(NSUrlCredential.FromTrust(challenge.ProtectionSpace.ServerTrust), challenge);
				// challenge.Sender.ContinueWithoutCredentialForAuthenticationChallenge (challenge);
			}
			
			public event EventHandler OnResponse;
		}
	}
}

