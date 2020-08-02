using System;
using System.Runtime.InteropServices;
using Dalamud.Plugin;
using System.Diagnostics;

namespace WindowTitleChanger
{
    public class WindowTitleChanger : IDalamudPlugin
    {
        [DllImport("user32.dll")]
        static extern int SetWindowText(IntPtr hWnd, string text);

        public string Name => "Window Title Changer";
        private string OriginalTitle { get; set; }
        private DalamudPluginInterface PluginInterface { get; set; }
        private IntPtr Handle { get; set; }

        public void Initialize(DalamudPluginInterface pluginInterface)
        {
            this.PluginInterface = pluginInterface;

            Process process = Process.GetCurrentProcess();
            this.Handle = process.MainWindowHandle;
            this.OriginalTitle = process.MainWindowTitle;

            pluginInterface.CommandManager.AddHandler("/wintitle", new Dalamud.Game.Command.CommandInfo((string command, string arguments) =>
            {
                if (string.IsNullOrWhiteSpace(arguments)) arguments = this.OriginalTitle;
                try { SetWindowText(this.Handle, arguments.Trim()); } catch (Exception e) { PluginLog.LogError($"{e}"); }
            }));

            process.Dispose();
        }

        public void Dispose()
        {
            try { SetWindowText(this.Handle, this.OriginalTitle); } catch (Exception e) { PluginLog.LogError($"{e}"); }
            this.PluginInterface.CommandManager.RemoveHandler("/wintitle");
            this.PluginInterface.Dispose();
        }
    }
}
