using System;
using System.Collections.Generic;
using System.Data;

namespace openPER.Helpers
{
    public class PatternMatchHelper
    {
        public static int EvaluateRule(string pattern, string sincomPattern)
        {
            var p2 = sincomPattern.Split(new[] { '+' }, StringSplitOptions.RemoveEmptyEntries);
            var Values = new Dictionary<string, bool>();
            foreach (var v in p2)
            {
                if (v.StartsWith('!'))
                    Values.Add(v.Substring(1), false);
                else
                    Values.Add(v, true);
            }
            return EvaluateRule(pattern, Values);

        }
        public static int EvaluateRule(string pattern, Dictionary<string, bool> values)
        {
            var dt = new DataTable();
            var ix = 0;
            var Symbol = false;
            var SymbolName = "";
            var AllSymbols = new Dictionary<string, bool>();
            var newPattern = "";
            while (ix < pattern.Length)
            {
                var c = pattern[ix];
                if (!Symbol)
                {
                    if (c == '(' || c == ')' || c == '+' || c == ',' || c == '!')
                    {
                        newPattern += c;
                    }
                    else
                    {
                        Symbol = true;
                        SymbolName += c;
                        newPattern += ("|" + c);
                    }
                }
                else
                {
                    if (c == '(' || c == ')' || c == '+' || c == ',' || c == '!')
                    {
                        newPattern += ("|" + c);
                        if (!AllSymbols.ContainsKey(SymbolName))
                            AllSymbols.Add(SymbolName, false);
                        SymbolName = "";
                        Symbol = false;
                    }
                    else
                    {
                        SymbolName += c;
                        newPattern += c;
                    }

                }
                ix++;
            }
            if (Symbol)
            {
                if (!AllSymbols.ContainsKey(SymbolName))
                    AllSymbols.Add(SymbolName, false);
                newPattern += "|";
            }
            foreach (var item in values)
            {
                if (AllSymbols.ContainsKey(item.Key))
                    AllSymbols[item.Key] = item.Value;
            }
            foreach (var item in AllSymbols)
            {
                newPattern = newPattern.Replace($"|{item.Key}|", item.Value ? " true " : " false ");
            }
            pattern = newPattern;
            pattern = pattern.Replace(",", " OR ");
            pattern = pattern.Replace("+", " AND ");
            pattern = pattern.Replace("!", " NOT ");
            pattern = pattern.Replace("(", " ( ");
            pattern = pattern.Replace(")", " ) ");
            pattern = " " + pattern + " ";

            bool v;
            try
            {
                v = (bool)dt.Compute(pattern, "");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
            return v ? 1 : 0;

        }
    }
}
