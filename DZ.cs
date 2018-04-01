//@dnaspider

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic;

namespace dz
{
    public partial class DZ : Form
    {
        [DllImport("user32")] private static extern bool GetAsyncKeyState(System.Windows.Forms.Keys vKey);
        [DllImport("user32", EntryPoint = "keybd_event")] private static extern Int32 Keybd_event(System.Windows.Forms.Keys bVk, byte bScan, int dwFlags, int dwExtraInfo);
        [DllImport("user32")] private static extern ushort SetCursorPos(Int32 X, Int32 Y);
        [DllImport("user32", EntryPoint = "mouse_event")] private static extern Int32 Mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        //globals
        System.Collections.ArrayList ar = new System.Collections.ArrayList();
        System.Windows.Forms.Keys g_specialKey = Properties.Settings.Default.SettingSpecialKey;
        string p_ = Properties.Settings.Default.SettingBracketOpen;
        string _p = Properties.Settings.Default.SettingBracketClose;
        string ws_ = Properties.Settings.Default.SettingIgnoreWhiteSpaceOpen;
        string _ws = Properties.Settings.Default.SettingIgnoreWhiteSpaceClose;
        int g_presses = 1;//default press | «up»
        int g_length = Properties.Settings.Default.SettingCodeLength;
        string g_n = "0";//number | *# or :#
        string g_s = "";//string | «code-» GlobalString
        int g_x; int g_y;//pointer x y
        int g_i = 0;//listbox1 item
        int g_kb_i = 0;//kb item c
        bool g_ignoreWhiteSpace = false;
        string g_code;
        bool g_drag;
        int g_drag_x;
        int g_drag_y;

        public DZ() { InitializeComponent(); }

