using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using CristiPotlog.Controls;
using Transcode;
using System.Collections;
using ImportManager;
using System.Configuration;
using System.Threading;
using System.Runtime.Remoting.Messaging;
using zbutt;
using BalloonCS;
using System.IO;
using MyMyanmar;    

namespace zbutt
{
    public partial class formMain : Form
    {
        private Thread myThread;
        private string globalFileName;
        private iController icon = new iController();
        public const int HT_CAPTION = 0x2;
        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd,
            int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        public const int WM_NCLBUTTONDOWN = 0xA1;

        public formMain()
        {
            //icon.assign(this);
            InitializeComponent();
        }

        public void miceMove()
        {
            ReleaseCapture();
            SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        }

        /// <summary>
        /// Rtf
        /// </summary>
        private void rtf_SelectFile()
        {
            openFileDialog1.Filter = "Rich Text Format (*.rtf)|*.rtf";
            openFileDialog1.Multiselect = zbutt.Registration.registered();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                globalFileName = openFileDialog1.FileName;
                rtfloading.Active = true;
                btnSelectRtf.Enabled = false;
                if (openFileDialog1.FileNames.Length > 1)
                {
                    RTFFileNames = openFileDialog1.FileNames;
                    lstRtfFont.Items.Clear();
                    RTFFileLoadDel dg = new RTFFileLoadDel(RTFFilesLoad);
                    dg.BeginInvoke(openFileDialog1.FileNames, RTFLoadFin, null);
                    GenericEncodingFramework gef = new GenericEncodingFramework();
                    if (!gef.loaded)
                        gef.Load();
                    enclist.Items.Clear();
                    enclist.Items.Add("No Action");
                    foreach (TEncoding t in gef.getEncodings())
                        enclist.Items.Add(t.getFrom());
                }
                else
                {
                    RTFFileNames = openFileDialog1.FileNames;
                    pageRtf2Load();
                }
            }
        }

        private delegate void RTFFileLoadDel(String[] p);
        private void RTFFilesLoad(string[] p)
        {
            RichTextParser rtp = new RichTextParser();
            RTFFileFonts = new ArrayList();
            fltran = new ArrayList();
            for (int i = 0; i < p.Length; i++)
            {
                ArrayList al = rtp.GetFonts(p[i]);
                RTFFileFonts.Add(al);
                foreach (String s in al)
                {
                    MethodInvoker mi = delegate
                    {
                        if (!FileExists(s))
                        {
                            GenericEncodingFramework gef = new GenericEncodingFramework();
                            if (!gef.loaded) gef.Load();
                            fltran.Add(gef.isTo(s));
                            lstRtfFont.Items.Add(s);
                        }
                    };
                    lstRtfFont.Invoke(mi);
                }
            }
        }

        private void RTFLoadFin(IAsyncResult ar)
        {
            AsyncResult result = (AsyncResult)ar;
            MethodInvoker fin = delegate
            {
                rtfloading.Active = false;
                loadword.Active = false;
                btnSelectRtf.Enabled = true;
                btnSelectWord.Enabled = true;
                wizard1.gotoPage(2);
            };
            wizard1.Invoke(fin);
        }
        private ArrayList RTFFileFonts;
        private bool FileExists(String font)
        {
            foreach (String s in lstRtfFont.Items)
            {
                if (s == font)
                    return true;
            }
            return false;
        }

        private ArrayList fltran;
        private String output;
        private void rtfConvert()
        {
            iController.start_transcode(globalFileName,fltran,output,checkBox3.Checked);
        }

        /// <summary>
        /// Word
        /// </summary>

