using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Resxul
{
    internal static class Global
    {
        public const string Name = "Resxul";

        public const string LogsFolderPath = "Logs";
        public const string ToolsFolderPath = "Tools";
        public const string ProfilesFolderPath = "Profiles";

        public const string LogFileName = "log.log";

        public const string Resgen = "resgen.exe";
        public const string Al = "al.exe";

        public const string ProgramReleaseNotesUri = "https://github.com/paulem/resxul/releases";

        //

        public static readonly string ExecutablePath = Assembly.GetEntryAssembly().Location;
        public static readonly string ExecutableFolderPath = Path.GetDirectoryName(ExecutablePath);

        public static readonly FileVersionInfo FileVersion = FileVersionInfo.GetVersionInfo(ExecutablePath);
    }
}
