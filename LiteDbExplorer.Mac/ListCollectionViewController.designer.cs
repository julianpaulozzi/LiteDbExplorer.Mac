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
	[Register ("ListCollectionViewController")]
	partial class ListCollectionViewController
	{
		[Outlet]
		AppKit.NSTableView listCollectionTable { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (listCollectionTable != null) {
				listCollectionTable.Dispose ();
				listCollectionTable = null;
			}
		}
	}
}