        private void word_Selectfile()
        {
            openFileDialog1.Multiselect = zbutt.Registration.registered();
            openFileDialog1.Filter = "Microsoft Word Files (*.doc;*.html;*.htm;*.mht;*.mhtml;*.xml;*.docx)|*.doc;*.html;*.htm;*.mht;*.mhtml;*.xml;*.docx";
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    btnSelectWord.Enabled = false;
                    loadword.Active = true;
                    if (openFileDialog1.FileNames.Length > 1)
                    {
                        WordFilesDel WFD = new WordFilesDel(Words2rtf);
                        WFD.BeginInvoke(openFileDialog1.FileNames, null, null);
                    }
                    else
                    {
                        globalFileName = openFileDialog1.FileName;
                        RTFFileNames = openFileDialog1.FileNames;
                        word2rtf(globalFileName);
                    }
                }
        }
        private delegate void WordFilesDel(string[] filenames);
        private string[] RTFFileNames;
        private void Words2rtf(string[] filenames)
        {
            for (int i = 0; i < filenames.Length; i++)
            {
                formatConvert.iConverter FC = new formatConvert.iConverter(filenames[i], "rtf");
                filenames[i] = filenames[i].Substring(0, filenames[i].LastIndexOf('.')) + ".rtf";
                FC.convert();
            }
            RTFFileNames = filenames;
            lstRtfFont.Items.Clear();
            RTFFileLoadDel dg = new RTFFileLoadDel(RTFFilesLoad);
            dg.BeginInvoke(filenames, RTFLoadFin, null);
            GenericEncodingFramework gef = new GenericEncodingFramework();
            if (!gef.loaded)
                gef.Load();
            enclist.Items.Clear();
            enclist.Items.Add("No Action");
            foreach (TEncoding t in gef.getEncodings())
                enclist.Items.Add(t.getFrom());
        }

        private void word2rtf(string fileName)
        {

            formatConvert.iConverter formCov = new formatConvert.iConverter(fileName, "rtf");
            MethodInvoker mk = new MethodInvoker(formCov.convert);
            mk.BeginInvoke(new AsyncCallback(CallWordFinished),null);
            //formCov.convert();
        }
        private void CallWordFinished(IAsyncResult ar)
        {
            AsyncResult result = (AsyncResult)ar;
            MethodInvoker del = (MethodInvoker)result.AsyncDelegate;
            string wordfilename = globalFileName.Substring(0, globalFileName.IndexOf('.'));
            globalFileName = wordfilename + ".rtf";
            pageRtf2Load();
        }

        private delegate ArrayList DelGetFons(String fname);
        private DelGetFons dgf;
        private void pageRtf2Load()
        {
            lstRtfFont.Items.Clear();
            RichTextParser rtp = new RichTextParser();
            dgf = new DelGetFons(rtp.GetFonts);
            IAsyncResult a =  dgf.BeginInvoke(globalFileName,new AsyncCallback(CallBack),null);
        }

        private void CallBack(IAsyncResult ar)
        {
            AsyncResult result = (AsyncResult)ar;
            DelGetFons del =
                (DelGetFons)result.AsyncDelegate;
            ArrayList al = del.EndInvoke(ar);
            fltran = new ArrayList();
            MethodInvoker updatePages = delegate
            {
                GenericEncodingFramework gef = new GenericEncodingFramework();
                if (!gef.loaded) gef.Load();
                foreach (String s in al)
                {
                    fltran.Add(gef.isTo(s));
                    lstRtfFont.Items.Add(s);
                }
                enclist.Items.Clear();
                enclist.Items.Add("No Action");
                foreach (TEncoding te in gef.getEncodings())
                {
                    enclist.Items.Add(te.getFrom());
                }
                rtfloading.Active = false;
                loadword.Active = false;
                btnSelectRtf.Enabled = true;
                btnSelectWord.Enabled = true;
                wizard1.gotoPage(2);
            };
            if (lstRtfFont.InvokeRequired)
            {
                this.Invoke(updatePages);
            }
            else
            {
                updatePages.Invoke();
            }
        }

        private void wordConvert()
        {
           //?? icon.start_transcode(globalFileName,fltran);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pageWelcome_MouseDown(object sender, MouseEventArgs e)
        {
            miceMove();
        }

        private int RTFfindTranValue(String fontname)
        {
            for (int i = 0; i < lstRtfFont.Items.Count; i++)
            {
                if (fontname == lstRtfFont.Items[i].ToString())
                    return (int)fltran[i];
            }
            return -1;
        }
        private void btnRtfTrans_Click(object sender, EventArgs e)
        {
            if (RTFFileNames.Length > 1)
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                fbd.SelectedPath = System.IO.Directory.GetCurrentDirectory();
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    wizard1.SelectedPage.Enabled = false;
                    MethodInvoker mk = delegate
                    {
                        for (int i = 0; i < RTFFileFonts.Count; i++)
                        {
                            ArrayList Fonts = (ArrayList)RTFFileFonts[i];
                            ArrayList FonTran = new ArrayList();
                            for (int j = 0; j < Fonts.Count; j++)
                            {
                                FonTran.Add(RTFfindTranValue(Fonts[j].ToString()));
                            }
                            string orgfile = RTFFileNames[i];
                            string file = orgfile.Substring(orgfile.LastIndexOf('\\'));
                            if (checkBox3.Checked)
                            {
                                output = fbd.SelectedPath + "\\" + file + ".html";
                            }
                            else
                            {
                                output = fbd.SelectedPath + "\\" + file + ".txt";
                            }
                            iController.start_transcode(orgfile, FonTran, output,checkBox3.Checked);
                        }
                    };
                    loadingRtfCircle.Active = true;
                    mk.BeginInvoke(FinSaveRTFs, null);
                }
            }
            else
            {
                saveFileDialog1.DefaultExt = ".txt";
                if (checkBox3.Checked) {
                    saveFileDialog1.FileName = globalFileName.Substring(0, globalFileName.Length - 3) + "html";
                }
                else
                {
                    saveFileDialog1.FileName = globalFileName.Substring(0, globalFileName.Length - 3) + "txt";
                }
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    wizard1.SelectedPage.Enabled = false;
                    output = saveFileDialog1.FileName;
                    loadingRtfCircle.Active = true;
                    timerRtf.Enabled = true;
                    myThread = new Thread(new ThreadStart(rtfConvert));
                    myThread.IsBackground = true;
                    myThread.Priority = ThreadPriority.Lowest;
                    myThread.Start();
                }
            }
        }

        private void FinSaveRTFs(IAsyncResult ar)
        {
            MethodInvoker mk = delegate
            {
                loadingRtfCircle.Active = false;
                wizard1.SelectedPage.Enabled = false;
                wizard1.gotoPage(8);
            };
            wizard1.Invoke(mk);
        }
        
        private void btnSelectRtf_Click(object sender, EventArgs e)
        {
            Config c = new Config("MyMyanmar\\Transcode");
            string s = c.Read("rtftohtml", "false");
            checkBox3.Checked = Convert.ToBoolean(s);
            rtf_SelectFile();
        }

        private void btnSelectWord_Click(object sender, EventArgs e)
        {
            Config c = new Config("MyMyanmar\\Transcode");
            string s = c.Read("rtftohtml", "false");
            checkBox3.Checked = Convert.ToBoolean(s); 
            word_Selectfile();
        }

        private void btnWordTrans_Click(object sender, EventArgs e)
        {
            myThread = new Thread(new ThreadStart(wordConvert));
            myThread.Start();
            preloadrtf.Enabled = true;
        }

        private void btnRtf_Click(object sender, EventArgs e)
        {
            wizard1.gotoPage(1);
        }

        private void btnWord_Click(object sender, EventArgs e)
        {
            wizard1.gotoPage(3);
        }

        private void formMain_Load(object sender, EventArgs e)
        {
            
            //String y = UnicodePass.pass2("အဂၤ​လိ‍ပ်​စာ​ ပါ​ေမာကၡ​");
            //int k = y.Length;

            this.Left = (Screen.PrimaryScreen.WorkingArea.Width / 2)  - (this.Width /2);
            this.Top = (Screen.PrimaryScreen.WorkingArea.Height / 2) - (this.Height / 2);
            GenericEncodingFramework gef = new GenericEncodingFramework();
            if (Registration.registered())
            {
                label31.Text = "Your copy of Transcode is activated";
                label29.Text = "Activated. Beta 6066";
            }
            else
            {
                label31.Text = "Your copy of Transcode is not activated";
                label29.Text = "Unactivated. Beta 6066";
            }
            gef.Load();
        }

        private void timerRtf_Tick(object sender, EventArgs e)
        {
            if (myThread.IsAlive)
                loadingRtfCircle.Active = true;
            else
            {
                loadingRtfCircle.Active = false;
                wizard1.SelectedPage.Enabled = false;
                wizard1.gotoPage(8);
                timerRtf.Enabled = false;
            }
        }


        private void pageRtf_Click(object sender, EventArgs e)
        {

        }

        private void lstRtfFont_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstRtfFont.SelectedIndex != -1)
            {
                enclist.SelectedIndex = (int)fltran[lstRtfFont.SelectedIndex] + 1;
            }
        }

        private void enclist_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (enclist.SelectedIndex != -1 && lstRtfFont.SelectedIndex != -1)
                fltran[lstRtfFont.SelectedIndex] = enclist.SelectedIndex - 1;
        }

        private void glassButton1_Click(object sender, EventArgs e)
        {
            wizard1.gotoPage(0);
        }

        private void btnText_Click(object sender, EventArgs e)
        {
            wizard1.gotoPage(7);
        }

        private void btnSql_Click(object sender, EventArgs e)
        {
            wizard1.gotoPage(4);
        }

        private void glassButton2_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "SQL Dump File (*.sql)|*.sql";
            openFileDialog1.Multiselect = false;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (!zbutt.Registration.registered())
                {
                    FileInfo fi = new FileInfo(openFileDialog1.FileName);
                    if (fi.Length > 512000)
                    {
                        MessageBox.Show("File size exceed the limit in beta");
                        return;
                    }
                }
                globalFileName = openFileDialog1.FileName;
                sqlloadbtn.Enabled = false;
                sqlcircle.Active = true;
                LoadSQLFile(openFileDialog1.FileName);
            }
        }

        private void LoadSQLFile(string p)
        {
            LoadSQLDel dl = new LoadSQLDel(SQLParser.getTables);
            dl.BeginInvoke(p,SQLLoadCallBack,null);
        }

        private delegate ArrayList LoadSQLDel(string p);

        private ArrayList SQLTables;
        private void SQLLoadCallBack(IAsyncResult ar)
        {
            AsyncResult result = (AsyncResult)ar;
            LoadSQLDel del =
                (LoadSQLDel)result.AsyncDelegate;
            SQLTables = del.EndInvoke(ar);
            MethodInvoker mk = delegate
            {
                lstsqltables.Items.Add("All Tables");
                foreach (TSQLTable s in SQLTables)
                {
                    lstsqltables.Items.Add(s.getname());
                }
                GenericEncodingFramework gef = new GenericEncodingFramework();
                if(!gef.loaded)gef.Load();
                encs.Items.Clear();
                encs.Items.Add("No Action");
                foreach (TEncoding te in gef.getEncodings())
                {
                    encs.Items.Add(te.getFrom());
                }
                sqlloadbtn.Enabled = true;
                sqlcircle.Active = false;
                wizard1.gotoPage(6);
            };
            lstsqltables.Invoke(mk);
        }

        private void lstsqltables_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstsqltables.SelectedIndex != -1)
            {
                sqlcols.Items.Clear();
                if (lstsqltables.SelectedIndex == 0)
                {
                    sqlcols.Items.Add("All Columns");
                    sqlcols.SelectedIndex = 0;
                }
                else
                {
                    TSQLTable t = (TSQLTable)SQLTables[lstsqltables.SelectedIndex - 1];
                    sqlcols.Items.Add("All Columns");
                    foreach (String s in t.getcols())
                    {
                        sqlcols.Items.Add(s);
                    }
                }
            }
        }

        private void encs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstsqltables.SelectedIndex != -1 && sqlcols.SelectedIndex != -1)
            {
                if (lstsqltables.SelectedIndex == 0 && sqlcols.SelectedIndex == 0)
                {
                    for (int i = 0; i < SQLTables.Count; i++)
                    {
                        TSQLTable t = (TSQLTable)SQLTables[i];
                        for (int j = 0; j < t.enc.Count; j++)
                        {
                            t.enc[j] = encs.SelectedIndex - 1;
                        }
                        SQLTables[i] = t;
                    }
                }
                else if (sqlcols.SelectedIndex == 0)
                {
                    TSQLTable t = (TSQLTable)SQLTables[lstsqltables.SelectedIndex - 1];
                    for (int j = 0; j < t.enc.Count; j++)
                    {
                        t.enc[j] = encs.SelectedIndex - 1;
                    }
                    SQLTables[lstsqltables.SelectedIndex - 1] = t;
                }
                else
                {
                    TSQLTable t = (TSQLTable)SQLTables[lstsqltables.SelectedIndex - 1];
                    t.enc[sqlcols.SelectedIndex - 1] = encs.SelectedIndex - 1;
                    SQLTables[lstsqltables.SelectedIndex - 1] = t;
                }
            }
        }

        private void sqlcols_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sqlcols.SelectedIndex > -1 && lstsqltables.SelectedIndex > -1)
            {
                if (sqlcols.SelectedIndex == 0 && lstsqltables.SelectedIndex == 0)
                {
                    int a = -1;
                    for (int i = 0; i < SQLTables.Count; i++)
                    {
                        TSQLTable t = (TSQLTable)SQLTables[i];
                        a = (int)t.enc[0];
                        for (int j = 0; j < t.enc.Count; j++)
                        {
                            if(a != (int)t.enc[j]){
                                encs.SelectedIndex = -1;
                                return;
                            }
                            a = (int)t.enc[j];
                        }
                    }
                    encs.SelectedIndex = a + 1;
                }
                else if (sqlcols.SelectedIndex == 0 && lstsqltables.SelectedIndex>0)
                {
                    TSQLTable t = (TSQLTable)SQLTables[lstsqltables.SelectedIndex - 1];
                    int a = (int)t.enc[0];
                    for (int j = 0; j < t.enc.Count; j++)
                    {
                        if (a != (int)t.enc[j])
                        {
                            encs.SelectedIndex = -1;
                            return;
                        }
                    }
                    encs.SelectedIndex = a + 1;
                }
                else
                {
                    //MessageBox.Show(t.enc[sqlcols - 1].toString());
                    TSQLTable t = (TSQLTable)SQLTables[lstsqltables.SelectedIndex - 1];
                    encs.SelectedIndex = (int)t.enc[sqlcols.SelectedIndex - 1] + 1;
                }
            }
        }

        private delegate void doTransqldel(string In, ArrayList tables, ArrayList encs,String Out,bool HTML);
        private void glassButton3_Click(object sender, EventArgs e)
        {
            saveFileDialog1.DefaultExt = ".sql";
            saveFileDialog1.FileName = globalFileName.Substring(0, globalFileName.Length - 3) + "transcoded.sql";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                sqlc.Active = true;
                GenericEncodingFramework gef = new GenericEncodingFramework();
                gef.Load();
                doTransqldel del = new doTransqldel(SQLParser.DoTranscode);
                del.BeginInvoke(globalFileName, SQLTables, gef.getEncodings(),saveFileDialog1.FileName,checkBox1.Checked, SQLTranCallBack, null);
            }
        }

        private void SQLTranCallBack(IAsyncResult ar)
        {
            MethodInvoker mk = delegate
            {
                wizard1.gotoPage(8);
                sqlc.Active = false;
            };
            wizard1.Invoke(mk);
        }

        private void btnXml_Click(object sender, EventArgs e)
        {
            Config c = new Config("MyMyanmar\\Transcode");
            string s = c.Read("rtfremote", "false");
            rtremote.Checked = Convert.ToBoolean(s);
            GenericEncodingFramework gef = new GenericEncodingFramework();
            if (!gef.loaded) gef.Load();
            encssum.Items.Clear();
            foreach (TEncoding te in gef.getEncodings())
            {
                encssum.Items.Add(te.getFrom());
            }
             c = new Config("MyMyanmar\\Transcode");
             s = c.Read("encssum", "0");
            encssum.SelectedIndex = Convert.ToInt32(s);
            s = c.Read("remsr", "false");
            remsr.Checked = Convert.ToBoolean(s);
            s = c.Read("remhtml", "false");
            remhtml.Checked = Convert.ToBoolean(s);
            remotesht.Text = c.Read("remotesht", "F4");
            kh = new KeyboardHook();
            kh.KeyIntercepted += new KeyboardHook.KeyboardHookEventHandler(CaptureKey);
            wizard1.gotoPage(5);
        }

        private void pageXml_Click(object sender, EventArgs e)
        {

        }
        KeyboardHook kh;
        private void glassButton2_Click_1(object sender, EventArgs e)
        {
            if (kh != null)
                kh.Dispose();
            kh = new KeyboardHook();
            kh.KeyIntercepted += new KeyboardHook.KeyboardHookEventHandler(CaptureKey);
        }

        private void CaptureKey(KeyboardHook.KeyboardHookEventArgs e)
        {
            //Check if this key event is being passed to
            //other applications and disable TopMost in 
            //case they need to come to the front
            if (e.PassThrough)
            {
                //this.TopMost = false;
            }
            if (remotesht.Text == e.KeyName)
            {
                clipcache = Clipboard.GetText();
                if (!remsr.Checked)
                {
                    System.Windows.Forms.SendKeys.SendWait("^x");
                }
                else
                {
                    System.Windows.Forms.SendKeys.SendWait("^c");
                }
                remtim.Enabled = true;
            }            
        }
        private string clipcache;
        private string remcache;
        private remresfm remresfmn;
        private void remtim_Tick(object sender, EventArgs ea)
        {
            remtim.Enabled = false;
            if (rtremote.Enabled && rtremote.Checked)
            {
                StreamWriter sw = new StreamWriter("temp.rtf");
                sw.Write(Clipboard.GetData(DataFormats.Rtf));
                sw.Close();
                RTFFileNames = openFileDialog1.FileNames;
                globalFileName = "temp.rtf";
                pageRtf2Load();
                this.TopMost = true;
                this.TopMost = false;
            }
            else
            {
                if (Clipboard.ContainsData(DataFormats.Rtf))
                {
                    StreamWriter sw = new StreamWriter("temp.rtf");
                    sw.Write(Clipboard.GetData(DataFormats.Rtf));
                    sw.Close();
                    RTFFileNames = openFileDialog1.FileNames;
                    globalFileName = "temp.rtf";
                    RichTextParser rtp = new RichTextParser();
                    ArrayList al = rtp.GetFonts(globalFileName);
                    ArrayList fltran = new ArrayList();
                    GenericEncodingFramework gef = new GenericEncodingFramework();
                    if (!gef.loaded) gef.Load();
                    foreach (string s in al)
                    {
                        fltran.Add(gef.isTo(s));
                    }
                    ArrayList enis = new ArrayList();
                    ArrayList enos = new ArrayList();
                    ArrayList Encodings = gef.getEncodings();
                    foreach (TEncoding e in Encodings)
                    {
                        enis.Add(e.getENI());
                        enos.Add(e.getENO());
                    }
                    rtp.DoTranscode(globalFileName, enis, enos, fltran, globalFileName + ".tmp2", false);
                    StreamReader sr = new StreamReader(globalFileName + ".tmp2");
                    String ss = sr.ReadToEnd();
                    sr.Close();
                    Clipboard.SetText(ss);
                    remtim2.Enabled = true;
                }
                else
                {
                    GenericEncodingFramework gef = new GenericEncodingFramework();
                    if (!gef.loaded) gef.Load();
                    TEncoding enc = (TEncoding)(gef.getEncodings()[encssum.SelectedIndex]);
                    String s = Clipboard.GetText();
                    s = s + " ";
                    remcache = Transcode.Transcode.transcode(enc.getENI(), enc.getENO(), Clipboard.GetText());
                    if (remhtml.Checked)
                        remcache = Transcode.Transcode.XMLEntNormalizer(remcache);
                    //remcache = remcache.ToString();
                    Clipboard.SetText(remcache);
                    remtim2.Enabled = true;
                }
            }
        }

        private void remtim2_Tick(object sender, EventArgs e)
        {
            remtim2.Enabled = false;
            if (!remsr.Checked)
            {
                System.Windows.Forms.SendKeys.SendWait("^v");
            }
            else
            {
                remresfmn = new remresfm();
                remresfmn.setText(remcache);
                remresfmn.TopMost = true;
                remresfmn.ShowDialog();
            }
            try
            {
                Clipboard.SetText(clipcache);
            }
            catch (Exception ex)
            {
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void remhtml_CheckedChanged(object sender, EventArgs e)
        {
            Config cfg = new Config("MyMyanmar\\Transcode");
            cfg.Write("remhtml", remhtml.Checked.ToString());
        }

        private void remsr_CheckedChanged(object sender, EventArgs e)
        {
            Config cfg = new Config("MyMyanmar\\Transcode");
            cfg.Write("remsr", remsr.Checked.ToString());
        }

        private void encssum_SelectedIndexChanged(object sender, EventArgs e)
        {
            Config cfg = new Config("MyMyanmar\\Transcode");
            cfg.Write("encssum", encssum.SelectedIndex.ToString());
        }

        private void remotesht_TextChanged(object sender, EventArgs e)
        {
            Config cfg = new Config("MyMyanmar\\Transcode");
            cfg.Write("remotesht", remotesht.Text);
        }

        private void glassButton6_Click(object sender, EventArgs e)
        {
            wizard1.Back();
            wizard1.gotoPage(0);
        }

        private void glassButton8_Click(object sender, EventArgs e)
        {
            wizard1.gotoPage(0);
        }

        private void glassButton1_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void DisposeKeyListen()
        {
            kh.Dispose();
        }

        private void glassButton5_Click(object sender, EventArgs e)
        {
            scrollingBox1.Items.Clear();
            scrollingBox1.Items.Add("Transcode Beta 3");
            scrollingBox1.Items.Add("\r\n");
            scrollingBox1.Items.Add("Produced by MyMyanmar");
            scrollingBox1.Items.Add("in association with iSoft");
            scrollingBox1.Items.Add("© Technomation Studios");
            scrollingBox1.Items.Add("All Rights Reserved.");
            scrollingBox1.Items.Add("\r\n");
            scrollingBox1.Items.Add("\r\n");
            scrollingBox1.Items.Add("Developers");
            scrollingBox1.Items.Add("Htoo Myint Naung");
            scrollingBox1.Items.Add("Htain Linn Htoo");
            scrollingBox1.Items.Add("Yan Aung");
            scrollingBox1.Items.Add("\r\n\r\n");
            scrollingBox1.Items.Add("Designer\r\nKyaw Swar Linn");
            scrollingBox1.Items.Add("\r\n\r\n");
            scrollingBox1.Items.Add("This program contains the results from following MyMyanmar Research Projects");
            scrollingBox1.Items.Add("\r\nMySummone\r\nMyTranscode 1,1.5,1.6,2\r\nMyBreak\r\nYAN#\r\ncodename-mylo\r\ncodename-butt\r\ncodename-Yoga\r\n");
            scrollingBox1.Items.Add("\r\n\r\n");
            scrollingBox1.Items.Add("This program contains the following Software Libraries\r\nNRTFTree\r\nMicrosoft Office 2007 COM Library");
            scrollingBox1.Items.Add("other codes/libraries");
            scrollingBox1.Items.Add("\r\n");
            scrollingBox1.Items.Add("Special Thanks to");
            scrollingBox1.Items.Add(@"U Myint Thu​ (Computer Journal)
U Thein Htut (Geocomp Computer)
Michael Everson (Evertype)
Martin Hosken (SIL International)
Kaung Htut Oo
Marc Durdin (Keyman)
San Myo Aung");
            scrollingBox1.Items.Add("\r\n");
            scrollingBox1.Items.Add("\r\n");
            scrollingBox1.Items.Add("\r\n");
            scrollingBox1.Items.Add("\r\n");
            scrollingBox1.Items.Add("\r\n");
            wizard1.gotoPage(9);
        }

        private void glassButton25_Click(object sender, EventArgs e)
        {
            DisposeKeyListen();
            wizard1.gotoPage(0);
        }

        private void glassButton4_Click(object sender, EventArgs e)
        {
            MessageBalloon m_mb = new MessageBalloon();
            m_mb.Parent = glassButton4;
            m_mb.Title = "Main Menu";
            m_mb.TitleIcon = TooltipIcon.None;
            m_mb.Text = @"Please click an appropirate button for type of conversion you need.
To convert Rich Text Documents, click Rich Text.
To convert formats Microsoft Word support, click Word. (require Word installed)
To convert text present in any application remotely, click Remote.
To convert SQL Database Dumps, click Database.
To convert text based files like HTML, XML, CSV, click Text";
            m_mb.Align = BalloonAlignment.BottomMiddle;
            m_mb.Show();
        }

        private void glassButton34_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = true;
            openFileDialog1.Filter = "Text File (*.txt)|*.txt";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                GenericEncodingFramework gef = new GenericEncodingFramework();
                if (!gef.loaded)
                    gef.Load();
                ptexplugs.Items.Clear();
                foreach (TEncoding t in gef.getEncodings())
                    ptexplugs.Items.Add(t.getFrom());
                wizard1.gotoPage(10);
            }
        }

        private void glassButton33_Click(object sender, EventArgs e)
        {
            GenericEncodingFramework gef = new GenericEncodingFramework();
            if (!gef.loaded) gef.Load();
            TEncoding t = (TEncoding)gef.getEncodings()[ptexplugs.SelectedIndex];
            if (openFileDialog1.FileNames.Length > 1)
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                fbd.SelectedPath = System.IO.Directory.GetCurrentDirectory();
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    wizard1.Pages[wizard1.SelectedIndex].Enabled = false;
                    plaintextcircle.Active = true;
                    MethodInvoker mk = delegate
                    {
                        foreach (string s in openFileDialog1.FileNames)
                        {
                            string x = fbd.SelectedPath + s.Substring(s.LastIndexOf('\\'));
                            if (!xml)
                            {
                                Transcode.PlainTextParser.parseandtranscode(s, x, t);
                            }
                            else
                            {
                                Transcode.XMLTextParser.parseandtranscode(s, x, t);
                            }
                        }
                    };
                    mk.BeginInvoke(PlainTextCB, null);
                }
            }
            else
            {
                saveFileDialog1.FileName = openFileDialog1.FileName + ".tran.txt";
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    wizard1.Pages[wizard1.SelectedIndex].Enabled = false;
                    plaintextcircle.Active = true;
                    MethodInvoker mk = delegate
                    {
                        if (!xml)
                        {
                            Transcode.PlainTextParser.parseandtranscode(openFileDialog1.FileName, saveFileDialog1.FileName, t);
                        }
                        else
                        {
                            Transcode.XMLTextParser.parseandtranscode(openFileDialog1.FileName, saveFileDialog1.FileName, t);
                        }
                    };
                    mk.BeginInvoke(PlainTextCB, null);
                }
            }
        }

        private void PlainTextCB(IAsyncResult ar)
        {
            MethodInvoker updatePages = delegate
            {
                wizard1.Pages[wizard1.SelectedIndex].Enabled = true;
                plaintextcircle.Active = false;
                xml = false;
                wizard1.gotoPage(8);
            };
            this.Invoke(updatePages);
        }

        private void glassButton43_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            Transcode.CSVTextParser.parseandtranscode(openFileDialog1.FileName,openFileDialog1.FileName + "x",null,null);
        }

        private ArrayList csvencl;
        private void glassButton32_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = false;
            openFileDialog1.Filter = "CSV File (*.csv)|*.csv";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (!zbutt.Registration.registered())
                {
                    FileInfo fi = new FileInfo(openFileDialog1.FileName);
                    if (fi.Length > 409600)
                    {
                        MessageBox.Show("File Size exceed the beta limit");
                        return;
                    }
                }
                csvcols.Items.Clear();
                int i = Transcode.CSVTextParser.howmanycolumns(openFileDialog1.FileName);
                csvencl = new ArrayList();
                for (int x = 0; x <= i; x++)
                {
                    csvcols.Items.Add("Column " + x.ToString());
                    csvencl.Add(-1);
                }
                GenericEncodingFramework gef = new GenericEncodingFramework();
                if (!gef.loaded) gef.Load();
                csvplugs.Items.Clear();
                csvplugs.Items.Add("No Action");
                foreach (TEncoding t in gef.getEncodings())
                    csvplugs.Items.Add(t.getFrom());
                wizard1.gotoPage(11);
            }
        }

        private void csvcols_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (csvcols.SelectedIndex != -1)
            {
                csvplugs.SelectedIndex = (int)csvencl[csvcols.SelectedIndex] + 1;
            }
        }

        private void csvplugs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (csvcols.SelectedIndex != -1 && csvplugs.SelectedIndex != -1)
            {
                csvencl[csvcols.SelectedIndex] = csvplugs.SelectedIndex - 1;
            }
        }

        private void glassButton43_Click_1(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                wizard1.SelectedPage.Enabled = false;
                GenericEncodingFramework gef = new GenericEncodingFramework();
                if(!gef.loaded) gef.Load();
                MethodInvoker mk = delegate
                {
                    Transcode.CSVTextParser.parseandtranscode(openFileDialog1.FileName, saveFileDialog1.FileName, gef.getEncodings(), csvencl);
                };
                csvload.Active = true;
                mk.BeginInvoke(csvCB, null);
            }
        }

        private void csvCB(IAsyncResult ar)
        {
            MethodInvoker updatePages = delegate
            {
                wizard1.Pages[wizard1.SelectedIndex].Enabled = true;
                csvload.Active = false;
                wizard1.gotoPage(8);
            };
            this.Invoke(updatePages);
        }

        private bool xml = false;
        private void glassButton31_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = true;
            openFileDialog1.Filter = "XML File (*.xml)|*.xml";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                GenericEncodingFramework gef = new GenericEncodingFramework();
                if (!gef.loaded)
                    gef.Load();
                ptexplugs.Items.Clear();
                foreach (TEncoding t in gef.getEncodings())
                    ptexplugs.Items.Add(t.getFrom());
                xml = true;
                wizard1.gotoPage(10);
            }
        }

        private void glassButton7_Click(object sender, EventArgs e)
        {
            MessageBalloon m_mb = new MessageBalloon();
            m_mb.Parent = glassButton4;
            m_mb.Title = "Help";
            m_mb.TitleIcon = TooltipIcon.None;
            m_mb.Text = @"Click Select File(s) to open Rich Text files you want to convert.
Transcode will analyze the fonts inside files for future process.
unlicensed version of Transcode cannot open Multiple files.";
            m_mb.Align = BalloonAlignment.BottomMiddle;
            m_mb.CenterStem = true;
            m_mb.UseAbsolutePositioning = false;
            m_mb.Show();
        }

        private void glassButton13_Click(object sender, EventArgs e)
        {
            MessageBalloon m_mb = new MessageBalloon();
            m_mb.Parent = glassButton13;
            m_mb.Title = "Help";
            m_mb.TitleIcon = TooltipIcon.None;
            m_mb.Text = @"Fonts list show the name of fonts used in your document(s).
Select font you want to convert and select the PlugIn from plugin list to apply conversion for the text written in that font.
Transcode can automatically recognize most of the font with thir appropirate encoding.
After selecting fonts and plugins, click save to save the converted files.";
            m_mb.Align = BalloonAlignment.BottomMiddle;
            m_mb.CenterStem = true;
            m_mb.UseAbsolutePositioning = false;
            m_mb.Show();
        }

        private void glassButton16_Click(object sender, EventArgs e)
        {
            MessageBalloon m_mb = new MessageBalloon();
            m_mb.Parent = glassButton16;
            m_mb.Title = "Help";
            m_mb.TitleIcon = TooltipIcon.None;
            m_mb.Text = @"Click Select File(s) to open files. The type of files you can open depends on your version of Microsoft Word. Opened files will be converted to Rich Text format by Microsoft Word 
for future process. Microsoft Word must be installed to use this feature. unlicensed version of Transcode cannot open Multiple files.";
            m_mb.Align = BalloonAlignment.BottomMiddle;
            m_mb.CenterStem = true;
            m_mb.UseAbsolutePositioning = false;
            m_mb.Show();
        }

        private void glassButton20_Click(object sender, EventArgs e)
        {
            MessageBalloon m_mb = new MessageBalloon();
            m_mb.Parent = glassButton20;
            m_mb.Title = "Help";
            m_mb.TitleIcon = TooltipIcon.None;
            m_mb.Text = @"Click Select File to open SQL dump files. Transcode will analyze tables and columns in the file for future process.
Transcode support SQL dumps produced by MySQLAdmin with Complete Inserts and many other SQL Server Managers. However, it is recommanded to download and use 'Php SQL Dump Utility for Transcode' for better
compatibility with Unicode UTF-8 Encoding.
unlicensed version of Transcode can convert SQL dumps not more than 500Kb.";
            m_mb.Align = BalloonAlignment.BottomMiddle;
            m_mb.CenterStem = true;
            m_mb.UseAbsolutePositioning = false;
            m_mb.Show();
        }

        private void glassButton24_Click(object sender, EventArgs e)
        {
            MessageBalloon m_mb = new MessageBalloon();
            m_mb.Parent = glassButton24;
            m_mb.Title = "Help";
            m_mb.TitleIcon = TooltipIcon.None;
            m_mb.Text = @"Select plugin, shortcut function key and go to any application like NotePad or Microsoft Word,
Select a paragraph of text in that application and press the function key. If you checked Show Result, the converted text will 
be shown in a new dialog box. If you did't checked Show Result, the text you selected in that application will be replaced with the converted version.
You can use any combination of function key if the target application blocks. If you set shortcut key as F7 and Transcode will capture all CTRL+F7,SHIFT+F7,ALT+F7; any combination.
Check HTML entities if you want Transcode to convert html character entites like &#4096;
Click Restart when Transcode no longer respond the shortcut key due to some other application blocks it.";
            m_mb.Align = BalloonAlignment.BottomMiddle;
            m_mb.CenterStem = true;
            m_mb.UseAbsolutePositioning = false;
            m_mb.Show();
        }

        private void glassButton28_Click(object sender, EventArgs e)
        {
            MessageBalloon m_mb = new MessageBalloon();
            m_mb.Parent = glassButton28;
            m_mb.Title = "Help";
            m_mb.TitleIcon = TooltipIcon.None;
            m_mb.Text = @"Table list and Column list show the list of tables and columns in the SQL Database Dump File.
Select the table and column you want to convert and select the PlugIn from plugin list to apply conversion for the data in the column in the table you selected.
You can also choose to convert entire table or column by selecting 'All Tables' or/or 'All Columns' and select the Plugin.
After selecting tables,columns and plugins, click save to save the converted database files.";
            m_mb.Align = BalloonAlignment.BottomMiddle;
            m_mb.CenterStem = true;
            m_mb.UseAbsolutePositioning = false;
            m_mb.Show();
        }

        private void glassButton37_Click(object sender, EventArgs e)
        {
            MessageBalloon m_mb = new MessageBalloon();
            m_mb.Parent = glassButton37;
            m_mb.Title = "Help";
            m_mb.TitleIcon = TooltipIcon.None;
            m_mb.Text = @"Click to open the type of file(s) you want to convert. Transcode will analyze the file(s) for future process.
unlicensed version of Transcode cannot convert multiple files and csv files more than 400Kb";
            m_mb.Align = BalloonAlignment.BottomMiddle;
            m_mb.CenterStem = true;
            m_mb.UseAbsolutePositioning = false;
            m_mb.Show();
        }

        private void glassButton48_Click(object sender, EventArgs e)
        {
            LoadConfig();
            wizard1.gotoPage(12);
        }

        private void LoadConfig()
        {
            Config c = new Config("MyMyanmar\\Transcode");
            checkBox2.Checked = Convert.ToBoolean(c.Read("unicodepass", "True"));
        }

        private void glassButton53_Click(object sender, EventArgs e)
        {
            zbutt.Registration reg = new Registration();
        }



        private void glassButton55_Click(object sender, EventArgs e)
        {
            GenericEncodingFramework gef = new GenericEncodingFramework();
            gef.Load();
            ArrayList al = gef.getEncodings();
            TEncoding t = (TEncoding)al[0];
            string x = "";
            foreach (string s in t.getENI())
            {
                x += "\"" + s + "\",";
            }
            StreamWriter sw = new StreamWriter("Encodings\\array");
            sw.WriteLine(x);
            x = "";
            foreach (string s in t.getENO())
            {
                x += "\"" + s + "\",";
            }
            sw.WriteLine(x);
            sw.Close();
        }

        private void checkBox2_CheckedChanged_1(object sender, EventArgs e)
        {
            Config c = new Config("MyMyanmar\\Transcode");
            c.Write("unicodepass", checkBox2.Checked.ToString());
        }

        private void label30_Click(object sender, EventArgs e)
        {

        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            Config cfg = new Config("MyMyanmar\\Transcode");
            cfg.Write("rtftohtml", checkBox3.Checked.ToString());
        }

        private void rtremote_CheckedChanged(object sender, EventArgs e)
        {
            Config cfg = new Config("MyMyanmar\\Transcode");
            cfg.Write("rtfremote", rtremote.Checked.ToString());
            encssum.Enabled = !rtremote.Checked;
            remsr.Enabled = !rtremote.Checked;
        }
    }
}