/*---------------------------------------------------------------------------------------------
*  Copyright (c) Nicolas Jinchereau. All rights reserved.
*  Licensed under the MIT License. See License.txt in the project root for license information.
*--------------------------------------------------------------------------------------------*/

using System;
using AppKit;
using System.Collections.Generic;

namespace InvoiceGenerator
{
    public class SessionListDataSource : NSTableViewDataSource
    {
        public List<Session> sessions = new List<Session>();

        public override nint GetRowCount(NSTableView tableView) {
            return sessions.Count;
        }
    }
}
