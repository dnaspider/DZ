//@dnaspider

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace csWindowsFormsApp1
{
    public partial class DZ : Form
    {
        [DllImport("user32")] private static extern bool GetAsyncKeyState(System.Windows.Forms.Keys vKey);
        [DllImport("user32", EntryPoint = "keybd_event")] private static extern Int32 Keybd_event(System.Windows.Forms.Keys bVk, byte bScan, int dwFlags, int dwExtraInfo);

        //globals
        System.Collections.ArrayList ar = new System.Collections.ArrayList();
        System.Windows.Forms.Keys g_specialKey = Properties.Settings.Default.SettingSpecialKey;
        string p_ = Properties.Settings.Default.SettingOpenBracket;
        string _p = Properties.Settings.Default.SettingCloseBracket;
        int g_presses = 1;

        public DZ(){ InitializeComponent(); }

        //Sub
        private void Key(System.Windows.Forms.Keys key, bool shft, int presses)
        {
            if (shft == true) { Keybd_event(Keys.RShiftKey, 0, 1, 0); };
            if (g_presses >= 1) { presses = g_presses; };
            for (int i = 0; i < presses; i++)
            {
                Keybd_event(key, 0, 0, 0);
                Keybd_event(key, 0, 2, 0);
            }
            if (shft) { Keybd_event(Keys.RShiftKey, 0, 2, 0); };
            GetAsyncKeyState(key);
        }
        private void KeyHold(System.Windows.Forms.Keys key) { Keybd_event(key, 0, 1, 0); }
        private void KeyRelease(System.Windows.Forms.Keys key) { Keybd_event(key, 0, 2, 0); }
        private void TextClear() { textBox1.SelectAll(); textBox1.SelectedText = ""; }
        private void Sleep(int ms) { System.Threading.Thread.Sleep(ms); Application.DoEvents(); }
        private void CleanMock() { Sleep(333); TextBox2.Clear(); }
        private void LoadArray() {
            ar.Clear();
            for (int i = 0; i < listBox1.Items.Count; i++) {
                if (listBox1.Items[i].ToString().StartsWith(p_)) {
                    ar.Add(i + ":" + listBox1.Items[i].ToString().Substring(1, listBox1.Items[i].ToString().IndexOf(_p) - 1));
                }

            }
            //for (int i = 0; i < ar.Count; i++){
            //    Console.WriteLine(ar.Count + ": " + ar[i].ToString());
            //}
        }
        private void AddDbItm() {
            Properties.Settings.Default.SettingDB.Add(textBox1.Text);
            listBox1.Items.Add(textBox1.Text);
            TextClear();
            listBox1.SelectedIndex = listBox1.Items.Count - 1;
            KeyRelease(Keys.S);
            KeyRelease(g_specialKey);
            CleanMock();
            LoadArray();
        }
        private void UpdateDbItm() {
            if (listBox1.SelectedIndex < 0 || textBox1.Text == "") { return; };

            int x = textBox1.SelectionStart;
            int i = listBox1.SelectedIndex;

            listBox1.Items.RemoveAt(i);
            Properties.Settings.Default.SettingDB.RemoveAt(i);
            listBox1.Items.Insert(i, textBox1.Text);
            Properties.Settings.Default.SettingDB.Insert(i, textBox1.Text);

            listBox1.SelectedItem = listBox1.Items.IndexOf(i);
            textBox1.SelectionStart = x;

            CleanMock();
            LoadArray();
        }
        private void LoadDb() {
            foreach (var item in Properties.Settings.Default.SettingDB) {
                listBox1.Items.Add(item);
            }
            LoadArray();
        }
        private void DarkMode() {
            if (Properties.Settings.Default.SettingDarkMode == true) {
                BackColor = Color.Black;
                splitContainer1.BackColor = Color.Black;
                listBox1.BackColor = Color.Black;
                listBox1.ForeColor = Color.Lime;
                textBox1.BackColor = Color.Black;
                textBox1.ForeColor = Color.Lime;
            }
        }
        private void FixedSize(){
            if (ControlBox == true){
                FormBorderStyle = FormBorderStyle.None;
                ControlBox = false;
                Text = "";
            }else{
                ControlBox = true;
                FormBorderStyle = FormBorderStyle.Sizable;
                Text = Properties.Settings.Default.SettingTitleText;
                if (Properties.Settings.Default.SettingIcon != "") { Icon = new Icon(Properties.Settings.Default.SettingIcon); }else{ Icon = Icon; }
            }
            if (Properties.Settings.Default.SettingBackgroundImage != "") { BackgroundImage = Image.FromFile(Properties.Settings.Default.SettingBackgroundImage); }
            if (splitContainer1.Visible){
                if (Properties.Settings.Default.SettingBackgroundImage != ""){
                    BackColor = Color.GhostWhite;
                    splitContainer1.Visible = false;
                }
            }else{
                if (Properties.Settings.Default.SettingDarkMode == true) { BackColor = Color.Black; }
                splitContainer1.Visible = true;
            }
        }


        private void TextBox1_DoubleClick(object sender, EventArgs e)
        {
            SendKeys.Send(p_ + _p + "{left}");
            //Keybd_event(Keys.A, 0, 0, 0);
            //Keybd_event(Keys.A, 0, 2, 0);
            //Key(Keys.A, true, 2);

        }
        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (GetAsyncKeyState(Keys.Back)) { if (TextBox2.TextLength > 0) { TextBox2.Text = TextBox2.Text.Substring(0, TextBox2.TextLength - 1); } };
            if (GetAsyncKeyState(Keys.A)) { TextBox2.AppendText("a"); };
            if (GetAsyncKeyState(Keys.B)) { TextBox2.AppendText("b"); };
            if (GetAsyncKeyState(Keys.C)) { TextBox2.AppendText("c"); };



            //trim
            if (TextBox2.TextLength > Properties.Settings.Default.SettingCodeLength) { TextBox2.Text = TextBox2.Text.Substring(TextBox2.TextLength - Properties.Settings.Default.SettingCodeLength); };
        }
        private void TextBox2_TextChanged(object sender, EventArgs e)
        {
            //scan
            if (TextBox2.Text == "abc")
            {



                TextBox2.Clear();
            }

            //mock
            if (Properties.Settings.Default.SettingTitleTip) { Text = Properties.Settings.Default.SettingTitleText + " > " + TextBox2.Text; };
        }
        private void TextBox1_KeyPress(object sender, KeyPressEventArgs e) {
            if (GetAsyncKeyState(Keys.LControlKey) && GetAsyncKeyState(Keys.X)) { GetAsyncKeyState(Keys.X); }//clear
            if (GetAsyncKeyState(Keys.LControlKey) && GetAsyncKeyState(Keys.C)) { GetAsyncKeyState(Keys.C); }

            if (GetAsyncKeyState(Keys.LControlKey) && GetAsyncKeyState(Keys.A) && textBox1.TextLength > 1) {
                textBox1.SelectionStart = 0; textBox1.SelectionLength = textBox1.TextLength;
            }
            
            if (e.KeyChar == 19){ AddDbItm(); }//ctrl+s
            if (e.KeyChar == 21){ UpdateDbItm(); }//ctrl+u

        }

        private void DZ_Load(object sender, EventArgs e){
            //If My.Settings.SettingFirstLoad = 0 Then Application.Restart()
            if (Properties.Settings.Default.SettingIcon != "") { Icon = new Icon(Properties.Settings.Default.SettingIcon); }
            textBox1.WordWrap = Properties.Settings.Default.SettingWordWrap;

            LoadDb();
            DarkMode();

            Timer1.Interval = Properties.Settings.Default.SettingFrequency;

            Opacity = Properties.Settings.Default.SettingOpacity;
            TopMost = Properties.Settings.Default.SettingTopMost;
            Top = Properties.Settings.Default.SettingLocationTop;
            Left = Properties.Settings.Default.SettingLocationLeft;
            Height = Properties.Settings.Default.SettingHeight;
            Width = Properties.Settings.Default.SettingWidth;

            if (listBox1.Items.Count > 0) { listBox1.SelectedIndex = Properties.Settings.Default.SettingListboxSelectedIndex; }
            listBox1.Font = new System.Drawing.Font(textBox1.Font.Name, Properties.Settings.Default.SettingListBoxFontSize);

            switch (Properties.Settings.Default.SettingTabIndex) {
                case 1:
                    listBox1.Focus(); break;
                case 3:
                    textBox1.Focus(); break;
                default:
                    listBox1.Focus(); break;
            }

            textBox1.Text = Properties.Settings.Default.SettingTextBox;
            if (textBox1.Focused) {
                textBox1.SelectionStart = Properties.Settings.Default.SettingSelectionStart;
                textBox1.SelectionLength = Properties.Settings.Default.SettingSelectionLength;
            }
            textBox1.ZoomFactor = Properties.Settings.Default.SettingTextBoxZoomFactor;
            splitContainer1.SplitterDistance = Properties.Settings.Default.SettingSplitterDistance;
            splitContainer1.SplitterWidth = Properties.Settings.Default.SettingSplitterWidth;

            if (Properties.Settings.Default.SettingBackgroundImage != "") {
                BackgroundImage = Image.FromFile(Properties.Settings.Default.SettingBackgroundImage);
                FixedSize();
            }
        }

        private void DZ_FormClosing(object sender, FormClosingEventArgs e){
            Properties.Settings.Default.Save();
        }
    }
}
