using System;
using System.Collections.Generic;
using System.Text;
using NoobCore.Model;
using NoobCore.Text;

namespace NoobCore.Validation
{
    /// <summary>
    /// The exception which is thrown when a validation error occurred.
    /// This validation is serialized in a extra clean and human-readable way by ServiceStack.
    /// </summary>
    public class ValidationError : ArgumentException, IResponseStatusConvertible
    {
        /// <summary>
        /// Gets the error message.
        /// </summary>
        /// <value>
        /// The error message.
        /// </value>
        public string ErrorMessage { get; }
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationError"/> class.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        public ValidationError(string errorCode)
            : this(errorCode, errorCode.SplitCamelCase())
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationError"/> class.
        /// </summary>
        /// <param name="validationResult">The validation result.</param>
        public ValidationError(ValidationErrorResult validationResult)
            : base(validationResult.ErrorMessage)
        {
            this.ErrorCode = validationResult.ErrorCode;
            this.ErrorMessage = validationResult.ErrorMessage;
            this.Violations = validationResult.Errors;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationError"/> class.
        /// </summary>
        /// <param name="validationError">The validation error.</param>
        public ValidationError(ValidationErrorField validationError)
            : this(validationError.ErrorCode, validationError.ErrorMessage)
        {
            this.Violations.Add(validationError);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationError"/> class.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="errorMessage">The error message.</param>
        public ValidationError(string errorCode, string errorMessage)
            : base(errorMessage)
        {
            this.ErrorCode = errorCode;
            this.ErrorMessage = errorMessage;
            this.Violations = new List<ValidationErrorField>();
        }

        /// <summary>
        /// Returns the first error code
        /// </summary>
        /// <value>The error code.</value>
        public string ErrorCode { get; }
        /// <summary>
        /// Gets the error message and the parameter name, or only the error message if no parameter name is set.
        /// </summary>
        public override string Message
        {
            get
            {
                //If there is only 1 validation error than we just show the error message
                if (this.Violations.Count == 0)
                    return this.ErrorMessage;

                if (this.Violations.Count == 1)
                    return this.ErrorMessage ?? this.Violations[0].ErrorMessage;

                var sb = StringBuilderCache.Allocate()
                    .Append(this.ErrorMessage).AppendLine();
                foreach (var error in this.Violations)
                {
                    if (!string.IsNullOrEmpty(error.ErrorMessage))
                    {
                        var fieldLabel = error.FieldName != null ? $" [{error.FieldName}]" : null;
                        sb.Append($"\n  - {error.ErrorMessage}{fieldLabel}");
                    }
                    else
                    {
                        var fieldLabel = error.FieldName != null ? ": " + error.FieldName : null;
                        sb.Append($"\n  - {error.ErrorCode}{fieldLabel}");
                    }
                }
                return StringBuilderCache.ReturnAndFree(sb);
            }
        }
        /// <summary>
        /// Gets the violations.
        /// </summary>
        /// <value>
        /// The violations.
        /// </value>
        public IList<ValidationErrorField> Violations { get; private set; }

        /// <summary>
        /// Used if we need to serialize this exception to XML
        /// </summary>
        /// <returns></returns>
        public string ToXml()
        {
            var sb = StringBuilderCache.Allocate();
            sb.Append("<ValidationException>");
            foreach (var error in this.Violations)
            {
                sb.Append("<ValidationError>")
                    .Append($"<Code>{error.ErrorCode}</Code>")
                    .Append($"<Field>{error.FieldName}</Field>")
                    .Append($"<Message>{error.ErrorMessage}</Message>")
                    .Append("</ValidationError>");
            }
            sb.Append("</ValidationException>");
            return StringBuilderCache.ReturnAndFree(sb);
        }
        /// <summary>
        /// Creates the exception.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <returns></returns>
        public static ValidationError CreateException(Enum errorCode) => 
            new ValidationError(errorCode.ToString());
        /// <summary>
        /// Creates the exception.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        public static ValidationError CreateException(Enum errorCode, string errorMessage) => 
            new ValidationError(errorCode.ToString(), errorMessage);
        /// <summary>
        /// Creates the exception.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        public static ValidationError CreateException(Enum errorCode, string errorMessage, string fieldName) => 
            CreateException(errorCode.ToString(), errorMessage, fieldName);
        /// <summary>
        /// Creates the exception.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <returns></returns>
        public static ValidationError CreateException(string errorCode) => new ValidationError(errorCode);
        /// <summary>
        /// Creates the exception.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <returns></returns>
        public static ValidationError CreateException(string errorCode, string errorMessage) => new ValidationError(errorCode, errorMessage);
        /// <summary>
        /// Creates the exception.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        public static ValidationError CreateException(string errorCode, string errorMessage, string fieldName)
        {
            var error = new ValidationErrorField(errorCode, fieldName, errorMessage);
            return new ValidationError(new ValidationErrorResult(new List<ValidationErrorField> { error }));
        }
        /// <summary>
        /// Creates the exception.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <returns></returns>
        public static ValidationError CreateException(ValidationErrorField error) => new ValidationError(error);
        /// <summary>
        /// Throws if not valid.
        /// </summary>
        /// <param name="validationResult">The validation result.</param>
        /// <exception cref="NoobCore.Validation.ValidationError"></exception>
        public static void ThrowIfNotValid(ValidationErrorResult validationResult)
        {
            if (!validationResult.IsValid)
            {
                throw new ValidationError(validationResult);
            }
        }

        /// <summary>
        /// Converts to responsestatus.
        /// </summary>
        /// <returns></returns>
        public ResponseStatus ToResponseStatus() => 
            ResponseStatusUtils.CreateResponseStatus(ErrorCode, Message, Violations);
    }
}