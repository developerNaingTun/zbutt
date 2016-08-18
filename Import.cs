using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;

namespace ImportManager
{
    public class Import
    {
        public ArrayList Import_eni(string filePath)
        {
            FileStream inputStream;
            ArrayList returnList = new ArrayList();
            StreamReader strReader1;
            string strLine;

            try
            {
                inputStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                strReader1 = new StreamReader(inputStream, Encoding.Unicode);
                while ((strLine = strReader1.ReadLine()) != null)
                {
                    returnList.Add(strLine);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                returnList = null;
            }
            return returnList;
        }
    }
    /// <summary>
    /// Not Use
    /// </summary>
    public class FileName
    {
        private String fullPath;
        private char pathSeparator, extensionSeparator;

        public FileName(String str, char sep, char ext)
        {
            fullPath = str;
            pathSeparator = sep;
            extensionSeparator = ext;

        }
        public String extension()
        {
            int dot = fullPath.LastIndexOf(extensionSeparator);
            return fullPath.Substring(dot + 1);

        }
        public String filename()
        {
            int dot = fullPath.LastIndexOf(extensionSeparator);
            int sep = fullPath.LastIndexOf(pathSeparator);
            return fullPath.Substring(sep + 1, dot - (sep + 1));

        }
        public String path()
        {
            int sep = fullPath.LastIndexOf(pathSeparator);
            return fullPath.Substring(0, sep);
        }

    }
}
