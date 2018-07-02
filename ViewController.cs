/*---------------------------------------------------------------------------------------------
*  Copyright (c) Nicolas Jinchereau. All rights reserved.
*  Licensed under the MIT License. See License.txt in the project root for license information.
*--------------------------------------------------------------------------------------------*/

using System;
using AppKit;
using Foundation;
using System.Collections.Generic;
using System.IO;

namespace InvoiceGenerator
{
    public partial class ViewController : NSViewController
    {
        public const string WindowTitle = "Invoice Generator";

        InvoiceEditor editor;
        SessionListDataSource sessionListDataSource;
        SessionListDelegate sessionListDelegate;

        AppDelegate AppDelegate {
            get { return NSApplication.SharedApplication.Delegate as AppDelegate; }
        }

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            invoiceDate.Activated += OnDateChanged;
            invoiceNumberText.Changed += OnInvoiceNumberChanged;
            incrementButton.Activated += OnIncrementPressed;
            consultantText.TextDidChange += OnConsultantChanged;
            clientText.TextDidChange += OnClientChanged;
            servicesText.TextDidChange += OnServicesChanged;
            rateText.Changed += OnRateChanged;
            taxRateText.Changed += OnTaxRateChanged;
            filenameText.Changed += OnFilenameChanged;
            exportButton.Activated += OnExportPressed;
            addSessionButton.Activated += OnAddSessionPressed;
            removeSessionButton.Activated += OnRemoveSessionPressed;

            sessionListDataSource = new SessionListDataSource();
            sessionListDelegate = new SessionListDelegate(sessionListDataSource);
            sessionListDelegate.SessionStartChanged += OnSessionsStartChanged;
            sessionListDelegate.SessionFinishChanged += OnSessionsFinishChanged;

            hoursTable.DataSource = sessionListDataSource;
            hoursTable.Delegate = sessionListDelegate;
            hoursTable.AllowsMultipleSelection = false;

            editor = (NSApplication.SharedApplication.Delegate as AppDelegate).Editor;
        }

        public override void ViewWillAppear()
        {
            base.ViewWillAppear();
            RefreshAll();
        }

        public override void ViewWillDisappear()
        {
            base.ViewWillDisappear();
        }

        public override void ViewDidDisappear()
        {
            base.ViewDidDisappear();
        }

        void OnDateChanged(object sender, EventArgs args)
        {
            editor.Document.date = ((DateTime)invoiceDate.DateValue).ToLocalTime();
            editor.SetDirty();
            RefreshWindowTitle();
        }

        void OnInvoiceNumberChanged(object sender, EventArgs args)
        {
            editor.Document.invoiceNumber = invoiceNumberText.IntValue;
            editor.SetDirty();
            RefreshWindowTitle();
        }

        void OnIncrementPressed(object sender, EventArgs args)
        {
            editor.Document.invoiceNumber += 1;
            editor.SetDirty();
            invoiceNumberText.IntValue = editor.Document.invoiceNumber;
            RefreshWindowTitle();
        }

        void OnConsultantChanged(object sender, EventArgs args)
        {
            editor.Document.consultant = consultantText.Value;
            editor.SetDirty();
            RefreshWindowTitle();
        }

        void OnClientChanged(object sender, EventArgs args)
        {
            editor.Document.client = clientText.Value;
            editor.SetDirty();
            RefreshWindowTitle();
        }

        void OnServicesChanged(object sender, EventArgs args)
        {
            editor.Document.services = servicesText.Value;
            editor.SetDirty();
            RefreshWindowTitle();
        }

        void OnSessionsStartChanged(NSDatePicker datePicker, Session session)
        {
            session.start = ((DateTime)datePicker.DateValue).ToLocalTime();
            editor.SetDirty();
            RefreshTotals();
            RefreshWindowTitle();
        }

        void OnSessionsFinishChanged(NSDatePicker datePicker, Session session)
        {
            session.finish = ((DateTime)datePicker.DateValue).ToLocalTime();
            editor.SetDirty();
            RefreshTotals();
            RefreshWindowTitle();
        }