        //Sub
        private void Key(System.Windows.Forms.Keys key, bool shft, int presses)
        {
            if (shft == true) { Keybd_event(Keys.RShiftKey, 0, 1, 0); }
            if (g_presses > 1) { presses = g_presses; }
            for (int i = 0; i < presses; i++)
            {
                Keybd_event(key, 0, 0, 0);
                Keybd_event(key, 0, 2, 0);
            }
            if (shft) { Keybd_event(Keys.RShiftKey, 0, 2, 0); }
            GetAsyncKeyState(key);
        }
        private void KeyHold(System.Windows.Forms.Keys key) { Keybd_event(key, 0, 1, 0); }
        private void KeyRelease(System.Windows.Forms.Keys key) { Keybd_event(key, 0, 2, 0); }
        private void TextClear() { TextBox1.SelectAll(); TextBox1.SelectedText = ""; }
        private void Sleep(int ms) { System.Threading.Thread.Sleep(ms); Application.DoEvents(); }
        private void SleepMS(int ms) { System.Threading.Thread.Sleep(ms); }
        private void CleanMock() { Sleep(333); textBox2.Clear(); }
        private void LoadArray() {
            ar.Clear();
            for (int i = 0; i < ListBox1.Items.Count; i++) {
                if (ListBox1.Items[i].ToString().StartsWith(p_)) {
                    ar.Add(i + ":" + ListBox1.Items[i].ToString().Substring(1, ListBox1.Items[i].ToString().IndexOf(_p) - 1));
                }

            }
            //for (int i = 0; i < ar.Count; i++){
            //    Console.WriteLine(ar.Count + ": " + ar[i].ToString());
            //}
        }
        private void AddDbItm() {
            Properties.Settings.Default.SettingDB.Add(TextBox1.Text);
            ListBox1.Items.Add(TextBox1.Text);
            TextClear();
            ListBox1.SelectedIndex = ListBox1.Items.Count - 1;
            KeyRelease(Keys.S);
            KeyRelease(g_specialKey);
            CleanMock();
            LoadArray();
        }
        private void UpdateDbItm() {
            if (ListBox1.SelectedIndex < 0 || TextBox1.Text == "") { return; };

            int x = TextBox1.SelectionStart;
            int i = ListBox1.SelectedIndex;

            ListBox1.Items.RemoveAt(i);
            Properties.Settings.Default.SettingDB.RemoveAt(i);
            ListBox1.Items.Insert(i, TextBox1.Text);
            Properties.Settings.Default.SettingDB.Insert(i, TextBox1.Text);

            ListBox1.SelectedIndex = i;
            TextBox1.SelectionStart = x;

            CleanMock();
            LoadArray();
        }
        private void RemoveDbItm() {
            if (ListBox1.SelectedIndex == -1) { return; }
            string x = TextBox1.Text;
            TextClear();
            TextBox1.AppendText(ListBox1.SelectedItem.ToString());
            Properties.Settings.Default.SettingDB.RemoveAt(ListBox1.SelectedIndex);
            ListBox1.Items.RemoveAt(ListBox1.SelectedIndex);
            Key(Keys.Down, false, 1);
            TextClear();
            TextBox1.AppendText(x);
            LoadArray();
        }
        private void LoadDb() {
            foreach (var item in Properties.Settings.Default.SettingDB) {
                ListBox1.Items.Add(item);
            }
            LoadArray();
        }
        private void DarkMode() {
            if (Properties.Settings.Default.SettingDarkMode == true) {
                BackColor = Color.Black;
                SplitContainer1.BackColor = Color.Black;
                ListBox1.BackColor = Color.Black;
                ListBox1.ForeColor = Color.Lime;
                TextBox1.BackColor = Color.Black;
                TextBox1.ForeColor = Color.Lime;
            }
        }
        private void FixedSize() {
            if (ControlBox == true) {
                FormBorderStyle = FormBorderStyle.None;
                ControlBox = false;
                Text = "";
            } else {
                ControlBox = true;
                FormBorderStyle = FormBorderStyle.Sizable;
                Text = Properties.Settings.Default.SettingTitleText;
                if (Properties.Settings.Default.SettingIcon != "") { Icon = new Icon(Properties.Settings.Default.SettingIcon); } else { Icon = Icon; }
            }
            if (Properties.Settings.Default.SettingBackgroundImage != "") { BackgroundImage = Image.FromFile(Properties.Settings.Default.SettingBackgroundImage); }
            if (SplitContainer1.Visible) {
                if (Properties.Settings.Default.SettingBackgroundImage != "") {
                    BackColor = Color.GhostWhite;
                    SplitContainer1.Visible = false;
                }
            } else {
                if (Properties.Settings.Default.SettingDarkMode == true) { BackColor = Color.Black; }
                SplitContainer1.Visible = true;
            }
        }
        private void EditDbItm() {
            if (ListBox1.GetItemText(ListBox1.SelectedItem) != "" && TextBox1.Text == "") {
                TextBox1.AppendText(ListBox1.SelectedItem.ToString());
                CleanMock();
            }
        }
        private void AutoComplete(String fill, String fill2, int right)
        {
            Key(Keys.Back, false, 1);
            g_s = fill;
            PD();
            for (int i = 0; i < right; i++) {
                Key(Keys.Right, false, 1);
            }
            if (fill2.Length > 0) {
                SendKeys.Send(p_);
                g_s = fill2;
                PD();
                SendKeys.Send(_p);
                Key(Keys.Left, false, fill2.Length + 2);
            }
        }
        private void PD() {
            //Console.WriteLine(ControlChars.NewLine + "#####start#####");
            //Console.WriteLine("string: " + g_s);
            if (g_s.Contains(p_ + "rp" + _p) || g_s.Contains(p_ + "rm" + _p)) { g_x = MousePosition.X; g_y = MousePosition.Y; }
            if (Properties.Settings.Default.SettingSendkeysOnlyMode) {
                SendKeys.Send(g_s);
            } else {
                for (g_kb_i = 0; g_kb_i < g_s.Length; g_kb_i++) {
                    if (GetAsyncKeyState(Keys.Escape)) { break; }//stop
                    if (g_kb_i >= g_s.Length) { break; }
                    //Console.WriteLine("print: " + g_s.Substring(g_kb_i, 1));
                    Kb(g_s.Substring(g_kb_i, 1));
                }
            }
            //$repeat
            if (ListBox1.Items.Count > 0 && ListBox1.SelectedIndex > -1) { /*g_s = Nothing;*/
                if (ListBox1.SelectedItem.ToString().StartsWith(p_)) {
                    g_s = ListBox1.SelectedItem.ToString().Substring(ListBox1.SelectedItem.ToString().IndexOf(_p) + 1, ListBox1.SelectedItem.ToString().Length - ListBox1.SelectedItem.ToString().IndexOf(_p) - 1);
                } else {
                    g_s = ListBox1.SelectedItem.ToString();
                }
            }
            //Console.WriteLine("repeat: " + g_s);
            //Console.WriteLine("#####finish#####" + ControlChars.NewLine);

        }
        private void Timeout(int ms) {
            DateTime x = DateTime.Now.AddMilliseconds(ms);
            while (x > (DateTime.Now)) {
                if (GetAsyncKeyState(Keys.Pause) || GetAsyncKeyState(Keys.Escape)) { return; }
                Application.DoEvents();
            }
        }
        private void SleepMinutes(int m) {
            System.DateTime x = DateTime.Now.AddMinutes(m);
            Application.DoEvents();
            while (x > DateTime.Now)
            {
                if (GetAsyncKeyState(Keys.Pause) || GetAsyncKeyState(Keys.Escape)) { break; }
                System.Threading.Thread.Sleep(m);
            }
            Application.DoEvents();
        }
        private void LeftClick() {
            Mouse_event(2, 0, 0, 0, 0);
            Mouse_event(4, 0, 0, 0, 0);
        }
        private void LeftHold() { Mouse_event(2, 0, 0, 0, 0); }
        private void LeftRelease() { Mouse_event(4, 0, 0, 0, 0); }
        private void MiddleClick() {
            Mouse_event(20, 0, 0, 0, 0);
            Mouse_event(40, 0, 0, 0, 0);
        }
        private void MiddleHold() { Mouse_event(20, 0, 0, 0, 0); }
        private void MiddleRelease() { Mouse_event(40, 0, 0, 0, 0); }
        private void RightClick() {
            Mouse_event(8, 0, 0, 0, 0);
            Mouse_event(16, 0, 0, 0, 0);
        }
        private void RightHold() { Mouse_event(8, 0, 0, 0, 0); }
        private void RightRelease() { Mouse_event(16, 0, 0, 0, 0); }
        private void TextMock() {
            if (textBox2.TextLength > 20) { textBox2.Clear(); return; }
            if (textBox2.Text.StartsWith(p_)) { return; }
            if (textBox2.TextLength > Properties.Settings.Default.SettingCodeLength) { textBox2.Text = textBox2.Text.Substring(textBox2.TextLength - Properties.Settings.Default.SettingCodeLength); };
        }
        private void ClearAllKeys() {
            GetAsyncKeyState(Keys.D0);
            GetAsyncKeyState(Keys.D1);
            GetAsyncKeyState(Keys.D2);
            GetAsyncKeyState(Keys.D3);
            GetAsyncKeyState(Keys.D4);
            GetAsyncKeyState(Keys.D5);
            GetAsyncKeyState(Keys.D6);
            GetAsyncKeyState(Keys.D7);
            GetAsyncKeyState(Keys.D8);
            GetAsyncKeyState(Keys.D9);

            GetAsyncKeyState(Keys.A);
            GetAsyncKeyState(Keys.B);
            GetAsyncKeyState(Keys.C);
            GetAsyncKeyState(Keys.D);
            GetAsyncKeyState(Keys.E);
            GetAsyncKeyState(Keys.F);
            GetAsyncKeyState(Keys.G);
            GetAsyncKeyState(Keys.H);
            GetAsyncKeyState(Keys.I);
            GetAsyncKeyState(Keys.J);
            GetAsyncKeyState(Keys.K);
            GetAsyncKeyState(Keys.L);
            GetAsyncKeyState(Keys.M);
            GetAsyncKeyState(Keys.N);
            GetAsyncKeyState(Keys.O);
            GetAsyncKeyState(Keys.P);
            GetAsyncKeyState(Keys.Q);
            GetAsyncKeyState(Keys.R);
            GetAsyncKeyState(Keys.S);
            GetAsyncKeyState(Keys.T);
            GetAsyncKeyState(Keys.U);
            GetAsyncKeyState(Keys.V);
            GetAsyncKeyState(Keys.W);
            GetAsyncKeyState(Keys.X);
            GetAsyncKeyState(Keys.Y);
            GetAsyncKeyState(Keys.Z);

            GetAsyncKeyState(g_specialKey);
            GetAsyncKeyState(Keys.Insert);
            GetAsyncKeyState(Keys.Space);
        }
        private void Kb(string c) {
            switch (c) {
                case "0":
                    Key(Keys.D0, false, 1);
                    break;
                case "9":
                    Key(Keys.D9, false, 1);
                    break;
                case "8":
                    Key(Keys.D8, false, 1);
                    break;
                case "7":
                    Key(Keys.D7, false, 1);
                    break;
                case "6":
                    Key(Keys.D6, false, 1);
                    break;
                case "5":
                    Key(Keys.D5, false, 1);
                    break;
                case "4":
                    Key(Keys.D4, false, 1);
                    break;
                case "3":
                    Key(Keys.D3, false, 1);
                    break;
                case "2":
                    Key(Keys.D2, false, 1);
                    break;
                case "1":
                    Key(Keys.D1, false, 1);
                    break;

                case "a":
                    Key(Keys.A, false, 1);
                    break;
                case "b":
                    Key(Keys.B, false, 1);
                    break;
                case "c":
                    Key(Keys.C, false, 1);
                    break;
                case "d":
                    Key(Keys.D, false, 1);
                    break;
                case "e":
                    Key(Keys.E, false, 1);
                    break;
                case "f":
                    Key(Keys.F, false, 1);
                    break;
                case "g":
                    Key(Keys.G, false, 1);
                    break;
                case "h":
                    Key(Keys.H, false, 1);
                    break;
                case "i":
                    Key(Keys.I, false, 1);
                    break;
                case "j":
                    Key(Keys.J, false, 1);
                    break;
                case "k":
                    Key(Keys.K, false, 1);
                    break;
                case "l":
                    Key(Keys.L, false, 1);
                    break;
                case "m":
                    Key(Keys.M, false, 1);
                    break;
                case "n":
                    Key(Keys.N, false, 1);
                    break;
                case "o":
                    Key(Keys.O, false, 1);
                    break;
                case "p":
                    Key(Keys.P, false, 1);
                    break;
                case "q":
                    Key(Keys.Q, false, 1);
                    break;
                case "r":
                    Key(Keys.R, false, 1);
                    break;
                case "s":
                    Key(Keys.S, false, 1);
                    break;
                case "t":
                    Key(Keys.T, false, 1);
                    break;
                case "u":
                    Key(Keys.U, false, 1);
                    break;
                case "v":
                    Key(Keys.V, false, 1);
                    break;
                case "w":
                    Key(Keys.W, false, 1);
                    break;
                case "x":
                    Key(Keys.X, false, 1);
                    break;
                case "y":
                    Key(Keys.Y, false, 1);
                    break;
                case "z":
                    Key(Keys.Z, false, 1);
                    break;

                case "A":
                    Key(Keys.A, true, 1);
                    break;
                case "B":
                    Key(Keys.B, true, 1);
                    break;
                case "C":
                    Key(Keys.C, true, 1);
                    break;
                case "D":
                    Key(Keys.D, true, 1);
                    break;
                case "E":
                    Key(Keys.E, true, 1);
                    break;
                case "F":
                    Key(Keys.F, true, 1);
                    break;
                case "G":
                    Key(Keys.G, true, 1);
                    break;
                case "H":
                    Key(Keys.H, true, 1);
                    break;
                case "I":
                    Key(Keys.I, true, 1);
                    break;
                case "J":
                    Key(Keys.J, true, 1);
                    break;
                case "K":
                    Key(Keys.K, true, 1);
                    break;
                case "L":
                    Key(Keys.L, true, 1);
                    break;
                case "M":
                    Key(Keys.M, true, 1);
                    break;
                case "N":
                    Key(Keys.N, true, 1);
                    break;
                case "O":
                    Key(Keys.O, true, 1);
                    break;
                case "P":
                    Key(Keys.P, true, 1);
                    break;
                case "Q":
                    Key(Keys.Q, true, 1);
                    break;
                case "R":
                    Key(Keys.R, true, 1);
                    break;
                case "S":
                    Key(Keys.S, true, 1);
                    break;
                case "T":
                    Key(Keys.T, true, 1);
                    break;
                case "U":
                    Key(Keys.U, true, 1);
                    break;
                case "V":
                    Key(Keys.V, true, 1);
                    break;
                case "W":
                    Key(Keys.W, true, 1);
                    break;
                case "X":
                    Key(Keys.X, true, 1);
                    break;
                case "Y":
                    Key(Keys.Y, true, 1);
                    break;
                case "Z":
                    Key(Keys.Z, true, 1);
                    break;

                case "\n": //vbLf:
                    if (g_ignoreWhiteSpace == false) { Key(Keys.Enter, false, 1); }
                    break;
                case "\t"://vbTab:
                    if (g_ignoreWhiteSpace == false) { Key(Keys.Tab, false, 1); }
                    break;
                case " "://space
                    if (g_ignoreWhiteSpace == false) { Key(Keys.Space, false, 1); }
                    break;

                case "?":
                    Key(Keys.OemQuestion, true, 1);
                    break;
                case "/":
                    Key(Keys.OemQuestion, false, 1);
                    break;
                case "~":
                    Key(Keys.Oem3, true, 1);
                    break;
                case "`":
                    Key(Keys.Oem3, false, 1);
                    break;
                case ")":
                    Key(Keys.D0, true, 1);
                    break;
                case "(":
                    Key(Keys.D9, true, 1);
                    break;
                case "*":
                    Key(Keys.D8, true, 1);
                    break;
                case "&":
                    Key(Keys.D7, true, 1);
                    break;
                case "^":
                    Key(Keys.D6, true, 1);
                    break;
                case "%":
                    Key(Keys.D5, true, 1);
                    break;
                case "$":
                    Key(Keys.D4, true, 1);
                    break;
                case "#":
                    Key(Keys.D3, true, 1);
                    break;
                case "@":
                    Key(Keys.D2, true, 1);
                    break;
                case "!":
                    Key(Keys.D1, true, 1);
                    break;

                case "_":
                    Key(Keys.OemMinus, true, 1);
                    break;
                case "-":
                    Key(Keys.OemMinus, false, 1);
                    break;
                case "+":
                    Key(Keys.Oemplus, true, 1);
                    break;
                case "=":
                    Key(Keys.Oemplus, false, 1);
                    break;
                case "{":
                    Key(Keys.OemOpenBrackets, true, 1);
                    break;
                case "[":
                    Key(Keys.OemOpenBrackets, false, 1);
                    break;
                case "}":
                    Key(Keys.OemCloseBrackets, true, 1);
                    break;
                case "]":
                    Key(Keys.OemCloseBrackets, false, 1);
                    break;
                case "|":
                    Key(Keys.OemBackslash, true, 1);
                    break;
                case "\\":
                    Key(Keys.OemBackslash, false, 1);
                    break;
                case ":":
                    Key(Keys.OemSemicolon, true, 1);
                    break;
                case ";":
                    Key(Keys.OemSemicolon, false, 1);
                    break;
                case "\"":
                    Key(Keys.OemQuotes, true, 1);
                    break;
                case "'":
                    Key(Keys.OemQuotes, false, 1);
                    break;
                case "<":
                    Key(Keys.Oemcomma, true, 1);
                    break;
                case ",":
                    Key(Keys.Oemcomma, false, 1);
                    break;
                case ">":
                    Key(Keys.OemPeriod, true, 1);
                    break;
                case ".":
                    Key(Keys.OemPeriod, false, 1);
                    break;
                case "‹"://ws_:
                    g_ignoreWhiteSpace = true;
                    break;
                case "›"://_ws:
                    g_ignoreWhiteSpace = false;
                    break;


                case "«"://p_
                    //Console.WriteLine("indexof «:" + g_s.IndexOf(p_) + 1);
                    //Console.WriteLine("indexof »:" + g_s.IndexOf(_p) +- 2); ///

                    string middle = g_s.Substring(g_s.IndexOf(p_) + 1, g_s.IndexOf(_p) + 1 - g_s.IndexOf(p_) - 2);//grab middle value
                    //Console.WriteLine("middle: " + middle);

                    if (middle.Contains(":")) {//grab x:#
                        g_n = middle.Substring(middle.IndexOf(":") + 1);
                        middle = middle.Substring(middle.IndexOf(p_) + 1, middle.IndexOf(":"));
                    }
                    if (middle.Contains("*")) {//grab x*#
                        g_presses = Convert.ToInt32(middle.Substring(middle.IndexOf("*") + 1));
                        middle = middle.Substring(middle.IndexOf(p_) + 1, middle.IndexOf("*"));
                        //Console.WriteLine("middle: " + middle);
                        //Console.WriteLine("g_presses: " + g_presses);
                    }

                    g_s = g_s.Substring(g_s.IndexOf(_p) + 1);//make new string
                    //Console.WriteLine("new string: " + g_s.IndexOf(_p) + 1);


                    switch (middle)
                    {
                        case "x":
                            SetCursorPos(Convert.ToInt32(g_n) + Cursor.Position.X, Cursor.Position.Y);
                            break;
                        case "y":
                            SetCursorPos(Cursor.Position.X, Convert.ToInt32(g_n) + Cursor.Position.Y);
                            break;
                        case "date":
                            string d = (DateTime.Now.Month.ToString() + "/" + DateTime.Now.Day.ToString() + "/" + DateTime.Now.Year.ToString());
                            if (g_n != "0") {
                                d = (DateTime.Now.Month.ToString() + "/" + DateTime.Now.Day.ToString() + "/" + DateTime.Now.Year.ToString());
                                d = d.Replace("/", g_n);
                            }
                            if (middle == "Date") {
                                SendKeys.Send(d);
                            } else {
                                Clipboard.SetText(d);
                            }
                            break;
                        case "time":
                            string h = DateTime.Now.Hour.ToString();
                            int hh = Convert.ToInt32(h);
                            string m = "AM";
                            if (Convert.ToInt32(h) > 12) { m = "PM"; hh -= 12; }
                            string t = hh + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString() + ":" + m;
                            if (g_n != "0") { t = t.Replace(":", g_n); }
                            Clipboard.SetText(t);
                            if (middle == "Time") {
                                SendKeys.Send(t);
                            } else {
                                Clipboard.SetText(t);
                            }
                            break;
                        case "to":
                            Timeout(Convert.ToInt32(g_n));
                            break;
                        case "replace":
                            Clipboard.SetText(Clipboard.GetText().Replace(g_n.Substring(0, g_n.IndexOf("|")), g_n.Substring(g_n.IndexOf("|") + 1)));
                            break;

                        case "yesno":
                            DialogResult = MessageBox.Show(g_n, "Verify", MessageBoxButtons.YesNo);
                            if (DialogResult == DialogResult.Yes) {; } else { g_s = ""; return; }
                            break;
                        case "audio":
                            System.Media.SoundPlayer player = new System.Media.SoundPlayer(g_n);
                            player.Play();
                            break;
                        case "stop-audio":
                            System.Media.SoundPlayer player1 = new System.Media.SoundPlayer(g_n);
                            player1.Stop();
                            break;
                        case ""://«:»
                            SendKeys.Send(g_n);
                            break;
                        case "<<":
                            SendKeys.Send(p_);//print open bracket
                            break;
                        case ">>":
                            SendKeys.Send(_p);
                            break;
                        case "iw":
                            g_ignoreWhiteSpace = true;
                            break;
                        case "-iw":
                            g_ignoreWhiteSpace = false;
                            break;
                        case "cb":
                            Clipboard.SetText(g_n);
                            break;
                        case "minute":
                            SleepMinutes(Convert.ToInt32(g_n));
                            break;
                        case "sleep":
                            SleepMS(Convert.ToInt32(g_n));
                            break;
                        case "Sleep":
                            Sleep(Convert.ToInt32(g_n));
                            break;
                        case ",":
                            if (g_n != "0") {
                                if (g_n != "") { Sleep(Convert.ToInt32(g_n)); }
                            } else {
                                Sleep(77);
                            }
                            break;
                        case "App":
                            Interaction.AppActivate(g_n);
                            break;
                        case "app":
                            Sleep(1);
                            int x = 0;
                            App:
                            try {
                                x += 1;
                                Interaction.AppActivate(g_n);
                            }
                            catch (Exception) {
                                if (x == 200 || GetAsyncKeyState(Keys.Escape)) {
                                    MessageBox.Show(p_ + "app:" + g_n + _p + " " + " not found", "🕓", MessageBoxButtons.OK);
                                    g_kb_i = -1;
                                    g_s = "";
                                    return;
                                }
                                Sleep(77);
                                goto App;
                                throw;
                            }
                            break;

                        case "win":
                            KeyHold(Keys.LWin);
                            break;
                        case "-win":
                            KeyRelease(Keys.LWin);
                            break;
                        case "shift":
                            KeyHold(Keys.LShiftKey);
                            break;
                        case "-shift":
                            KeyRelease(Keys.LShiftKey);
                            Keybd_event(Keys.RShiftKey, 0, 2, 0);
                            break;
                        case "alt":
                            Keybd_event(Keys.LMenu, 0, 0, 0);
                            break;
                        case "-alt":
                            Keybd_event(Keys.LMenu, 0, 2, 0);
                            break;
                        case "ctrl":
                            Keybd_event(Keys.LControlKey, 0, 0, 0);
                            break;
                        case "-ctrl":
                            KeyRelease(Keys.LControlKey);
                            Keybd_event(Keys.LControlKey, 0, 2, 0);
                            break;
                        case "up":
                            Key(Keys.Up, false, Convert.ToInt32(g_presses));
                            break;
                        case "right":
                            Key(Keys.Right, false, Convert.ToInt32(g_presses));
                            break;
                        case "down":
                            Key(Keys.Down, false, Convert.ToInt32(g_presses));
                            break;
                        case "left":
                            Key(Keys.Left, false, Convert.ToInt32(g_presses));
                            break;
                        case "tab":
                            Key(Keys.Tab, false, Convert.ToInt32(g_presses));
                            break;
                        case "space":
                            Key(Keys.Space, false, Convert.ToInt32(g_presses));
                            break;
                        case "menu":
                            Key(Keys.Apps, false, 1);
                            break;
                        case "enter":
                            Key(Keys.Enter, false, Convert.ToInt32(g_presses));
                            break;
                        case "backspace":
                        case "bs":
                            Key(Keys.Back, false, Convert.ToInt32(g_presses));
                            break;
                        case "escape":
                        case "esc":
                            Key(Keys.Escape, false, 1);
                            break;
                        case "home":
                            Key(Keys.Home, false, Convert.ToInt32(g_presses));
                            break;
                        case "end":
                            Key(Keys.End, false, Convert.ToInt32(g_presses));
                            break;
                        case "pu":
                            Key(Keys.PageUp, false, Convert.ToInt32(g_presses));
                            break;
                        case "pd":
                            Key(Keys.PageDown, false, Convert.ToInt32(g_presses));
                            break;
                        case "insert":
                            Key(Keys.Insert, false, Convert.ToInt32(g_presses));
                            break;
                        case "delete":
                            Key(Keys.Delete, false, Convert.ToInt32(g_presses));
                            break;

                        case "f1":
                            Key(Keys.F1, false, Convert.ToInt32(g_presses));
                            break;
                        case "f2":
                            Key(Keys.F2, false, Convert.ToInt32(g_presses));
                            break;
                        case "f3":
                            Key(Keys.F3, false, Convert.ToInt32(g_presses));
                            break;
                        case "f4":
                            Key(Keys.F4, false, Convert.ToInt32(g_presses));
                            break;
                        case "f5":
                            Key(Keys.F5, false, Convert.ToInt32(g_presses));
                            break;
                        case "f6":
                            Key(Keys.F6, false, Convert.ToInt32(g_presses));
                            break;
                        case "f7":
                            Key(Keys.F7, false, Convert.ToInt32(g_presses));
                            break;
                        case "f8":
                            Key(Keys.F8, false, Convert.ToInt32(g_presses));
                            break;
                        case "f9":
                            Key(Keys.F9, false, Convert.ToInt32(g_presses));
                            break;
                        case "f10":
                            Key(Keys.F10, false, Convert.ToInt32(g_presses));
                            break;
                        case "f11":
                            Key(Keys.F11, false, Convert.ToInt32(g_presses));
                            break;
                        case "f12":
                            Key(Keys.F12, false, Convert.ToInt32(g_presses));
                            break;

                        case "break":
                        case "pause":
                            Key(Keys.Pause, false, Convert.ToInt32(g_presses));
                            break;
                        case "printscreen":
                        case "ps":
                            Key(Keys.PrintScreen, false, Convert.ToInt32(g_presses));
                            break;
                        case "vu":
                            Key(Keys.VolumeUp, false, Convert.ToInt32(g_presses));
                            break;
                        case "vd":
                            Key(Keys.VolumeDown, false, Convert.ToInt32(g_presses));
                            break;
                        case "vm":
                            Key(Keys.VolumeMute, false, 1);
                            break;

                        case "caps":
                            Key(Keys.CapsLock, false, Convert.ToInt32(g_presses));
                            break;

                        case "nl":
                            Key(Keys.NumLock, false, Convert.ToInt32(g_presses));
                            break;
                        case "sl":
                            Key(Keys.Scroll, false, Convert.ToInt32(g_presses));
                            break;

                        case "MediaStop":
                            Key(Keys.MediaStop, false, 1);
                            break;
                        case "MediaPlayPause":
                            Key(Keys.MediaPlayPause, false, 1);
                            break;
                        case "MediaNextTrack":
                            Key(Keys.MediaNextTrack, false, 1);
                            break;
                        case "MediaPreviousTrack":
                            Key(Keys.MediaPreviousTrack, false, 1);
                            break;
                        case "SelectMedia":
                            Key(Keys.SelectMedia, false, 1);
                            break;

                        case "xy":
                            SetCursorPos(Convert.ToInt32(g_n.Substring(0, g_n.IndexOf("-"))), Convert.ToInt32(g_n.Substring(g_n.IndexOf("-") + 1)));
                            break;
                        case "rp": //return pointer
                        case "rm":
                            SetCursorPos(g_x, g_y);
                            break;
                        case "lc":
                            LeftClick();
                            break;
                        case "lh":
                            LeftHold();
                            break;
                        case "lr":
                            LeftRelease();
                            break;
                        case "mc":
                            MiddleClick();
                            break;
                        case "mh":
                            MiddleHold();
                            break;
                        case "mr":
                            MiddleRelease();
                            break;
                        case "rc":
                            RightClick();
                            break;
                        case "rh":
                            RightHold();
                            break;
                        case "rr":
                            RightRelease();
                            break;

                        case "lb":
                            Key(Keys.LButton, false, Convert.ToInt32(g_presses));
                            break;
                        case "rb":
                            Key(Keys.RButton, false, Convert.ToInt32(g_presses));
                            break;
                        case "mb":
                            Key(Keys.MButton, false, Convert.ToInt32(g_presses));
                            break;


                        case "n0":
                            Key(Keys.NumPad0, false, 1);
                            break;
                        case "n1":
                            Key(Keys.NumPad2, false, 1);
                            break;
                        case "n2":
                            Key(Keys.NumPad2, false, 1);
                            break;
                        case "n3":
                            Key(Keys.NumPad3, false, 1);
                            break;
                        case "n4":
                            Key(Keys.NumPad4, false, 1);
                            break;
                        case "n5":
                            Key(Keys.NumPad5, false, 1);
                            break;
                        case "n6":
                            Key(Keys.NumPad6, false, 1);
                            break;
                        case "n7":
                            Key(Keys.NumPad7, false, 1);
                            break;
                        case "n8":
                            Key(Keys.NumPad8, false, 1);
                            break;
                        case "n9":
                            Key(Keys.NumPad9, false, 1);
                            break;

                        default://connect
                            if (middle.StartsWith("'")) { break; }
                            for (int i = 0; i < ar.Count; i++) {
                                if (GetAsyncKeyState(Keys.Escape)) { break; }
                                if (ar[i].ToString().Substring(ar[i].ToString().IndexOf(":") + 1).Contains(middle)) {
                                    g_i = Convert.ToInt32(ar[i].ToString().Substring(0, ar[i].ToString().IndexOf(":")));
                                    g_s = ListBox1.Items[g_i].ToString().Substring(ListBox1.Items[g_i].ToString().IndexOf(_p) + 1, ListBox1.Items[g_i].ToString().Length - ListBox1.Items[g_i].ToString().IndexOf(_p) - 1) + g_s;
                                    //Console.WriteLine("connect: " + ar[i].ToString().Substring(ar[i].ToString().IndexOf(":") + 1));
                                    if (Properties.Settings.Default.SettingInfiniteLoop == false) {
                                        if (g_s.Contains(p_ + g_code + _p) || g_s.Contains(middle) && ar[i].ToString().Substring(ar[i].ToString().IndexOf(":") + 1) != middle || g_code == middle && g_s.Length == 0) {
                                            MessageBox.Show("Infinite loop\n" + p_ + g_code + _p + " >" + g_s, "🕓", MessageBoxButtons.OK);
                                            g_kb_i = -1;
                                            g_s = "";
                                            return;
                                        }
                                    }
                                    PD();
                                    g_s = "";
                                    //Console.WriteLine("get value: " + ar[i].ToString().Substring(ar[i].ToString().IndexOf(":") + 1));
                                    //Console.WriteLine("get index: " + ar[i].ToString().IndexOf(":"));
                                    break;
                                }
                            }
                            break;
                    }

                    g_kb_i = -1;// update to 0
                    g_presses = 1;
                    g_n = "0";
                    middle = "";
                    break;
                default:
                    SendKeys.Send(c);
                    break;
            }
        }
        private void Sk(int opt) {
            textBox2.Text = "'";
            ListBox1.SelectedIndex = g_i;

            //Console.WriteLine("code: " + listBox1.SelectedItem.ToString().Substring(1, listBox1.SelectedItem.ToString().IndexOf(_p) - 1));//code
            //Console.WriteLine("string: " + listBox1.SelectedItem.ToString().Substring(listBox1.SelectedItem.ToString().IndexOf(_p) + 1, listBox1.SelectedItem.ToString().Length - listBox1.SelectedItem.ToString().IndexOf(_p) - 1));
            g_s = ListBox1.SelectedItem.ToString().Substring(ListBox1.SelectedItem.ToString().IndexOf(_p) + 1, ListBox1.SelectedItem.ToString().Length - ListBox1.SelectedItem.ToString().IndexOf(_p) - 1);

            switch (opt)
            {
                case 1:
                    g_code = ListBox1.SelectedItem.ToString().Substring(1, ListBox1.SelectedItem.ToString().IndexOf(_p) - 1);
                    PD();
                    break;
                case 2:
                    g_code = ListBox1.SelectedItem.ToString().Substring(1, ListBox1.SelectedItem.ToString().IndexOf(_p) - 1);
                    Key(Keys.Back, false, (g_code.Replace(Properties.Settings.Default.SettingInsertSymbol, "").Replace("-", "").Length));//auto bs*#
                    PD();
                    break;
                case 3:
                    //Console.WriteLine(listBox1.SelectedItem.ToString().Substring(g_length) );
                    g_s = ListBox1.SelectedItem.ToString().Substring(g_length);
                    if (Properties.Settings.Default.SettingSendkeysOnlyMode)
                    {
                        SendKeys.Send(g_s);
                    }
                    else
                    {
                        PD();
                    }
                    break;

            }

            ClearAllKeys();
            textBox2.Clear();

        }

