using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Call_of_Duty_Launcher
{
    public partial class FormMain : Form
    {

        public FormMain()
        {
            InitializeComponent();

            parameters();

            // Get the registry entry and format it accordingly
            RegistryKey myKey  = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Activision\Call of Duty");
            RegistryKey myKey2 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Activision\Call of Duty United Offensive");

            if (myKey != null)
            {
                string formatted_key = myKey.GetValue("codkey").ToString().ToUpper();
                if (formatted_key.Length == 20)
                {
                    formatted_key = Regex.Replace(formatted_key, ".{4}", "$0-");
                }
                comboBoxOrigCDKey.Text = formatted_key.Remove(formatted_key.Length - 1, 1);
            }

            if (myKey2 != null)
            {
                string formatted_key = myKey2.GetValue("key").ToString().ToUpper();
                if (formatted_key.Length == 20)
                {
                    formatted_key = Regex.Replace(formatted_key, ".{4}", "$0-");
                }
                comboBoxExCDKey.Text = formatted_key.Remove(formatted_key.Length - 1, 1);
            }

            // Get the list of cdkeys from the cdkeys.txt file
            if (File.Exists("cdkeys.txt"))
            {
                string[] cdkeys = File.ReadAllLines("cdkeys.txt");
                comboBoxOrigCDKey.Items.Clear();
                int i = 1;
                foreach (string cdkey in cdkeys)
                {

                    if (cdkey.Length != 24 && cdkey.Length != 20) continue;
                    comboBoxOrigCDKey.Items.Add(i + " - " + cdkey);
                    i++;
                }

            }

            if (File.Exists("cdkeysuo.txt"))
            {
                string[] cdkeys = File.ReadAllLines("cdkeysuo.txt");
                comboBoxExCDKey.Items.Clear();
                int i = 1;
                foreach (string cdkey in cdkeys)
                {

                    if (cdkey.Length != 24 && cdkey.Length != 20) continue;
                    comboBoxExCDKey.Items.Add(i + " - " + cdkey);
                    i++;
                }

            }


        }

        private void parameters()
        {
            string[] args = Environment.GetCommandLineArgs();

            string command = "";
            bool updated = false;

            foreach (string arg in args)
            {
                if (command == "--cdkey")
                {
                    string cdkey = Regex.Replace(arg, "[-]", string.Empty).Trim().ToLower();
                    update_registry(cdkey, "orig");
                    updated = true;
                }

                if (command == "--cdkeyex")
                {
                    string cdkey = Regex.Replace(arg, "[-]", string.Empty).Trim().ToLower();
                    update_registry(cdkey, "exp");
                    updated = true;
                }

                command = arg;
            }

            if(updated)
            {
                MessageBox.Show("Settings Updated");
                Environment.Exit(0);
            }
            
        }

        private void update_registry(string cdkey, string mode)
        {
            if (mode == "exp")
            {
                string formatted_cdkey = Regex.Replace(cdkey, "[-]", string.Empty).Trim().ToUpper();
                comboBoxOrigCDKey.Text = formatted_cdkey;
                RegistryKey myKey1 = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Activision\Call of Duty United Offensive");

                if (myKey1 != null)
                {
                    myKey1.SetValue("key", formatted_cdkey, RegistryValueKind.String);
                    myKey1.SetValue("InstallPath", Directory.GetCurrentDirectory(), RegistryValueKind.String);
                    myKey1.SetValue("InstallDrive", Path.GetPathRoot(System.Reflection.Assembly.GetEntryAssembly().Location), RegistryValueKind.String);
                    myKey1.SetValue("Language", "1", RegistryValueKind.String);
                    myKey1.SetValue("Version", "1.0", RegistryValueKind.String);
                    myKey1.SetValue("EXEString", Path.GetFullPath("CoDSP.exe"), RegistryValueKind.String);
                    myKey1.SetValue("MultiEXEString", Path.GetFullPath("CoDMP.exe"), RegistryValueKind.String);
                    myKey1.SetValue("QA", "15.0", RegistryValueKind.String);
                    myKey1.Close();
                }
            }

            if (mode == "orig")
            {
                string formatted_cdkey = Regex.Replace(cdkey, "[-]", string.Empty).Trim().ToUpper();
                comboBoxOrigCDKey.Text = formatted_cdkey;
                RegistryKey myKey1 = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Activision\Call of Duty");

                if (myKey1 != null)
                {
                    myKey1.SetValue("codkey", formatted_cdkey, RegistryValueKind.String);
                    myKey1.SetValue("InstallPath", Directory.GetCurrentDirectory(), RegistryValueKind.String);
                    myKey1.SetValue("InstallDrive", Path.GetPathRoot(System.Reflection.Assembly.GetEntryAssembly().Location), RegistryValueKind.String);
                    myKey1.SetValue("Language", "1", RegistryValueKind.String);
                    myKey1.SetValue("Version", "1.0", RegistryValueKind.String);
                    myKey1.SetValue("EXEString", Path.GetFullPath("CoDSP.exe"), RegistryValueKind.String);
                    myKey1.SetValue("MultiEXEString", Path.GetFullPath("CoDMP.exe"), RegistryValueKind.String);
                    myKey1.SetValue("QA", "15.0", RegistryValueKind.String);
                    myKey1.Close();
                }
            }

        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void FormMain_Load(object sender, EventArgs e)
        {

        }

        private void comboBoxOrigCDKey_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selected_item = comboBoxOrigCDKey.SelectedItem.ToString();
            string formatted_text = selected_item.Substring(selected_item.IndexOf(" - ") + 3);

            BeginInvoke(new Action(() => comboBoxOrigCDKey.Text = formatted_text));
        }

        private void comboBoxExCDKey_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selected_item = comboBoxExCDKey.SelectedItem.ToString();
            string formatted_text = selected_item.Substring(selected_item.IndexOf(" - ") + 3);

            BeginInvoke(new Action(() => comboBoxExCDKey.Text = formatted_text));
        }

        private void buttonUpdateKeys_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Are you sure you want to update your keys? This will reset your profile",
                 "Update CD Key and resolution?",
                 MessageBoxButtons.YesNo);

            if (confirmResult != DialogResult.Yes) return;

            update_resolution();

            Process proc = new Process();
            proc.StartInfo.FileName = System.AppDomain.CurrentDomain.FriendlyName;
            proc.StartInfo.UseShellExecute = true;
            proc.StartInfo.Verb = "runas";
            proc.StartInfo.Arguments = "--cdkey " + comboBoxOrigCDKey.Text + " --cdkeyex " + comboBoxExCDKey.Text;
            proc.Start();
        }

        private void update_resolution()
        {
            if (textBoxResWidth.Text.Length == 0 || textBoxResWidth.Text.Length == 0) return;

            string multiplayer_config = "Main\\config_mp.cfg";
            string singleplayer_config = "Main\\config.cfg";

            string uo_multiplayer_config = "uo\\uoconfig_mp.cfg";
            string uo_singleplayer_config = "uo\\uoconfig.cfg";

            if (!File.Exists(multiplayer_config) || !File.Exists(singleplayer_config) || !File.Exists(uo_multiplayer_config) || !File.Exists(uo_singleplayer_config))
            {
                MessageBox.Show("You'll need to run the game at least once before trying to set the resolution. Main\\config_mp.cfg and uo\\config_mp.cfg needs to exist, continuing to update keys...");
                return;
            }

            // Enable Custom Resolution
            update_config(singleplayer_config, "seta r_mode", "-1");
            update_config(multiplayer_config, "seta r_mode", "-1");

            // Update Original Singleplayer
            update_config(singleplayer_config, "seta r_customheight", textBoxResHeight.Text);
            update_config(singleplayer_config, "seta r_customwidth", textBoxResWidth.Text);

            // Update Original Multiplayer
            update_config(multiplayer_config, "seta r_customheight", textBoxResHeight.Text);
            update_config(multiplayer_config, "seta r_customwidth", textBoxResWidth.Text);


            // ############ Expansion settings ##########

            // Enable Custom Resolution
            update_config(uo_singleplayer_config, "seta r_mode", "-1");
            update_config(uo_multiplayer_config, "seta r_mode", "-1");

            // Update Expansion Singleplayer
            update_config(uo_singleplayer_config, "seta r_customheight", textBoxResHeight.Text);
            update_config(uo_singleplayer_config, "seta r_customwidth", textBoxResWidth.Text);

            // Update Expansion Multiplayer
            update_config(uo_multiplayer_config, "seta r_customheight", textBoxResHeight.Text);
            update_config(uo_multiplayer_config, "seta r_customwidth", textBoxResWidth.Text);

        }

        private void update_config(string file, string setting, string new_value)
        {
            int linenumber = 0;

            // Find Line Number
            foreach (var match in File.ReadLines(file).Select((text, index) => new { text, lineNumber = index + 1 }).Where(x => x.text.Contains(setting)))
            {
                linenumber = match.lineNumber;
            }

            if (linenumber == 0)
            {
                File.AppendAllText(file, setting  +" \"" + new_value + "\"" + Environment.NewLine);
            }
            else
            {
                lineChanger(setting + " \"" + new_value + "\"", file, linenumber);
            }
        }

        static void lineChanger(string newText, string fileName, int line_to_edit)
        {
            string[] arrLine = File.ReadAllLines(fileName);
            arrLine[line_to_edit - 1] = newText;
            File.WriteAllLines(fileName, arrLine);
        }

        private void textBoxResWidth_TextChanged(object sender, EventArgs e)
        {

        }

        private void buttonOrigSingleplayer_Click(object sender, EventArgs e)
        {
            if (File.Exists("CoDSP.exe"))
                Process.Start("CoDSP.exe");
            Environment.Exit(0);
        }

        private void buttonExMultiplayer_Click(object sender, EventArgs e)
        {
            if (File.Exists("CoDMP.exe"))
                Process.Start("CoDMP.exe");
            Environment.Exit(0); 
        }

        private void buttonExSingleplayer_Click(object sender, EventArgs e)
        {
            if (File.Exists("CoDUOSP.exe"))
                Process.Start("CoDUOSP.exe");
            Environment.Exit(0);
        }

        private void buttonOrigMultiplayer_Click(object sender, EventArgs e)
        {
            if(File.Exists("CoDUOMP.exe"))
                Process.Start("CoDUOMP.exe");
            Environment.Exit(0);
        }

        private void textBoxResHeight_TextChanged(object sender, EventArgs e)
        {
            if (Regex.IsMatch(textBoxResHeight.Text, "  ^ [0-9]"))
            {
                textBoxResHeight.Text = "";
            }
        }

        // Click anywhere to move hack
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        private void FormMain_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }
    }
}
