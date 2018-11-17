using AppKit;
using Foundation;

namespace LiteDbExplorer.Mac
{
    [Register("AppDelegate")]
    public class AppDelegate : NSApplicationDelegate
    {
        public AppDelegate()
        {
        }

        public override void DidFinishLaunching(NSNotification notification)
        {
            // Insert code here to initialize your application
        }

        public override void WillTerminate(NSNotification notification)
        {
            // Insert code here to tear down your application
        }

        #region Override Methods

        public override bool OpenFile(NSApplication sender, string filename)
        {
            // Trap all errors
            try
            {
                filename = filename.Replace(" ", "%20");
                var url = new NSUrl("file://" + filename);
                return OpenFile(url);
            }
            catch
            {
                return false;
            }
        }

        #endregion

        private bool OpenFile(NSUrl url)
        {
            // NSApplication.sharedApplication().mainWindow?.windowController;
            var windowController = NSApplication.SharedApplication.MainWindow?.WindowController as MainWindowController;
            if(windowController != null)
            {
                return windowController.HandleOpenDatabase(url);
            }
            return true;
        }

    }
}
