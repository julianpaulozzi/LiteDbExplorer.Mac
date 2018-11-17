using System;
using AppKit;
using CoreGraphics;
using Foundation;
using ObjCRuntime;

namespace LiteDbExplorer.Mac
{
    [Register("NSMenuSegmentedCell")]
    public class NSMenuSegmentedCell : NSSegmentedCell
    {
        #region Constructors
        public NSMenuSegmentedCell()
        {
        }

        public NSMenuSegmentedCell(IntPtr handle) : base(handle)
        {
        }
        #endregion

        public override Selector Action
        {
            get => base.GetMenu(SelectedSegment) == null ? base.Action : null;
            set => base.Action = value;
        }
    }
}