        void OnRateChanged(object sender, EventArgs args)
        {
            editor.Document.rate = rateText.FloatValue;
            editor.SetDirty();
            RefreshTotals();
            RefreshWindowTitle();
        }

        void OnTaxRateChanged(object sender, EventArgs args)
        {
            editor.Document.taxRate = taxRateText.FloatValue;
            editor.SetDirty();
            RefreshTotals();
            RefreshWindowTitle();
        }

        void OnFilenameChanged(object sender, EventArgs args)
        {
            editor.Document.filename = filenameText.StringValue;
            editor.SetDirty();
            RefreshWindowTitle();
        }

        void OnExportPressed(object sender, EventArgs args) {
            AppDelegate.ExportDocument(GetExportFilename());
        }

        void OnAddSessionPressed(object sender, EventArgs args)
        {
            var date = DateTime.Now.Date;

            if(editor.Document.sessions.Count > 0)
                date = editor.Document.sessions[editor.Document.sessions.Count - 1].finish;
            
            editor.Document.sessions.Add(new Session(){ start = date, finish = date });
            editor.SetDirty();
            RefreshHours();
            RefreshTotals();
            RefreshWindowTitle();
        }

        void OnRemoveSessionPressed(object sender, EventArgs args)
        {
            int row = (int)hoursTable.SelectedRow;
            if(row >= 0)
            {
                editor.Document.sessions.RemoveAt(row);
                editor.SetDirty();
                RefreshHours();
                RefreshTotals();
                RefreshWindowTitle();
            }
        }

        public void RefreshInputs()
        {
            invoiceDate.DateValue = (NSDate)editor.Document.date;
            invoiceNumberText.IntValue = editor.Document.invoiceNumber;
            consultantText.Value = editor.Document.consultant;
            clientText.Value = editor.Document.client;
            servicesText.Value = editor.Document.services;
            rateText.FloatValue = editor.Document.rate;
            taxRateText.FloatValue = editor.Document.taxRate;
            filenameText.StringValue = editor.Document.filename;
        }

        public void RefreshHours()
        {
            var sel = hoursTable.SelectedRow;
            sessionListDataSource.sessions = editor.Document.sessions;
            hoursTable.ReloadData();
            hoursTable.SelectRow(sel, false);
        }

        public void RefreshTotals()
        {
            totalHoursText.StringValue = editor.Document.TotalHours.ToString("0.0#");
            subtotalText.StringValue = editor.Document.Subtotal.ToString("N2");
            taxAmountText.StringValue = editor.Document.Tax.ToString("N2");
            totalPriceText.StringValue = editor.Document.Total.ToString("C2");
        }

        public void RefreshWindowTitle()
        {
            var star = editor.Dirty ? "*" : "";
            var filename = !string.IsNullOrEmpty(editor.DocumentPath) ? Path.GetFileName(editor.DocumentPath) : "Untitled";
            var title = $"{star}{WindowTitle} - {filename}";
            NSApplication.SharedApplication.KeyWindow.Title = title;
        }

        public void RefreshAll()
        {
            RefreshInputs();
            RefreshHours();
            RefreshTotals();
            RefreshWindowTitle();
        }

        public string GetExportFilename()
        {
            var invoiceNumberString = editor.Document.invoiceNumber.ToString("0000");
            var dateString = editor.Document.date.ToString("yyyy_MM_dd");

            var exportFilename = filenameText.StringValue;
            exportFilename = exportFilename.Replace("$INV", invoiceNumberString);
            exportFilename = exportFilename.Replace("$DATE", dateString);

            return exportFilename;
        }

        float[] SplitFloatCSV(string csv)
        {
            try {
                var ret = new List<float>(32);

                foreach(var f in csv.Split(new char[]{ ',' }, StringSplitOptions.RemoveEmptyEntries))
                    ret.Add(float.Parse(f.Trim()));

                return ret.ToArray();
            }
            catch(Exception) {
                return new float[0];
            }
        }
    }
}
