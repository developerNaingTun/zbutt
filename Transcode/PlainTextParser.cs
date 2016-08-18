using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using System.Windows.Forms;

namespace Transcode
{
    class PlainTextParser
    {
        public static void parseandtranscode(string input, string output, TEncoding en)
        {
            StreamReader sr = new StreamReader(input);
            StreamWriter sw = new StreamWriter(output, false);
            while (!sr.EndOfStream)
            {
                string s = sr.ReadLine();
                s = Transcode.transcode(en.getENI(), en.getENO(), s);
                sw.WriteLine(s);
            }
            sw.Close();
            sr.Close();
        }
    }

    class XMLTextParser
    {
        public static void parseandtranscode(string input, string output, TEncoding en)
        {
            StreamReader sr = new StreamReader(input);
            StreamWriter sw = new StreamWriter(output);
            int state = 0;
            string temp = "";
            while (!sr.EndOfStream)
            {
                char x = (char)sr.Read();
                switch (state)
                {
                    case 0:
                        if (x == '<')
                        {
                            state = 1;
                            temp = Transcode.transcode(en.getENI(), en.getENO(), temp);
                            sw.Write(temp);
                            temp = "";
                            sw.Write(x);
                        }
                        else
                        {
                            temp += x;
                        }
                        break;
                    case 1:
                        if (x == '"')
                        {
                            state = 2;
                            sw.Write(x);
                        }
                        else if (x == '>')
                        {
                            state = 1;
                            sw.Write(x);
                        }
                        else
                        {
                            sw.Write(x);
                        }
                        break;
                    case 2:
                        if (x == '"')
                        {
                            state = 1;
                            sw.Write(x);
                        }
                        break;
                }
            }
        }
    }

    class CSVTextParser
    {
        public static int howmanycolumns(string file)
        {
            StreamReader sr = new StreamReader(file);
            int state = 0;
            int lastvalue = 0;
            int values = 0;
            while (!sr.EndOfStream)
            {
                char x = (char)sr.Read();
                switch (state)
                {
                    case 0:
                        if (x == '"')
                        {
                            state = 1;
                        }
                        else if (x == ',')
                        {
                            values++;
                        }
                        else if (x == ' ')
                        {
                        }
                        else if (x == '\r')
                        {
                        }
                        else if (x == '\n')
                        {
                            if (values > lastvalue)
                            {
                                lastvalue = values;
                                
                            }
                            values = 0;
                        }
                        else
                        {
                            //this will get data
                        }
                        break;
                    case 1:
                        if (x == '"')
                        {
                            state = 2;
                        }
                        else
                        {
                            //get data
                        }
                        break;
                    case 2:
                        if (x == '"')
                        {
                            //add data
                            state = 1;
                        }
                        else if (x == ',')
                        {
                            state = 0;
                            values++;
                        }
                        else if (x == '\n')
                        {
                            if (values > lastvalue)
                            {
                                lastvalue = values;
                            }
                            state = 0;
                            values = 0;
                        }
                        else
                        {
                        }
                        break;
                }
            }
            if (values > lastvalue)
                lastvalue = values;
            return lastvalue;
        }

        public static void parseandtranscode(string input, string output, ArrayList encs, ArrayList eninds)
        {
            StreamReader sr = new StreamReader(input);
            StreamWriter sw = new StreamWriter(output);
            int state = 0;
            int values = 0;
            string temp = "";
            while (!sr.EndOfStream)
            {
                char x = (char)sr.Read();
                switch (state)
                {
                    case 0:
                        if (x == '"')
                        {
                            temp = "";
                            state = 1;
                        }
                        else if (x == ',')
                        {
                            if ((int)eninds[values] > -1)
                            {
                                TEncoding te = ((TEncoding)encs[(int)eninds[values]]);
                                temp = Transcode.transcode(te.getENI(), te.getENO(), temp);
                            }
                            sw.Write(temp + ",");
                            temp = "";
                            state = 0;
                            values++;
                        }
                        else if (x == '\r')
                        {
                        }
                        else if (x == '\n')
                        {
                            if ((int)eninds[values] > -1)
                            {
                                TEncoding te = ((TEncoding)encs[(int)eninds[values]]);
                                temp = Transcode.transcode(te.getENI(), te.getENO(), temp);
                            }
                            sw.WriteLine(temp);
                            temp = "";
                            state = 0;
                            values = 0;
                        }
                        else
                        {
                            temp += x;
                            //this will get data
                        }
                        break;
                    case 1:
                        if (x == '"')
                        {
                            state = 2;
                        }
                        else
                        {
                            temp += x;
                            //get data
                        }
                        break;
                    case 2:
                        if (x == '"')
                        {
                            temp += '"';
                            state = 1;
                        }
                        else if (x == ',')
                        {
                            if ((int)eninds[values] > -1)
                            {
                                TEncoding te = ((TEncoding)encs[(int)eninds[values]]);
                                temp = Transcode.transcode(te.getENI(), te.getENO(), temp);
                            }
                            sw.Write("\"" + temp + "\",");
                            temp = "";
                            state = 0;
                            values++;
                        }
                        else if (x == '\n')
                        {
                            if ((int)eninds[values] > -1)
                            {
                                TEncoding te = ((TEncoding)encs[(int)eninds[values]]);
                                temp = Transcode.transcode(te.getENI(), te.getENO(), temp);
                            }
                            sw.WriteLine("\"" + temp + "\"");
                            temp = "";
                            state = 0;
                            values = 0;
                        }
                        else
                        {
                        }
                        break;
                }
            }
            sr.Close();
            sw.Close();
        }
    }
}
