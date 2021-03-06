using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Transcode
{
    class UnicodePass
    {

        public static string pass2(string s)
        {
            StringBuilder vowels = new StringBuilder();
            StringBuilder svowels = new StringBuilder();
            StringBuilder mesials = new StringBuilder();
            string staker = "";
            string consonant = "";
            string killer = "";
            string tones = "";
            string kinzi = "";
            StringBuilder final = new StringBuilder();
            bool stacked = false;
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                string _case = classReturner(c);
                if (i == s.Length) _case = "break";
                switch (_case)
                {
                    case "c-con":
                        if ((i + 1) <= s.Length - 1)
                        {
                            if (s[i + 1] == '\u1039' || s[i + 1] == '\u103a')
                            {
                                killer += NormalizeCon(c);
                                continue;
                            }
                        }
                        consonant += NormalizeCon(c);
                        break;
                    case "c-vowel":
                        if (stacked)
                        {
                            svowels.Append(NormalizeVowels(c));
                        }
                        else
                        {
                            vowels.Append(NormalizeVowels(c));
                        }
                        break;
                    case "c-mesial":
                        mesials.Append(NormalizeMesials(c));
                        break;
                    case "c-stack":
                        staker += NormalizeStackers(c);
                        stacked = true;
                        break;
                    case "c-killer":
                        if ((i - 1) > -1)
                        {
                            if (classReturner(s[i - 1]) == "c-vowel")
                            {
                                vowels.Append('\u103a');
                            }
                        }
                        break;
                    case "c-kinzi":
                        if (c == 'ၦ')
                        {
                            vowels.Append('ီ');
                        }
                        else if (c == 'ၧ')
                        {
                            vowels.Append('ံ');
                        }
                        else if (c == 'ၥ')
                        {
                            vowels.Append('ိ');
                        }
                        kinzi = "ၤ";
                        break;
                    case "c-tone":
                        tones += NormalizeTones(c);
                        break;
                    case "c-combined":
                        if (c == '႒')
                        {
                            mesials.Append('\u103e');
                            vowels.Append('ု');
                        } 
                        break;
                    case "c-ignore":
                        break;
                    case "break":
                    case "c-undefined":
                        bool donesome = false;
                        if (consonant.Length > 1)
                        {
                            if (kinzi.Length > 0)
                            {
                                final.Append(consonant[0]);
                                final.Append('င');
                                final.Append('\u103a');
                                final.Append('\u1039');
                                final.Append(consonant[1]);
                                kinzi = "";
                            }
                            else if (staker.Length > 0)
                            {
                                final.Append(consonant[0]);
                                final.Append(RegulateVowels(vowels.ToString())); //just added
                                final.Append(consonant[1]);
                                final.Append('\u1039');
                                final.Append(staker[0]);
                                staker = "";
                            }
                            donesome = true;
                        }
                        else if(consonant.Length >0)
                        {
                            final.Append(consonant);
                            donesome = true;
                        }
                        if (mesials.Length > 0)
                        {
                            final.Append(RegulateMesials(mesials.ToString()));
                        }
                        if (vowels.Length > 0)
                        {
                            if (stacked)
                            {
                                final.Append(RegulateVowels(svowels.ToString()));
                            }
                            else
                            {
                                final.Append(RegulateVowels(vowels.ToString()));
                            }
                        }
                        if (killer.Length > 0)
                        {
                            if (killer.Length > 1)
                            {
                                foreach (char cx in killer)
                                {
                                    final.Append(cx);
                                    final.Append('\u103a');
                                }
                            }
                            else
                            {
                                final.Append(killer);
                                final.Append('\u103a');
                            }
                        }
                        if (tones.Length > 0)
                        {
                            final.Append(tones);
                        }
                        consonant = "";
                        mesials = new StringBuilder();
                        vowels = new StringBuilder();
                        svowels = new StringBuilder();
                        stacked = false;
                        staker = "";
                        killer = "";
                        kinzi = "";
                        tones = "";
                        if (donesome)
                        {
                            final.Append('\u200b');
                            donesome = false;
                        }
                        if (_case == "c-undefined")
                        {
                            final.Append(c);
                        }
                        break;
                }
            }

            return final.ToString();
        }

        private static char NormalizeTones(char c)
        {
            if (c == 'း')
                return c;
            else
                return '့';
        }

        private static string NormalizeCon(char c)
        {
            if (c == 'ၙ')
                return "န";
            else
                return c.ToString();
        }

        private static string NormalizeStackers(char c)
        {
            char[] sk = "ၠၡၢၣၨၩၪၫၴၵၷၸၹၺၻၼၽၾၿႌၮၯၰၱၲၳ".ToCharArray();
            char[] sp = "ကခဂဃစဆဇဈဏတထဒဓနပဖဗဘမလဋဋဌဍဍဎ".ToCharArray();
            for (int i = 0; i < sk.Length; i++)
            {
                if (sk[i] == c)
                {
                    return sp[i].ToString();
                }
            }
            return c.ToString();
        }

        private static string RegulateVowels(string p)
        {
            char[] order = { 'ေ', 'ိ', 'ီ', 'ဲ', 'ု', 'ူ', 'ံ', 'ာ', 'ါ', '်', 'ဿ' };
            return Regulate(order,p);
        }

        private static string RegulateMesials(string p)
        {
            char[] order = { '\u103b', '\u103c', '\u103d', '\u103e' };
            return Regulate(order, p);
        }

        private static string Regulate(char[] order, string p)
        {
            string s = "";
            foreach (char c in order)
            {
                if (p.IndexOf(c) > -1)
                    s += c;
            }
            return s;
        }

        private static string NormalizeVowels(char c)
        {
            string s = c.ToString();
            if (c == '႒')
            {
                s = "ု";
            }
            else if (c == 'ဳ')
            {
                s = "ု";
            }
            else if (c == 'ဴ')
            {
                s = "ူ";
            }
            else if (c == 'ဵ')
            {
                s = "ိံ";
            }
            else if (c == '႔' || c == '႕')
            {
                s = "့";
            }
            else if (c == 'ၗ')
            {
                return "\u103a";
            }
            else if (c == '\u108F')
            {
                return "ဿ";
            }
            return s;
        }

        private static string NormalizeMesials(char c)
        {
            string s;
            if (c == '\u1080')
            {
                s = "\u103b";
            }
            else if (c == '\u1081')
            {
                s = "\u103b\u103d";
            }
            else if (c == '\u1082')
            {
                s = "\u103b\u103e";
            }
            else if (c == '\u1083')
            {
                s = "\u103b\u103d\u103e";
            }
            else if (c == '\u1084' || c == '\u1085')
            {
                s = "\u103c";
            }
            else if (c == '\u108d')
            {
                s = "\u103d";
            }
            else if (c == '\u108e')
            {
                s = "\u103d\u103e";
            }
            else if (c == '\u1090' || c == '\u1091')
            {
                s = "\u103e";
            }
            else
                s = c.ToString();
            return s;
        }

        private static string classReturner(char ch)
        {
            if (ch == '\u200b')
                return "break";
            char[] c_consonants = "ကခဂဃငစဆဇဈဉညဋဌဍဎဏတထဒဓနၙပဖဗဘမယရလဝသဟဠအဣဤဥဦဧဩဪ၌၍၎၏".ToCharArray();
            char[] c_attach = "ႀႁႂႃႍႎ႐႒ါာိီုူဲဳဴံ့းႏ၊။႑ဵ႔႕".ToCharArray();
            char[] c_vowel = "ေါာိီုူဲဳဴံဵ\u108f".ToCharArray();
            char[] c_mesial = "ႀႁႂႃႄႅႍႎ႐႑".ToCharArray();
            char[] c_fattach = "ႅႄႆႇႈႉႊႋေ".ToCharArray();
            char[] c_killer = "်္ၗ".ToCharArray();
            char[] c_stack = "ၠၡၢၣၨၩၪၫၴၵၷၸၹၺၻၼၽၾၿႌၮၯၰၱၲၳ".ToCharArray();
            char[] c_kinzi = "ၤၥၦၧ".ToCharArray();
            char[] c_tone = "့႔႕း".ToCharArray();
            char[] c_ignore = "\u200d\u200c".ToCharArray();
            if (exists(c_consonants, ch))
            {
                return "c-con";
            }
            else if (exists(c_mesial, ch))
            {
                return "c-mesial";
            }
            else if (exists(c_vowel, ch))
            {
                return "c-vowel";
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
            else if (exists(c_tone, ch))
            {
                return "c-tone";
            }
            else if (exists(c_ignore, ch))
            {
                return "c-ignore";
            }
            else if (ch == '႒')
            {
                return "c-combined";
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
