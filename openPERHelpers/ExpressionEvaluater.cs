using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace openPERHelpers
{
    public enum openPERBoolean
    {
        False = 0,
        True = 1,
        Unknown = 2
    }
    public class ExpressionEvaluater
    {
        public ExpressionEvaluater() { }
        public bool Evaluate(string expression, Dictionary<string, openPERBoolean> values)
        {
            var rc = false;
            // Count how many unknowns?
            var unKnownCount = values.Count(x => x.Value == openPERBoolean.Unknown);
            // Deal with trivial case
            if (unKnownCount == 0) return ProcessQueue(ShuntingYard(TokenizeInput(expression)), values);
            // The combinations are now (2^unKnowns); 
            // Build an indirection 
            var indirectValues = new List<int>();
            for (int i = 0; i < values.Count; i++)
            {
                if (values.ElementAt(i).Value == openPERBoolean.Unknown)
                    indirectValues.Add(i);
            }
            long totalCombinations = 1 << unKnownCount;
            var baseYard = ShuntingYard(TokenizeInput(expression));
            for (long combinations = 0; combinations < totalCombinations; combinations++)
            {
                var newValues = new Dictionary<string, openPERBoolean>();
                foreach (var item in values.Where(x => x.Value != openPERBoolean.Unknown))
                {
                    newValues.Add(item.Key, item.Value);
                }
                // Combinations is a bit mask that let us control the values of the unknowns
                for (int b = 0; b < unKnownCount; b++)
                {
                    long mask = 1 << b;
                    if ((combinations & mask) != 0)
                        newValues.Add(values.ElementAt(indirectValues[b]).Key, openPERBoolean.True);
                    else
                        newValues.Add(values.ElementAt(indirectValues[b]).Key, openPERBoolean.False);

                }
                var yard = new Queue<string>(baseYard);
                if (ProcessQueue(yard, newValues))
                    return true;

            }
            return false;

        }
        public Queue<string> ShuntingYard(string input)
        {
            return ShuntingYard(TokenizeInput(input));
        }
        private bool ProcessQueue(Queue<string> queue, Dictionary<string, openPERBoolean> values)
        {
            var t = new Stack<bool>();
            string x;
            while (queue.TryDequeue(out x))
            {
                if (x != "+" && x != "," && x != "!")
                {
                    if (values.ContainsKey(x))
                    {
                        var i = values[x];
                        if (i == openPERBoolean.False)
                            t.Push(false);
                        if (i == openPERBoolean.True)
                            t.Push(true);
                    }
                    else
                        t.Push(false);
                }
                else
                {
                    if (x == "!")
                    {
                        t.Push(!t.Pop());
                    }
                    else
                    {
                        var o2 = t.Pop();
                        var o1 = t.Pop();
                        if (x == "+")
                            t.Push(o1 && o2);
                        if (x == ",")
                            t.Push(o1 || o2);
                    }
                }
            }
            return t.Pop();

        }
        public Queue<string> ShuntingYard(List<string> tokens)
        {
            Stack<string> operatorStack = new Stack<string>();
            Queue<string> outputQueue = new Queue<string>();
            foreach (var token in tokens)
            {
                if (token == "!")
                {
                    while (operatorStack.Count() > 0 && operatorStack.Peek() != "(" && (operatorStack.Peek() != "+" && operatorStack.Peek() != ","))
                    {
                        outputQueue.Enqueue(operatorStack.Pop());
                    }
                    operatorStack.Push(token);
                }
                else if (token == "+")
                {
                    while (operatorStack.Count() > 0 && operatorStack.Peek() != "(" && (operatorStack.Peek() != ","))
                    {
                        outputQueue.Enqueue(operatorStack.Pop());
                    }
                    operatorStack.Push(token);
                }
                else if (token == ",")
                {
                    while (operatorStack.Count() > 0 && operatorStack.Peek() != "(")
                    {
                        outputQueue.Enqueue(operatorStack.Pop());
                    }
                    operatorStack.Push(token);
                }
                else if (token == "(")
                {
                    operatorStack.Push(token);
                }
                else if (token == ")")
                {
                    while (operatorStack.Count() > 0 && operatorStack.Peek() != "(")
                    {
                        outputQueue.Enqueue(operatorStack.Pop());
                    }
                    operatorStack.Pop();
                }
                else
                {
                    outputQueue.Enqueue(token);
                }
            }
            while (operatorStack.Count > 0)
            {
                outputQueue.Enqueue(operatorStack.Pop());
            }
            return outputQueue;
        }
        private List<string> TokenizeInput(string input)
        {
            List<string> tokens = new List<string>();
            StringBuilder sb = new StringBuilder();
            input = input.Replace(" ", "").ReplaceLineEndings("").Trim();
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                switch (c)
                {
                    case ' ':
                        if (sb.Length == 0)
                            continue;

                        sb.Append(c);
                        break;

                    case '(':
                    case ')':
                    case '+':
                    case ',':
                    case '!':
                        if (sb.Length > 0)
                        {
                            tokens.Add(sb.ToString());
                            sb.Clear();
                        }

                        tokens.Add(c.ToString(CultureInfo.InvariantCulture));
                        break;

                    default:
                        sb.Append(c);
                        break;
                }
            }

            if (sb.Length > 0)
            {
                tokens.Add(sb.ToString());
            }
            return tokens;
        }
    }
}
