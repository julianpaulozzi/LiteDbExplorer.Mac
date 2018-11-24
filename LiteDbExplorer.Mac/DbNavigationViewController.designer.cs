// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace LiteDbExplorer.Mac
{
    [Register ("DbNavigationViewController")]
    partial class DbNavigationViewController
    {
        [Outlet]
        AppKit.NSOutlineView dbNavOutlineView { get; set; }

        [Action ("dbNavClick:")]
        partial void dbNavClick (Foundation.NSObject sender);

        [Action ("dbNavDoubleClick:")]
        partial void dbNavDoubleClick (Foundation.NSObject sender);
        
        void ReleaseDesignerOutlets ()
        {
            if (dbNavOutlineView != null) {
                dbNavOutlineView.Dispose ();
                dbNavOutlineView = null;
            }
        }
    }
}
