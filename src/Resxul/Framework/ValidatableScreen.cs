using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using Caliburn.Micro;

namespace Resxul.Framework
{
    class ValidatableScreen : Screen, INotifyDataErrorInfo
    {
        #region Fields

        private readonly ErrorsContainer<string> _errorsContainer;

        #endregion

        #region Ctor

        public ValidatableScreen()
        {
            _errorsContainer = new ErrorsContainer<string>(OnErrorsChanged);
        }

        #endregion

        #region Properties

        public bool HasErrors => _errorsContainer.HasErrors;

        #endregion

        #region Public Methods

        public IEnumerable GetErrors(string propertyName)
        {
            return _errorsContainer.GetErrors(propertyName);
        }

        #endregion

        #region Protected Methods

        protected virtual void ValidateProperty(object propertyValue, [CallerMemberName] string propertyName = null)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(this, null, null) { MemberName = propertyName };

            if (Validator.TryValidateProperty(propertyValue, validationContext, validationResults))
                _errorsContainer.ClearErrors(propertyName);
            else
                _errorsContainer.SetErrors(propertyName, validationResults.Select(x => x.ErrorMessage));
        }

        protected virtual void Validate()
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(this, null, null);

            Validator.TryValidateObject(this, validationContext, validationResults, true);

            var q = from r in validationResults
                from m in r.MemberNames
                group r by m into g
                select g;

            var propertyNames = new List<string>();

            foreach (IGrouping<string, ValidationResult> group in q)
            {
                propertyNames.Add(group.Key);
                _errorsContainer.SetErrors(group.Key, group.Select(x => x.ErrorMessage));
            }

            var otherPropertyNames = _errorsContainer.RegisteredPropertyNames.Except(propertyNames).ToList();

            foreach (var propertyName in otherPropertyNames)
            {
                _errorsContainer.ClearErrors(propertyName);
            }
        }

        #endregion

        #region Events

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        private void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            NotifyOfPropertyChange(nameof(HasErrors));
        }

        #endregion
    }
}