        private void TextBox1_DoubleClick(object sender, EventArgs e) {
            if (TextBox1.Text == "" && textBox2.TextLength == g_length) {
                TextBox1.AppendText(textBox2.Text);//get code
                return;
            }
            if (TextBox1.SelectedText == "") {
                SendKeys.Send(p_ + _p + "{left}");//«»
                return;
            }
        }
        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (GetAsyncKeyState(Keys.Back)) { if (textBox2.TextLength > 0) { textBox2.Text = textBox2.Text.Substring(0, textBox2.TextLength - 1); } }

            if (GetAsyncKeyState(Keys.Scroll))
            {
                if (TextBox1.ContainsFocus) { return; }
                textBox2.Text = "'";
                if (Properties.Settings.Default.SettingTitleTip == true && ControlBox == true) { Text = Properties.Settings.Default.SettingTitleText + " > " + g_s; }
                if (TextBox1.Text != "")
                {
                    if (TextBox1.SelectedText.Length > 0) { g_s = TextBox1.SelectedText; }
                    if (TextBox1.SelectedText.Length == 0) { g_s = TextBox1.Text; }
                    if (Properties.Settings.Default.SettingTitleTip == true && ControlBox == true) { Text = Properties.Settings.Default.SettingTitleText + " > " + g_s; }
                    PD();
                }
                if (TextBox1.Text == "" && ListBox1.Items.Count > 0) { PD(); }
                if (Properties.Settings.Default.SettingTitleTip == true && ControlBox == true) { Text = Properties.Settings.Default.SettingTitleText; }
                ClearAllKeys();
                textBox2.Clear();
            }

