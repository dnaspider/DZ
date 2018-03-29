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

        public DZ()
        {
            InitializeComponent();

            //settings
            if (Properties.Settings.Default.SettingDarkMode == true) {
                splitContainer1.BackColor = Color.Black;
                listBox1.BackColor = Color.Black;
                listBox1.ForeColor = Color.Lime;
                TextBox1.BackColor = Color.Black;
                TextBox1.ForeColor = Color.Lime;
            }
            Text = Properties.Settings.Default.SettingTitleText;
            Timer1.Interval = Properties.Settings.Default.SettingFrequency;

        }

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
        private void TextClear() { TextBox1.SelectAll(); TextBox1.SelectedText = ""; }
        private void Sleep(int ms) { System.Threading.Thread.Sleep(ms); Application.DoEvents(); }
        private void CleanMock() { Sleep(333); TextBox2.Clear(); }
        private void LoadArray(){
            ar.Clear();
            for (int i = 0; i < listBox1.Items.Count; i++){
                if (listBox1.Items[i].ToString().StartsWith(p_)) {
                    ar.Add(i + ":" + listBox1.Items[i].ToString().Substring(1, listBox1.Items[i].ToString().IndexOf(_p) - 1));
                }
                
            }
            //for (int i = 0; i < ar.Count; i++){
            //    Console.WriteLine(ar.Count + ": " + ar[i].ToString());
            //}
        }
        private void AddDbItm(){
            Properties.Settings.Default.SettingDB.Add(TextBox1.Text);
            listBox1.Items.Add(TextBox1.Text);
            TextClear();
            listBox1.SelectedIndex = listBox1.Items.Count - 1;
            KeyRelease(Keys.S);
            KeyRelease(g_specialKey);
            CleanMock();
            LoadArray();
        }


        private void TextBox1_DoubleClick(object sender, EventArgs e)
        {
            SendKeys.Send(p_ + _p + "{left}");

            //test
            //Keybd_event(Keys.A, 0, 0, 0);
            //Keybd_event(Keys.A, 0, 2, 0);

            //test
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
            if (e.KeyChar == 19){ AddDbItm(); }//ctrl+s

        }
    }
}
