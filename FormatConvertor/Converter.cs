using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace formatConvert
{
    public class iConverter
    {
        private string name;
        private string format;
        private myItem[] myItemCollection = new myItem[7];

        public iConverter(string argFileName, string argFileFormat)
        {
            name = argFileName;
            format = argFileFormat;
            myItemCollection[0] = new myItem("Text File (*.txt)", ".txt", Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatText);
            myItemCollection[1] = new myItem("RTF Doc(*.rtf)", ".rtf", Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatRTF);
            myItemCollection[2] = new myItem("Word Doc (*.doc)", ".doc", Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatDocument);
            myItemCollection[3] = new myItem("Web Doc (*.html)", ".html", Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatHTML);
            myItemCollection[4] = new myItem("Single File Web Doc (*.mhtml)", ".mhtml", Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatWebArchive);
            myItemCollection[5] = new myItem("XML Doc(*.xml)", ".xml", Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatXML);
            myItemCollection[6] = new myItem("Web Doc filtered (*.html)", ".html", Microsoft.Office.Interop.Word.WdSaveFormat.wdFormatFilteredHTML);
        }
        public void convert()
        {
            myItem tmpItem = new myItem() ; 
            string inFileName = name;
            string inFileFormat=format;
            if (!File.Exists(inFileName))
            {
                Console.WriteLine(inFileName + " does not exist.  Please select an existing file to convert.");
            }
            switch (inFileFormat)
            {
                case "txt":
                    tmpItem = myItemCollection[0];
                    break;
                case "rtf":
                    tmpItem = myItemCollection[1];
                    break;
                case "doc":
                    tmpItem = myItemCollection[2];
                    break;
                case "html":
                    tmpItem = myItemCollection[3];
                    break;
                case "mhtml":
                    tmpItem = myItemCollection[4];
                    break;
                default:
                   Console.WriteLine("Unknown Format");
                    break;
            }
            object fileName = inFileName;
            object fileSaveName = inFileName.Substring(0, inFileName.LastIndexOf(".")) + tmpItem.ItemExtension; //".txt";
            object vk_read_only = false;
            object vk_visible = true;
            object vk_true = true;
            object vk_false = false;
            object vk_dynamic = 2;

            object missing = System.Reflection.Missing.Value; 
            object vk_range = missing;
            object vk_to = missing;
            object vk_from = missing;
            Microsoft.Office.Interop.Word.ApplicationClass vk_word_app = new Microsoft.Office.Interop.Word.ApplicationClass();


            Microsoft.Office.Interop.Word.Document aDoc = null;
            try
            {
                aDoc = vk_word_app.Documents.Open(
                    ref fileName, ref missing, ref vk_read_only,
                    ref missing, ref missing,
                    ref missing, ref missing, ref missing, ref missing,
                    ref missing, ref missing, ref vk_visible, ref missing, ref missing, ref missing, ref missing);
            }
            catch (System.Exception ex)
            {

                Console.WriteLine("There was a problem opening " + fileName + " error:" + ex.ToString());
            }
            try
            {
                object vk_saveformat = tmpItem.ItemWord;

                aDoc.SaveAs(ref fileSaveName, ref vk_saveformat, ref missing, ref missing, ref missing, ref missing,
                    ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing,
                    ref missing, ref missing, ref missing);
            }
            catch (System.Exception ex)
            { Console.WriteLine("Error : " + ex.ToString()); }
            finally
            {
                if (aDoc != null)
                {
                    aDoc.Close(ref vk_false, ref missing, ref missing);
                }
                vk_word_app.Quit(ref vk_false, ref missing, ref missing);
            }
        }
        }
}