            if (TextBox1.ContainsFocus && GetAsyncKeyState(Keys.F5))
            {
                if (TextBox1.Text == "") { return; }
                textBox2.Text = "'";
                bool x = Visible;
                Visible = false;
                Sleep(1);
                if (TextBox1.SelectedText.Length > 0)
                {
                    g_s = TextBox1.SelectedText;
                    PD();
                }
                else
                {
                    if (TextBox1.TextLength > 0) { g_s = TextBox1.Text; PD(); }
                }
                Visible = x;
                ClearAllKeys();
                textBox2.Clear();
            }

            if (GetAsyncKeyState(g_specialKey)) { if (textBox2.Text.StartsWith(p_)) { textBox2.Clear(); } else { ClearAllKeys(); textBox2.Text = p_; } }

            if (Properties.Settings.Default.SettingBracketModeOnlyScan && textBox2.Text.StartsWith(p_) == false) { return; }

            if (GetAsyncKeyState(Keys.Insert)) { textBox2.AppendText(Properties.Settings.Default.SettingInsertSymbol); }


            if (GetAsyncKeyState(Keys.Z)) { textBox2.AppendText("z"); }
            if (GetAsyncKeyState(Keys.X)) { textBox2.AppendText("x"); }
            if (GetAsyncKeyState(Keys.C)) { textBox2.AppendText("c"); }
            if (GetAsyncKeyState(Keys.V)) { textBox2.AppendText("v"); }
            if (GetAsyncKeyState(Keys.B)) { textBox2.AppendText("b"); }
            if (GetAsyncKeyState(Keys.N)) { textBox2.AppendText("n"); }
            if (GetAsyncKeyState(Keys.M)) { textBox2.AppendText("m"); }

