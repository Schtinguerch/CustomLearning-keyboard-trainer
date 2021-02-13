using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace WPFMeteroWindow
{
    public struct Rule
    {
        public string Expression { get; set; }

        public string Result { get; set; }

        public Rule(string expression, string result)
        {
            Expression = expression;
            Result = result;
        }
    }

    public class TextProcessor
    {
        private List<Rule> Rules;

        public TextProcessor() => 
            Rules = new List<Rule>();

        public TextProcessor(List<Rule> rules) =>
            Rules = rules;

        public TextProcessor(List<string> expressions, List<string> results)
        {
            Rules = new List<Rule>();

            int
                expCount = expressions.Count,
                resCoust = results.Count;

            if (expCount == resCoust)
                for (int i = 0; i < expCount; i++)
                    Rules.Add(new Rule(expressions[i], results[i]));
            
            else throw new ArgumentException("Количество аргументов не совпадает!");
        }

        public void AddNewRule(Rule rule) => 
            Rules.Add(rule);
        
        public void AddNewRule(string expression, string result) => 
            Rules.Add(new Rule(expression, result));

        public void AddNewRules(List<Rule> rules) 
        {
            foreach (var rule in rules)
                Rules.Add(rule);
        }

        public void AddnewRules(List<string> expressions, List<string> results)
        {
            int
                expCount = expressions.Count,
                resCoust = results.Count;

            if (expCount == resCoust)
                for (int i = 0; i < expCount; i++)
                    Rules.Add(new Rule(expressions[i], results[i]));
            
            else throw new ArgumentException("Количество аргументов не совпадает!");
        }

        public string ProcessedText(string inputText)
        {
            var value = inputText;

            foreach (var rule in Rules)
                value = Regex.Replace(value, rule.Expression, rule.Result);
                
            return value;
        }
    }
}