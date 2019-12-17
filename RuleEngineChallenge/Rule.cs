using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuleEngineChallenge
{
    /// <summary>
    /// Implements a rule used by the parser. This class also provide the built-in rules defined in Rules.CSV
    /// </summary>
    /// <seealso cref="RuleEngine.IRule" />
    public class Rule : IRule
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Rule" /> class.
        /// </summary>
        /// <param name="signal">The signal.</param>
        /// <param name="value">The value.</param>
        /// <param name="comparisonType">Type of the comparison.</param>
        /// <exception cref="System.ArgumentNullException">signalName
        /// or
        /// value</exception>
        public Rule(string signal, dynamic value, Comparison comparisonType)
        {
            if (string.IsNullOrWhiteSpace(signal)) { throw new ArgumentNullException("signal"); }
            if (value == null) { throw new ArgumentNullException("value"); }

            Signal = signal;
            Value = value;
            ComparisonType = comparisonType;
            ValueType = value.GetType().ToString().Split('.')[1];
        }

        #endregion

        #region Fields & Properties

        private static List<Rule> _rules;

        /// <summary>
        /// Gets the signal.
        /// </summary>
        /// <value>
        /// The name of the signal.
        /// </value>
        public string Signal { get; private set; }

        /// <summary>
        /// Gets the type of the comparison.
        /// </summary>
        /// <value>
        /// The type of the comparison.
        /// </value>
        public Comparison ComparisonType { get; private set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public dynamic Value { get; private set; }

        /// <summary>
        /// Gets or sets the type of the value.
        /// </summary>
        /// <value>
        /// The type of the value.
        /// </value>
        public string ValueType { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns true if the input signal doesn't match with this rule.
        /// </summary>
        /// <param name="input">The input signal.</param>
        /// <returns>
        ///   <c>true</c> if the input signal doesn't match with this rule; otherwise, <c>false</c>.
        /// </returns>
        public bool IsValid(SignalData input)
        {
            bool result = true;

            if (input != null)
            {
                if (Signal == input.Signal && ValueType.ToLowerInvariant() == input.ValueType.ToLowerInvariant() && Value.GetType() == input.Value.GetType())
                {
                    switch (ComparisonType)
                    {
                        case Comparison.Equal:
                            result = !(Signal == input.Signal && input.Value == Value);
                            break;

                        case Comparison.NotEqual:
                            result = !(Signal == input.Signal && input.Value != Value);
                            break;

                        case Comparison.GreaterThan:
                            result = !(Signal == input.Signal && input.Value > Value);
                            break;

                        case Comparison.GreaterThanOrEqual:
                            result = !(Signal == input.Signal && input.Value >= Value);
                            break;

                        case Comparison.LessThan:
                            result = !(Signal == input.Signal && input.Value < Value);
                            break;

                        case Comparison.LessThanOrEqual:
                            result = !(Signal == input.Signal && input.Value <= Value);
                            break;

                        default:
                            return result;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets the built-in rules defined in Rules.CSV
        /// </summary>
        /// <value>
        /// The rules.
        /// </value>
        public static Rule[] GetRules()
        {
            if (_rules == null)
            {
                LoadRulesFromCsv();
            }

            return _rules.ToArray();
        }

        /// <summary>
        /// Adds the rule to current rule list.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <exception cref="System.ArgumentNullException">rule</exception>
        public void AddRule(Rule rule)
        {
            if (rule == null) { throw new ArgumentNullException("rule"); }

            if (_rules == null)
            {
                LoadRulesFromCsv();
            }

            _rules.Add(rule);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Loads the rules from CSV.
        /// </summary>
        /// <param name="filename">The filename.</param>
        private static void LoadRulesFromCsv(string filename = "Rules.csv")
        {
            try
            {
                string rulePath = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.FullName, filename);
                if (_rules == null) _rules = new List<Rule>();
                if (File.Exists(rulePath))
                {
                    var rules = File.ReadAllLines(rulePath);
                    if (rules != null)
                    {
                        bool isHeader = true;
                        foreach (var rule in rules)
                        {
                            if (isHeader) { isHeader = false; continue; } // Skip header
                            var data = rule.Split(Constants.ElementSeparator);

                            var signal = data[0].Split(Constants.ObjectWrapper)[1];
                            Enum.TryParse(data[1].Split(Constants.ObjectWrapper)[1], out Comparison comparisonType);
                            var valueType = data[3].Split(Constants.ObjectWrapper)[1];

                            var typeInfo = Type.GetType($"System.{valueType}", false, true);
                            dynamic value = null;
                            if (typeInfo != null)
                            {
                                value = Convert.ChangeType(data[2].Split(Constants.ObjectWrapper)[1], typeInfo);
                            }
                            else
                            {
                                value = data[2].Split('"')[1];
                            }
                            _rules.Add(new Rule(signal, value, comparisonType));
                        }
                    }
                }
            }
            catch (Exception)
            {
                //TODO: Log Error
                throw;
            }
        }

        #endregion
    }
}