            if (GetAsyncKeyState(Keys.A)) { textBox2.AppendText("a"); }
            if (GetAsyncKeyState(Keys.S)) { textBox2.AppendText("s"); }
            if (GetAsyncKeyState(Keys.D)) { textBox2.AppendText("d"); }
            if (GetAsyncKeyState(Keys.F)) { textBox2.AppendText("f"); }
            if (GetAsyncKeyState(Keys.G)) { textBox2.AppendText("g"); }
            if (GetAsyncKeyState(Keys.H)) { textBox2.AppendText("h"); }
            if (GetAsyncKeyState(Keys.J)) { textBox2.AppendText("j"); }
            if (GetAsyncKeyState(Keys.K)) { textBox2.AppendText("k"); }
            if (GetAsyncKeyState(Keys.L)) { textBox2.AppendText("l"); }

            if (GetAsyncKeyState(Keys.Q)) { textBox2.AppendText("q"); }
            if (GetAsyncKeyState(Keys.W)) { textBox2.AppendText("w"); }
            if (GetAsyncKeyState(Keys.E)) { textBox2.AppendText("e"); }
            if (GetAsyncKeyState(Keys.R)) { textBox2.AppendText("r"); }
            if (GetAsyncKeyState(Keys.T)) { textBox2.AppendText("t"); }
            if (GetAsyncKeyState(Keys.Y)) { textBox2.AppendText("y"); }
            if (GetAsyncKeyState(Keys.U)) { textBox2.AppendText("u"); }
            if (GetAsyncKeyState(Keys.I)) { textBox2.AppendText("i"); }
            if (GetAsyncKeyState(Keys.O)) { textBox2.AppendText("o"); }
            if (GetAsyncKeyState(Keys.P)) { textBox2.AppendText("p"); }

