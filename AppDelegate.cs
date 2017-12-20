/*---------------------------------------------------------------------------------------------
*  Copyright (c) Nicolas Jinchereau. All rights reserved.
*  Licensed under the MIT License. See License.txt in the project root for license information.
*--------------------------------------------------------------------------------------------*/

using AppKit;
using Foundation;

namespace InvoiceGenerator
{
    [Register("AppDelegate")]
    public partial class AppDelegate : NSApplicationDelegate
    {
        public AppDelegate()
        {
        }

        public override bool ApplicationShouldTerminateAfterLastWindowClosed(NSApplication sender) {
            return true;
        }

        public override void DidFinishLaunching(NSNotification notification)
        {
            // Insert code here to initialize your application
        }

        public override void WillTerminate(NSNotification notification)
        {
            //System.Console.WriteLine(NSApplication.SharedApplication.KeyWindow.ContentViewController.GetType());
        }
    }
}
