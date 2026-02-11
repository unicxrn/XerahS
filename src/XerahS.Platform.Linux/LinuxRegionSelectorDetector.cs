#region License Information (GPL v3)

/*
    XerahS - The Avalonia UI implementation of ShareX
    Copyright (c) 2007-2026 ShareX Team

    This program is free software; you can redistribute it and/or
    modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 2
    of the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

    Optionally you can also view the license at <http://www.gnu.org/licenses/>.
*/

#endregion License Information (GPL v3)

using System.Diagnostics;
using XerahS.Common;

namespace XerahS.Platform.Linux
{
    /// <summary>
    /// Detects which region selection tools are installed on the system.
    /// Results are cached to avoid repeated 'which' calls.
    /// </summary>
    public static class LinuxRegionSelectorDetector
    {
        private static readonly Lazy<HashSet<string>> _availableTools = new(DetectAvailableTools);

        /// <summary>
        /// Known region selector tool names that can be checked.
        /// </summary>
        public static readonly IReadOnlyList<string> KnownTools = new[]
        {
            "slurp",
            "spectacle",
            "gnome-screenshot",
            "xfce4-screenshooter"
        };

        /// <summary>
        /// Check if a specific tool is available on the system.
        /// </summary>
        public static bool IsToolAvailable(string toolName)
        {
            return _availableTools.Value.Contains(toolName);
        }

        /// <summary>
        /// Get the set of all detected tool names that are installed.
        /// Always includes "portal" since XDG Portal is available on modern Linux.
        /// </summary>
        public static IReadOnlySet<string> GetAvailableTools()
        {
            return _availableTools.Value;
        }

        private static HashSet<string> DetectAvailableTools()
        {
            var available = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "portal" // XDG Portal is always considered available on modern Linux
            };

            foreach (var tool in KnownTools)
            {
                if (CheckWhich(tool))
                {
                    available.Add(tool);
                    DebugHelper.WriteLine($"LinuxRegionSelectorDetector: '{tool}' is available");
                }
            }

            DebugHelper.WriteLine($"LinuxRegionSelectorDetector: Detected tools: {string.Join(", ", available)}");
            return available;
        }

        private static bool CheckWhich(string toolName)
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = "which",
                    Arguments = toolName,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                };
                using var process = Process.Start(startInfo);
                process?.WaitForExit(3000);
                return process?.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }
    }
}
