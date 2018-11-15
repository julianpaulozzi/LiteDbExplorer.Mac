using System;
using System.Reflection;
using AppKit;
using Foundation;

namespace LiteDbExplorer.Mac
{
    public partial class ViewController : NSViewController
    {
        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewWillAppear ()
        {
            base.ViewWillAppear ();

            var version = NSBundle.MainBundle.ObjectForInfoDictionary("CFBundleShortVersionString").ToString();

            // Set Window Title
            this.View.Window.Title = $"LiteDB Explorer {version}";
        }
        
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Do any additional setup after loading the view.
        }

        public override NSObject RepresentedObject
        {
            get
            {
                return base.RepresentedObject;
            }
            set
            {
                base.RepresentedObject = value;
                // Update the view, if already loaded.
            }
        }
    }
}
