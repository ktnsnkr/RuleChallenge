using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuleEngineChallenge
{
    /// <summary>
    /// This class is responsible for parsing the incoming data.
    /// </summary>
    public class Parser
    {
        #region Constants

        private const string fieldSignalName = "signal";
        private const string fieldValueName = "value";
        private const string fieldValueTypeName = "value_type";

        #endregion

        #region Public Methods

        /// <summary>
        /// Parses the specified JSON input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="validSignals">The valid signals.</param>
        /// <param name="invalidSignals">The invalid signals.</param>
        /// <param name="rules">The rules.</param>
        /// <returns>
        /// true if the input parameter was converted successfully; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException">input</exception>
        public bool TryParse(string input, out List<SignalData> validSignals, out List<SignalData> invalidSignals, bool canStopOnFirstValidationFailure, params IRule[] rules)
        {
            try
            {
                if (string.IsNullOrEmpty(input)) { throw new ArgumentNullException("input"); }

                validSignals = new List<SignalData>();
                invalidSignals = new List<SignalData>();

                foreach (var signal in input.Split(Constants.ObjectEnd))
                {
                    var elements = signal.Split(new[] { Constants.ElementSeparator }, StringSplitOptions.RemoveEmptyEntries);
                    if (elements.Length == 3) // Signal, Value and ValueType
                    {
                        string parsedSignal = string.Empty, parsedValue = string.Empty, parsedValueType = string.Empty;

                        foreach (var element in elements)
                        {
                            if (!string.IsNullOrWhiteSpace(element))
                            {
                                int indexOfObjectSeparator = element.IndexOf(Constants.ObjectSeparator);
                                string fieldName = element.Substring(0, indexOfObjectSeparator);
                                string fieldValue = element.Substring(indexOfObjectSeparator + 1);

                                if (!string.IsNullOrWhiteSpace(fieldName) && !string.IsNullOrWhiteSpace(fieldValue))
                                {
                                    fieldName = fieldName.ToLowerInvariant().Split(Constants.ObjectWrapper)[1];
                                    fieldValue = fieldValue.Split(Constants.ObjectWrapper)[1];

                                    switch (fieldName.ToLowerInvariant())
                                    {
                                        case fieldSignalName:
                                            parsedSignal = fieldValue;
                                            continue;

                                        case fieldValueTypeName:
                                            parsedValueType = fieldValue;
                                            continue;

                                        case fieldValueName:
                                            parsedValue = fieldValue;
                                            continue;

                                        default:
                                            break;
                                    }
                                }
                                else { return false; } // Parsing failed due to invalid input format
                            }
                        }

                        if (string.IsNullOrWhiteSpace(parsedSignal) ||
                            string.IsNullOrWhiteSpace(parsedValue) ||
                            string.IsNullOrWhiteSpace(parsedValueType))
                        {
                            return false; // Parsing failed due to invalid input format
                        }

                        var signalData = new SignalData(parsedSignal, parsedValue, parsedValueType);
                        bool isValidSignal = true;
                        foreach (var rule in rules)
                        {
                            if (!rule.IsValid(signalData))
                            {
                                isValidSignal = false;
                                invalidSignals.Add(signalData); // Add to invalid signal list

                                if (canStopOnFirstValidationFailure) return true; // Parsed successfully but stopping on validation failure.
                                break;
                            }
                        }
                        if (isValidSignal) { validSignals.Add(signalData); } // Add to valid signal list
                    }
                }
                return true;
            }
            catch (Exception)
            {
                //TODO: Implement logger
                //return false; once logger is implemented.
                throw;
            }
        }

        #endregion
    }
}
