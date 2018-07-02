/*---------------------------------------------------------------------------------------------
*  Copyright (c) Nicolas Jinchereau. All rights reserved.
*  Licensed under the MIT License. See License.txt in the project root for license information.
*--------------------------------------------------------------------------------------------*/

using System;
using AppKit;
using Foundation;

namespace InvoiceGenerator
{
    public class SessionListDelegate : NSTableViewDelegate
    {
        private const string CellIdentifier = "SessionCell";
        private SessionListDataSource DataSource;

        public event Action<NSDatePicker, Session> SessionStartChanged;
        public event Action<NSDatePicker, Session> SessionFinishChanged;

        public SessionListDelegate(SessionListDataSource dataSource) {
            this.DataSource = dataSource;
        }

        void OnSessionStartChanged(NSDatePicker datePicker, Session session) {
            SessionStartChanged?.Invoke(datePicker, session);
        }

        void OnSessionFinishChanged(NSDatePicker datePicker, Session session) {
            SessionFinishChanged?.Invoke(datePicker, session);
        }

        public override NSView GetViewForItem(NSTableView tableView, NSTableColumn tableColumn, nint row)
        {
            NSView ret = null;
            var session = DataSource.sessions[(int)row];

            switch(tableColumn.Title)
            {
                case "Start":
                    {
                        var view = new NSDatePicker();
                        view.DatePickerElements = NSDatePickerElementFlags.YearMonthDateDay | NSDatePickerElementFlags.HourMinute;
                        view.Identifier = CellIdentifier;
                        view.BackgroundColor = NSColor.Clear;
                        view.Bordered = false;
                        view.DateValue = (NSDate)session.start;
                        view.Activated += (s, e) => {
                            OnSessionStartChanged(view, session);
                            var field = tableView.GetView(2, row, true) as NSTextField;
                            if(field != null) field.FloatValue = session.TotalHours;
                        };

                        ret = view;
                        break;
                    }
                case "Finish":
                    {
                        var view = new NSDatePicker();
                        view.DatePickerElements = NSDatePickerElementFlags.YearMonthDateDay | NSDatePickerElementFlags.HourMinute;
                        view.Identifier = CellIdentifier;
                        view.BackgroundColor = NSColor.Clear;
                        view.Bordered = false;
                        view.DateValue = (NSDate)session.finish;
                        view.Activated += (s, e) => {
                            OnSessionFinishChanged(view, session);
                            var field = tableView.GetView(2, row, true) as NSTextField;
                            if(field != null) field.FloatValue = session.TotalHours;
                        };

                        ret = view;
                        break;
                    }
                case "Hours":
                    {
                        var view = new NSTextField();

                        view.Formatter = new NSNumberFormatter() {
                            MinimumIntegerDigits = 1,
                            MinimumFractionDigits = 1,
                            MaximumFractionDigits = 2,
                            NumberStyle = NSNumberFormatterStyle.Decimal,
                        };

                        view.FloatValue = session.TotalHours;
                        view.Identifier = CellIdentifier;
                        view.BackgroundColor = NSColor.Clear;
                        view.Bordered = false;
                        view.Editable = false;

                        ret = view;
                        break;
                    }
            }

            return ret;
        }
    }
}
