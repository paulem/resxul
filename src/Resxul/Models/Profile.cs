using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using Caliburn.Micro;

namespace Resxul.Models
{
    [DataContract]
    public sealed class Profile : PropertyChangedBase
    {
        private string _name;
        private string _appName;
        private string _resxNamespace;
        private string _satelliteAssemblyName;
        private string _appStartupArgs;
        private string _satelliteAssemblyFolderPath;
        private string _appFilePath;

        public string FileName { get; set; }

        [DataMember(Name = "ProfileName")]
        public string Name
        {
            get => _name;
            set
            {
                if (value == _name) return;
                _name = value;
                NotifyOfPropertyChange();
            }
        }

        [DataMember]
        public string AppName
        {
            get => _appName;
            set
            {
                if (value == _appName) return;
                _appName = value;
                NotifyOfPropertyChange();
            }
        }

        [DataMember]
        public string AppFilePath
        {
            get => _appFilePath;
            set
            {
                if (value == _appFilePath) return;
                _appFilePath = value;
                NotifyOfPropertyChange();
            }
        }

        [DataMember]
        public string AppStartupArgs
        {
            get => _appStartupArgs;
            set
            {
                if (value == _appStartupArgs) return;
                _appStartupArgs = value;
                NotifyOfPropertyChange();
            }
        }

        [DataMember]
        public string ResxNamespace
        {
            get => _resxNamespace;
            set
            {
                if (value == _resxNamespace) return;
                _resxNamespace = value;
                NotifyOfPropertyChange();
            }
        }

        [DataMember]
        public string SatelliteAssemblyName
        {
            get => _satelliteAssemblyName;
            set
            {
                if (value == _satelliteAssemblyName) return;
                _satelliteAssemblyName = value;
                NotifyOfPropertyChange();
            }
        }

        [DataMember]
        public string SatelliteAssemblyFolderPath
        {
            get => _satelliteAssemblyFolderPath;
            set
            {
                if (value == _satelliteAssemblyFolderPath) return;
                _satelliteAssemblyFolderPath = value;
                NotifyOfPropertyChange();
            }
        }

        public string SatelliteAssemblyFilePath => Path.Combine(SatelliteAssemblyFolderPath, SatelliteAssemblyName);

        public Profile ShallowCopy()
        {
            return (Profile)MemberwiseClone();
        }
    }
}