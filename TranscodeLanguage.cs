using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using System.Windows.Forms;

namespace TranscodeLanguage
{
    public class Parser
    {
        public Parser(string file,string key)
        {
            if (key != "98761197agde5d2g13asdh8wjktwa6f5")
            {
                RES = "ERROR NO PERMISSION";
                return;
            }
            StreamReader sr = new StreamReader(file);
            line = 0;
            string state = "header";
            headers = new ArrayList();
            ENI = new ArrayList();
            Arrays = new ArrayList();
            ENO = new ArrayList();
            autos = new ArrayList();
            while (!sr.EndOfStream)
            {
                string currentline = sr.ReadLine();
                line++;
                currentline = currentline.Trim();
                if (!currentline.StartsWith("##") && !currentline.StartsWith(" "))
                {
                    switch (state)
                    {
                        case "header":
                            if (currentline.StartsWith("#"))
                            {
                                if (!ProcessHeader(currentline))
                                {
                                    RES = "Invalid Header";
                                    return;
                                }
                            }
                            else if (currentline.StartsWith("begin"))
                            {
                                string tmp = ProcessBegin(currentline);
                                if (tmp == "error")
                                {
                                    RES = "ERROR Invalid Statement";
                                    return;
                                }
                                else
                                {
                                    state = tmp;
                                }
                            }
                            else if (currentline.Length == 0)
                            {
                            }
                            else
                            {
                                RES = "ERROR Invalid Statement";
                                return;
                            }
                            break;
                        case "nocontext":
                            if (currentline.StartsWith("end"))
                            {
                                if (ProcessEnd(currentline) == "nocontext")
                                {
                                    state = "header";
                                }
                                else
                                {
                                    RES = "ERROR Block End";
                                    return;
                                }
                            }
                            else
                            {
                                if (currentline.Length != 0)
                                {
                                    string xx = ProcessNoContext(currentline);
                                    if (xx != "OK")
                                    {
                                        RES = xx;
                                        return;
                                    }
                                }
                            }
                            break;
                        case "context":
                            if (currentline.StartsWith("declare"))
                            {
                                if (currentline.EndsWith("{"))
                                {
                                    state = "context-inside-declare";
                                }
                                else
                                {
                                    state = "context-inside";
                                }
                            }
                            else if (currentline.Length == 0)
                            {
                            }
                            else
                            {
                                RES = "Error Context Array Block";
                                return;
                            }
                            break;
                        case "context-inside":
                            if (currentline == "{")
                            {
                                state = "context-inside-declare";
                            }
                            else
                            {
                                RES = "Error Context Array Block";
                            }
                            break;
                        case "context-inside-declare":
                            if (currentline.StartsWith("array"))
                            {
                                string aerr = ProcessArray(currentline);
                                if (aerr != "OK")
                                {
                                    RES = aerr;
                                    return;
                                }
                                if(currentline.EndsWith("}"))
                                    state = "context-statement";
                            }
                            else if (currentline.StartsWith("}"))
                            {
                                state = "context-statement";
                            }
                            break;
                        case "context-statement":
                            if (currentline.Length != 0)
                            {
                                if (currentline.StartsWith("end"))
                                {
                                    if(currentline.EndsWith("context"))
                                        state = "header";
                                    else{
                                        RES = "ERROR Block End";
                                        return;
                                    }
                                }
                                else
                                {
                                    string xy = ProcessContext(currentline);
                                    if(xy != "OK"){
                                        RES = xy;
                                        return;
                                    }
                                }
                            }
                            break;
                        case "auto":
                            if (currentline.StartsWith("end"))
                            {
                                if (currentline.EndsWith("auto"))
                                {
                                    state = "header";
                                }
                                else
                                {
                                    RES = "ERROR Block End";
                                    return;
                                }
                            }
                            else
                            {
                                autos.Add(currentline);
                            }
                            break;
                    }
                }
            }
            RES = "OK";
            sr.Close();
        }

        public ArrayList getENI(){
            return ENI;
        }

        public ArrayList getAutos()
        {
            return autos;
        }

        public ArrayList getENO()
        {
            return ENO;
        }

        public ArrayList getHeaders()
        {
            return headers;
        }

