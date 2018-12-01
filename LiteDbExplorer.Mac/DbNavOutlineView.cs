using System;
using AppKit;
using Foundation;

namespace LiteDbExplorer.Mac
{
    [Register("DbNavOutlineView")]
    public class DbNavOutlineView : NSOutlineView
    {
        #region Constructors
        public DbNavOutlineView()
        {
        }

        public DbNavOutlineView(IntPtr handle) : base(handle)
        {
        }
        #endregion

        public override NSMenu MenuForEvent(NSEvent theEvent)
        {

            var point = ConvertPointFromView(theEvent.LocationInWindow, null);
            var row = GetRow(point);
            var item = ItemAtRow(row);

            if(item == null)
            {
                return null;
            }

            return (Delegate as DbNavOutlineDelegate)?.MenuForItem?.Invoke(this, item);
        }
    }
}
