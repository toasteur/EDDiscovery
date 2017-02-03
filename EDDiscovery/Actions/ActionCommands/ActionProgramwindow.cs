﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EDDiscovery.Actions
{
    class ActionProgramwindow : Action
    {
        public override bool AllowDirectEditingOfUserData { get { return true; } }

        public override bool ConfigurationMenu(Form parent, EDDiscovery2.EDDTheme theme, List<string> eventvars)
        {
            string promptValue = PromptSingleLine.ShowDialog(parent, "ProgramWindow command", UserData, "Configure Program Window Command");
            if (promptValue != null)
            {
                userdata = promptValue;
            }

            return (promptValue != null);
        }

        public override bool ExecuteAction(ActionProgramRun ap)
        {
            string res;
            if (ap.functions.ExpandString(UserData, ap.currentvars, out res) != ConditionLists.ExpandResult.Failed)
            {
                StringParser sp = new StringParser(res);

                string nextcmd = sp.NextWord(" ",true);

                if (nextcmd == null)
                {
                    ap.ReportError("Missing command in ProgramWindow");
                }
                else if (nextcmd.Equals("tab"))
                {
                    string tabname = sp.NextWord(" ", true);
                    if (!ap.actioncontroller.DiscoveryForm.SelectTabPage(tabname))
                        ap.ReportError("Tab page name " + tabname + " not found");
                }
                else if (nextcmd.Equals("topmost"))
                    ap.actioncontroller.DiscoveryForm.TopMost = true;
                else if (nextcmd.Equals("normalz"))
                    ap.actioncontroller.DiscoveryForm.TopMost = false;
                else if (nextcmd.Equals("showintaskbar"))
                    ap.actioncontroller.DiscoveryForm.ShowInTaskbar = true;
                else if (nextcmd.Equals("notshowintaskbar"))
                    ap.actioncontroller.DiscoveryForm.ShowInTaskbar = false;
                else if (nextcmd.Equals("minimize"))
                    ap.actioncontroller.DiscoveryForm.WindowState = FormWindowState.Minimized;
                else if (nextcmd.Equals("normal"))
                    ap.actioncontroller.DiscoveryForm.WindowState = FormWindowState.Normal;
                else if (nextcmd.Equals("maximize"))
                    ap.actioncontroller.DiscoveryForm.WindowState = FormWindowState.Maximized;
                else if (nextcmd.Equals("location"))
                {
                    int? x = sp.GetInt();
                    sp.IsCharMoveOn(',');
                    int? y = sp.GetInt();
                    sp.IsCharMoveOn(',');
                    int? w = sp.GetInt();
                    sp.IsCharMoveOn(',');
                    int? h = sp.GetInt();

                    if (x.HasValue && y.HasValue && w.HasValue && h.HasValue)
                    {
                        ap.actioncontroller.DiscoveryForm.Location = new Point(x.Value, y.Value);
                        ap.actioncontroller.DiscoveryForm.Size = new Size(w.Value, h.Value);
                    }
                    else
                        ap.ReportError("Location needs x,y,w,h in Popout");
                }
                else if (nextcmd.Equals("position"))
                {
                    int? x = sp.GetInt();
                    sp.IsCharMoveOn(',');
                    int? y = sp.GetInt();
                    sp.IsCharMoveOn(',');

                    if (x.HasValue && y.HasValue)
                        ap.actioncontroller.DiscoveryForm.Location = new Point(x.Value, y.Value);
                    else
                        ap.ReportError("Position needs x,y in Popout");
                }
                else if (nextcmd.Equals("size"))
                {
                    int? w = sp.GetInt();
                    sp.IsCharMoveOn(',');
                    int? h = sp.GetInt();

                    if (w.HasValue && h.HasValue)
                        ap.actioncontroller.DiscoveryForm.Size = new Size(w.Value, h.Value);
                    else
                        ap.ReportError("Size needs x,y,w,h in Popout");
                }
                else
                    ap.ReportError("Unknown command " + nextcmd + " in Popout");
            }
            else
                ap.ReportError(res);

            return true;
        }

    }
}