            if (GetAsyncKeyState(Keys.D1)) { textBox2.AppendText("1"); }
            if (GetAsyncKeyState(Keys.D2)) { textBox2.AppendText("2"); }
            if (GetAsyncKeyState(Keys.D3)) { textBox2.AppendText("3"); }
            if (GetAsyncKeyState(Keys.D4)) { textBox2.AppendText("4"); }
            if (GetAsyncKeyState(Keys.D5)) { textBox2.AppendText("5"); }
            if (GetAsyncKeyState(Keys.D6)) { textBox2.AppendText("6"); }
            if (GetAsyncKeyState(Keys.D7)) { textBox2.AppendText("7"); }
            if (GetAsyncKeyState(Keys.D8)) { textBox2.AppendText("8"); }
            if (GetAsyncKeyState(Keys.D9)) { textBox2.AppendText("9"); }
            if (GetAsyncKeyState(Keys.D0)) { textBox2.AppendText("0"); }

            if (GetAsyncKeyState(Keys.Space)) { textBox2.AppendText(" "); }

            if (GetAsyncKeyState(Keys.Escape) && GetAsyncKeyState(Keys.X)) {
                Clipboard.SetText(p_ + "xy:" + MousePosition.X + "-" + MousePosition.Y + _p);
            }

            if (GetAsyncKeyState(Keys.Escape) && GetAsyncKeyState(Keys.H))
            {
                KeyRelease(Keys.H);
                KeyRelease(Keys.Escape);
                if (!Visible)
                {
                    Show();
                    if (Text != "") { Interaction.AppActivate(Properties.Settings.Default.SettingTitleText); }
                    return;
                }
                if (Visible)
                {
                    Hide();
                    return;
                }
            }//toggle visibility 

        }
        private void TextBox2_TextChanged(object sender, EventArgs e) {
            if (textBox2.Text == "'") { return; }
            if (Properties.Settings.Default.SettingTitleTip == true && ControlBox == true) { Text = Properties.Settings.Default.SettingTitleText + " > " + textBox2.Text; }

            if (((textBox2.TextLength == g_length) || (textBox2.Text.StartsWith(p_) && textBox2.TextLength >= 1)) == false) {
                TextMock();
                return;
            }

            for (int i = 0; i < ListBox1.Items.Count; i++)
            {
                if (GetAsyncKeyState(Keys.Escape)) { break; }
                if (textBox2.Text == p_) { return; }
                if (ListBox1.Items[i].ToString() == "" || ListBox1.Items[i].ToString().StartsWith("'")) { continue; }//rem

                g_i = i;
                if (textBox2.Text.StartsWith(p_)) {//«x»|«x-»
                    if (ListBox1.Items[i].ToString().StartsWith(textBox2.Text + _p)) {//«x»
                        Sk(1);
                        break;
                    }
                    if (ListBox1.Items[i].ToString().StartsWith(textBox2.Text + "-" + _p)) {//«x-»
                        Sk(2);
                        break;
                    }
                    continue;
                }

                //Console.WriteLine(textBox2.Text.Substring(0, g_length));
                if (ListBox1.Items[i].ToString().Length > g_length) {
                    if (textBox2.Text.Substring(0, g_length) == ListBox1.Items[i].ToString().Substring(0, g_length))
                    {
                        Sk(3);
                        break;
                    }//x
                }
            }

            TextMock();
        }
        private void TextBox1_KeyPress(object sender, KeyPressEventArgs e) {
            if (GetAsyncKeyState(Keys.LControlKey) && GetAsyncKeyState(Keys.X)) { GetAsyncKeyState(Keys.X); }//clear
            if (GetAsyncKeyState(Keys.LControlKey) && GetAsyncKeyState(Keys.C)) { GetAsyncKeyState(Keys.C); }

            if (GetAsyncKeyState(Keys.LControlKey) && GetAsyncKeyState(Keys.A) && TextBox1.TextLength > 1) {
                TextBox1.SelectionStart = 0; TextBox1.SelectionLength = TextBox1.TextLength;
            }
            if (e.KeyChar == 19) { AddDbItm(); }//ctrl+s
            if (e.KeyChar == 21) { UpdateDbItm(); }//ctrl+u
            if (e.KeyChar == 5) { TextBox1.Undo(); EditDbItm(); }//ctrl+e
            if (e.KeyChar == 22) {
                GetAsyncKeyState(Keys.V);
                if (Clipboard.GetText() != "") { Clipboard.SetText(Clipboard.GetText().ToString()); }//double paste, raw txt
                TextBox1.Undo();
                TextBox1.Paste();
                CleanMock();
            }//ctrl+v

            if (GetAsyncKeyState(Keys.Tab)) {
                if (Properties.Settings.Default.SettingSendkeysOnlyMode) { return; }
                KeyRelease(Keys.Tab);

                if (TextBox1.SelectionStart == 0 || TextBox1.SelectionStart == TextBox1.TextLength) {
                    Key(Keys.Back, false, 1);
                    SendKeys.Send(p_ + _p + "{left}");
                    return;
                }

                string x;
                if (TextBox1.SelectionStart > 2) {
                    x = (TextBox1.Text.Substring(TextBox1.SelectionStart - 2, 2));
                    //Console.WriteLine("2: " + x);
                    
                    switch (x) {
                        case "ap":
                            AutoComplete("p:", "", 0);
                            return;
                        case "au":
                        case "Au":
                            AutoComplete("dio:", "", 0);
                            return;
                        case "da":
                            AutoComplete("te", "", 1);
                            return;
                        case "de":
                            AutoComplete("lete*", "", 0);
                            return;
                        case "en":
                            AutoComplete("d", "", 1);
                            return;
                        case "es":
                            AutoComplete("c", "", 1);
                            return;
                        case "iw":
                            AutoComplete("", "-iw", 1);//ignore whitespace 
                            return;
                        case "me":
                            AutoComplete("nu", "", 1);
                            return;
                        case "mi":
                            AutoComplete("nute:", "", 0);
                            return;
                        case "re":
                            AutoComplete("place:|", "", 0);
                            Key(Keys.Left, false, 1);
                            return;
                        case "se"://exe settings
                            g_s = p_ + "win" + _p + "r" + p_ + "-win" + _p + p_ + "app:run" + _p + Application.LocalUserAppDataPath.ToString().Replace("\\DZ\\" + Application.ProductVersion, "") + p_ + "enter" + _p;
                            PD();
                            Interaction.AppActivate(Properties.Settings.Default.SettingTitleText);
                            Key(Keys.Right, false, 1);
                            Key(Keys.Back, false, 5);
                            textBox2.Clear();
                            return;
                        case "sl":
                            AutoComplete("eep:", "", 0);
                            return;
                        case "sp":
                            AutoComplete("ace*", "", 0);
                            return;
                        case "st":
                            AutoComplete("op-audio", "", 1);
                            return;
                        case "ti":
                            AutoComplete("me", "", 1);
                            return;
                        case "to"://timeout
                            AutoComplete(":", "", 0);
                            return;
                        case "wr"://run
                            textBox2.Clear();
                            Key(Keys.Right, false, 1);
                            Key(Keys.Back, false, 5);
                            SendKeys.Send(p_ + "win" + _p + "r" + p_ + "-win" + _p + p_ + "app:run" + _p + p_ + "enter" + _p);
                            Key(Keys.Left, false, 7);
                            return;
                        case "ws"://ignore whitespace
                            Key(Keys.Right, false, 1);
                            Key(Keys.Back, false, 5);
                            SendKeys.Send(ws_ + _ws + "{left}");
                            return;
                        case "xy":///
                            for (int i = 3; i > 1; i--) {
                                Text = Properties.Settings.Default.SettingTitleText + " > " + p_ + "xy:" + i.ToString() + _p;
                                Sleep(1000);
                            }
                            Key(Keys.Back, false, 1);
                            textBox2.Clear();
                            g_s = (":" + MousePosition.X + "-" + MousePosition.Y);
                            PD();
                            Key(Keys.Right, false, 1);
                            return;
                        case "ye":
                            AutoComplete("sno:", "", 0);
                            return;
                    }

                }// >2

                if (TextBox1.SelectionStart > 1)
                {
                    x = (TextBox1.Text.Substring(TextBox1.SelectionStart - 1, 1));
                    //Console.WriteLine("1: " + x);
                    switch (x)
                    {
                        case "a":
                            AutoComplete("lt", "-alt", 1);
                            return;
                        case "b":
                            AutoComplete("s*", "", 0);
                            return;
                        case "c":
                            AutoComplete("trl", "-ctrl", 1);
                            return;
                        case "d":
                            AutoComplete("own*", "", 0);
                            return;
                        case "e":
                            AutoComplete("nter*", "", 0);
                            return;
                        case "h":
                            AutoComplete("ome", "", 1);
                            return;
                        case "u":
                            AutoComplete("p*", "", 0);
                            return;
                        case "i":
                            AutoComplete("nsert", "", 1);
                            return;
                        case "l":
                            AutoComplete("eft*", "", 0);
                            return;
                        case "m":
                            AutoComplete("enu", "", 1);
                            return;
                        case "r":
                            AutoComplete("ight*", "", 0);
                            return;
                        case "s":
                            AutoComplete("hift", "-shift", 1);
                            return;
                        case "t":
                            AutoComplete("ab*", "", 0);
                            return;
                        case "w":
                            AutoComplete("in", "-win", 1);
                            return;
                        case ",":
                        case "y":
                        case "x":
                            AutoComplete(":", "", 0);
                            return;

                        case "«"://p_:
                            Key(Keys.Right, false, 1);
                            Key(Keys.Back, false, 3);
                            Key(Keys.Tab, false, 1);
                            return;
                        case "»"://_p:
                        case "\t"://ControlChars.Tab: //Chr(9):
                            Key(Keys.Back, false, 1);
                            SendKeys.Send(p_ + _p + "{left}");
                            return;

                        case "*":
                            Key(Keys.Back, false, 2);
                            Key(Keys.Right, true, 1);
                            return;
                        case "9":
                        case "8":
                        case "7":
                        case "6":
                        case "5":
                        case "4":
                        case "3":
                        case "2":
                        case "1":
                        case "0":
                            Key(Keys.Back, false, 1);
                            Key(Keys.Right, false, 1);
                            return;
                    }

                }// >1

                Key(Keys.Back, false, 1);
                SendKeys.Send(p_ + _p + "{left}");
            }//tab
        }
        private void CleanSelect() {
            Sleep(333);
            ListBox1.SelectedIndex = ListBox1.SelectedIndex;
            CleanMock();
        }
        private void DragFormInit() {
            g_drag = true;
            g_drag_x = Cursor.Position.X - Left;
            g_drag_y = Cursor.Position.Y - Top;
        }
        private void DragForm() {
            if (g_drag) {
                Top = Cursor.Position.Y - g_drag_y;
                Left = Cursor.Position.X - g_drag_x;
            }
        }