        private string ProcessContext(string currentline)
        {
            int state = 0;
            string tmp = "";
            ArrayList leftlex = new ArrayList();
            ArrayList rightlex = new ArrayList();
            bool isleft = true;
            int point = 0;
            foreach (char c in currentline.ToCharArray())
            {
                switch (state)
                {
                    case 0:
                        if (c == '\'')
                        {
                            state = 1;
                        }
                        else if (c == 'U')
                        {
                            state = 4;
                            tmp = "";
                        }
                        else if (c == ' ' || c == '\t')
                        {
                        }
                        else if (c == '>')
                        {
                            isleft = false;
                            tmp = "";
                        }
                        else
                        {
                            tmp += c;
                            state = 2;
                        }
                        break;
                    case 1:
                        if (c == '\\')
                        {
                            state = 3;
                        }
                        else if (c == '\'')
                        {
                            LexObject lex = new LexObject();
                            lex.isText = true;
                            lex.text = tmp;
                            if (isleft)
                            {
                                leftlex.Add(lex);
                            }
                            else
                            {
                                rightlex.Add(lex);
                            }
                            tmp = "";
                            state = 0;
                        }
                        else
                        {
                            tmp += c;
                        }
                        break;
                    case 2:
                        if (c == ' ' || c == '\t')
                        {
                            state = 0;
                            if (isleft)
                            {
                                int pt = FindArray(tmp);
                                if (pt < 0)
                                {
                                    return "Error No Such Array";
                                }
                                else
                                {
                                    LexObject lex = new LexObject();
                                    lex.isText = false;
                                    lex.pointer = pt;
                                    point++;
                                    lex.index = point;
                                    leftlex.Add(lex);
                                }
                            }
                            else
                            {
                                if (tmp[tmp.Length - 2] == '@')
                                {
                                    string s = tmp.Substring(0, tmp.Length - 2);
                                    int pt = FindArray(s);
                                    if (pt >= 0)
                                    {
                                        string ind = tmp[tmp.Length - 1].ToString();
                                        bool found = false;
                                        for (int i = 0; i < leftlex.Count; i++)
                                        {
                                            LexObject tmplex = (LexObject)leftlex[i];
                                            if (tmplex.index.ToString() == ind)
                                            {
                                                if (((TLArray)Arrays[pt]).content.Count == ((TLArray)Arrays[tmplex.pointer]).content.Count)
                                                {
                                                    LexObject lex = new LexObject();
                                                    lex.isText = false;
                                                    lex.pointer = pt;
                                                    lex.index = tmplex.index;
                                                    lex.text = s;
                                                    rightlex.Add(lex);
                                                    found = true;
                                                    tmp = "";
                                                    state = 0;
                                                }
                                                else
                                                {
                                                    return "Error Array Length Mismatch";
                                                }
                                            }
                                        }
                                        if (!found)
                                            return "Error Reference Pointer in Array - " + s;
                                    }else{
                                        return "Error No Such Array - " + s;
                                    }
                                }
                                else
                                {
                                    return "Error no pointer @ in Array - " + tmp;
                                }
                            }
                            tmp = "";
                        }
                        else
                        {
                            tmp += c;
                        }
                        break;
                    case 3:
                        if (c == '\\' || c == '\'')
                            tmp += c;
                        else
                            return "Error Invalid Escape Character";
                        break;
                    case 4:
                        if (c == '+')
                            state = 5;
                        else
                            return "Error Invalid Character";
                        break;
                    case 5:
                        if (checkHex(c))
                        {
                            tmp += c;
                        }
                        else if (c == ' ' || c == '\t')
                        {
                            if (tmp.Length == 4)
                            {
                                int chx = Convert.ToInt32(tmp, 16);
                                LexObject lex = new LexObject();
                                lex.isText = true;
                                lex.text = ((char)chx).ToString();
                                if (isleft)
                                {
                                    leftlex.Add(lex);
                                }
                                else rightlex.Add(lex);
                                state = 0;
                                tmp = "";
                            }
                            else
                            {
                                return "Error Invalid Unicode Character";
                            }
                        }
                        else
                        {
                            return "Error Invalid Unicode Character";
                        }
                        break;
                }
            }

            if (tmp.Length != 0)
            {
                if (state == 2)
                {
                    if (tmp[tmp.Length - 2] == '@')
                    {
                        string s = tmp.Substring(0, tmp.Length - 2);
                        int pt = FindArray(s);
                        if (pt >= 0)
                        {
                            string ind = tmp[tmp.Length - 1].ToString();
                            bool found = false;
                            for (int i = 0; i < leftlex.Count; i++)
                            {
                                LexObject tmplex = (LexObject)leftlex[i];
                                if (tmplex.index.ToString() == ind)
                                {
                                    if (((TLArray)Arrays[pt]).content.Count == ((TLArray)Arrays[tmplex.pointer]).content.Count)
                                    {
                                        LexObject lex = new LexObject();
                                        lex.isText = false;
                                        lex.pointer = pt;
                                        lex.index = tmplex.index;
                                        lex.text = s;
                                        rightlex.Add(lex);
                                        found = true;
                                        tmp = "";
                                        state = 0;
                                    }
                                    else
                                    {
                                        return "Error Array Length Mismatch";
                                    }
                                }
                            }
                            if (!found)
                                return "Error Reference Pointer in Array - " + s;
                        }
                        else
                        {
                            return "Error No Such Array - " + s;
                        }
                    }
                    else
                    {
                        return "Error no pointer @ in Array - " + tmp;
                    }
                }
                else if (state == 5)
                {
                    if (tmp.Length == 4)
                    {
                        int chx = Convert.ToInt32(tmp, 16);
                        LexObject lex = new LexObject();
                        lex.isText = true;
                        lex.text = ((char)chx).ToString();
                        if (isleft)
                        {
                            leftlex.Add(lex);
                        }
                        else rightlex.Add(lex);
                        state = 0;
                        tmp = "";
                    }
                    else
                    {
                        return "Error Invalid Unicode Character";
                    }
                }
            }
            ProcessLex(leftlex, rightlex);
            return "OK";
        }

