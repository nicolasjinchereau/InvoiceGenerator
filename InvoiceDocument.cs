/*---------------------------------------------------------------------------------------------
*  Copyright (c) Nicolas Jinchereau. All rights reserved.
*  Licensed under the MIT License. See License.txt in the project root for license information.
*--------------------------------------------------------------------------------------------*/

using System;
using System.Linq;
using System.Collections.Generic;

namespace InvoiceGenerator
{
    public class Session
    {
        public DateTime start;
        public DateTime finish;

        public float TotalHours {
            get { return (float)(finish - start).TotalHours; }
        }
    }

    public class InvoiceDocument
    {
        public DateTime date = DateTime.Now.Date;
        public int invoiceNumber = 1;
        public string consultant = "";
        public string client = "";
        public string filename = "";
        public string services = "";
        public List<Session> sessions = new List<Session>();
        public float rate = 0;
        public float taxRate = 0;

        public float TotalHours {
            get { return sessions.Select(s => s.TotalHours).Sum(); }
        }

        public float Subtotal {
            get { return TotalHours * rate; }
        }

        public float Tax {
            get { return Subtotal * taxRate / 100.0f; }
        }

        public float Total {
            get { return Subtotal + Tax; }
        }
    }
}