        private void DZ_Load(object sender, EventArgs e) {
            if (Properties.Settings.Default.SettingFirstLoad == 0) { Application.Restart(); }
            if (Properties.Settings.Default.SettingIcon != "") { Icon = new Icon(Properties.Settings.Default.SettingIcon); }
            TextBox1.WordWrap = Properties.Settings.Default.SettingWordWrap;

            LoadDb();
            DarkMode();

            Timer1.Interval = Properties.Settings.Default.SettingFrequency;

            Opacity = Properties.Settings.Default.SettingOpacity;
            TopMost = Properties.Settings.Default.SettingTopMost;
            Top = Properties.Settings.Default.SettingLocationTop;
            Left = Properties.Settings.Default.SettingLocationLeft;
            Height = Properties.Settings.Default.SettingHeight;
            Width = Properties.Settings.Default.SettingWidth;

            if (ListBox1.Items.Count > 0) { ListBox1.SelectedIndex = Properties.Settings.Default.SettingListboxSelectedIndex; }
            ListBox1.Font = new System.Drawing.Font(TextBox1.Font.Name, Properties.Settings.Default.SettingListBoxFontSize);

            switch (Properties.Settings.Default.SettingTabIndex) {
                case 1:
                    ListBox1.Focus(); break;
                case 3:
                    TextBox1.Focus(); break;
                default:
                    ListBox1.Focus(); break;
            }

            TextBox1.Text = Properties.Settings.Default.SettingTextBox;
            if (TextBox1.Focused) {
                TextBox1.SelectionStart = Properties.Settings.Default.SettingSelectionStart;
                TextBox1.SelectionLength = Properties.Settings.Default.SettingSelectionLength;
            }
            TextBox1.ZoomFactor = Properties.Settings.Default.SettingTextBoxZoomFactor;
            SplitContainer1.SplitterDistance = Properties.Settings.Default.SettingSplitterDistance;
            SplitContainer1.SplitterWidth = Properties.Settings.Default.SettingSplitterWidth;

            if (Properties.Settings.Default.SettingBackgroundImage != "") {
                BackgroundImage = Image.FromFile(Properties.Settings.Default.SettingBackgroundImage);
                FixedSize();
            }
        }
        private void DZ_FormClosing(object sender, FormClosingEventArgs e) {
            Timer1.Dispose();
            if (Top != -32000) { Properties.Settings.Default.SettingLocationTop = Top; }
            if (Left != -32000) { Properties.Settings.Default.SettingLocationLeft = Left; }
            if (WindowState == 0 && ControlBox == true) {
                Properties.Settings.Default.SettingHeight = Height;
                Properties.Settings.Default.SettingWidth = Width;
            }
            if (ListBox1.Focused) { Properties.Settings.Default.SettingTabIndex = 1; }
            if (TextBox1.Focused) { Properties.Settings.Default.SettingTabIndex = 5; }
            Properties.Settings.Default.SettingSelectionStart = TextBox1.SelectionStart;
            Properties.Settings.Default.SettingSelectionLength = TextBox1.SelectionLength;
            Properties.Settings.Default.SettingTextBoxZoomFactor = TextBox1.ZoomFactor;
            Properties.Settings.Default.SettingTextBox = TextBox1.Text;
            Properties.Settings.Default.SettingListboxSelectedIndex = ListBox1.SelectedIndex;
            Properties.Settings.Default.SettingListBoxFontSize = ListBox1.Font.Size;
            Properties.Settings.Default.SettingSplitterDistance = SplitContainer1.SplitterDistance;

            //config
            if (Properties.Settings.Default.SettingFirstLoad == 0) {
                Properties.Settings.Default.SettingIcon = Properties.Settings.Default.SettingIcon;
                Properties.Settings.Default.SettingTitleText = Properties.Settings.Default.SettingTitleText;
                Properties.Settings.Default.SettingWordWrap = Properties.Settings.Default.SettingWordWrap;
                Properties.Settings.Default.SettingCodeLength = Properties.Settings.Default.SettingCodeLength;
                Properties.Settings.Default.SettingSpecialKey = Properties.Settings.Default.SettingSpecialKey;
                Properties.Settings.Default.SettingTitleTip = Properties.Settings.Default.SettingTitleTip;
                Properties.Settings.Default.SettingBracketModeOnlyScan = Properties.Settings.Default.SettingBracketModeOnlyScan;
                Properties.Settings.Default.SettingFrequency = Properties.Settings.Default.SettingFrequency;
                Properties.Settings.Default.SettingDarkMode = Properties.Settings.Default.SettingDarkMode;
                Properties.Settings.Default.SettingOpacity = Properties.Settings.Default.SettingOpacity;
                Properties.Settings.Default.SettingTopMost = Properties.Settings.Default.SettingTopMost;
                Properties.Settings.Default.SettingSendkeysOnlyMode = Properties.Settings.Default.SettingSendkeysOnlyMode;
                Properties.Settings.Default.SettingBracketOpen = Properties.Settings.Default.SettingBracketOpen;
                Properties.Settings.Default.SettingBracketClose = Properties.Settings.Default.SettingBracketClose;
                Properties.Settings.Default.SettingBackgroundImage = Properties.Settings.Default.SettingBackgroundImage;
                Properties.Settings.Default.SettingInfiniteLoop = Properties.Settings.Default.SettingInfiniteLoop;
                Properties.Settings.Default.SettingIgnoreWhiteSpaceOpen = Properties.Settings.Default.SettingIgnoreWhiteSpaceOpen;
                Properties.Settings.Default.SettingIgnoreWhiteSpaceClose = Properties.Settings.Default.SettingIgnoreWhiteSpaceClose;
                Properties.Settings.Default.SettingInsertSymbol = Properties.Settings.Default.SettingInsertSymbol;
                Properties.Settings.Default.SettingFirstLoad += 1;
            }

            Properties.Settings.Default.Save();
        }
        private void TextBox1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e) {//prevent default tab
            if (e.KeyCode == Keys.Tab) e.IsInputKey = true;
        }
        private void ListBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 5) {
                TextClear();
                EditDbItm();
                TextBox1.Focus();
                CleanSelect();
            }//ctrl + e
            if (e.KeyChar == 21) {
                UpdateDbItm();
                CleanSelect();
            }//ctrl + u
            if (GetAsyncKeyState(Keys.LControlKey) && GetAsyncKeyState(Keys.X)) {
                Clipboard.SetText(ListBox1.Text);
                Key(Keys.Delete, false, 1);
                CleanSelect();
            }
            if (GetAsyncKeyState(Keys.LControlKey) && GetAsyncKeyState(Keys.C)) {
                Clipboard.SetText(ListBox1.Text);
                CleanSelect();
            }
            if (GetAsyncKeyState(Keys.LControlKey) && GetAsyncKeyState(Keys.V) && Clipboard.GetText().Length > 0 && ListBox1.Items.Count > 0) {
                ListBox1.Items.Insert(ListBox1.SelectedIndex, Clipboard.GetText());
                Properties.Settings.Default.SettingDB.Insert(ListBox1.SelectedIndex - 1, Clipboard.GetText());
                CleanSelect();
                ListBox1.SelectedIndex = ListBox1.SelectedIndex - 1;
                LoadArray();
            }
            if (GetAsyncKeyState(Keys.Oemplus)) {
                ListBox1.Font = new System.Drawing.Font(ListBox1.Font.Name, ListBox1.Font.Size + +1);
            }
            if (GetAsyncKeyState(Keys.OemMinus)) {
                if (ListBox1.Font.Size <= 0.25) { ListBox1.Font = new System.Drawing.Font(ListBox1.Font.Name, TextBox1.Font.Size); }//reset
                ListBox1.Font = new System.Drawing.Font(ListBox1.Font.Name, ListBox1.Font.Size + -1);

            }
        }
        private void ListBox1_KeyUp(object sender, KeyEventArgs e) {
            if (e.KeyValue == 46) { RemoveDbItm(); }//delete
        }
        private void TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (GetAsyncKeyState(Keys.LControlKey) && (GetAsyncKeyState(Keys.F))) {
                if (ListBox1.SelectedIndex == -1) { if (ListBox1.Items.Count > 0) { ListBox1.SelectedIndex = 0; } else { return; } }
                if (ListBox1.SelectedIndex == ListBox1.Items.Count - 1) { ListBox1.SelectedIndex = 0; return; }
                if (ListBox1.SelectedItem.ToString().Contains(TextBox1.Text.ToLower())) { ListBox1.SelectedIndex += 1; }
                for (int i = ListBox1.SelectedIndex; i < ListBox1.Items.Count - 1; i++) {
                    if (ListBox1.SelectedIndex == ListBox1.Items.Count - 1) { ListBox1.SelectedIndex = 0; return; }
                    if (ListBox1.Items[i].ToString().Contains(TextBox1.Text.ToLower())) { ListBox1.SelectedIndex = i; return; }
                    if (i == ListBox1.Items.Count - 1) { ListBox1.SelectedIndex = 0; return; }
                }
            }

            if (GetAsyncKeyState(Keys.F4)) { TextClear(); }

            //move cursor home or end
            if (GetAsyncKeyState(Keys.Down) && TextBox1.Text != "")
            {
                int getLastLineNumb = TextBox1.GetLineFromCharIndex(TextBox1.SelectionStart + TextBox1.TextLength) + 1;
                int getLineNumb = TextBox1.GetLineFromCharIndex(TextBox1.SelectionStart) + 1;
                if (getLineNumb == getLastLineNumb) { TextBox1.SelectionStart = TextBox1.TextLength; return; }//Bottom
            }//end
            if (GetAsyncKeyState(Keys.Up) && TextBox1.Text != "")
            {
                int getLineNumb = TextBox1.GetLineFromCharIndex(TextBox1.SelectionStart) + 1;
                if (getLineNumb == 1) { TextBox1.SelectionStart = 0; return; }//Bottom
            }//home            
        }
        private void TextBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (TextBox1.Text.StartsWith("'"))
            {
                if (Timer1.Enabled == true) { Text = Properties.Settings.Default.SettingTitleText + " > Off"; Timer1.Enabled = false; }
            }
            else
            {
                if (Timer1.Enabled == false) { ClearAllKeys(); textBox2.Clear(); Text = Properties.Settings.Default.SettingTitleText; Timer1.Enabled = true; }
            }//off/on
        }        
        private void DZ_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) { DragFormInit(); }
        }
        private void DZ_MouseMove(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) { DragForm(); }
        }
        private void DZ_MouseUp(object sender, MouseEventArgs e) { g_drag = false; }

        private void DZ_DoubleClick(object sender, EventArgs e) {
            GetAsyncKeyState(Keys.LControlKey);
            if (Properties.Settings.Default.SettingBackgroundImage != "") { FixedSize(); }
            if (GetAsyncKeyState(Keys.LControlKey)) { CenterToScreen(); }
        }

        private void SplitContainer1_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right) { 
                if (ListBox1.SelectedIndex == ListBox1.Items.Count - 1) {
                    ListBox1.SelectedIndex = 0;
                }else{
                    ListBox1.SelectedIndex = ListBox1.Items.Count - 1;
                }
            }
        }//select top or bottom item
    }
}
