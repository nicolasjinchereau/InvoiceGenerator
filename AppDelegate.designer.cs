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
	partial class AppDelegate
	{
		[Outlet]
		AppKit.NSMenuItem mnuExportFile { get; set; }

		[Outlet]
		AppKit.NSMenuItem mnuNew { get; set; }

		[Outlet]
		AppKit.NSMenuItem mnuOpen { get; set; }

		[Outlet]
		AppKit.NSMenuItem mnuPreferences { get; set; }

		[Outlet]
		AppKit.NSMenuItem mnuQuit { get; set; }

		[Outlet]
		AppKit.NSMenuItem mnuRevertFile { get; set; }

		[Outlet]
		AppKit.NSMenuItem mnuSaveFile { get; set; }

		[Outlet]
		AppKit.NSMenuItem mnuSaveFileAs { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (mnuPreferences != null) {
				mnuPreferences.Dispose ();
				mnuPreferences = null;
			}

			if (mnuQuit != null) {
				mnuQuit.Dispose ();
				mnuQuit = null;
			}

			if (mnuNew != null) {
				mnuNew.Dispose ();
				mnuNew = null;
			}

			if (mnuOpen != null) {
				mnuOpen.Dispose ();
				mnuOpen = null;
			}

			if (mnuRevertFile != null) {
				mnuRevertFile.Dispose ();
				mnuRevertFile = null;
			}

			if (mnuSaveFile != null) {
				mnuSaveFile.Dispose ();
				mnuSaveFile = null;
			}

			if (mnuSaveFileAs != null) {
				mnuSaveFileAs.Dispose ();
				mnuSaveFileAs = null;
			}

			if (mnuExportFile != null) {
				mnuExportFile.Dispose ();
				mnuExportFile = null;
			}
		}
	}
}
