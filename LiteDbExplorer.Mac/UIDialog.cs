using System;
using System.Linq;
using System.Threading.Tasks;
using AppKit;
using CoreGraphics;
using CSharpFunctionalExtensions;

namespace LiteDbExplorer.Mac
{
    public class UIDialog
    {
        public static void ShowAlert(string message, NSAlertStyle style = NSAlertStyle.Informational)
        {
            var alert = new NSAlert
            {
                AlertStyle = style,
                MessageText = message,
            };
            alert.RunModal();
        }
        
        public static void ShowAlert(string message, string information, NSAlertStyle style = NSAlertStyle.Informational)
        {
            var alert = new NSAlert
            {
                AlertStyle = style,
                InformativeText = information,
                MessageText = message,
            };
            alert.RunModal();
        }
        
        public static bool ShowInputAlert(NSWindow window, string question, string title, string defaultValue, out string result)
        {
            result = null;

            var msg = new NSAlert();
            
            msg.AddButton("OK");      // 1st button
            msg.AddButton("Cancel");  // 2nd button
            msg.MessageText = title;
            msg.InformativeText = question;

            var txt = new NSTextField(new CGRect(x: 0,y: 0,width: 200,height: 24))
            {
                StringValue = defaultValue
            };

            msg.AccessoryView = txt;
            msg.Window.InitialFirstResponder = txt;
            var response = msg.RunSheetModal(window);

            if (response == 1)
            {
                result = txt.StringValue;
                return true;
            }
            
            return false;
        }

        public static Task<Maybe<string>> OpenFileDialog(string title, NSWindow window = null)
        {
            var completionSource = new TaskCompletionSource<Maybe<string>>();
            
            var dlg = NSOpenPanel.OpenPanel;
            dlg.Title = title;
            dlg.CanChooseFiles = true;
            dlg.CanChooseDirectories = false;
            
            void HandleResult(nint result)
            {
                if (result == 1)
                {
                    // Nab the first file
                    var url = dlg.Urls.FirstOrDefault();
                    if (url != null)
                    {
                        completionSource.SetResult(Maybe<string>.From(url.Path));
                        return;
                    }
                }
                
                completionSource.SetResult(Maybe<string>.None);
            }

            if (window != null)
            {
                dlg.BeginSheet(window, HandleResult);
            }
            else
            {
                HandleResult(dlg.RunModal());
            }
            

            return completionSource.Task;
        }

        public static Task<Maybe<string>> SaveFileDialog(string title, NSWindow window = null)
        {
            var completionSource = new TaskCompletionSource<Maybe<string>>();
            var dlg = NSSavePanel.SavePanel;
            dlg.Title = title;

            void HandleResult(nint result)
            {
                if (result == 1)
                {
                    completionSource.SetResult(Maybe<string>.From(dlg.Url.Path));
                    return;
                }
                
                completionSource.SetResult(Maybe<string>.None);
            }

            if (window != null)
            {
                dlg.BeginSheet(window, HandleResult);
            }
            else
            {
                HandleResult(dlg.RunModal());
            }

            return completionSource.Task;
        }
        
    }
}