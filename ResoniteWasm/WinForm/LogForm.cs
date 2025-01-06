using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

using Elements.Core;

using ResoniteWasm.Config;
using ResoniteWasm.Util;

#pragma warning disable CS8618

namespace ResoniteWasm.WinForm;

public class LogForm : Form {
    private MenuStrip mMenuStrip;
    private ToolStripMenuItem mAutoScrollMenuItem;
    private ToolStripMenuItem mClearMenuItem;
    private ToolStripMenuItem mLogLengthToolStripMenuItem;
    private ToolStripMenuItem mLengthMenuItem50;
    private ToolStripMenuItem mLengthMenuItem20;
    private ToolStripMenuItem mLengthMenuItem5;
    private ToolStripMenuItem mLengthMenuItem1;
    private ToolStripMenuItem mLengthMenuItemUnLimit;
    private RichTextBox mLogView;
    private bool mAutoScroll = true;
    private int mLogLength = 50000;

    public LogForm() {
        InitializeComponent();
        Init();
        ClientSize = new Size(1280, 720);
    }

    protected override void OnClosing(CancelEventArgs e) {
        base.OnClosing(e);
        if (ConsoleLogger.ShowLog) {
            ModConfig.Set(ConfigDefinition.ShowConsoleLog, false);
        }
    }

    public void OnLog(string obj) => AddLog(obj, Color.White);

    public void OnWarning(string obj) => AddLog(obj, Color.Orange);

    public void OnError(string obj) => AddLog(obj, Color.LightCoral);

    private void AddLog(string obj, Color color) {
        if (IsUiThread) Action();
        else RunOnUIThread(Action);
        return;

        void Action() {
            if (mLogLength > 0 && mLogView.TextLength + obj.Length > mLogLength) {
                mLogView.Select(0, obj.Length + mLogView.Text.Length - mLogLength);
                mLogView.SelectedText = string.Empty;
            }

            mLogView.SelectionStart = mLogView.TextLength;
            mLogView.SelectionLength = 0;
            mLogView.SelectionColor = color;
            mLogView.AppendText(obj + Environment.NewLine);
            mLogView.ScrollToCaret();
        }
    }

    public void ChangeLanguage() {
        Text = "WinForm.LogWindow.Title".Local();
        mAutoScrollMenuItem.Text = "WinForm.LogWindow.AutoScroll.Text".Local(mAutoScroll ? "\u2611" : "\u2612");
        mClearMenuItem.Text = "WinForm.LogWindow.Clear.Text".Local();
        mLogLengthToolStripMenuItem.Text = "WinForm.LogWindow.LogLength.Text".Local();
        mLengthMenuItemUnLimit.Text = "WinForm.LogWindow.LogLength.UnLimit.Text".Local();
    }

    private void Init() {
        // Auto Scroll
        mAutoScrollMenuItem.Click += (_, _) => {
            mAutoScroll = !mAutoScroll;
            mAutoScrollMenuItem.Text = "WinForm.LogWindow.AutoScroll.Text".Local(mAutoScroll ? "\u2611" : "\u2612");
        };
        
        // LogLimit
        var limits = new[] {
            mLengthMenuItem50, mLengthMenuItem20, mLengthMenuItem5, mLengthMenuItem1, mLengthMenuItemUnLimit
        };
        foreach (var it in limits) {
            it.Click += (sender, _) => {
                var obj = sender as ToolStripMenuItem;
                var length = int.Parse(obj.Tag as string);
                mLogLength = length;
                foreach (var l in limits) l.Checked = false;

                obj.Checked = true;
            };
        }
        
        // Clear
        mClearMenuItem.Click += (_, _) => {
            mLogView.Text = string.Empty;
        };
    }

