using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Caliburn.Micro;
using Resxul.Framework;
using Resxul.Models;
using Resxul.Properties;
using Resxul.Services;
using Serilog;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace Resxul.ViewModels
{
    internal sealed class MainViewModel : ValidatableScreen, IShell
    {
        #region Fields

        private readonly IWindowManager _windowManager;
        private readonly ProfileService _profileService;
        private readonly CompilerService _compilerService;

        private string _toolsFolderPath = Global.ToolsFolderPath;
        private string _resxFilePath;
        private bool _isRunApplicationAfterCompile;
        private string _logMessage;
        private Profile _selectedProfile;
        private readonly CancellationTokenSource _ctsTools;
        private bool _isOpenOutputFolderAfterCompile;
        private bool _isSearchingTools;
        private bool _isToolsSearchBoxVisible;
        private bool _canRunApplicationAfterCompile;
        private bool _canCompile;

        private Profile _resolvedProfile;

        #endregion

        #region Ctor

        public MainViewModel()
        {
            DisplayName = Global.Name;
        }

        public MainViewModel(IWindowManager windowManager, ProfileService profileService, CompilerService compilerService) : this()
        {
            _windowManager = windowManager;
            _profileService = profileService;
            _compilerService = compilerService;

            _ctsTools = new CancellationTokenSource();

            Summary = new BindableCollection<KeyValuePair<string, string>>();
            Profiles = new BindableCollection<Profile>(_profileService.Profiles);

            ((INotifyCollectionChanged)_profileService.Profiles).CollectionChanged += (sender, args) =>
            {
                switch (args.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        Profiles.AddRange(args.NewItems.Cast<Profile>());
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        Profiles.RemoveRange(args.OldItems.Cast<Profile>());
                        break;
                }
            };

            //

            if (!ToolsExistValidation.CheckToolsExistence(ToolsFolderPath))
                FindToolsAsync();

            //

            Validate();
        }

        #endregion

        #region Properties

        public string ApplicationName => Global.Name.ToLower();

        public BindableCollection<Profile> Profiles { get; }
        public BindableCollection<KeyValuePair<string, string>> Summary { get; }

        [Required]
        public Profile SelectedProfile
        {
            get => _selectedProfile;
            set
            {
                if (value == _selectedProfile) return;
                _selectedProfile = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(nameof(IsProfileSelected));
                ValidateProperty(value);

                ResxFilePath = string.Empty;

                RefreshStatus();
            }
        }

        [ToolsExistValidation(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "SpecifyToolsFolderPath")]
        public string ToolsFolderPath
        {
            get => _toolsFolderPath;
            set
            {
                if (value == _toolsFolderPath) return;
                _toolsFolderPath = value;
                NotifyOfPropertyChange();
                ValidateProperty(value);

                RefreshStatus();
            }
        }

        [FileExistsValidation]
        public string ResxFilePath
        {
            get => _resxFilePath;
            set
            {
                if (value == _resxFilePath) return;
                _resxFilePath = value;
                NotifyOfPropertyChange();
                ValidateProperty(value);

                NotifyOfPropertyChange(nameof(ResxFileName));
                NotifyOfPropertyChange(nameof(ResxLangTag));
                NotifyOfPropertyChange(nameof(ResxCulture));

                RefreshStatus();
            }
        }

        // Make cache for these 3 properties below

        public string ResxFileName => !string.IsNullOrEmpty(ResxFilePath) ? Path.GetFileName(ResxFilePath) : null;

        public string ResxLangTag
        {
            get
            {
                string langTag = null;

                if (!string.IsNullOrEmpty(ResxFilePath) && ResxFilePath.Split('.') is string[] parts && parts.Length == 3)
                    langTag = parts[1];

                return langTag;
            }
        }

        public CultureInfo ResxCulture
        {
            get
            {
                CultureInfo culture = null;

                try
                {
                    culture = ResxLangTag == null ? null : CultureInfo.GetCultureInfo(ResxLangTag);
                }
                catch
                {
                    // ignored
                }

                return culture;
            }
        }

        public bool IsProfileSelected => SelectedProfile != null;

        public bool CanCompile
        {
            get => _canCompile;
            private set
            {
                if (value == _canCompile) return;
                _canCompile = value;
                NotifyOfPropertyChange();
            }
        }

        public bool CanRunApplicationAfterCompile
        {
            get => _canRunApplicationAfterCompile;
            private set
            {
                if (value == _canRunApplicationAfterCompile) return;
                _canRunApplicationAfterCompile = value;
                NotifyOfPropertyChange();

                if (!value)
                    IsRunApplicationAfterCompile = false;
            }
        }

        public bool IsRunApplicationAfterCompile
        {
            get => _isRunApplicationAfterCompile;
            set
            {
                if (value == _isRunApplicationAfterCompile) return;
                _isRunApplicationAfterCompile = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(nameof(CompileBtnText));

                RefreshStatus();
            }
        }

        public bool IsOpenOutputFolderAfterCompile
        {
            get => _isOpenOutputFolderAfterCompile;
            set
            {
                if (value == _isOpenOutputFolderAfterCompile) return;
                _isOpenOutputFolderAfterCompile = value;
                NotifyOfPropertyChange();
            }
        }

        public bool IsToolsSearchBoxVisible
        {
            get => _isToolsSearchBoxVisible;
            private set
            {
                if (value == _isToolsSearchBoxVisible) return;
                _isToolsSearchBoxVisible = value;
                NotifyOfPropertyChange(nameof(IsToolsSearchBoxVisible));
            }
        }

        public bool IsSearchingTools
        {
            get => _isSearchingTools;
            private set
            {
                if (value == _isSearchingTools) return;
                _isSearchingTools = value;
                NotifyOfPropertyChange();
            }
        }

        public string CompileBtnText => IsRunApplicationAfterCompile ? Resources.CompileAndRun : Resources.Compile;

        public string LogMessage
        {
            get => _logMessage;
            private set
            {
                if (value == _logMessage) return;
                _logMessage = value;
                NotifyOfPropertyChange();
            }
        }

        #endregion

        #region Public Methods

        public void OpenReleasesLink()
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = Global.ProgramReleaseNotesUri,
                    UseShellExecute = true
                };

                Process.Start(psi);
            }
            catch (Exception ex)
            {
                var vm = new DialogViewModel
                {
                    Title = Resources.Error_Title,
                    Message = ex.Message,
                    NoButtonText = Resources.Ok,
                    HideYesButton = true
                };

                _windowManager.ShowDialog(vm);
            }
        }

        public void SelectToolsFolderPath()
        {
            _ctsTools.Cancel();
            IsSearchingTools = false;

            //

            var dlg = new FolderBrowserDialog();

            if (string.IsNullOrEmpty(ToolsFolderPath))
                dlg.RootFolder = Environment.SpecialFolder.ProgramFilesX86;
            else
                dlg.SelectedPath = ToolsFolderPath;

            dlg.ShowNewFolderButton = false;
            dlg.Description = Resources.SpecifyToolsFolderPath;

            if (dlg.ShowDialog() == DialogResult.OK)
                ToolsFolderPath = dlg.SelectedPath;
        }

        public void SelectResxFilePath()
        {
            var path = SelectFile("", ".resx", ResxFilePath);
            if (!string.IsNullOrEmpty(path))
                ResxFilePath = path;
        }

        #region Profile Operations

        public void EditProfile()
        {
            var vm = new EditProfileViewModel(SelectedProfile.ShallowCopy());

            if (_windowManager.ShowDialog(vm) != true)
                return;

            vm.Profile.CopyTo(SelectedProfile);
            _profileService.SaveProfiles();

            RefreshStatus();
        }

        public void DeleteProfile()
        {
            if (SelectedProfile == null)
                return;

            var vm = new DialogViewModel
            {
                DialogWidth = 430,
                Title = string.Format(Resources.Profile_Delete_Title, SelectedProfile.Name)
            };

            if (_windowManager.ShowDialog(vm) != true)
                return;

            _profileService.DeleteProfile(SelectedProfile);
            _profileService.SaveProfiles();

            SelectedProfile = Profiles.FirstOrDefault();
        }

        public void AddProfile()
        {
            var vm = new EditProfileViewModel(null);

            if (_windowManager.ShowDialog(vm) != true)
                return;

            _profileService.AddProfile(vm.Profile);
            _profileService.SaveProfiles();

            SelectedProfile = vm.Profile;
        }

        #endregion

        public void Compile()
        {
            LogMessage = string.Empty;

            _profileService.SaveProfiles();

            //

            _compilerService.Compile(_resolvedProfile, ResxFilePath, ToolsFolderPath, new Progress<string>(LogLine));

            //

            if (IsOpenOutputFolderAfterCompile)
            {
                try
                {
                    if (File.Exists(_resolvedProfile.SatelliteAssemblyFilePath))
                        Process.Start("explorer.exe", $"/select,\"{_resolvedProfile.SatelliteAssemblyFilePath}\"");
                    else if (Directory.Exists(_resolvedProfile.SatelliteAssemblyFolderPath))
                        Process.Start(_resolvedProfile.SatelliteAssemblyFolderPath);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unable to open output folder.");
                }
            }

            if (IsRunApplicationAfterCompile)
            {
                try
                {
                    Process.Start(_resolvedProfile.AppFilePath, _resolvedProfile.AppStartupArgs);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"Unable to start \"{_resolvedProfile.AppFilePath}\" with args \"{_resolvedProfile.AppStartupArgs}\".");
                }
            }
        }

        #endregion

        #region Protected Methods

        protected override void OnDeactivate(bool close)
        {
            _profileService.SaveProfiles();
        }

        #endregion

        #region Private Methods

        private void RefreshStatus()
        {
            if (SelectedProfile == null)
            {
                _resolvedProfile = null;
                return;
            }

            // Set variables

            var variables = new Variables();

            if (!string.IsNullOrEmpty(ResxLangTag))
                variables.SetVariableValue(VariableType.LangTag, ResxLangTag);
            if (!string.IsNullOrEmpty(SelectedProfile.AppName))
                variables.SetVariableValue(VariableType.AppName, SelectedProfile.AppName);

            //

            _resolvedProfile = SelectedProfile.ShallowCopy().ResolveVariables(variables);

            var appFileExists = File.Exists(_resolvedProfile.AppFilePath);
            CanRunApplicationAfterCompile = appFileExists;

            Summary.Clear();
            Summary.IsNotifying = false;

            try
            {
                // Move strings to resources

                Summary.Add(new KeyValuePair<string, string>(Resources.Profile, _resolvedProfile.IsReady() ? Resources.Profile_Populated : Resources.Profile_NotPopulated));

                Summary.Add(new KeyValuePair<string, string>(Resources.Profile_AppName, TryGetString(_resolvedProfile.AppName)));
                Summary.Add(new KeyValuePair<string, string>(Resources.Profile_AppFilePath, $"{TryGetString(_resolvedProfile.AppFilePath)}{(appFileExists ? "" : $" {Resources.File_NotExist}")}"));
                Summary.Add(new KeyValuePair<string, string>(Resources.ResxFile, TryGetString(ResxFileName, Resources.File_NotSelected)));
                Summary.Add(new KeyValuePair<string, string>(Resources.Language_ResourceLanguage, TryGetString(ResxCulture?.NativeName, string.IsNullOrEmpty(ResxFileName) ? Resources.Language_ResxNotSelected : Resources.Language_UnableToDetect)));
                Summary.Add(new KeyValuePair<string, string>(Resources.Profile_SatelliteAssemblyName, TryGetString(_resolvedProfile.SatelliteAssemblyName)));
                Summary.Add(new KeyValuePair<string, string>(Resources.Profile_SatelliteAssemblyOutputFolder, TryGetString(_resolvedProfile.SatelliteAssemblyFolderPath)));

                if (IsRunApplicationAfterCompile)
                    Summary.Add(new KeyValuePair<string, string>(Resources.StartApp, _resolvedProfile.AppFilePath + " " + _resolvedProfile.AppStartupArgs));
            }
            finally
            {
                Summary.IsNotifying = true;
                Summary.Refresh();
            }

            CanCompile = !HasErrors && ResxCulture != null && _resolvedProfile.IsReady();
        }

        private void LogLine(string line)
        {
            LogMessage += $"{line}\r\n";
        }

        private async void FindToolsAsync()
        {
            IsToolsSearchBoxVisible = true;
            IsSearchingTools = true;
            ToolsFolderPath = string.Empty;

            var crawler = new FileCrawler { TargetFileName = Global.Resgen };

            if (Environment.Is64BitOperatingSystem)
            {
                crawler.Locations.Add(@"C:\Program Files (x86)\Microsoft SDKs");
                crawler.Locations.Add(@"C:\Program Files (x86)");
            }

            crawler.Locations.Add(@"C:\Program Files\Microsoft SDKs");
            crawler.Locations.Add(@"C:\Program Files");

            //

            FileInfo fileInfo = await crawler.SearchFileAsync(_ctsTools.Token);

            if (fileInfo != null)
                ToolsFolderPath = fileInfo.DirectoryName;

            IsSearchingTools = false;
        }

        private static string SelectFile(string fileName, string extension, string initialFolder)
        {
            var dlg = new OpenFileDialog
            {
                FileName = fileName,
                DefaultExt = extension,
                Filter = $"{extension}|*{extension}"
            };

            if (!string.IsNullOrEmpty(initialFolder) && Path.GetDirectoryName(initialFolder) is string folder)
                dlg.InitialDirectory = folder;

            bool? result = dlg.ShowDialog();

            if (result == true)
                return dlg.FileName;

            return null;
        }

        private static string TryGetString(string value, string fallbackValue = null)
        {
            var fallback = !string.IsNullOrEmpty(fallbackValue) ? fallbackValue : Resources.NotAvailable;
            return !string.IsNullOrEmpty(value) ? value : fallback;
        }

        #endregion
    }
}