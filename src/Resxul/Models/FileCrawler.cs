using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Resxul.Models
{
    internal sealed class FileCrawler
    {
        private readonly HashSet<string> _unavailableLocations;

        public FileCrawler()
        {
            Locations = new List<string>();
            _unavailableLocations = new HashSet<string>();
        }

        public IList<string> Locations { get; set; }
        public string TargetFileName { get; set; }

        public IReadOnlyCollection<string> UnavailableLocations => _unavailableLocations;

        public Task<FileInfo> SearchFileAsync(CancellationToken ct)
        {
            return Task.Run(() =>
            {
                foreach (string location in Locations)
                {
                    if (ct.IsCancellationRequested)
                        break;

                    FileInfo file = FindFile(location, TargetFileName, ct);

                    if (file != null)
                        return file;
                }

                return null;
            }, ct);
        }

        private FileInfo FindFile(string path, string fileName, CancellationToken ct)
        {
            FileInfo[] files = null;
            path = Environment.ExpandEnvironmentVariables(path);

            try
            {
                files = new DirectoryInfo(path).GetFiles();
            }
            catch (Exception)
            {
                _unavailableLocations.Add(path);
            }

            if (files != null)
            {
                foreach (FileInfo fileInfo in files)
                {
                    if (ct.IsCancellationRequested)
                        break;

                    if (fileInfo.Name.Equals(fileName, StringComparison.OrdinalIgnoreCase))
                        return fileInfo;
                }
            }

            //

            string[] directories = null;

            try
            {
                directories = Directory.GetDirectories(path);
            }
            catch (Exception)
            {
                _unavailableLocations.Add(path);
            }

            if (directories != null)
            {
                foreach (var subpath in directories)
                {
                    FileInfo fileInfo = FindFile(subpath, fileName, ct);

                    if (ct.IsCancellationRequested)
                        break;

                    if (fileInfo != null)
                        return fileInfo;
                }
            }

            return null;
        }
    }
}
