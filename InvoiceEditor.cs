/*---------------------------------------------------------------------------------------------
*  Copyright (c) Nicolas Jinchereau. All rights reserved.
*  Licensed under the MIT License. See License.txt in the project root for license information.
*--------------------------------------------------------------------------------------------*/

using System;
using System.IO;
using JsonFx;
using Novacode;
using System.Collections.Generic;

namespace InvoiceGenerator
{
    public class InvoiceEditor
    {
        public string DocumentPath { get; private set; } = null;
        public InvoiceDocument Document  { get; private set; } = new InvoiceDocument();
        public bool Dirty { get; private set; } = false;

        public void NewDocument()
        {
            DocumentPath = null;
            Document = new InvoiceDocument();
            Dirty = false;
        }

        public void Load(string path)
        {
            if(!File.Exists(path))
                throw new Exception("File was not found: " + path);

            var extension = Path.GetExtension(path);
            if(string.Compare(extension, ".json", true) != 0)
                throw new Exception("Invalid file type");
            
            var settings = new JsonReaderSettings()
            {
                DateTimeDeserializer = (JsonReader reader) => {
                    var str = reader.Read(typeof(string), false) as string;
                    var date = DateTime.Parse(str);
                    date = DateTime.SpecifyKind(date, DateTimeKind.Local);
                    return date;
                }
            };

            var json = File.ReadAllText(path);
            var doc = JsonReader.Deserialize<InvoiceDocument>(json, settings);

            Document = doc;
            DocumentPath = path;
            Dirty = false;
        }

        public void SetDirty() {
            Dirty = true;
        }

        public void Save() {
            SaveAs(DocumentPath);
        }

        public void SaveAs(string path)
        {
            if(string.IsNullOrEmpty(path))
                throw new Exception("No filename was specified");

            if(Document == null)
                throw new Exception("No document is currently open");
            
            var settings = new JsonWriterSettings()
            {
                PrettyPrint = true,
                Tab = "  ",
                DateTimeSerializer = (JsonWriter writer, DateTime value) => {
                    var str = value.ToString("yyyy-MM-ddTHH:mm:ss");
                    writer.Write(str);
                }
            };

            var json = JsonWriter.Serialize(Document, settings);
            File.WriteAllText(path, json);

            DocumentPath = path;
            Dirty = false;
        }

        public void Export(string templatePath, string savePath)
        {
            using(DocX doc = DocX.Load(templatePath))
            {
                var hoursString = Document.TotalHours.ToString("0.0#");
                var rateString = Document.rate.ToString("0.00");
                var subtotalString = Document.Subtotal.ToString("N2");
                var taxString = Document.Tax.ToString("N2");
                var totalString = Document.Total.ToString("C2");

                var sessions = new List<Session>(Document.sessions);
                sessions.Sort((x, y) => x.start.CompareTo(y.start));

                var firstDayString = sessions[0].start.Date.ToString("yyyy-MM-dd");
                var lastDayString = sessions[sessions.Count - 1].start.Date.ToString("yyyy-MM-dd");

                var services = $"{firstDayString} to {lastDayString}\n";
                services += $"{hoursString} hours @ ${rateString}/h\n\n";
                services += Document.services;

                doc.ReplaceText("[DATE]", Document.date.ToString("yyyy-MM-dd"));
                doc.ReplaceText("[INVOICE]", Document.invoiceNumber.ToString("0000"));
                doc.ReplaceText("[CONSULTANT]", Document.consultant);
                doc.ReplaceText("[CLIENT]", Document.client);
                doc.ReplaceText("[SERVICE]", services);
                doc.ReplaceText("[PRICE]", subtotalString);
                doc.ReplaceText("[SUBTOT]", subtotalString);
                doc.ReplaceText("[TAX]", taxString);
                doc.ReplaceText("[TOTAL]", totalString);
                doc.SaveAs(savePath);
            }
        }
    }
}
