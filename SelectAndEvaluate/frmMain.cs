using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;


namespace SelectAndEvaluate
{
    public partial class frmMain : SelectAndEvaluate.CustomForm
    {
        KeyboardHook keyboardHook = new KeyboardHook();

        private int HotKeyChar = -1;

        HookingProtector HookingProtector = new HookingProtector();

        public static frmMain Instance = null;

        public frmMain()
        {
            InitializeComponent();

            ///Properties.Settings.Default.Initialized = false;

            if (Properties.Settings.Default.Initialized && Properties.Settings.Default.MinimizeToTray)
            {
                this.Visible = false;
                WindowState = FormWindowState.Minimized;
                this.ShowInTaskbar = false;
            }

            Instance = this;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            keyboardHook.KeyDown += new KeyEventHandler(keyboardHook_KeyDown);
            keyboardHook.KeyUp += new KeyEventHandler(keyboardHook_KeyUp);
            keyboardHook.KeyPress += new KeyPressEventHandler(keyboardHook_KeyPress);

            txtHotKey.Text = Properties.Settings.Default.ShortcutKeyString;

            HotKeyChar = Properties.Settings.Default.ShortcutKeyString[0];

            if (Properties.Settings.Default.CheckWeek)
            {
                UpdateHelper.InitializeCheckVersionWeek();
            }

            checkForNewVersionEachWeekToolStripMenuItem.Checked = Properties.Settings.Default.CheckWeek;
            minimizeToWindowsSystemTrayToolStripMenuItem.Checked = Properties.Settings.Default.MinimizeToTray;
            copyToClipboardResultToolStripMenuItem.Checked = !Properties.Settings.Default.PasteResult;
            pasteResultToolStripMenuItem.Checked = Properties.Settings.Default.PasteResult;

            chkControl.Checked = Properties.Settings.Default.KeyControl;
            chkAlt.Checked = Properties.Settings.Default.KeyAlt;
            chkShift.Checked = Properties.Settings.Default.KeyShift;

            keyboardHook.Start();

            if (!Properties.Settings.Default.Initialized)
            {
                RunAtWndowsStartupManager.RunAtWindowsStartup = true;

                frmMessageCheckbox fm = new frmMessageCheckbox();
                fm.Show(this);

            }

            runAtWindowsStartupToolStripMenuItem.Checked = RunAtWndowsStartupManager.RunAtWindowsStartup;

            if (Properties.Settings.Default.Initialized && Properties.Settings.Default.MinimizeToTray)
            {
                btnClose_Click(null, null);
            }

            SetTitle();

            if (Properties.Settings.Default.Initialized && Properties.Settings.Default.MinimizeToTray)
            {
                this.Visible = false;
                this.Hide();
                WindowState = FormWindowState.Minimized;
                this.ShowInTaskbar = false;
            }
            else
            {
                this.ShowInTaskbar = true;
            }

            if (!Properties.Settings.Default.Initialized)
            {
                Properties.Settings.Default.Initialized = true;
                Properties.Settings.Default.Save();
            }

            for (int k = 0; k < Module.args.Length; k++)
            {
                if (Module.args[k].Trim() == "/minimized")
                {
                    this.WindowState = FormWindowState.Minimized;
                }
                else if (Module.args[k].Trim() == "/novisible")
                {
                    this.Visible = false;

                    if (Properties.Settings.Default.MinimizeToTray)
                    {
                        notMain.Visible = true;
                    }
                }
            }

            HookingProtector.Setup();

        }

        private bool ControlCIsPressed = false;

        void keyboardHook_KeyPress(object sender, KeyPressEventArgs e)
        {                        
        }

        void keyboardHook_KeyUp(object sender, KeyEventArgs e)
        {
            if ((char)e.KeyCode == 'C')
            {
                if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
                {
                    ControlCIsPressed = true;                    
                }
            }           
        }

