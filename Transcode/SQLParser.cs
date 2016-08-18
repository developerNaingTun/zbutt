using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Transcode
{
    class TSQLTable
    {
        private string name;
        private ArrayList columns;
        public ArrayList enc;
        public TSQLTable(string name, ArrayList cols, ArrayList enc)
        {
            this.name = name;
            this.columns = cols;
            this.enc = enc;
        }
        public string getname()
        {
            return name;
        }
        public ArrayList getcols()
        {
            return columns;
        }
    }

    class SQLParser
    {
        public static void DoTranscode(string In, ArrayList SQLTables, ArrayList Encs,string Out,bool HTML)
        {
            StreamReader sr = new StreamReader(In);
            StreamWriter sw = new StreamWriter(Out, false, Encoding.UTF8);
            String temp = "";
            int state = 0;
            bool newline = true;
            while (!sr.EndOfStream)
            {
                char c = (char)sr.Read();
                switch (state)
                {
                    case 0:
                        if (c == '\'')
                        {
                            state = 1;
                            temp += c;
                        }
                        else if (c == ';')
                        {
                            temp += c;
                            temp = temp.Trim();
                            if (temp.StartsWith("INSERT INTO"))
                            {
                                string table = FetchTableName(temp);
                                for (int i = 0; i < SQLTables.Count; i++)
                                {
                                    if (table == ((TSQLTable)SQLTables[i]).getname())
                                    {
                                        String ted = FiniteAutoTranscode(temp, Encs,((TSQLTable)SQLTables[i]).enc,HTML);
                                        sw.WriteLine(ted);
                                    }
                                }
                            }
                            else
                            {
                                sw.WriteLine(temp);
                            }
                            temp = "";

                        }
                        else if (c == '\n' || c == '\r')
                        {
                            //
                        }
                        else if (c == '-' && newline)
                        {
                            char t = (char)sr.Read();
                            while (t != '\r' && t != '\n')
                            {
                                t = (char)sr.Read();
                            }
                        }
                        else
                        {
                            temp += c;
                        }
                        break;
                    case 1:
                        if (c == '\\')
                            state = 2;
                        else if (c == '\'')
                        {
                            state = 0;
                            temp += c;
                        }
                        else
                        {
                            temp += c;
                        }
                        break;
                    case 2:
                        if (c == 'n')
                            temp += '\n';
                        else if (c == 'r')
                            temp += '\r';
                        state = 1;
                        break;
                }
                if (c == '\n' || c == '\r')
                {
                    newline = true;
                }
                else newline = false;
            }
            sw.Close();
        }

        public static string FiniteAutoTranscode(string s, ArrayList encs, ArrayList enclst,bool HTML)
        {
            string result = "";
            s = s.Trim();
            if (s.StartsWith("INSERT"))
            {
                s = s.Substring(6);
                s = s.Trim();
                result += "INSERT ";
            }
            if (s.StartsWith("INTO"))
            {
                s = s.Substring(4);
                s = s.Trim();
                result += "INTO ";
            }
            if (s.StartsWith("VALUES"))
            {
                s = s.Substring(6);
                s = s.Trim();
                result += "VALUES ";
            }
            else
            {
                result += s.Substring(0, s.IndexOf("VALUES"));
                s = s.Substring(s.IndexOf("VALUES") + 6);
                result += "VALUES ";
            }
            string tmp = "";
            ArrayList al = new ArrayList();
            int state = 0;
            int colnum = 0;
            for (int i = 0; i < s.Length; i++)
            {
                char x = s[i];
                switch (state)
                {
                    case 0:
                        if (x == '(')
                        {
                            state = 1;
                            result += x;
                        }
                        break;
                    case 1:
                        if (x == ' ')
                        {
                        }
                        else if (x == '\'')
                        {
                            state = 2;
                        }
                        else if (x == ',')
                        {
                            if (tmp == "") return null;
                            result += tmp + ",";
                            colnum++;
                            tmp = "";
                        }
                        else if (x == ')')
                        {
                            if (tmp == "") return null;
                            result += tmp;
                            colnum++;
                            tmp = "";
                        }
                        else
                        {
                            tmp += x;
                        }
                        break;
                    case 2:
                        if (x == '\'')
                        {
                            int tr = ((int)enclst[colnum]);
                            if (tr > -1)
                            {
                                TEncoding ten = (TEncoding)encs[tr];
                                string ted;
                                if (HTML) ted = Transcode.XMLEntNormalizer(tmp);
                                else ted = tmp;
                                ted = Transcode.transcode(ten.getENI(), ten.getENO(), ted);
                                if (HTML) ted = Transcode.NomalizedtoXML(ted);
                                result += "'" + ted + "'";
                            }
                            else
                            {
                                result += "'" + tmp + "'";
                            }
                            tmp = "";
                            state = 4;
                        }
                        else if (x == '\\')
                        {
                            state = 3;
                        }
                        else
                        {
                            tmp += x;
                        }
                        break;
                    case 3:
                        tmp += x;
                        state = 2;
                        break;
                    case 4:
                        if (x == ',')
                        {
                            state = 1;
                            result += x;
                        }
                        else if (x == ')')
                        {
                            result += x;
                            state = 5;
                        }
                        break;
                }
            }
            return result + ";";
        }

        public static ArrayList getTables(string filename)
        {
            StreamReader sr = new StreamReader(filename);
            ArrayList al = new ArrayList();
            String temp = "";
            int state = 0;
            while(!sr.EndOfStream){
                char c = (char)sr.Read();
                switch (state)
                {
                    case 0:
                        if (c == '\'')
                        {
                            state = 1;
                            temp += c;
                        }
                        else if (c == ';')
                        {
                            temp += c;
                            temp = temp.Trim();
                            if (temp.StartsWith("INSERT INTO"))
                            {
                                String name = FetchTableName(temp);
                                ArrayList ax = FetchColumns(temp);
                                if (ax != null)
                                {
                                    ArrayList exls = new ArrayList();
                                    foreach (string x in ax)
                                        exls.Add(-1);
                                    TSQLTable t = new TSQLTable(name, ax, exls);
                                    if (!Exists(al, t))
                                    {
                                        al.Add(t);
                                    }
                                }
                            }
                            //MessageBox.Show(temp);
                            temp = "";
                        }
                        else if (c == '\n' || c == '\r')
                        {
                            //nothing
                        }
                        else
                        {
                            temp += c;
                        }
                        break;
                    case 1:
                        if (c == '\\')
                            state = 2;
                        else if (c == '\'')
                        {
                            state = 0;
                            temp += c;
                        }
                        else
                        {
                            temp += c;
                        }
                            break;
                    case 2:
                        if (c == 'n')
                            temp += '\n';
                        else if (c == 'r')
                            temp += '\r';
                        state = 1;
                        break;
                }
            }
            return al;
        }

        public static ArrayList FetchColumns(string s)
        {
            // INSERT INTO `phpbb_categories` VALUES (2, 'xyzabcd', 20);
            ArrayList ax = new ArrayList();
            Regex r = new Regex(@"INSERT[\s]+INTO[\s]+`?\w+`?");
            s = s.Replace(r.Match(s).Value, "");
            s = s.Trim();
            if (s.StartsWith("VALUES"))
            {
                s = s.Replace("VALUES", "");
                ArrayList al = innerFetchColumn(s);
                if (al == null) return null;
                for (int i = 0; i < al.Count; i++)
                {
                    ax.Add("Col#" + i.ToString());
                }
            }
            else
            {
                s = s.Substring(0,s.IndexOf("VALUES"));
                ax = innerFetchColumnNames(s);
            }
            return ax;
        }

        public static ArrayList innerFetchColumnNames(String s)
        {
            Regex r = new Regex(@"`?\w+`?,?");
            MatchCollection mc =  r.Matches(s);
            ArrayList al = new ArrayList();
            foreach (Match m in mc)
            {
                string a = m.Value;
                if (a[a.Length - 1] == ',')
                    a = a.Substring(0, a.Length - 1);
                al.Add(a);
            }
            return al;
        }

        public static ArrayList innerFetchColumn(string s)
        {
            //"(2, 'xyzabcd', 20)";
            string tmp = "";
            ArrayList al = new ArrayList();
            int state = 0;
            for(int  i=0;i<s.Length;i++)
            {
                char x = s[i];
                switch (state)
                {
                    case 0:
                        if (x == '(')
                            state = 1;
                        break;
                    case 1:
                        if (x == ' ')
                        {
                        }
                        else if (x == '\'')
                        {
                            state = 2;
                        }
                        else if (x == ',')
                        {
                            if (tmp == "") return null;
                            al.Add(tmp);
                            tmp = "";
                        }
                        else if (x == ')')
                        {
                            if (tmp == "") return null;
                            al.Add(tmp);
                            tmp = "";
                        }
                        else
                        {
                            tmp += x;
                        }
                        break;
                    case 2:
                        if (x == '\'')
                        {
                            al.Add(tmp);
                            tmp = "";
                            state = 4;
                        }
                        else if (x == '\\')
                        {
                            state = 3;
                        }
                        else
                        {
                            tmp += x;
                        }
                        break;
                    case 3:
                        tmp += x;
                        state = 2;
                        break;
                    case 4:
                        if (x == ',')
                        {
                            state = 1;
                        }
                        else if (x == ')')
                        {
                            state = 5;
                        }
                        break;
                }
            }
            return al;
        }

        private static bool Exists(ArrayList al, TSQLTable s)
        {
            foreach (TSQLTable x in al)
                if (x.getname() == s.getname())
                    return true;
            return false;
        }

        public static string FetchTableName(string s)
        {
            Regex r = new Regex(@"INSERT[\s]+INTO[\s]+`?\w+`?");
            s = r.Match(s).Value;
            s = Regex.Match(s, @"\s+`?\w+`?$").Value;
            s = s.Trim();
            return s;
        }
    }
}
