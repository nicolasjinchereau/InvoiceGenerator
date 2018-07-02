/*---------------------------------------------------------------------------------------------
*  Copyright (c) Nicolas Jinchereau. All rights reserved.
*  Licensed under the MIT License. See License.txt in the project root for license information.
*--------------------------------------------------------------------------------------------*/

using AppKit;
using Foundation;
using System;
using System.IO;

namespace InvoiceGenerator
{
    [Register("AppDelegate")]
    public partial class AppDelegate : NSApplicationDelegate
    {
        public InvoiceEditor Editor {
            get; private set;
        } = new InvoiceEditor();

        public static ViewController ContentView {
            get { return NSApplication.SharedApplication.KeyWindow?.ContentViewController as ViewController; }
        }

        public override bool ApplicationShouldTerminateAfterLastWindowClosed(NSApplication sender) {
            return true;
        }

        public override void DidFinishLaunching(NSNotification notification)
        {
            this.mnuQuit.Activated += OnQuitPressed;
            this.mnuPreferences.Activated += OnPreferencesPressed;
            this.mnuNew.Activated += OnNewPressed;
            this.mnuOpen.Activated += OnOpenPressed;
            this.mnuSaveFile.Activated += OnSavePressed;
            this.mnuSaveFileAs.Activated += OnSaveAsPressed;
            this.mnuRevertFile.Activated += OnRevertPressed;
            this.mnuExportFile.Activated += OnExportPressed;

            NSApplication.SharedApplication.KeyWindow.WindowShouldClose += (sender) => {
                return PromptSaveAndProceed();
            };

            var lastFile = NSUserDefaults.StandardUserDefaults.StringForKey("lastFile");
            if(!string.IsNullOrEmpty(lastFile) && File.Exists(lastFile))
            {
                Editor.Load(lastFile);
                ContentView.RefreshAll();
            }
        }

        public override void WillTerminate(NSNotification notification) {
            NSUserDefaults.StandardUserDefaults.SetString(Editor.DocumentPath ?? "", "lastFile");
        }

        // menu events...
        void OnQuitPressed(object sender, EventArgs args) {
            NSApplication.SharedApplication.KeyWindow?.PerformClose(this);
        }

        void OnPreferencesPressed(object sender, EventArgs args) {
            // ...
        }

        void OnNewPressed(object sender, EventArgs args)
        {
            if(!PromptSaveAndProceed())
                return;
            
            Editor.NewDocument();
            ContentView.RefreshAll();
        }

        void OnOpenPressed(object sender, EventArgs args)
        {
            if(!PromptSaveAndProceed())
                return;

            string openPath = null;

            if(!string.IsNullOrEmpty(Editor.DocumentPath))
                openPath = Path.GetDirectoryName(Editor.DocumentPath);
            else
                openPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            
            NSOpenPanel openPanel = new NSOpenPanel();
            openPanel.AllowedFileTypes = new string[]{ "json" };
            openPanel.Directory = openPath;

            if(openPanel.RunModal() != 0 && !string.IsNullOrEmpty(openPanel.Url.Path))
            {
                Editor.Load(openPanel.Url.Path);
                ContentView.RefreshAll();
            }
        }

        void OnSavePressed(object sender, EventArgs args)
        {
            if(!string.IsNullOrEmpty(Editor.DocumentPath)) {
                Editor.Save();
                ContentView.RefreshWindowTitle();
            }
            else {
                OnSaveAsPressed(sender, args);
            }
        }

        void OnSaveAsPressed(object sender, EventArgs args)
        {
            string filename = null;
            string directory = null;

            if(Editor.DocumentPath != null)
            {
                filename = Path.GetFileName(Editor.DocumentPath);
                directory = Path.GetDirectoryName(Editor.DocumentPath);
            }
            else
            {
                var invoiceNumberString = Editor.Document.invoiceNumber.ToString("0000");
                var invoiceDateString = Editor.Document.date.ToString("yyyy-MM-dd");
                filename = $"invoice_{invoiceNumberString}_{invoiceDateString}.json";
                directory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            }

            NSSavePanel sp = NSSavePanel.SavePanel;
            sp.AllowedFileTypes = new string[] { "json" };
            sp.Directory = directory;
            sp.NameFieldStringValue = filename;

            if(sp.RunModal() != 0 && !string.IsNullOrEmpty(sp.Url.Path)) {
                Editor.SaveAs(sp.Url.Path);
                ContentView.RefreshWindowTitle();
            }
        }

        void OnRevertPressed(object sender, EventArgs args)
        {
            if(!PromptSaveAndProceed())
                return;
            
            Editor.Load(Editor.DocumentPath);
            ContentView.RefreshAll();
        }

        void OnExportPressed(object sender, EventArgs args)
        {
            var savePath = ContentView?.GetExportFilename();

            if(!string.IsNullOrEmpty(savePath))
                ExportDocument(savePath);
        }

        public bool PromptSaveAndProceed()
        {
            if(!Editor.Dirty)
                return true;
            
            var alert = new NSAlert() {
                MessageText = "File has been modified",
                InformativeText = "Save changes?",
            };

            alert.AddButton("Save");
            alert.AddButton("Cancel");
            alert.AddButton("Don't Save");

            int ret = (int)alert.RunModal();
            if(ret < (int)NSAlertButtonReturn.First)
                throw new Exception("Unexpected error has occured");
            
            int buttonIndex = ret - (int)NSAlertButtonReturn.First;
            if(buttonIndex == 0)
                Editor.Save();
            
            return buttonIndex != 1;
        }

        public void ExportDocument(string filename)
        {
            string exportPath = null;

            if(!string.IsNullOrEmpty(Editor.DocumentPath))
                exportPath = Path.GetDirectoryName(Editor.DocumentPath);
            else
                exportPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            
            NSSavePanel sp = NSSavePanel.SavePanel;
            sp.AllowedFileTypes = new string[] { "docx" };
            sp.Directory = exportPath;
            sp.NameFieldStringValue = filename;

            if(sp.RunModal() != 0 && !string.IsNullOrEmpty(sp.Url.Path))
            {
                string templatePath = NSBundle.MainBundle.PathForResource("template.docx", null);
                Editor.Export(templatePath, sp.Url.Path);
            }
        }
    }
}