        private void ProcessLex(ArrayList leftlex, ArrayList rightlex)
        {
            ArrayList LeftItems = new ArrayList();
            int x = getNumbers(leftlex);
            for (int i = 0; i < leftlex.Count; i++)
            {
                LeftItems.Add(GenerateLex((LexObject)leftlex[i], x, i, leftlex));
            }
            ArrayList RightItems = new ArrayList();
            for (int i = 0; i < rightlex.Count; i++)
            {
                RightItems.Add(DereferenceLeft( (LexObject)rightlex[i], x,LeftItems,leftlex,(LexObject)rightlex[i]));
            }

            for (int i = 0; i < x; i++)
            {
                string left = "";
                string right = "";
                for (int j = 0; j < LeftItems.Count; j++)
                {
                    left += ((ArrayList)LeftItems[j])[i].ToString();
                }
                for (int j = 0; j < RightItems.Count; j++)
                {
                    right += ((ArrayList)RightItems[j])[i].ToString();
                }
                ENI.Add(left);
                ENO.Add(right);
            }
        }

        private object DereferenceLeft(LexObject lexObject, int x, ArrayList LeftItems, ArrayList leftlex,LexObject rlex)
        {
            ArrayList al = new ArrayList();
            if (lexObject.isText)
            {
                for (int i = 0; i < x; i++)
                {
                    al.Add(lexObject.text);
                }
                return al;
            }
            else
            {
                int lindex = getRealIndex(lexObject.index, leftlex);
                ArrayList cutitem = (ArrayList)LeftItems[lindex];
                for (int i = 0; i < cutitem.Count; i++)
                {
                    al.Add(TranslateArray(cutitem[i].ToString(), (LexObject)leftlex[lindex],rlex));
                }
            }
            return al;
        }

        private string TranslateArray(string s,LexObject lex,LexObject rlex)
        {
            TLArray tl = (TLArray)Arrays[lex.pointer];
            TLArray tx = (TLArray)Arrays[rlex.pointer];
            for (int i = 0; i < tl.content.Count; i++)
            {
                if (s == tl.content[i].ToString())
                {
                    return tx.content[i].ToString();
                }
            }
            return null;
        }

        private int getRealIndex(int index, ArrayList leftlex)
        {
            int x = 0;
            for (int i = 0; i < leftlex.Count; i++)
            {
                LexObject lx = (LexObject)leftlex[i];
                if (!lx.isText)
                {
                    x++;
                    if (x == index)
                        return i;
                }
            }
            return -1;
        }

        private ArrayList GenerateLex(LexObject L, int x, int ind, ArrayList leftlex)
        {
            ArrayList al = new ArrayList();
            if (L.isText)
            {
                for (int i = 0; i < x; i++)
                {
                    al.Add(L.text);
                }
                return al;
            }
            else
            {
                TLArray tl = (TLArray)Arrays[L.pointer];
                int fx = getFactor(leftlex, ind);
                int factor = x / fx;
                int h = 0;
                int y = 0;
                while ( al.Count<x)
                {
                    al.Add(tl.content[y]);
                    h++;
                    if (h == factor)
                    {
                        h = 0;
                        y++;
                        if (y == tl.content.Count)
                            y = 0;
                    }
                }
                return al;
            }
            return null;
        }

