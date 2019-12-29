using Caliburn.Micro;

namespace Resxul.Models
{
    public sealed class Variable : PropertyChangedBase
    {
        private string _value;
        private string _name;
        private string _description;

        public Variable(VariableType type, string name, string description)
        {
            Type = type;
            Name = name;
            Description = description;
        }

        public VariableType Type { get; }

        public bool IsSet => !string.IsNullOrEmpty(Value);

        public string Name
        {
            get => _name;
            private set
            {
                if (value == _name) return;
                _name = value;
                NotifyOfPropertyChange();
            }
        }

        public string Description
        {
            get => _description;
            private set
            {
                if (value == _description) return;
                _description = value;
                NotifyOfPropertyChange();
            }
        }

        public string Value
        {
            get => _value;
            set
            {
                if (value == _value) return;
                _value = value;
                NotifyOfPropertyChange();
            }
        }
    }
}
