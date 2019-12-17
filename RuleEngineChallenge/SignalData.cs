using System;

namespace RuleEngineChallenge
{
    /// <summary>
    /// Defines a incoming signal data
    /// </summary>
    public class SignalData
    {
        #region Constants

        private const string _valueTypeInteger = "integer";
        private const string _valueTypeDatetime = "datetime";

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SignalData"/> class.
        /// </summary>
        /// <param name="signal">The signal.</param>
        /// <param name="value">The value.</param>
        /// <param name="valueType">Type of the value.</param>
        /// <exception cref="System.ArgumentNullException">
        /// signal
        /// or
        /// value
        /// or
        /// valueType
        /// </exception>
        public SignalData(string signal, string value, string valueType)
        {
            if (string.IsNullOrWhiteSpace(signal)) { throw new ArgumentNullException("signal"); }
            if (string.IsNullOrWhiteSpace(value)) { throw new ArgumentNullException("value"); }
            if (string.IsNullOrWhiteSpace(valueType)) { throw new ArgumentNullException("valueType"); }

            Signal = signal;
            ValueType = valueType;

            switch (valueType.ToLowerInvariant())
            {
                case _valueTypeInteger:
                    if (double.TryParse(value, out double resultInteger)) { Value = resultInteger; }
                    else { Value = value; }
                    break;

                case _valueTypeDatetime:
                    if (DateTime.TryParse(value, out DateTime resultDateTime)) { Value = resultDateTime; }
                    else { Value = value; }
                    break;

                default:
                    Value = value;
                    break;
            }
        }

        #endregion

        #region Fields & Properties

        /// <summary>
        /// Gets the signal.
        /// </summary>
        /// <value>
        /// The signal.
        /// </value>
        public string Signal { get; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public dynamic Value { get; }

        /// <summary>
        /// Gets or sets the type of the value.
        /// </summary>
        /// <value>
        /// The type of the value.
        /// </value>
        public string ValueType { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is SignalData)
            {
                var signal = obj as SignalData;
                return
                    signal.Signal == Signal &&
                    signal.Value == Value &&
                    signal.ValueType == ValueType;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((string.IsNullOrWhiteSpace(Signal) ? 0 : Signal.GetHashCode() + Signal.Length) +
                (string.IsNullOrWhiteSpace(ValueType) ? 0 : ValueType.GetHashCode() + ValueType.Length) +
                (Value == null ? 0 : this.Value.GetHashCode())) * 7;
            }
        }

        #endregion
    }
}