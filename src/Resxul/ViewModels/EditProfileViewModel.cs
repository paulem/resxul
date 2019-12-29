using Caliburn.Micro;
using Resxul.Models;
using Resxul.Properties;

namespace Resxul.ViewModels
{
    internal sealed class EditProfileViewModel : Screen
    {
        private string _submitButtonText;

        public EditProfileViewModel()
        {
            DisplayName = Resources.Profile_Wnd_Title;
            Variables = new Variables();
        }

        public EditProfileViewModel(Profile profileToEdit = null) : this()
        {
            if (profileToEdit == null)
            {
                SubmitButtonText = Resources.Profile_Add;
                Profile = new Profile();
            }
            else
            {
                SubmitButtonText = Resources.Profile_Save;
                Profile = profileToEdit;
            }
        }

        //

        public Profile Profile { get; }
        public Variables Variables { get; }

        //

        public string SubmitButtonText
        {
            get => _submitButtonText;
            private set
            {
                if (value == _submitButtonText) return;
                _submitButtonText = value;
                NotifyOfPropertyChange();
            }
        }

        //

        public bool CanTryClose => !string.IsNullOrEmpty(Profile.Name)
                                   && !string.IsNullOrEmpty(Profile.AppName)
                                   && !string.IsNullOrEmpty(Profile.AppFilePath)
                                   && !string.IsNullOrEmpty(Profile.ResxNamespace)
                                   && !string.IsNullOrEmpty(Profile.SatelliteAssemblyName)
                                   && !string.IsNullOrEmpty(Profile.SatelliteAssemblyFolderPath);

        protected override void OnActivate()
        {
            Profile.PropertyChanged += Profile_PropertyChanged;
        }

        protected override void OnDeactivate(bool close)
        {
            Profile.PropertyChanged -= Profile_PropertyChanged;
        }

        private void Profile_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            NotifyOfPropertyChange(nameof(CanTryClose));
        }
    }
}