        void keyboardHook_KeyDown(object sender, KeyEventArgs e)
        {
            /*
            if (e.KeyData == Keys.F14)
            {
                frmMain.CheckSetHookTimer = false;

                return;
            }
            */

            if ((e.KeyValue==Properties.Settings.Default.ShortcutKey) && ControlCIsPressed)                
            {
                bool cancel = false;

                if (Properties.Settings.Default.KeyControl && ((Control.ModifierKeys & Keys.Control) != Keys.Control))
                {
                    cancel = true;
                }

                if (Properties.Settings.Default.KeyAlt && ((Control.ModifierKeys & Keys.Control) != Keys.Alt))
                {
                    cancel = true;
                }

                if (Properties.Settings.Default.KeyShift && ((Control.ModifierKeys & Keys.Control) != Keys.Shift))
                {
                    cancel = true;
                }

                if (!cancel)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;

                    Calculate();
                }
            }

            if ((char)e.KeyCode == 'C')
            {
                if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
                {
                    ControlCIsPressed = true;
                }
                else
                {
                    ControlCIsPressed = false;
                }
            }
            else
            {
                ControlCIsPressed = false;
            }            
        }

        MSScriptControl.ScriptControl sc = new MSScriptControl.ScriptControl();

        private void Calculate()
        {
            try
            {
                string str = Clipboard.GetText();

                //calculate
                
                sc.Language = "VBScript";
                string expression = str;
                object result = sc.Eval(expression);
                
                Clipboard.Clear();
                Clipboard.SetText(result.ToString());

                if (Properties.Settings.Default.PasteResult)
                {
                    KeyboardSimulator.SimulateStandardShortcut(StandardShortcut.Paste);
                }

            }
            catch (Exception ex)
            {
                Clipboard.Clear();
                Clipboard.SetText("Invalid Vbscript Expression");

                if (Properties.Settings.Default.PasteResult)
                {
                    KeyboardSimulator.SimulateStandardShortcut(StandardShortcut.Paste);
                }

                return;
            }
        }      

        private void txtHotKey_KeyPress(object sender, KeyPressEventArgs e)
        {
            
            
        }

        #region Help Menu

