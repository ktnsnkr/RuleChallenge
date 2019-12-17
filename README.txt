Approach
A console app is the startup project.
The separate library of RuleEngineChallenge has a Parser class which is responsible to parse the incoming data stream and validates them based on the rules. 
Parsing and Validation are an teo separate processes and validation is not a responsibility of the parser but the rules itself.
The rule is defined by an interface IRule to enforce the constraint on rule creation.
The output is then fed back into the console.
Incase of successful parsing the invalid data is shown, or success message in case of none.
Incase of unsuccessful parsing the message is shown as failure in parsing.

The code as such is not complex 

As improvement , i would have tried to reduce the number of lines of code by using LINQ.