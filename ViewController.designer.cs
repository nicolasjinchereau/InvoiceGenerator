// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace InvoiceGenerator
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		AppKit.NSButton addSessionButton { get; set; }

		[Outlet]
		AppKit.NSTextView clientText { get; set; }

		[Outlet]
		AppKit.NSTextView consultantText { get; set; }

		[Outlet]
		AppKit.NSButton exportButton { get; set; }

		[Outlet]
		AppKit.NSTextField filenameText { get; set; }

		[Outlet]
		AppKit.NSTableView hoursTable { get; set; }

		[Outlet]
		AppKit.NSButton incrementButton { get; set; }

		[Outlet]
		AppKit.NSDatePicker invoiceDate { get; set; }

		[Outlet]
		AppKit.NSTextField invoiceNumberText { get; set; }

		[Outlet]
		AppKit.NSTextField rateText { get; set; }

		[Outlet]
		AppKit.NSButton removeSessionButton { get; set; }

		[Outlet]
		AppKit.NSTextView servicesText { get; set; }

		[Outlet]
		AppKit.NSTextField subtotalText { get; set; }

		[Outlet]
		AppKit.NSTextField taxAmountText { get; set; }

		[Outlet]
		AppKit.NSTextField taxRateText { get; set; }

		[Outlet]
		AppKit.NSTextField totalHoursText { get; set; }

		[Outlet]
		AppKit.NSTextField totalPriceText { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (addSessionButton != null) {
				addSessionButton.Dispose ();
				addSessionButton = null;
			}

			if (clientText != null) {
				clientText.Dispose ();
				clientText = null;
			}

			if (consultantText != null) {
				consultantText.Dispose ();
				consultantText = null;
			}

			if (exportButton != null) {
				exportButton.Dispose ();
				exportButton = null;
			}

			if (filenameText != null) {
				filenameText.Dispose ();
				filenameText = null;
			}

			if (hoursTable != null) {
				hoursTable.Dispose ();
				hoursTable = null;
			}

			if (incrementButton != null) {
				incrementButton.Dispose ();
				incrementButton = null;
			}

			if (invoiceDate != null) {
				invoiceDate.Dispose ();
				invoiceDate = null;
			}

			if (invoiceNumberText != null) {
				invoiceNumberText.Dispose ();
				invoiceNumberText = null;
			}

			if (rateText != null) {
				rateText.Dispose ();
				rateText = null;
			}

			if (removeSessionButton != null) {
				removeSessionButton.Dispose ();
				removeSessionButton = null;
			}

			if (servicesText != null) {
				servicesText.Dispose ();
				servicesText = null;
			}

			if (subtotalText != null) {
				subtotalText.Dispose ();
				subtotalText = null;
			}

			if (taxAmountText != null) {
				taxAmountText.Dispose ();
				taxAmountText = null;
			}

			if (taxRateText != null) {
				taxRateText.Dispose ();
				taxRateText = null;
			}

			if (totalHoursText != null) {
				totalHoursText.Dispose ();
				totalHoursText = null;
			}

			if (totalPriceText != null) {
				totalPriceText.Dispose ();
				totalPriceText = null;
			}
		}
	}
}