        private int getFactor(ArrayList leftlex, int ind)
        {
            int factor = 1;
            for (int i = 0; i <= ind; i++)
            {
                LexObject lex = (LexObject)leftlex[i];
                if (!lex.isText)
                {
                    TLArray tl = (TLArray)Arrays[lex.pointer];
                    factor *= tl.content.Count;
                }
            }
            return factor;
        }

        private int getNumbers(ArrayList leftlex)
        {
            int x = 1;
            for (int i = 0; i < leftlex.Count; i++)
            {
                LexObject lex = (LexObject)leftlex[i];
                if(!lex.isText){
                    TLArray ta = (TLArray)Arrays[lex.pointer];
                    x *= ta.content.Count;
                }
            }
            return x;
        }

        private int FindArray(string tmp)
        {
            for (int i = 0; i < Arrays.Count; i++)
            {
                TLArray t = (TLArray)Arrays[i];
                if (t.name == tmp)
                    return i;
            }
            return -1;
        }

        private string ProcessArray(string currentline)
        {
            currentline = currentline.Substring(6);
            currentline = currentline.Trim();
            ArrayList al = new ArrayList();
            int state = 0;
            string tmp = "";
            bool fin = false;
            foreach (char c in currentline.ToCharArray())
            {
                switch (state)
                {
                    case 0:
                        if (c == '[')
                        {
                            state = 1;
                        }
                        else
                        {
                            return "Array Decleration Error";
                        }
                        break;
                    case 1:
                        if (c == '\\')
                        {
                            state = 2;
                        }
                        else if (c == '.')
                        {
                            state = 3;
                        }
                        else if (c == ']')
                        {
                            if (tmp.Length > 1)
                            {
                                return "Error Array Item Contain More Than One Character";
                            }
                            else if (tmp.Length == 0)
                            {
                            }
                            else
                            {
                                al.Add(tmp);
                            }
                            state = 5;
                        }
                        else if (c == ',')
                        {
                            if (tmp.Length > 1)
                            {
                                return "Error Array Item Contain More Than One Character";
                            }
                            else if (tmp.Length == 0)
                            {
                            }
                            else
                            {
                                al.Add(tmp);
                            }
                            tmp = "";
                        }
                        else if (c == ' ' || c == '\t')
                        {
                        }
                        else
                        {
                            tmp += c;
                        }
                        break;
                    case 2:
                        if (c == '\\' || c == '.' || c==']')
                        {
                            tmp += c;
                        }
                        else if (c == ',')
                        {
                            tmp += c;
                        }
                        else if (c == ' ' || c == '\t')
                        {
                            tmp += c;
                        }
                        else
                            return "Invalid Escape Character - " + c;
                        state = 1;
                        break;
                    case 3:
                        if (c == '.')
                        {
                            state = 4;
                        }
                        else if (c == ',')
                        {
                            if (tmp.Length > 1)
                            {
                                return "Error Array Item Contain More Than One Character";
                            }
                            else if (tmp.Length == 0)
                            {
                            }
                            else
                            {
                                al.Add(tmp);
                            }
                            tmp = "";
                        }
                        else
                        {
                            tmp += c;
                        }
                        break;
                    case 4:
                        char start = tmp[tmp.Length - 1];
                        char end = c;
                        tmp = "";
                        for (int j = (int)start; j < (int)end; j++)
                        {
                            al.Add(((char)j).ToString());
                        }
                        state = 1;
                        break;
                    case 5:
                        if (c == ' ' || c == '\t')
                        {
                            //do nothing
                        }
                        else if (c == '>')
                        {
                            state = 6;
                            tmp = "";
                        }
                        else
                        {
                            return "Error Array Name";
                        }
                        break;
                    case 6:
                        if (c == ' ' || c == '\t')
                        {
                            if (tmp.Length > 0)
                            {
                                return "Error Array Name";
                            }
                        }
                        else if (c == '}')
                        {
                            fin = true;
                        }
                        else if (fin == false)
                        {
                            tmp += c;
                        }
                        else
                        {
                            return "Error Array Name";
                        }
                        break;
                }
            }
            TLArray tla = new TLArray();
            tla.name = tmp;
            tla.content = al;
            Arrays.Add(tla);
            return "OK";
        }

