using System;
using System.Collections.Generic;
using System.Linq;

namespace Resxul.Framework
{
    /// <summary>
    /// Manages validation errors for an object, notifying when the error state changes.
    /// </summary>
    /// <typeparam name="T">The type of the error object.</typeparam>
    public class ErrorsContainer<T>
    {
        private static readonly T[] NoErrors = new T[0];
        protected readonly Action<string> RaiseErrorsChanged;
        protected readonly Dictionary<string, List<T>> ValidationResults;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorsContainer{T}"/> class.
        /// </summary>
        /// <param name="raiseErrorsChanged">The action that invoked if when errors are added for an object./>
        /// event.</param>
        public ErrorsContainer(Action<string> raiseErrorsChanged)
        {
            RaiseErrorsChanged = raiseErrorsChanged ?? throw new ArgumentNullException(nameof(raiseErrorsChanged));
            ValidationResults = new Dictionary<string, List<T>>();
        }

        /// <summary>
        /// Gets a value indicating whether the object has validation errors. 
        /// </summary>
        public bool HasErrors => ValidationResults.Count != 0;

        /// <summary>
        /// Gets the validation errors for a specified property.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>The validation errors of type <typeparamref name="T"/> for the property.</returns>
        public IEnumerable<T> GetErrors(string propertyName)
        {
            var localPropertyName = propertyName ?? string.Empty;
            List<T> currentValidationResults;
            return ValidationResults.TryGetValue(localPropertyName, out currentValidationResults)
                ? (IEnumerable<T>) currentValidationResults
                : NoErrors;
        }

        public IEnumerable<string> RegisteredPropertyNames => ValidationResults.Keys;

        /// <summary>
        /// Clears the errors for a property.
        /// </summary>
        /// <param name="propertyName">The name of the property for which to clear errors.</param>
        /// <example>
        ///     container.ClearErrors("SomeProperty");
        /// </example>
        public void ClearErrors(string propertyName)
        {
            SetErrors(propertyName, new List<T>());
        }

        /// <summary>
        /// Sets the validation errors for the specified property.
        /// </summary>
        /// <remarks>
        /// If a change is detected then the errors changed event is raised.
        /// </remarks>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="newValidationResults">The new validation errors.</param>
        public void SetErrors(string propertyName, IEnumerable<T> newValidationResults)
        {
            var localPropertyName = propertyName ?? string.Empty;
            IList<T> newValidationResultsList = newValidationResults == null ? new List<T>() : new List<T>(newValidationResults);

            bool hasCurrentValidationResults = ValidationResults.ContainsKey(localPropertyName);
            bool hasNewValidationResults = newValidationResultsList.Any();

            if (hasCurrentValidationResults || hasNewValidationResults)
            {
                if (hasNewValidationResults)
                {
                    ValidationResults[localPropertyName] = new List<T>(newValidationResultsList);
                    RaiseErrorsChanged(localPropertyName);
                }
                else
                {
                    ValidationResults.Remove(localPropertyName);
                    RaiseErrorsChanged(localPropertyName);
                }
            }
        }
    }
}