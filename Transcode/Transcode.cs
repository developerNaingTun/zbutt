using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;
using zbutt;

namespace Transcode
{
    class Transcode
    {
        public static string transcode(ArrayList ENI, ArrayList ENO, string s)
        {
            StringBuilder sb = new StringBuilder();
            bool x;
            for (int i = 0; i < s.Length; i++)
            {
                x = true;
                for (int j = 3; j>0 ; j--)
                {
                    if ((i + j -1) < s.Length)
                    {
                        String temp = s.Substring(i, j);
                        String res = getTo(temp, ENI, ENO);
                        if (res != null)
                        {
                            x = false;
                            sb.Append(res);
                            i += j;
                            i--;
                            break;
                        }
                    }
                }
                if(x == true)
                    sb.Append(s.Substring(i,1));
            }
            //return (sb.ToString());
            if (unicode == null)
            {
                Config c = new Config("MyMyanmar\\Transcode");
                unicode = c.Read("unicodepass", "true").ToLower(); 
            }
            if (unicode == "true")
            {
                return UnicodePass.pass2(parser(sb.ToString()));
            }
            if (File.Exists("nobreak"))
            {
                return sb.ToString();
            }
            return parser(sb.ToString());
        }

        public static string unicode = null;
        public static string XMLEntNormalizer(string s)
        {
            for (int i = 4096; i < 4256; i++)
            {
                string x = "&#" + i.ToString() + ";";
                string u = "";
                u += (char)i;
                s = s.Replace(x, u);
            }
            return s;
        }

        public static string NomalizedtoXML(string s)
        {
            for (int i = 4096; i < 4256; i++)
            {
                string x = "&#" + i.ToString() + ";";
                string u = "";
                u += (char)i;
                s = s.Replace(u, x);
            }
            return s;
        }

        private static string getTo(string from, ArrayList ENI, ArrayList ENO)
        {
            for (int i = 0; i < ENI.Count; i++)
            {
                if (ENI[i].ToString() == from)
                {
                    return (String)ENO[i];
                }
            }
            return null;
        }

