using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Configuration;
using System.Collections;
using ImportManager;
using Transcode;
using System.Windows.Forms;
using System.IO;

namespace zbutt
{
    class iController
    {
        private formMain frm;
        public void assign(formMain argForm)
        {
            frm = argForm;
        }
        public bool checkVersion()
        {
            return true;
        }
        public void getEnio(string path,ArrayList super)
        {
            Import imp1 = new Import();
            string[] stAry,fileList;
            ArrayList temp;
            string extension;
            if (Directory.Exists(path))
            {
                fileList=Directory.GetFiles(path);
                for (int i = 0; i < fileList.Length; i++)
                {
                    extension=fileList[i].Substring(fileList[i].IndexOf('.')+1);
                    if (extension == "eni" || extension == "eno")
                    {
                        //MessageBox.Show(fileList[i]);
                        temp = imp1.Import_eni(fileList[i]);
                        stAry = new string[temp.Count];
                        for (int j = 0; j < temp.Count; j++)
                            stAry[j] = temp[j].ToString();
                        super.Add(stAry);
                    }
                }
            }
            else
            {
                MessageBox.Show("input folder Missing");
            }
        }

        public static void start_transcode(string filename,ArrayList fltran,string output,bool html)
        {
            /*
            Import imp = new Import();
            string eni_path =Application.StartupPath + "\\input\\eni\\";
            string eno_path =Application.StartupPath + "\\input\\eno\\";
            ArrayList enis = new ArrayList();
            ArrayList enos = new ArrayList();
            getEnio(eni_path, enis);
            getEnio(eno_path, enos);
            */
            ArrayList enis = new ArrayList();
            ArrayList enos = new ArrayList();
            GenericEncodingFramework gef = new GenericEncodingFramework();
            if (!gef.loaded)
                gef.Load();
            ArrayList Encodings = gef.getEncodings();
            foreach (TEncoding e in Encodings)
            {
                enis.Add(e.getENI());
                enos.Add(e.getENO());
            }
            RichTextParser rtp = new RichTextParser();
            ArrayList al = rtp.GetFonts(filename);
            rtp.DoTranscode(filename, enis, enos, fltran, output,html);
            //MessageBox.Show("Success");
        } 
    }
}
