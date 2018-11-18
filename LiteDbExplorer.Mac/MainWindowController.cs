// This file has been autogenerated from a class added in the UI designer.

using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

using Foundation;
using AppKit;
using LiteDbExplorer.Mac.Models;
using LiteDB;

namespace LiteDbExplorer.Mac
{
	public partial class MainWindowController : NSWindowController, IDisposable
	{
		public MainWindowController (IntPtr handle) : base (handle)
		{

		}

        public bool ShowSaveAsSheet { get; set; } = true;

        public bool ShowOpenAsSheet { get; set; } = true;

        public override void WindowDidLoad()
        {
            base.ShouldCascadeWindows = false;
            base.WindowFrameAutosaveName = @"PrefsMainWindow";
            base.WindowDidLoad();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            SessionData.Current.CloseDatabases();
        }

        // To default main menu
        [Export("newDocument:")]
        protected void NewDocument(NSObject sender)
        {
            NewDatabase(sender);
        }

        [Export("newDatabase:")]
        protected async void NewDatabase(NSObject sender)
        {
            var url = await UIDialog.SaveFileDialog("Save Database", ShowSaveAsSheet ? Window : null);
            if (url.HasValue)
            {
                try
                {
                    HandleCreateDatabase(url.Value);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    UIDialog.ShowAlert("Database Error", $"Failed to create database: {e.Message}", NSAlertStyle.Critical);
                }
            }
        }

        // To default main menu
        [Export("openDocument:")]
        protected void OpenDocument(NSObject sender) 
        {
            OpenDatabase(sender);
        }


        [Export("openDatabase:")]
        protected async void OpenDatabase(NSObject sender)
        {
            var url = await UIDialog.OpenFileDialog("Open Database", ShowOpenAsSheet ? Window : null);
            if (url.HasValue)
            {
                try
                {
                    HandleOpenDatabase(url.Value);
                }
                catch (Exception e)
                {
                    UIDialog.ShowAlert("Database Error", $"Failed to open database: {e.Message}", NSAlertStyle.Critical);
                }
            }
        }

        [Export("openDatabaseSegment:")]
        protected void OpenDatabaseSegment(NSObject sender)
        {
            if (!(sender is NSSegmentedControl segmentedCtrl))
            {
                return;
            }

            if (segmentedCtrl.SelectedSegment == 0)
            {
                OpenDatabase(sender);
            }

        }

        public bool HandleCreateDatabase(NSUrl url) 
        {
            var path = url.Path;
            using (var stream = new FileStream(path, System.IO.FileMode.Create))
            {
                LiteEngine.CreateDatabase(stream);
            }

            HandleOpenDatabase(url);

            return true;
        }

        public bool HandleOpenDatabase(NSUrl url)
        {
            var path = url.Path;
            if (SessionData.Current.HasDatabaseReference(path))
            {
                return false;
            }

            if (!File.Exists(path))
            {
                UIDialog.ShowAlert(
                    "File not found",
                    "Cannot open database, file not found.",
                    NSAlertStyle.Critical);
                return false;
            }
            
            try
            {
                string password = null;
                if (DatabaseReference.IsDbPasswordProtected(path))
                {
                    if (UIDialog.ShowInputAlert(Window, "Database is password protected, enter password:", "Database password.", "", out password) != true)
                    {
                        return false;
                    }
                }

                SessionData.Current.AddDatabase(new DatabaseReference(path, password));

                // Add document to the Open Recent menu
                NSDocumentController.SharedDocumentController.NoteNewRecentDocumentURL(url);

                return true;
            }
            catch (Exception e)
            {
                UIDialog.ShowAlert(
                    "Database Error",
                    "Failed to open database:" + Environment.NewLine + e.Message,  
                    NSAlertStyle.Critical);
            }

            return false;
        }
        

    }
}