        public static string parser(string s)
        {
            char bk = '\u200b';
            char bd = '\u200d';
            StringBuilder sb = new StringBuilder();
            foreach (char ch in s.ToCharArray())
            {
                switch (classReturner(ch))
                {
                    case "c-con":
                        sb.Append(ch);
                        sb.Append(bk);
                        break;
                    case "c-att":
                        try
                        {
                            if (sb[sb.Length - 1] == bk)
                            {
                                if (sb[sb.Length - 2] == '၎')
                                {
                                    continue;
                                }
                                if (sb[sb.Length - 2] == '၀')
                                    sb[sb.Length - 2] = 'ဝ';
                                if (sb[sb.Length - 2] == '၇')
                                    sb[sb.Length - 2] = 'ရ';
                                sb[sb.Length - 1] = ch;
                                sb.Append(bk);

                            }
                            else
                            {
                                sb.Append(ch);
                                sb.Append(bk);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                        break;
                    case "c-fatt":
                        sb.Append(ch);
                        break;
                    case "c-killer":
                        try
                        {
                            if (classReturner(sb[sb.Length - 2]) == "c-con") 
                            {
                                if (sb[sb.Length - 3] == '၀')
                                {
                                    sb[sb.Length - 3] = 'ဝ';
                                    sb[sb.Length - 1] = sb[sb.Length - 2];
                                    sb[sb.Length - 2] = bk;
                                    sb.Append(bk);
                                }
                                else if (sb[sb.Length - 3] == '၇')
                                {
                                    sb[sb.Length - 3] = 'ရ';
                                    sb[sb.Length - 1] = sb[sb.Length - 2];
                                    sb[sb.Length - 2] = bk;
                                    sb.Append(bk);
                                }
                                else if (sb[sb.Length - 3] == '၄')
                                {
                                    if (sb[sb.Length - 2] == 'င')
                                    {
                                        sb = sb.Remove(sb.Length - 3, 3);
                                        sb.Append("၎");
                                        sb.Append(bk);
                                        continue;
                                    }
                                }
                                sb[sb.Length - 3] = bd;
                                sb[sb.Length - 1] = ch;
                                sb.Append(bk);
                            }
                            else if (classReturner(sb[sb.Length - 4]) == "c-kinzi")
                            {
                                sb[sb.Length - 3] = sb[sb.Length - 2];
                                sb[sb.Length - 2] = ch;
                                sb[sb.Length - 1] = bk;
                            }
                            else
                            {
                                sb[sb.Length - 1] = ch;
                                sb.Append(bk);
                            }
                        }
                        catch (Exception ex)
                        {
                            sb.Append(ch);
                            sb.Append(bk);
                            // nothing;
                        }
                        break;
                    case "c-stack":
                    case "c-kinzi":
                        try
                        {
                            if (classReturner(sb[sb.Length - 3]) == "c-fatt")
                            {
                                if (classReturner(sb[sb.Length - 4]) == "c-fatt")
                                {
                                    if (sb[sb.Length - 5] == bk)
                                        sb[sb.Length - 5] = bd;
                                    sb[sb.Length - 1] = ch;
                                    sb.Append(bk);
                                }
                                else if (sb[sb.Length - 4] == bk)
                                {
                                    sb[sb.Length - 4] = bd;
                                    sb[sb.Length - 1] = ch;
                                    sb.Append(bk);
                                }
                            }
                            else
                            {
                                if (sb[sb.Length - 1] == bk)
                                {
                                    sb.Remove(sb.Length - 1, 1);
                                }
                                int len = sb.Length - 1;
                                while (true)
                                {
                                    if (len < 0)
                                        break;
                                    if (sb[len] == bk)
                                    {
                                        break;
                                    }
                                    len--;
                                }
                                String left = sb.ToString().Substring(0, len);
                                String right = sb.ToString().Substring(len + 1);
                                sb = new StringBuilder(left + right);
                                sb.Append(ch);
                                sb.Append(bk);
                                /*
                                sb[sb.Length - 3] = sb[sb.Length - 2];
                                sb[sb.Length - 2] = ch;
                                sb[sb.Length - 1] = bk;
                                 */
                            }

                        }
                        catch (Exception ex)
                        {
                            // nothing;
                            Console.WriteLine(ex.ToString());
                        }
                        break;
                    default:
                        sb.Append(ch);
                        break;
                }
            }
            return sb.ToString();
        }

        private static string classReturner(char ch)
        {
            char[] c_consonants = "ကခဂဃငစဆဇဈဉညဋဌဍဎဏတထဒဓနၙပဖဗဘမယရလဝသဟဠအဣဤဥဦဧဩဪ၌၍၎၏".ToCharArray();
            char[] c_attach = "ႀႁႂႃႍႎ႐႒ါာိီုူဲဳဴံ့းႏ၊။႑ဵ႔႕ၗ".ToCharArray();
            char[] c_fattach = "ႅႄႆႇႈႉႊႋေ".ToCharArray();
            char[] c_killer = "်္".ToCharArray();
            char[] c_stack = "ၠၡၢၣၨၩၪၫၴၵၷၸၹၺၻၼၽၾၿႌၮၯၰၱၲၳ".ToCharArray();
            char[] c_kinzi = "ၤၥၦၧ".ToCharArray();
            if (exists(c_consonants, ch))
            {
                return "c-con";
            }
            else if (exists(c_attach, ch))
            {
                return "c-att";
            }
            else if (exists(c_fattach, ch))
            {
                return "c-fatt";
            }
            else if (exists(c_killer, ch))
            {
                return "c-killer";
            }
            else if (exists(c_stack, ch))
            {
                return "c-stack";
            }
            else if (exists(c_kinzi, ch))
            {
                return "c-kinzi";
            }
            return "c-undefined";

        }

        private static bool exists(char[] ch_ar, char test)
        {
            foreach (char chx in ch_ar)
            {
                if (test == chx)
                    return true;
            }
            return false;
        }
    }
}
