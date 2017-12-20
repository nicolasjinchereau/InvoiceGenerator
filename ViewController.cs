/*---------------------------------------------------------------------------------------------
*  Copyright (c) Nicolas Jinchereau. All rights reserved.
*  Licensed under the MIT License. See License.txt in the project root for license information.
*--------------------------------------------------------------------------------------------*/

using System;
using AppKit;
using Foundation;
using Ionic.Zip;
using Ionic.Zlib;
using System.Linq;
using System.IO;
using JsonFx;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;

namespace InvoiceGenerator
{
    public class Invoice
    {
        public DateTime date;
        public int invoiceNumber;
        public string consultant;
        public string client;
        public string filename;
        public string services;
        public float[] hours;
        public float rate;
        public float taxRate;
    }

    public partial class ViewController : NSViewController
    {
        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            invoiceDate.DateValue = (NSDate)DateTime.Now;
            hoursText.Activated += UpdatePrice;
            rateText.Activated += UpdatePrice;
            taxRateText.Activated += UpdatePrice;
            invoiceNumberText.EditingEnded += OnEditingEnded;
            hoursText.EditingEnded += OnEditingEnded;
            rateText.EditingEnded += OnEditingEnded;
            taxRateText.EditingEnded += OnEditingEnded;
            saveButton.Activated += OnSavePressed;
            incrementButton.Activated += OnIncrementPressed;
            revertButton.Activated += OnRevertPressed;

            Load();

            UpdatePrice(null, null);
        }

        public override void ViewDidDisappear()
        {
            base.ViewDidDisappear();
            Save();
        }

        void OnIncrementPressed(object sender, EventArgs args) {
            ++invoiceNumberText.IntValue;
        }

        void OnRevertPressed(object sender, EventArgs args) {
            Load();
        }

        void OnEditingEnded(object sender, EventArgs args)
        {
            var errorColor = NSColor.FromRgb(0.9f, 0.0f, 0.0f);
            var validFont = NSFont.SystemFontOfSize(NSFont.SystemFontSize, 0);
            var errorFont = NSFont.SystemFontOfSize(NSFont.SystemFontSize, 1);

            int intResult;
            float floatResult;

            var invoiceValid = int.TryParse(invoiceNumberText.StringValue, out intResult);
            var hoursValid = SplitFloatCSV(hoursText.StringValue).Length > 0;
            var rateValid = float.TryParse(rateText.StringValue, out floatResult);
            var taxRateValid = float.TryParse(taxRateText.StringValue, out floatResult);

            invoiceNumberText.TextColor = invoiceValid ? NSColor.ControlText : errorColor;
            invoiceNumberText.Font = invoiceValid ? validFont : errorFont;
            hoursText.TextColor = hoursValid ? NSColor.ControlText : errorColor;
            hoursText.Font = hoursValid ? validFont : errorFont;
            rateText.Font = rateValid ? validFont : errorFont;
            rateText.TextColor = rateValid ? NSColor.ControlText : errorColor;
            taxRateText.TextColor = taxRateValid ? NSColor.ControlText : errorColor;
            taxRateText.Font = taxRateValid ? validFont : errorFont;
        }

        string InvoiceFilePath
        {
            get {
                var applicationSupport = NSFileManager.DefaultManager.GetUrls(
                    NSSearchPathDirectory.ApplicationSupportDirectory,
                    NSSearchPathDomain.User)[0].Path;
                
                return applicationSupport + "/com.showdownsoftware.InvoiceGenerator/invoice.json";
            }
        }

        void Load()
        {
            var invoiceFilePath = InvoiceFilePath;

            try
            {
                if(File.Exists(invoiceFilePath))
                {
                    var json = File.ReadAllText(invoiceFilePath);
                    var invoice = JsonReader.Deserialize<Invoice>(json);

                    invoiceDate.DateValue = (NSDate)invoice.date;
                    invoiceNumberText.IntValue = invoice.invoiceNumber;
                    consultantText.Value = invoice.consultant;
                    clientText.Value = invoice.client;
                    filenameText.StringValue = invoice.filename;
                    servicesText.Value = invoice.services;
                    hoursText.StringValue = string.Join(", ", invoice.hours.Select(f => f.ToString("0.0#")));
                    rateText.FloatValue = invoice.rate;
                    taxRateText.FloatValue = invoice.taxRate;
                }
            }
            catch(Exception ex) {
                MessageBox.Show("Error", "'invoice.json' could not be loaded.\n\n" + ex.Message);
            }
        }

