// IMPORTANTE: Paquetes NuGet requeridos: NAudio, Costura.Fody

using Hype.Helpers;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using static Hype.Helpers.Win32Helper;

namespace Hype
{
    public partial class HypeForm : Form
    {
        // --- COMPONENTES LÓGICOS ---
        private AudioManager audioManager;
        private TreeView treeApps;
        private HotkeyListener hotkeyListener;

        // --- VARIABLES DE ESTADO ---
        private int currentModifier = MOD_NONE;
        private int currentKey = (int)Keys.F8;
        private Keys currentKeyEnum = Keys.F8;

        private DateTime lastHotkeyPressTime = DateTime.MinValue;
        private bool ventanasOcultasNormal = false;
        private bool ventanasOcultasInvertido = false;
        private bool modoInvertido = false;
        private bool isListeningForHotkey = false;
        private bool isProcessingAction = false;
        private bool useEnglish = false;
        private bool isDragging = false;

        private Point dragCursorPoint;
        private Point dragFormPoint;
        private IntPtr lastActiveWindow = IntPtr.Zero;

        public delegate bool EnumWindowsProc(IntPtr hWnd, int lParam);
        private Dictionary<IntPtr, string> windowsFound = new Dictionary<IntPtr, string>();

        private HashSet<IntPtr> checkedHandles = new HashSet<IntPtr>();
        private readonly string recoveryFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "HypeState.dat");
        private HashSet<IntPtr> recoveryHandles = new HashSet<IntPtr>();
        private HashSet<IntPtr> hiddenByUs = new HashSet<IntPtr>();
        private bool updatingTree = false;

        public HypeForm()
        {
            InitializeComponent();
            this.Text = "HYPE";
            try { this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); } catch { }
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadRecoveryState();
            audioManager = new AudioManager();

            this.KeyPreview = true;
            this.TopMost = true;

            Rectangle areaTrabajo = Screen.PrimaryScreen.WorkingArea;
            int x = areaTrabajo.Right - this.Width - 15;
            int y = areaTrabajo.Bottom - this.Height - 15;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(x, y);

            InicializarDiseñoCompleto();

            currentModifier = Hype.Properties.Settings.Default.SavedHotkeyMod;
            currentKey = (Hype.Properties.Settings.Default.SavedHotkeyValue == 0) ? (int)Keys.F8 : Hype.Properties.Settings.Default.SavedHotkeyValue;
            currentKeyEnum = (Keys)currentKey;
            UpdateHotkeyButtonText();

            Action hotkeyAction = () => { OnHotkeyPress(); };
            hotkeyListener = new HotkeyListener(hotkeyAction);
            if (!hotkeyListener.Register(currentModifier, currentKey))
            {
                MessageBox.Show("No se pudo registrar la tecla global.");
            }

            modoInvertido = Hype.Properties.Settings.Default.SavedInvertMode;
            UpdateToggleModeLabel();

            if (Hype.Properties.Settings.Default.SavedChecks == null)
                Hype.Properties.Settings.Default.SavedChecks = new System.Collections.Specialized.StringCollection();

            pbToggleMode.Paint += new PaintEventHandler(pbToggleMode_Paint);
            pbNoob.Click += PbNoob_Click;

            timer1_Tick(null, null);

