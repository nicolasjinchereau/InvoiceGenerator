/*---------------------------------------------------------------------------------------------
*  Copyright (c) Nicolas Jinchereau. All rights reserved.
*  Licensed under the MIT License. See License.txt in the project root for license information.
*--------------------------------------------------------------------------------------------*/

using System;
using Foundation;
using AppKit;

namespace InvoiceGenerator
{
    public class MessageBox
    {
        public static nint Show(string title, string message, NSAlertStyle style = NSAlertStyle.Informational)
        {
            var alert = new NSAlert() {
                AlertStyle = style,
                MessageText = title,
                InformativeText = message,
            };

            return alert.RunModal();
        }
    }
}
