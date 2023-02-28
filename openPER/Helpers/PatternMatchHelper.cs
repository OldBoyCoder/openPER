using System;
using System.Collections.Generic;
using System.Data;

namespace openPER.Helpers
{
    /// <summary>
    /// Class to evaluate whether applicability rules apply for a drawing or a part
    /// Rules are held in the ePer database in a format such as
    /// CC1.1,CC1.2+CMBBZ+!40Q+!407+ECOCF4+(LL0,LL2,LL4),CC1.2+CMBBZ+ECOCF5+(LL0,LL1,LL2,LL4)
    /// In summary:
    /// comma separated values mean OR
    /// plus separated values mean AND
    /// ! means NOT
    /// Parentheses mean what you think they do!
    /// This class resolves the pattern down to a string that can be processed by using values
    /// from the specific VIN or the model of vehicle
    /// </summary>
    public class PatternMatchHelper
    {
        public static bool EvaluateRule(string pattern, string sincomPattern)
        {
            var p2 = sincomPattern.Split(new[] { '+' }, StringSplitOptions.RemoveEmptyEntries);
            var Values = new Dictionary<string, bool>();
            foreach (var v in p2)
            {
                if (v.StartsWith('!'))
                    Values.Add(v[1..], false);
                else
                    Values.Add(v, true);
            }
            return EvaluateRule(pattern, Values);
        }
        public static bool EvaluateRule(string pattern, Dictionary<string, bool> values)
        {
            var dt = new DataTable();
            var Symbol = false;
            var SymbolName = "";
            var AllSymbols = new Dictionary<string, bool>();
            var newPattern = "";
            foreach (var c in pattern)
            {
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
                        AllSymbols[SymbolName] = false;
                        SymbolName = "";
                        Symbol = false;
                    }
                    else
                    {
                        SymbolName += c;
                        newPattern += c;
                    }
                }
            }
            // We might still be handling a symbol at the end of the pattern
            // so make sure we don't miss it
            if (Symbol)
            {
                AllSymbols[SymbolName] = false;
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
            catch (Exception)
            {
                // return true if there is an exception as we want to err on
                // the side of including items.
                return true;
            }
            return v;

        }
    }
}
