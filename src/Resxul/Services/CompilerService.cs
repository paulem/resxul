using System;
using System.Diagnostics;
using System.IO;
using Resxul.Models;

namespace Resxul.Services
{
    internal class CompilerService
    {
        public void Compile(Profile profile, string resxFilePath, string toolsFolderPath, IProgress<string> progress)
        {
            string cultureName = null;

            if (resxFilePath.Split('.') is string[] parts && parts.Length == 3)
                cultureName = parts[1];

            if (string.IsNullOrEmpty(cultureName))
            {
                progress?.Report("Can't detect culture");
                return;
            }

            if (!Directory.Exists(profile.SatelliteAssemblyFolderPath))
                Directory.CreateDirectory(profile.SatelliteAssemblyFolderPath);

            var binName = Path.GetFileNameWithoutExtension(resxFilePath) + ".resources";
            var binFilePath = Path.Combine(profile.SatelliteAssemblyFolderPath, binName);

            var resgenProc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Path.Combine(toolsFolderPath, Global.Resgen),
                    Arguments = $"\"{resxFilePath}\" \"{binFilePath}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            resgenProc.Start();

            progress?.Report("- resgen\r\n");

            while (!resgenProc.StandardError.EndOfStream)
                progress?.Report(resgenProc.StandardError.ReadLine());

            while (!resgenProc.StandardOutput.EndOfStream)
                progress?.Report(resgenProc.StandardOutput.ReadLine());

            progress?.Report($"\r\n- resgen exit code: {resgenProc.ExitCode}\r\n");

            if (resgenProc.ExitCode != 0)
                return;

            //

            var binNamespace = $"{binName},{profile.ResxNamespace}.{binName}";
            var alArgs = $"/t:lib /embed:{binNamespace} /culture:{cultureName} /out:\"{profile.SatelliteAssemblyFilePath}\"";

            var alProc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Path.Combine(toolsFolderPath, Global.Al),
                    Arguments = alArgs,
                    WorkingDirectory = profile.SatelliteAssemblyFolderPath,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            alProc.Start();

            progress?.Report("- al\r\n");

            while (!alProc.StandardError.EndOfStream)
                progress?.Report(alProc.StandardError.ReadLine());

            while (!alProc.StandardOutput.EndOfStream)
                progress?.Report(alProc.StandardOutput.ReadLine());

            progress?.Report($"\r\n- al exit code: {alProc.ExitCode}");

            //

            if (File.Exists(binFilePath))
                File.Delete(binFilePath);
        }
    }
}