        private void helpGuideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Module.HelpURL);
        }

        private void pleaseDonateToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.4dots-software.com/donate.php");
        }

        private void dotsSoftwarePRODUCTCATALOGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.4dots-software.com/downloads/4dots-Software-PRODUCT-CATALOG.pdf");
        }

        private void checkForNewVersionEachWeekToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.CheckWeek = checkForNewVersionEachWeekToolStripMenuItem.Checked;
            Properties.Settings.Default.Save();
        }

        private void tiHelpFeedback_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.4dots-software.com/support/bugfeature.php?app=" + System.Web.HttpUtility.UrlEncode(Module.ShortApplicationTitle));
        }

        private void checkForNewVersionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateHelper.CheckVersion(false);
        }

        private void followUsOnTwitterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.twitter.com/4dotsSoftware");
        }

        private void visit4dotsSoftwareWebsiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.4dots-software.com");
        }

        private void youtubeChannelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.youtube.com/channel/UCovA-lld9Q79l08K-V1QEng");
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAbout f = new frmAbout();
            f.ShowDialog();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        #endregion

        private void minimizeToWindowsSystemTrayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.MinimizeToTray = minimizeToWindowsSystemTrayToolStripMenuItem.Checked;
            Properties.Settings.Default.Save();
        }

        private void notMain_MouseDoubleClick(object sender, MouseEventArgs e)
        {            
            this.Visible = true;
            this.Show();
            this.BringToFront();
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            this.CenterToScreen();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.ShortcutKeyString = txtHotKey.Text;

            HotKeyChar = Properties.Settings.Default.ShortcutKeyString[0];

            Properties.Settings.Default.Save();

            this.Visible = !Properties.Settings.Default.MinimizeToTray;

            if (Properties.Settings.Default.MinimizeToTray)
            {
                this.Hide();
                WindowState = FormWindowState.Minimized;
            }

            this.notMain.Visible = Properties.Settings.Default.MinimizeToTray;
        }

        private void txtHotKey_Enter(object sender, EventArgs e)
        {            
            txtHotKey.SelectAll();
        }

        private void frmMain_Resize(object sender, EventArgs e)
        {
            if (WindowState==FormWindowState.Minimized)
            {
                if (Properties.Settings.Default.MinimizeToTray)
                {
                    notMain.Visible = true;
                    this.Visible = false;
                }
            }
        }

        private void txtHotKey_Click(object sender, EventArgs e)
        {
            txtHotKey_Enter(null, null);
        }

        private void txtHotKey_Validating(object sender, CancelEventArgs e)
        {
            
        }

        private void txtHotKey_KeyDown(object sender, KeyEventArgs e)
        {
            int if13 = (int)Keys.F13;
            int if14 = (int)Keys.F14;
            int if15 = (int)Keys.F15;
            int if16 = (int)Keys.F16;
            int if17 = (int)Keys.F17;

            int vkCode = (int)e.KeyData;

            if ((vkCode == if13) || (vkCode == if14) || (vkCode == if15) || (vkCode == if16) || (vkCode == if17))
            {
                e.Handled = true;

                return;
            }

            e.Handled = true;

            Properties.Settings.Default.KeyControl = e.Control;

            Properties.Settings.Default.KeyAlt = e.Alt;

            Properties.Settings.Default.KeyShift = e.Shift;

            chkControl.Checked = e.Control;

            chkAlt.Checked = e.Alt;

            chkShift.Checked = e.Shift;

            if (!(e.KeyCode.ToString().Contains("Control") || e.KeyCode.ToString().Contains("Shift") || e.KeyCode.ToString().Contains("Alt") || e.KeyCode.ToString().Contains("Menu")))
            {

                txtHotKey.Text = e.KeyCode.ToString();
                Properties.Settings.Default.ShortcutKey = e.KeyValue;

                Properties.Settings.Default.ShortcutKeyString = e.KeyCode.ToString();

            }

            Properties.Settings.Default.Save();
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            notMain_MouseDoubleClick(null, null);
        }

        private void runAtWindowsStartupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunAtWndowsStartupManager.RunAtWindowsStartup = !runAtWindowsStartupToolStripMenuItem.Checked;

            runAtWindowsStartupToolStripMenuItem.Checked = RunAtWndowsStartupManager.RunAtWindowsStartup;

        }

        bool FreeForPersonalUse = false;
        bool FreeForPersonalAndCommercialUse = true;

        private void SetTitle()
        {
            string str = "";
                        
            if (FreeForPersonalUse)
            {
                str += " - " + TranslateHelper.Translate("Free for Personal Use Only - Please Donate !");
            }
            else if (FreeForPersonalAndCommercialUse)
            {
                str += " - " + TranslateHelper.Translate("Free for Personal and Commercial Use - Please Donate !");
            }

            this.Text = Module.ApplicationTitle + str.ToUpper();
        }

        private void copyToClipboardResultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            copyToClipboardResultToolStripMenuItem.Checked = !copyToClipboardResultToolStripMenuItem.Checked;
            Properties.Settings.Default.PasteResult = !copyToClipboardResultToolStripMenuItem.Checked;
            pasteResultToolStripMenuItem.Checked = !copyToClipboardResultToolStripMenuItem.Checked;

            Properties.Settings.Default.Save();
        }

        private void pasteResultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pasteResultToolStripMenuItem.Checked = !pasteResultToolStripMenuItem.Checked;
            Properties.Settings.Default.PasteResult = pasteResultToolStripMenuItem.Checked;
            copyToClipboardResultToolStripMenuItem.Checked = !pasteResultToolStripMenuItem.Checked;

            Properties.Settings.Default.Save();
        }
    }
}