        private string ProcessEnd(string currentline)
        {
            currentline = currentline.Substring(4);
            currentline = currentline.Trim();
            switch (currentline)
            {
                case "context":
                    return "context";
                    break;
                case "nocontext":
                    return "nocontext";
                    break;
                case "auto":
                    return "auto";
                    break;
                default:
                    return "error";
                    break;
            }
        }

        private string ProcessNoContext(string currentline)
        {
            string left = "";
            string right = "";
            string temp = "";
            int state = 0;
            bool isleft = true;
            foreach (char c in currentline.ToCharArray())
            {
                switch (state)
                {
                    case 0:
                        if (c == '\'')
                        {
                            state = 1;
                        }
                        else if (c == 'U')
                        {
                            state = 3;
                        }
                        else if (c == ' ' || c == '\t')
                        {
                        }
                        else if (c == '>')
                        {
                            isleft = false;
                        }
                        else if (c == '#')
                        {
                            state = 5;
                        }
                        else
                        {
                            return "Invalid Character - " + c;
                        }
                        break;
                    case 1:
                        if (c == '\\')
                        {
                            state = 2;
                        }
                        else if (c == '\'')
                        {
                            state = 0;
                        }
                        else
                        {
                            if (isleft)
                                left += c;
                            else
                                right += c;
                        }
                        break;
                    case 2:
                        if (c == '\\')
                        {
                            if (isleft)
                                left += c;
                            else
                                right += c;
                        }
                        else if (c == '\'')
                        {
                            if (isleft)
                                left += c;
                            else
                                right += c;
                        }
                        else
                        {
                            return "Invalid Escape Character - " + c;
                        }
                        state = 1;
                        break;
                    case 3:
                        if (c == '+')
                        {
                            state = 4;
                        }
                        else
                        {
                            return "Invalid Unicode Point";
                        }
                        break;
                    case 4:
                        if (checkHex(c))
                        {
                            temp += c;
                        }
                        else if (c == ' ' || c == '\t')
                        {
                            if (temp.Length == 4)
                            {
                                int i = Convert.ToInt32(temp, 16);
                                if (isleft)
                                    left += (char)i;
                                else
                                    right += (char)i;
                                temp = "";
                                state = 0;
                            }
                            else
                            {
                                return "Invalid Unicode Point";
                            }
                        }
                        else
                        {
                            return "Invalid Unicode Point";
                        }
                        break;
                    case 5:
                        if (c == '#')
                        {
                            state = 6;
                        }
                        else
                        {
                            return "Invalid Character - #";
                        }
                        break;
                    case 6:
                        break;
                }
            }
            if (state != 0 && state != 6)
            {
                if (state == 4 && isleft == false)
                {
                    int i = Convert.ToInt32(temp, 16);
                    right += (char)i;
                    temp = "";
                    state = 0;
                }
                else
                {
                    return "Error in End of Line";
                }
            }

            ENI.Add(left);
            ENO.Add(right);
            return "OK";
        }

        private bool checkHex(char c)
        {
            switch (c)
            {
                case 'A':
                case 'B':
                case 'C':
                case 'D':
                case 'E':
                case 'F':
                    return true;
                    break;
                default:
                    int i = (int)c;
                    if (i >= 48 && i <= 57)
                        return true;
                    else
                        return false;
                    break;
            }
        }

        private string ProcessBegin(string currentline)
        {
            currentline = currentline.Substring(6);
            currentline = currentline.Trim();
            switch(currentline){
                case "context":
                    return "context";
                    break;
                case "nocontext":
                    return "nocontext";
                    break;
                case "auto":
                    return "auto";
                    break;
                default:
                    return "error";
                    break;
            }
        }

        private ArrayList Arrays;
        private ArrayList headers;
        private ArrayList ENI;
        char[] wspaces = { ' ', '\t' };
        private ArrayList ENO;
        private ArrayList autos;
        private bool ProcessHeader(string currentline)
        {
            int i = currentline.IndexOfAny(wspaces);
            if (i > 0)
            {
                string hname = currentline.Substring(1, i - 1);
                string hvalue = currentline.Substring(i+1);
                header h = new header();
                h.hname = hname;
                h.hvalue = hvalue;
                headers.Add(h);
                return true;
            }
            return false;
        }
        public string RES;
        public int line;
    }

    class LexObject
    {
        public bool isText;
        public int index = -1;
        public int pointer;
        public string text;
    }
    struct TLArray
    {
        public string name;
        public ArrayList content;
    }
    public struct header
    {
        public string hname;
        public string hvalue;
    }
}
