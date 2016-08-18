using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using zbutt;

namespace Transcode
{
    class GenericEncodingFramework
    {
        public GenericEncodingFramework()
        {
            Encodings = new ArrayList();
        }

        public void Load()
        {
            //AddENCLIST();
            LoadTranscodeLanguage();
            loaded = true;
        }

        private void LoadTranscodeLanguage()
        {
            String[] enifiles = Directory.GetFiles("Encodings", "*.tl");
            regexs = new ArrayList();
            string key = "98761197agde5d2g13asdh8wjktwa6f5";
            for (int i = 0; i < enifiles.Length; i++)
            {
                TranscodeLanguage.Parser pl = new TranscodeLanguage.Parser(enifiles[i],key);
                if (pl.RES == "OK")
                {
                    ArrayList eni = pl.getENI();
                    ArrayList eno = pl.getENO();
                    foreach (TranscodeLanguage.header h in pl.getHeaders())
                    {
                        if (h.hname == "from")
                        {
                            TEncoding tntemp = new TEncoding(eni, eno, h.hvalue, "XPartial");
                            Encodings.Add(tntemp);
                            foreach (string s in pl.getAutos())
                            {
                                string[] x = new string[2];
                                x[0] = s;
                                x[1] = tntemp.getFrom();
                                regexs.Add(x);
                            }
                        }
                    }
                }
                else
                {
                    remresfm fm = new remresfm();
                    fm.setText("Error: Cannot Compile Transcode Language File\r\n"+ enifiles[i] + "\r\n"+ pl.RES + "\r\nLine: " + pl.line.ToString());
                    fm.ShowDialog();
                }
            }
        }

        public Boolean loaded = false;

        public ArrayList getEncodings()
        {
            return Encodings;
        }

        public TEncoding getEncoding(String from)
        {
            for (int i = 0; i < Encodings.Count; i++)
            {
                if( ((TEncoding)Encodings[i]).getFrom() == from)
                    return (TEncoding)Encodings[i];
            }
            return null;
        }

        public int isTo(String s){
            foreach (string[] ss in regexs)
            {
                
                if (Regex.IsMatch(s, ss[0]))
                {
                    
                    for (int i = 0; i < Encodings.Count; i++)
                    {
                        TEncoding t = (TEncoding)Encodings[i];
                        if (t.getFrom() == ss[1])
                        {
                            return i;
                        }
                    }
                }
            }
            return -1;
        }

        private void AddENCLIST()
        {
            String[] enifiles = Directory.GetFiles("Encodings", "*.eni");
            for (int i = 0; i < enifiles.Length; i++)
            {
                ArrayList eni = ReadFile(enifiles[i]);
                ArrayList eno = ReadFile(enifiles[i].Substring(0, enifiles[i].Length - 4) + ".eno");
                String fromname = enifiles[i].Substring(0, enifiles[i].Length - 4);
                fromname = fromname.Replace("Encodings\\", "");
                TEncoding tntemp = new TEncoding(eni, eno, fromname, "XPartial");
                Encodings.Add(tntemp);
            }

            StreamReader sr = new StreamReader("Encodings\\namesregex.fmx");
            regexs = new ArrayList();
            while (!sr.EndOfStream)
            {
                string s = sr.ReadLine();
                regexs.Add(s.Split('\t'));
            }

        }


        private ArrayList regexs;
        private ArrayList ReadFile(String filename)
        {
            StreamReader sr = new StreamReader(filename);
            String s;
            ArrayList al = new ArrayList();
            while ((s = sr.ReadLine()) != null)
            {
                al.Add(s);
            }
            return al;
        }

        private static ArrayList Encodings;

    }

    class TEncoding
    {
        private ArrayList ENI;
        private ArrayList ENO;
        private String from;
        private String to;
        public ArrayList getENI()
        {
            return ENI;
        }
        public ArrayList getENO()
        {
            return ENO;
        }
        public String getTo()
        {
            return to;
        }
        public String getFrom()
        {
            return from;
        }
        public TEncoding(ArrayList ENI,ArrayList ENO,String from,String to)
        {
            this.ENI = ENI;
            this.ENO = ENO;
            this.from = from;
            this.to = to;
        }

    }
}
