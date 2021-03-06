﻿// This file has been autogenerated from a class added in the UI designer.

using System;

using Foundation;
using AppKit;
using LiteDbExplorer.Mac.Models;

namespace LiteDbExplorer.Mac
{
    public class DbNavOutlineDelegate : NSOutlineViewDelegate
    {
        public event EventHandler<ElementNodeEventArgs> NodeSelected;

        public Func<NSOutlineView, NSObject, NSMenu> MenuForItem { get; set; }

        public override bool ShouldSelectItem(NSOutlineView outlineView, NSObject item)
        {
            var node = item as NSTreeNode;
            if (node?.RepresentedObject is DbNavigationNode dbNode)
            {
                OnNodeSelected(dbNode.NodeType, dbNode.InstanceId);
            }

            return true;
        }

        protected virtual void OnNodeSelected(DbNavigationNodeType nodeType, string instanceId)
        {
            NodeSelected?.Invoke(this, new ElementNodeEventArgs(nodeType, instanceId));
        }
    }
}
