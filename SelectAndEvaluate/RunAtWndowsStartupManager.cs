﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using System.Windows.Forms;

namespace SelectAndEvaluate
{
    public class RunAtWndowsStartupManager
    {        
        public static bool RunAtWindowsStartup
        {
            get
            {
                RegistryKey key = Registry.CurrentUser;

                try
                {
                    key = key.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);

                    return (key.GetValue("SelectAndEvaluate") != null);
                }
                catch
                {
                    return false;
                }                
            }
            set
            {
                bool enable = value;

                RegistryKey key = Registry.CurrentUser;

                try
                {
                    key = key.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);

                    if (key == null)
                    {
                        Module.ShowMessage("Error. Could not Save if Application will start automatically with Windows");
                        //return;

                        return;
                    }

                    if (enable)
                    {
                        if (key.GetValue("SelectAndEvaluate") == null)
                        {
                            key.SetValue("SelectAndEvaluate", "\"" + Application.StartupPath + "\\SelectAndEvaluate.exe\" /hide");
                        }
                    }
                    else
                    {
                        if (key.GetValue("SelectAndEvaluate") != null)
                        {
                            key.DeleteValue("SelectAndEvaluate");
                        }
                    }

                }
                catch (Exception ex)
                {
                    Module.ShowMessage("Error. Could not Save if Application will start automatically with Windows");
                    //return;

                    return;
                }
            }
        }
    }
}