            if (ventanasOcultasNormal || ventanasOcultasInvertido)
            {
                this.WindowState = FormWindowState.Minimized;
                this.ShowInTaskbar = false;
                this.Hide();
                notifyIcon1.Visible = false;
            }
            else
            {
                this.ShowInTaskbar = false;
            }
        }

        private int CountVisibleWindows(int processId, IntPtr handleToIgnore)
        {
            int count = 0;
            EnumWindows((hWnd, lParam) =>
            {
                if (hWnd == handleToIgnore) return true;
                if (IsWindowVisible(hWnd))
                {
                    int windowPid;
                    GetWindowThreadProcessId(hWnd, out windowPid);
                    if (windowPid == processId) count++;
                }
                return true;
            }, 0);
            return count;
        }

        private void SaveState(bool isHiddenMode, List<IntPtr> hiddenHandles)
        {
            try
            {
                if (!isHiddenMode)
                {
                    if (File.Exists(recoveryFile)) File.Delete(recoveryFile);
                    return;
                }
                StringBuilder sb = new StringBuilder();
                sb.Append("TRUE|");
                foreach (var handle in hiddenHandles) sb.Append(handle.ToInt64() + ",");
                File.WriteAllText(recoveryFile, sb.ToString());
            }
            catch { }
        }

        private void LoadRecoveryState()
        {
            try
            {
                if (File.Exists(recoveryFile))
                {
                    string content = File.ReadAllText(recoveryFile);
                    if (content.StartsWith("TRUE|"))
                    {
                        string[] handles = content.Substring(5).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string h in handles)
                        {
                            if (long.TryParse(h, out long val))
                            {
                                IntPtr ptr = (IntPtr)val;
                                recoveryHandles.Add(ptr);
                                hiddenByUs.Add(ptr);
                            }
                        }
                        if (recoveryHandles.Count > 0)
                        {
                            ventanasOcultasNormal = true;
                            this.ShowInTaskbar = false;
                            this.ShowInTaskbar = false;
                            notifyIcon1.Visible = false;
                        }
                    }
                }
            }
            catch { }
        }

        private void OnHotkeyPress()
        {
            if (isProcessingAction) return;
            if ((DateTime.Now - lastHotkeyPressTime).TotalMilliseconds < 500) return;
            lastHotkeyPressTime = DateTime.Now;
            isProcessingAction = true;

            try
            {
                bool panicoActivo = modoInvertido ? ventanasOcultasInvertido : ventanasOcultasNormal;
                List<IntPtr> hiddenHandles = new List<IntPtr>();

                if (!panicoActivo)
                {
                    lastActiveWindow = GetForegroundWindow();
                    if (modoInvertido) { ventanasOcultasInvertido = true; hiddenHandles = ProcessVisibility(0, true); }
                    else { ventanasOcultasNormal = true; hiddenHandles = ProcessVisibility(SW_HIDE, false); }
                    SaveState(true, hiddenHandles);
                    ActivarSenuelo();
                    this.Hide();
                    this.ShowInTaskbar = false;
                    notifyIcon1.Visible = false;
                }
                else
                {
                    if (modoInvertido) { ventanasOcultasInvertido = false; ProcessVisibility(0, true); }
                    else { ventanasOcultasNormal = false; ProcessVisibility(SW_SHOW, false); }
                    SaveState(false, null);
                    if (lastActiveWindow != IntPtr.Zero)
                    {
                        if (IsIconic(lastActiveWindow)) ShowWindow(lastActiveWindow, SW_RESTORE);
                        SetForegroundWindow(lastActiveWindow);
                    }
                    MostrarHypeCorrectamente();
                }
            }
            finally { isProcessingAction = false; }
        }

        private List<IntPtr> ProcessVisibility(int command, bool invert)
        {
            List<IntPtr> affected = new List<IntPtr>();
            foreach (TreeNode parent in treeApps.Nodes)
            {
                foreach (TreeNode child in parent.Nodes)
                {
                    IntPtr handle = (IntPtr)child.Tag;
                    bool isChecked = checkedHandles.Contains(handle);
                    bool didHide = false;
                    try
                    {
                        if (invert)
                        {
                            if (ventanasOcultasInvertido)
                            {
                                SetWindowState(handle, isChecked ? SW_SHOW : SW_HIDE);
                                if (!isChecked) didHide = true;
                            }
                            else SetWindowState(handle, SW_SHOW);
                        }
                        else
                        {
                            if (isChecked)
                            {
                                SetWindowState(handle, command);
                                if (command == SW_HIDE) didHide = true;
                            }
                        }
                        if (didHide) affected.Add(handle);
                    }
                    catch { }
                }
            }
            return affected;
        }

        private void SetWindowState(IntPtr handle, int command)
        {
            ShowWindow(handle, command);
            if (command == SW_HIDE) hiddenByUs.Add(handle);
            else hiddenByUs.Remove(handle);
            try
            {
                int targetPid;
                GetWindowThreadProcessId(handle, out targetPid);
                if (targetPid <= 0) return;
                bool shouldMute = (command == SW_HIDE);
                bool safeToMute = true;
                if (shouldMute && CountVisibleWindows(targetPid, handle) > 0) safeToMute = false;
                audioManager.SetProcessMute(targetPid, shouldMute, safeToMute);
            }
            catch { }
        }

        private void MostrarHypeCorrectamente()
        {
            this.Show();
            if (this.WindowState == FormWindowState.Minimized) this.WindowState = FormWindowState.Normal;
            this.Update();
            Rectangle area = Screen.PrimaryScreen.WorkingArea;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(area.Right - this.Width - 15, area.Bottom - this.Height - 15);
            this.TopMost = false;
            this.TopMost = true;
            this.Activate();
            SetForegroundWindow(this.Handle);
            this.ShowInTaskbar = false;
            notifyIcon1.Visible = true;
        }

        private void ActivarSenuelo()
        {
            try
            {
                if (cbSenuelo.SelectedIndex > 0)
                {
                    string titulo = cbSenuelo.SelectedItem.ToString();
                    foreach (TreeNode p in treeApps.Nodes)
                        foreach (TreeNode c in p.Nodes)
                            if (c.Text == titulo)
                            {
                                IntPtr h = (IntPtr)c.Tag;
                                if (IsIconic(h)) ShowWindow(h, SW_RESTORE);
                                SetForegroundWindow(h);
                                return;
                            }
                }
            }
            catch { }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (isProcessingAction || updatingTree) return;
            try
            {
                windowsFound.Clear();
                EnumWindows(EnumerateWindowsCallback, 0);
                var currentGroups = new Dictionary<string, List<WindowInfo>>();
                foreach (var window in windowsFound)
                {
                    IntPtr handle = window.Key;
                    string title = window.Value;
                    int processId;
                    GetWindowThreadProcessId(handle, out processId);
                    try
                    {
                        Process proceso = Process.GetProcessById(processId);
                        if (proceso.Id == Process.GetCurrentProcess().Id) continue;
                        string groupName = proceso.ProcessName;
                        if (!currentGroups.ContainsKey(groupName)) currentGroups[groupName] = new List<WindowInfo>();
                        currentGroups[groupName].Add(new WindowInfo { Handle = handle, Title = $"{title} ({processId})", RawTitle = title });
                    }
                    catch { }
                }

                foreach (var handleRecuperado in recoveryHandles.ToList())
                {
                    checkedHandles.Add(handleRecuperado);
                    recoveryHandles.Remove(handleRecuperado);
                }

                treeApps.BeginUpdate();
                updatingTree = true;
                List<TreeNode> nodesToRemove = new List<TreeNode>();
                foreach (TreeNode parentNode in treeApps.Nodes)
                    if (!currentGroups.ContainsKey(parentNode.Text)) nodesToRemove.Add(parentNode);
                foreach (TreeNode n in nodesToRemove) treeApps.Nodes.Remove(n);

                foreach (var group in currentGroups)
                {
                    string groupName = group.Key;
                    List<WindowInfo> windows = group.Value;
                    TreeNode parentNode = null;
                    foreach (TreeNode n in treeApps.Nodes) { if (n.Text == groupName) { parentNode = n; break; } }
                    if (parentNode == null) { parentNode = new TreeNode(groupName); treeApps.Nodes.Add(parentNode); }

                    List<TreeNode> childrenToRemove = new List<TreeNode>();
                    foreach (TreeNode child in parentNode.Nodes)
                    {
                        IntPtr childHandle = (IntPtr)child.Tag;
                        if (!windows.Any(w => w.Handle == childHandle)) childrenToRemove.Add(child);
                    }
                    foreach (TreeNode c in childrenToRemove) parentNode.Nodes.Remove(c);

                    foreach (var winInfo in windows)
                    {
                        bool exists = false;
                        foreach (TreeNode child in parentNode.Nodes)
                        {
                            if ((IntPtr)child.Tag == winInfo.Handle)
                            {
                                exists = true;
                                if (child.Text != winInfo.Title) child.Text = winInfo.Title;
                                bool shouldBeChecked = checkedHandles.Contains(winInfo.Handle);
                                if (child.Checked != shouldBeChecked) child.Checked = shouldBeChecked;
                                break;
                            }
                        }
                        if (!exists)
                        {
                            TreeNode newChild = new TreeNode(winInfo.Title);
                            newChild.Tag = winInfo.Handle;
                            bool isSaved = checkedHandles.Contains(winInfo.Handle);
                            if (!isSaved && Hype.Properties.Settings.Default.SavedChecks != null &&
                                Hype.Properties.Settings.Default.SavedChecks.Contains(winInfo.Title))
                            {
                                isSaved = true;
                            }
                            if (parentNode.Checked)
                            {
                                isSaved = true;
                            }
                            if (isSaved)
                            {
                                newChild.Checked = true;
                                checkedHandles.Add(winInfo.Handle);
                                bool panicoActivo = modoInvertido ? ventanasOcultasInvertido : ventanasOcultasNormal;
                                if (panicoActivo)
                                {
                                    if (!modoInvertido) ApplyInstantStateChange(winInfo.Handle, true);
                                    else ApplyInstantStateChange(winInfo.Handle, true);
                                }
                            }
                            parentNode.Nodes.Add(newChild);
                        }
                    }
                }

                foreach (TreeNode parent in treeApps.Nodes)
                {
                    if (parent.Nodes.Count == 0) continue;
                    bool allChecked = true;
                    foreach (TreeNode child in parent.Nodes) if (!child.Checked) allChecked = false;
                    if (parent.Checked != allChecked) parent.Checked = allChecked;
                }
                UpdateSenueloCombo();
            }
            catch { }
            finally { updatingTree = false; treeApps.EndUpdate(); }
        }

        private bool EnumerateWindowsCallback(IntPtr hWnd, int lParam)
        {
            bool isRecoveringThisWindow = recoveryHandles.Contains(hWnd);
            bool isHiddenByUs = hiddenByUs.Contains(hWnd);
            if (!IsWindowVisible(hWnd) && !isRecoveringThisWindow && !isHiddenByUs) return true;
            StringBuilder sb = new StringBuilder(256);
            GetWindowText(hWnd, sb, sb.Capacity);
            string title = sb.ToString().Trim();
            if (string.IsNullOrEmpty(title) || title.Length == 0) return true;
            if (title == "Default IME" || title == "Cortana" || title == "HYPE") return true;
            if (!windowsFound.ContainsKey(hWnd)) windowsFound.Add(hWnd, title);
            return true;
        }

        private void TreeApps_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (updatingTree) return;
            updatingTree = true;
            try
            {
                TreeNode node = e.Node;
                bool state = node.Checked;
                if (node.Nodes.Count > 0)
                {
                    foreach (TreeNode child in node.Nodes)
                    {
                        child.Checked = state;
                        UpdateHandleCheckState(child, state);
                    }
                }
                else UpdateHandleCheckState(node, state);
            }
            finally { updatingTree = false; }
        }

        private void UpdateHandleCheckState(TreeNode node, bool isChecked)
        {
            if (node.Tag is IntPtr handle)
            {
                if (isChecked) checkedHandles.Add(handle);
                else checkedHandles.Remove(handle);
                ApplyInstantStateChange(handle, isChecked);
            }
        }

        private void ApplyInstantStateChange(IntPtr handle, bool isChecked)
        {
            if (isProcessingAction) return;
            try
            {
                if (modoInvertido && ventanasOcultasInvertido) SetWindowState(handle, isChecked ? SW_SHOW : SW_HIDE);
                else if (!modoInvertido && ventanasOcultasNormal) SetWindowState(handle, isChecked ? SW_HIDE : SW_SHOW);
            }
            catch { }
        }

        private struct WindowInfo { public IntPtr Handle; public string Title; public string RawTitle; }

        private void PbNoob_Click(object sender, EventArgs e) { using (AboutForm about = new AboutForm(pbNoob.Image, useEnglish)) about.ShowDialog(this); }
        private void pbToggleMode_Click(object sender, EventArgs e) => ToggleMode_Click(sender, e);

        private void ToggleMode_Click(object sender, EventArgs e)
        {
            isProcessingAction = true;
            modoInvertido = !modoInvertido;
            if (modoInvertido) { ventanasOcultasNormal = false; ProcessVisibility(SW_SHOW, false); }
            else { ventanasOcultasInvertido = false; ProcessVisibility(0, true); }
            UpdateToggleModeLabel();
            pbToggleMode.Invalidate();
            isProcessingAction = false;
        }

        private void lblHotkey_Click(object sender, EventArgs e)
        {
            isListeningForHotkey = true;
            lblHotkeyLabel.Visible = false;
            lblHotkey.Text = useEnglish ? "Press a key..." : "Presiona una tecla...";
            lblHotkey.Location = new Point((this.ClientSize.Width - lblHotkey.Width) / 2, 78);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!isListeningForHotkey) return;
            e.SuppressKeyPress = true;
            if (e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.Menu) return;
            int nm = MOD_NONE;
            if (e.Control) nm |= MOD_CONTROL;
            if (e.Alt) nm |= MOD_ALT;
            if (e.Shift) nm |= MOD_SHIFT;
            if (hotkeyListener.Register(nm, (int)e.KeyCode)) { currentModifier = nm; currentKey = (int)e.KeyCode; currentKeyEnum = e.KeyCode; }
            else { MessageBox.Show("Tecla ocupada."); hotkeyListener.Register(currentModifier, currentKey); }
            isListeningForHotkey = false;
            UpdateHotkeyButtonText();
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e) { isDragging = true; dragCursorPoint = Cursor.Position; dragFormPoint = this.Location; }
        private void Form1_MouseMove(object sender, MouseEventArgs e) { if (isDragging) { Point dif = Point.Subtract(Cursor.Position, new Size(dragCursorPoint)); this.Location = Point.Add(dragFormPoint, new Size(dif)); } }
        private void Form1_MouseUp(object sender, MouseEventArgs e) => isDragging = false;
        private void lblClose_Click(object sender, EventArgs e) => Application.Exit();
        private void lblMinimize_Click(object sender, EventArgs e) => this.WindowState = FormWindowState.Minimized;

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                this.ShowInTaskbar = false;
                notifyIcon1.Visible = true;
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (isProcessingAction) return;
            isProcessingAction = true;
            try { ventanasOcultasNormal = false; ventanasOcultasInvertido = false; ProcessVisibility(SW_SHOW, false); MostrarHypeCorrectamente(); }
            finally { isProcessingAction = false; }
        }

        private void mostrarToolStripMenuItem_Click(object sender, EventArgs e) => notifyIcon1_MouseDoubleClick(sender, null);
        private void mostrarTodoToolStripMenuItem_Click(object sender, EventArgs e) => notifyIcon1_MouseDoubleClick(sender, null);
        private void salirToolStripMenuItem_Click(object sender, EventArgs e) => Application.Exit();

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            hotkeyListener?.Dispose();
            foreach (TreeNode p in treeApps.Nodes) foreach (TreeNode c in p.Nodes) try { SetWindowState((IntPtr)c.Tag, SW_SHOW); } catch { }
            if (Hype.Properties.Settings.Default.SavedChecks != null)
            {
                Hype.Properties.Settings.Default.SavedChecks.Clear();
                foreach (IntPtr h in checkedHandles)
                    foreach (TreeNode p in treeApps.Nodes) foreach (TreeNode c in p.Nodes) if ((IntPtr)c.Tag == h) Hype.Properties.Settings.Default.SavedChecks.Add(c.Text);
            }
            Hype.Properties.Settings.Default.Save();
        }

        private void ListaDeAplicaciones_MouseClick(object sender, MouseEventArgs e) { }
        private void ListaDeAplicaciones_MouseDoubleClick(object sender, MouseEventArgs e) { }
        private void ListaDeAplicaciones_DrawItem(object sender, DrawItemEventArgs e) { }
    }
}