    private void InitializeComponent() {
            this.mLogView = new System.Windows.Forms.RichTextBox();
            this.mMenuStrip = new System.Windows.Forms.MenuStrip();
            this.mAutoScrollMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mClearMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mLogLengthToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mLengthMenuItem50 = new System.Windows.Forms.ToolStripMenuItem();
            this.mLengthMenuItem20 = new System.Windows.Forms.ToolStripMenuItem();
            this.mLengthMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.mLengthMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.mLengthMenuItemUnLimit = new System.Windows.Forms.ToolStripMenuItem();
            this.mMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // mLogView
            // 
            this.mLogView.BackColor = System.Drawing.SystemColors.Desktop;
            this.mLogView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mLogView.Font = new System.Drawing.Font("黑体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.mLogView.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.mLogView.Location = new System.Drawing.Point(0, 31);
            this.mLogView.Name = "mLogView";
            this.mLogView.ReadOnly = true;
            this.mLogView.Size = new System.Drawing.Size(984, 480);
            this.mLogView.TabIndex = 0;
            this.mLogView.Text = "";
            // 
            // mMenuStrip
            // 
            this.mMenuStrip.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F);
            this.mMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.mMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mAutoScrollMenuItem,
            this.mClearMenuItem,
            this.mLogLengthToolStripMenuItem});
            this.mMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.mMenuStrip.Name = "mMenuStrip";
            this.mMenuStrip.Size = new System.Drawing.Size(984, 31);
            this.mMenuStrip.TabIndex = 1;
            this.mMenuStrip.Text = "menuStrip1";
            // 
            // mAutoScrollMenuItem
            // 
            this.mAutoScrollMenuItem.Name = "mAutoScrollMenuItem";
            this.mAutoScrollMenuItem.Size = new System.Drawing.Size(109, 27);
            this.mAutoScrollMenuItem.Text = "AutoScroll";
            // 
            // mClearMenuItem
            // 
            this.mClearMenuItem.Name = "mClearMenuItem";
            this.mClearMenuItem.Size = new System.Drawing.Size(65, 27);
            this.mClearMenuItem.Text = "Clear";
            // 
            // mLogLengthToolStripMenuItem
            // 
            this.mLogLengthToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mLengthMenuItem50,
            this.mLengthMenuItem20,
            this.mLengthMenuItem5,
            this.mLengthMenuItem1,
            this.mLengthMenuItemUnLimit});
            this.mLogLengthToolStripMenuItem.Name = "mLogLengthToolStripMenuItem";
            this.mLogLengthToolStripMenuItem.Size = new System.Drawing.Size(111, 27);
            this.mLogLengthToolStripMenuItem.Text = "LogLength";
            // 
            // mLengthMenuItem50
            // 
            this.mLengthMenuItem50.Checked = true;
            this.mLengthMenuItem50.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mLengthMenuItem50.Name = "mLengthMenuItem50";
            this.mLengthMenuItem50.Size = new System.Drawing.Size(224, 26);
            this.mLengthMenuItem50.Tag = "50000";
            this.mLengthMenuItem50.Text = "50000";
            // 
            // mLengthMenuItem20
            // 
            this.mLengthMenuItem20.Name = "mLengthMenuItem20";
            this.mLengthMenuItem20.Size = new System.Drawing.Size(224, 26);
            this.mLengthMenuItem20.Tag = "20000";
            this.mLengthMenuItem20.Text = "20000";
            // 
            // mLengthMenuItem5
            // 
            this.mLengthMenuItem5.Name = "mLengthMenuItem5";
            this.mLengthMenuItem5.Size = new System.Drawing.Size(224, 26);
            this.mLengthMenuItem5.Tag = "5000";
            this.mLengthMenuItem5.Text = "5000";
            // 
            // mLengthMenuItem1
            // 
            this.mLengthMenuItem1.Name = "mLengthMenuItem1";
            this.mLengthMenuItem1.Size = new System.Drawing.Size(224, 26);
            this.mLengthMenuItem1.Tag = "1000";
            this.mLengthMenuItem1.Text = "1000";
            // 
            // mLengthMenuItemUnLimit
            // 
            this.mLengthMenuItemUnLimit.Name = "mLengthMenuItemUnLimit";
            this.mLengthMenuItemUnLimit.Size = new System.Drawing.Size(224, 26);
            this.mLengthMenuItemUnLimit.Tag = "-1";
            this.mLengthMenuItemUnLimit.Text = "UnLimit";
            // 
            // LogForm
            // 
            this.BackColor = System.Drawing.SystemColors.Desktop;
            this.ClientSize = new System.Drawing.Size(984, 511);
            this.Controls.Add(this.mLogView);
            this.Controls.Add(this.mMenuStrip);
            this.MainMenuStrip = this.mMenuStrip;
            this.Name = "LogForm";
            this.ShowIcon = false;
            this.Text = "Resonite Log";
            this.mMenuStrip.ResumeLayout(false);
            this.mMenuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

    }
}