        void Save()
        {
            var invoice = new Invoice() {
                date = ((DateTime)invoiceDate.DateValue).ToLocalTime(),
                invoiceNumber = invoiceNumberText.IntValue,
                consultant = consultantText.Value,
                client = clientText.Value,
                filename = filenameText.StringValue,
                services = servicesText.Value,
                hours = SplitFloatCSV(hoursText.StringValue),
                rate = rateText.FloatValue,
                taxRate = taxRateText.FloatValue
            };

            var settings = new JsonWriterSettings(){ PrettyPrint = true, Tab = "  " };
            var json = JsonWriter.Serialize(invoice, settings);

            var dir = Path.GetDirectoryName(InvoiceFilePath);
            if(!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            
            File.WriteAllText(InvoiceFilePath, json);
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

        public void UpdatePrice(object sender, EventArgs args)
        {
            float hours = SplitFloatCSV(hoursText.StringValue).Sum();
            float rate = rateText.FloatValue;
            float taxRate = taxRateText.FloatValue / 100.0f;

            float subtotal = hours * rate;
            float taxAmount = subtotal * taxRate;
            float total = subtotal + taxAmount;

            totalHoursText.StringValue = hours.ToString("0.0#");
            subtotalText.StringValue = subtotal.ToString("N2");
            taxAmountText.StringValue = taxAmount.ToString("N2");
            totalPriceText.StringValue = total.ToString("C2");
        }

        public void OnSavePressed(object sender, EventArgs args)
        {
            var date = ((DateTime)invoiceDate.DateValue).ToLocalTime();

            var invoiceNumberString = invoiceNumberText.IntValue.ToString("0000");
            var dateString = date.ToString("yyyy_MM_dd");

            var filename = filenameText.StringValue;
            filename = filename.Replace("$INV", invoiceNumberString);
            filename = filename.Replace("$DATE", dateString);

            NSSavePanel sp = NSSavePanel.SavePanel;
            sp.AllowedFileTypes = new string[] { "docx" };
            sp.Directory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            sp.NameFieldStringValue = filename;

            var ret = sp.RunModal();
            if(ret != 0)
            {
                string path = NSBundle.MainBundle.PathForResource("template.docx", null);

                using(ZipFile zip = ZipFile.Read(path))
                {
                    var entry = zip.SelectEntries("name = document.xml", "word/").First();
                    var stream = new MemoryStream(4096);
                    entry.Extract(stream);
                    stream.Position = 0;
                    var content = new StreamReader(stream).ReadToEnd();

                    content = content.Replace("[DATE]", date.ToString("yyyy-MM-dd"));
                    content = content.Replace("[INVOICE]", invoiceNumberString);
                    content = content.Replace("[CONSULTANT]", ConvertLineEndingsToDocxTags(consultantText.Value));
                    content = content.Replace("[CLIENT]", ConvertLineEndingsToDocxTags(clientText.Value));
                    content = content.Replace("[SERVICE]", ConvertLineEndingsToDocxTags(servicesText.Value));
                    content = content.Replace("[ITEM]", subtotalText.StringValue);
                    content = content.Replace("[TOTAL_HOURS]", totalHoursText.StringValue);
                    content = content.Replace("[SUBTOT]", subtotalText.StringValue);
                    content = content.Replace("[TAX]", taxAmountText.StringValue);
                    content = content.Replace("[TOTAL]", totalPriceText.StringValue);

                    zip.UpdateEntry("word/document.xml", content);
                    zip.CompressionLevel = CompressionLevel.BestCompression;
                    zip.Save(sp.Url.Path);
                }

                Save();
            }
        }

        string ConvertLineEndingsToDocxTags(string str)
        {
            var sb = new StringBuilder(str.Length * 2);
            sb.Append("</w:t><w:t xml:space=\"preserve\">");
            sb.Append(Regex.Replace(str, @"\r\n|\n\r|\n|\r", "</w:t><w:br/><w:t xml:space=\"preserve\">"));
            return sb.ToString();
        }

        public override NSObject RepresentedObject {
            get { return base.RepresentedObject; }
            set { base.RepresentedObject = value; }
        }
    }
}
