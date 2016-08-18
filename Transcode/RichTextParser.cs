using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Net.Sgoliver.NRtfTree.Core;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace Transcode
{
    class RichTextParser
    {
        public ArrayList GetFonts(String RTFFile)
        {
            RtfTree tree = new RtfTree();
            try
            {
                tree.LoadRtfFile(RTFFile);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                Application.Exit();
            }
            gon = true;
            travaseForFonts(tree.RootNode);
            return fonts;
        }
        private ArrayList fonts;
        private ArrayList fontids;
        private Boolean gon = false;
        private void travaseForFonts(RtfTreeNode tnd)
        {
            for (int i = 0; i < tnd.ChildNodes.Count; i++)
            {
                if (!gon) return;
                if (tnd.ChildNodes[i].NodeType == RtfNodeType.Keyword && tnd.ChildNodes[i].NodeKey == "fonttbl")
                {
                    fonts = new ArrayList();
                    fontids = new ArrayList();
                    FetchFontTable(tnd);
                    gon = false;
                    return;
                }
                if (tnd.ChildNodes[i].ChildNodes.Count > 0)
                {
                    if (gon)
                       travaseForFonts(tnd.ChildNodes[i]);
                }
            }
        }

        private void FetchFontTable(RtfTreeNode tnd)
        {
            for (int i = 0; i < tnd.ChildNodes.Count; i++)
            {
                if (tnd.ChildNodes[i].NodeType == RtfNodeType.Group)
                {
                    fonts.Add(FetchFont(tnd.ChildNodes[i]));
                    fontids.Add(FetchFontId(tnd.ChildNodes[i]));
                }
            }
        }

        private String FetchFont(RtfTreeNode tnd)
        {
            for (int i = 0; i < tnd.ChildNodes.Count; i++)
            {
                if (tnd.ChildNodes[i].NodeType == RtfNodeType.Text)
                    return ProcessFontName(tnd.ChildNodes[i].NodeKey);
            }
            return "null";
        }

        private String ProcessFontName(String s)
        {
            s = s.Substring(0, s.Length - 1);
            return s;
        }
  
        private int FetchFontId(RtfTreeNode tnd)
        {
            for (int i = 0; i < tnd.ChildNodes.Count; i++)
            {
                if (tnd.ChildNodes[i].NodeType == RtfNodeType.Keyword && tnd.ChildNodes[i].NodeKey.StartsWith("f"))
                    return tnd.ChildNodes[i].Parameter;
            }
            return -1;
        }


        public void DoTranscode(String RTFFile, ArrayList Enis, ArrayList Enos, ArrayList ToENids,String Output, bool html)
        {
            //RTFFile is where input
            //Enis is a list of Eni arrays
            //Enos is a list of Eno arrays
            //FromFontNames is a list of fonts to transcode
            //ToENids is a list of indexes that corresponds to FromFontName to transcode...
            //Output is output text file
            this.Enis = Enis;
            this.Enos = Enos;
            this.ToENids = ToENids;
            output_html = html;
            Write = new StreamWriter(Output,false,Encoding.UTF8);
            if (output_html)
            {
                Write.Write("<html><body>");
            }
            RtfTree tree = new RtfTree();
            try
            {
                tree.LoadRtfFile(RTFFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Exit();
            }
            //Write.Write(tree.ToStringEx());
            //MessageBox.Show(tree.ToString());
            traverse(tree.RootNode.ChildNodes[0]);
            if (output_html)
            {
                Write.Write("</body></html>");
            }
            Write.Close();
        }

        ArrayList Enis, Enos, ToENids;
        StreamWriter Write;
        bool output_html = false;
        int level = 0;
        int EnIndex = -1;
        int fontsize = 3;
        int ftableindex = -1;
        bool b = false;
        bool it = false;
        bool u = false;
        //string cache = "";
        StringBuilder sbcache = new StringBuilder();
        void traverse(RtfTreeNode tnd)
        {
            FlushLast();
            EnIndex = -1;
            ftableindex = -1;
            fontsize = -1;
            b = false;
            it = false;
            u = false;
            for (int i = 0; i < tnd.ChildNodes.Count; i++)
            {
                if (level >= 0)
                {
                    switch (tnd.ChildNodes[i].NodeKey)
                    {
                        case "fonttbl":
                        case "colortbl":
                        case "stylesheet":
                        case "generator":
                        case "info":
                        case "pnseclvl":
                        case "xmlnstbl":
                        case "wgrffmtfilter":
                        case "themedata":
                        case "colorschememapping":
                        case "latentstyles":
                        case "listtable":
                        case "datastore":
                            return;
                    }
                    if (tnd.ChildNodes[i].NodeType == RtfNodeType.Keyword && tnd.ChildNodes[i].NodeKey == "f")
                    {
                        int x = tnd.ChildNodes[i].Parameter;
                        ftableindex = x;
                        x = GetIdFont(x);
                        if (EnIndex != x)
                        {
                            FlushLast();
                            EnIndex = x;
                        }
                    }
                    else if (tnd.ChildNodes[i].NodeType == RtfNodeType.Keyword && tnd.ChildNodes[i].NodeKey == "fs")
                    {
                        int x = tnd.ChildNodes[i].Parameter;
                        x = x / 2;
                        if (fontsize != x)
                        {
                            FlushLast();
                            fontsize = x;
                        }
                    }
                    else if (tnd.ChildNodes[i].NodeType == RtfNodeType.Keyword && tnd.ChildNodes[i].NodeKey == "b")
                    {
                        bool x = (tnd.ChildNodes[i].HasParameter && tnd.ChildNodes[i].Parameter == 0) ? false : true;
                        if (b != x)
                        {
                            FlushLast();
                            b = x;
                        }
                    }
                    else if (tnd.ChildNodes[i].NodeType == RtfNodeType.Keyword && tnd.ChildNodes[i].NodeKey == "i")
                    {

                        bool x = (tnd.ChildNodes[i].HasParameter && tnd.ChildNodes[i].Parameter == 0) ? false : true;
                        if (it != x)
                        {
                            FlushLast();
                            it = x;
                        }
                    }
                    else if (tnd.ChildNodes[i].NodeType == RtfNodeType.Keyword && tnd.ChildNodes[i].NodeKey == "ul")
                    {
                        bool x = (tnd.ChildNodes[i].HasParameter && tnd.ChildNodes[i].Parameter == 0) ? false : true;
                        if (u != x)
                        {
                            FlushLast();
                            u = x;
                        }
                    }
                    else if (tnd.ChildNodes[i].NodeType == RtfNodeType.Keyword && tnd.ChildNodes[i].NodeKey == "ulnone")
                    {
                        bool x = false;
                        if (u != x)
                        {
                            FlushLast();
                            u = x;
                        }
                    }
                    else if (tnd.ChildNodes[i].NodeType == RtfNodeType.Text)
                    {
                        sbcache.Append(tnd.ChildNodes[i].NodeKey);
                        //cache += tnd.ChildNodes[i].NodeKey;
                        //MessageBox.Show(tnd.ChildNodes[i].NodeKey + level.ToString());
                    }
                    else if (tnd.ChildNodes[i].NodeType == RtfNodeType.Keyword && tnd.ChildNodes[i].NodeKey == "plain")
                    {
                        FlushLast();
                        fontsize = 3;
                        b = false;
                        it = false;
                        u = false;
                        EnIndex = -1;
                    }
                    else if (tnd.ChildNodes[i].NodeType == RtfNodeType.Control)
                    {
                        if (tnd.ChildNodes[i].NodeKey == "'")
                        {
                            if (sbcache.Length > 0 && sbcache[sbcache.Length - 1] == (char)tnd.ChildNodes[i].Parameter)
                            {
                            }
                            else
                            {
                                sbcache.Append((char)tnd.ChildNodes[i].Parameter);
                            }
                            //cache += (char)tnd.ChildNodes[i].Parameter;
                        }
                        else if (tnd.ChildNodes[i].NodeKey != "*")
                        {
                            sbcache.Append(tnd.ChildNodes[i].NodeKey);
                            //cache += tnd.ChildNodes[i].NodeKey;
                        }
                    }
                    else if (tnd.ChildNodes[i].NodeType == RtfNodeType.Keyword && tnd.ChildNodes[i].NodeKey == "u")
                    {
                        sbcache.Append((char)tnd.ChildNodes[i].Parameter);
                        //cache += (char)tnd.ChildNodes[i].Parameter;
                    }
                    else if (tnd.ChildNodes[i].NodeType == RtfNodeType.Keyword)
                    {

                        switch (tnd.ChildNodes[i].NodeKey)
                        {
                            case "par":
                                FlushLast();
                                if (!output_html)
                                    Write.Write("\r\n");
                                else
                                {
                                    Write.Write("\r\n");
                                    Write.Write("<br />");
                                }
                                break;
                            case "tab":
                                FlushLast();
                                Write.Write("\t");
                                break;
                            case "ldblquote":
                                sbcache.Append((char)8220);
                                break;
                            case "lquote":
                                sbcache.Append((char)8216);
                                break;
                            case "rquote":
                                sbcache.Append((char)8217);
                                break;
                            case "rdblquote":
                                sbcache.Append((char)8221);
                                break;
                            case "cell":
                                sbcache.Append("\t");
                                break;
                            case "row":
                                sbcache.Append("\r\n");
                                break;
                        }
                    }
                }
                if (tnd.ChildNodes[i].ChildNodes.Count > 0)
                {
                    level++;
                    traverse(tnd.ChildNodes[i]);
                    level--;
                }
            }
            FlushLast();
        }

        private void FlushLast()
        {
            string css = sbcache.ToString();
            if(EnIndex>=0){
                css = Transcode.transcode((ArrayList)Enis[EnIndex], (ArrayList)Enos[EnIndex], css);
                //cache = Transcode.transcode((ArrayList)Enis[EnIndex], (ArrayList)Enos[EnIndex], cache);
            }

            if (output_html)
            {
                if (EnIndex >= 0)
                {
                    int sizz = (fontsize * 3) / 12;
                    string formatting = "<font face=\"MyMyanmar\" size=\"" + sizz.ToString() + "\">";
                    formatting += b ? "<b>" : "";
                    formatting += u ? "<u>" : "";
                    formatting += it ? "<i>" : "";
                    Write.Write(formatting);
                    Write.Write(css);
                    string ending = "</font>";
                    ending += b ? "</b>" : "";
                    ending += u ? "</u>" : "";
                    ending += it ? "</i>" : "";
                    Write.Write(ending);
                }
                else
                {
                    int sizz = (fontsize * 3) / 12;
                    string formatting = "<font face=\"" + GetFontName(ftableindex) +"\" size=\"" + sizz.ToString() + "\">";
                    formatting += b ? "<b>" : "";
                    formatting += u ? "<u>" : "";
                    formatting += it ? "<i>" : "";
                    Write.Write(formatting);
                    Write.Write(css);
                    string ending = "</font>";
                    ending += b ? "</b>" : "";
                    ending += u ? "</u>" : "";
                    ending += it ? "</i>" : "";
                    Write.Write(ending);
                }
            }
            else
            {
                Write.Write(css);
            }
            sbcache = new StringBuilder();
        }

        int GetIdFont(int id)
        {
            for (int i = 0; i < fontids.Count; i++)
            {
                if ((int)fontids[i] == id)
                    return (int)ToENids[i];
            }
            return -1;
        }

        string GetFontName(int id)
        {
            for (int i = 0; i < fontids.Count; i++)
            {
                if ((int)fontids[i] == id)
                {
                    return fonts[i].ToString();
                }
            }
            return "Arial";
        }
    }
}
