using Caliburn.Micro;

namespace Resxul.Framework
{
    public abstract class ListItem
    {
        private bool _isFake;
        private bool _isEnabled;

        protected ListItem()
        {
            IsEnabled = true;
        }

        public bool IsFake
        {
            get => _isFake;
            set
            {
                if (value == _isFake) return;
                _isFake = value;
                //NotifyOfPropertyChange();
            }
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (value == _isEnabled) return;
                _isEnabled = value;
                //NotifyOfPropertyChange();
            }
        }
    }
}
