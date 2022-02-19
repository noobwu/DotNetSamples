using System;
using System.Collections.Generic;

namespace NoobCore.Validation
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="NoobCore.IMeta" />
    public class ValidationErrorField : IMeta
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationErrorField"/> class.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="fieldName">Name of the field.</param>
        public ValidationErrorField(string errorCode, string fieldName) 
            : this(errorCode, fieldName, null) {}
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationErrorField"/> class.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        public ValidationErrorField(string errorCode)
            : this(errorCode, null, null) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationErrorField"/> class.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        public ValidationErrorField(Enum errorCode)
            : this(errorCode.ToString(), null, null) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationErrorField"/> class.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="fieldName">Name of the field.</param>
        public ValidationErrorField(Enum errorCode, string fieldName)
            : this(errorCode.ToString(), fieldName, null) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationErrorField"/> class.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="errorMessage">The error message.</param>
        public ValidationErrorField(Enum errorCode, string fieldName, string errorMessage)
            : this(errorCode.ToString(), fieldName, errorMessage) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationErrorField"/> class.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="errorMessage">The error message.</param>
        public ValidationErrorField(string errorCode, string fieldName, string errorMessage)
			: this (errorCode, fieldName, errorMessage, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationErrorField"/> class.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="attemptedValue">The attempted value.</param>
        public ValidationErrorField(string errorCode, string fieldName, string errorMessage, object attemptedValue)
        {
            this.ErrorCode = errorCode;
            this.FieldName = fieldName;
            this.ErrorMessage = errorMessage ?? errorCode.ToEnglish();
			this.AttemptedValue = attemptedValue;
        }
        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        /// <value>
        /// The error code.
        /// </value>
        public string ErrorCode { get; set; }
        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        /// <value>
        /// The error message.
        /// </value>
        public string ErrorMessage { get; set; }
        /// <summary>
        /// Gets or sets the name of the field.
        /// </summary>
        /// <value>
        /// The name of the field.
        /// </value>
        public string FieldName { get; set; }
        /// <summary>
        /// Gets or sets the attempted value.
        /// </summary>
        /// <value>
        /// The attempted value.
        /// </value>
        public object AttemptedValue { get; set; }
        /// <summary>
        /// Gets or sets the meta.
        /// </summary>
        /// <value>
        /// The meta.
        /// </value>
        public Dictionary<string, string> Meta { get; set; }
    }
}