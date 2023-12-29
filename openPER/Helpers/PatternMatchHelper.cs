using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using openPER.ViewModels;
using openPERModels;

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
        public static bool EvaluateRule(string pattern, string sincomPattern, List<VmkModel> vmkCodes, bool preciseMatch)
        {
            sincomPattern = sincomPattern.Replace("|", "");
            pattern = pattern.Replace("\r", "").Replace("\n", "");
            var p2 = sincomPattern.Split(new[] { '+' }, StringSplitOptions.RemoveEmptyEntries);
            var values = new Dictionary<string, bool>();
            foreach (var v in p2)
            {
                if (v.StartsWith('!'))
                    values.Add(v[1..], false);
                else
                    values.Add(v, true);
            }
            return EvaluateRule(pattern, values, vmkCodes, preciseMatch);
        }
        public static Dictionary<string, bool> GetSymbolsFromPattern(string pattern, out string newPattern)
        {
            var symbol = false;
            var symbolName = "";
            var allSymbols = new Dictionary<string, bool>();
            newPattern = "";
            foreach (var c in pattern)
            {
                if (!symbol)
                {
                    if (c == '(' || c == ')' || c == '+' || c == ',' || c == '!')
                    {
                        newPattern += c;
                    }
                    else
                    {
                        symbol = true;
                        symbolName += c;
                        newPattern += ("|" + c);
                    }
                }
                else
                {
                    if (c == '(' || c == ')' || c == '+' || c == ',' || c == '!')
                    {
                        newPattern += ("|" + c);
                        allSymbols[symbolName] = false;
                        symbolName = "";
                        symbol = false;
                    }
                    else
                    {
                        symbolName += c;
                        newPattern += c;
                    }
                }
            }
            // We might still be handling a symbol at the end of the pattern
            // so make sure we don't miss it
            if (symbol)
            {
                allSymbols[symbolName] = false;
                newPattern += "|";
            }

            return allSymbols;

        }
        public static bool EvaluateRule(string pattern, Dictionary<string, bool> values, List<VmkModel> vmkCodes, bool preciseMatch)
        {
            var dt = new DataTable();
            bool v;
            var allSymbols = GetSymbolsFromPattern(pattern, out var newPattern);
            // Enhance the values list by adding in extra ones with the prefix from the vmk_dsc table
            // This is beacuse often the CARATT has stuff like E008+E152+...
            // but the values held against the VIN are 008 152 etc
            // this is a hack to basically take all the 008 style numbers, see if there is
            // a VMK value that ends in that and then add an extra value with the VIN value set to the
            // same true/false as the VIN
            var newValues = new Dictionary<string, bool>();
            foreach (var item in values)
            {
                if (item.Key.Length == 3)
                {
                    if (int.TryParse(item.Key, out int tmp))
                    {
                        var m = allSymbols.FirstOrDefault(x => x.Key.EndsWith(item.Key));
                        if (m.Key != null  && !values.ContainsKey(m.Key))
                            newValues[m.Key] = item.Value;
                    }
                }
            }
            foreach (var item in newValues)
            {
                values[item.Key] = item.Value;
            }
            // Fake in LHD and RHD values as we don't have a user option for these
            if (!values.ContainsKey("GD")) values["GD"] = true;
            if (!values.ContainsKey("GS")) values["GS"] = true;
            if (!values.ContainsKey("GDX")) values["GDX"] = true;
            if (!values.ContainsKey("GSX")) values["GSX"] = true;
            foreach (var item in values)
            {
                newPattern = newPattern.Replace($"|{item.Key}|", item.Value ? " true " : " false ");
                allSymbols.Remove(item.Key);
            }
            pattern = newPattern;
            if (!preciseMatch && allSymbols.Count > 0)
            {
                for (int j = 0; j < allSymbols.Count; j++)
                {
                    var key = allSymbols.ElementAt(j).Key;
                    var vmkElement = vmkCodes.FirstOrDefault(x => (x.Type + x.Code) == key);
                    if (vmkElement is { MultiValue: true })
                        allSymbols[key] = false;
                    else
                    {
                        allSymbols[key] = true;
                    }
                }
                // Now try the pattern
                foreach (var item in allSymbols)
                {
                    pattern = pattern.Replace($"|{item.Key}|", item.Value ? " true " : " false ");
                }
                pattern = pattern.Replace(",", " OR ");
                pattern = pattern.Replace("+", " AND ");
                pattern = pattern.Replace("!", " NOT ");
                pattern = pattern.Replace("(", " ( ");
                pattern = pattern.Replace(")", " ) ");
                pattern = " " + pattern + " ";

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
                if (v) return true;

                return false;
            }
            foreach (var item in allSymbols)
            {
                pattern = pattern.Replace($"|{item.Key}|", " false ");
            }
            pattern = pattern.Replace(",", " OR ");
            pattern = pattern.Replace("+", " AND ");
            pattern = pattern.Replace("!", " NOT ");
            pattern = pattern.Replace("(", " ( ");
            pattern = pattern.Replace(")", " ) ");
            pattern = " " + pattern + " ";

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
        public static string ConsolidatePatterns(string sinComPattern, string vehiclePattern)
        {
            string consolidatedPattern;
            if (string.IsNullOrEmpty(vehiclePattern))
            {
                consolidatedPattern = sinComPattern;
            }
            else
            {
                consolidatedPattern = vehiclePattern.Replace("|", "").Replace("\r", "").Replace("\n", "");
                var sp = sinComPattern.Replace("\r", "").Replace("\n", "").Replace("|", "");
                var sincomElements = sp.Split("+");
                foreach (var item in sincomElements)
                {
                    var key = item.Replace("!", "");
                    if (!consolidatedPattern.Contains(key))
                        consolidatedPattern += "+" + item;

                }


            }
            return consolidatedPattern;

        }
        public static bool ApplyPatternAndModificationRules(string pattern, string sinComPattern, List<VmkModel> vmkCodes,
            string vehiclePattern, List<ModificationViewModel> modifications, Dictionary<string, string> vehicleModificationFilters)
        {
            // If we have a vehicle pattern then we need to merge the SINCOM pattern into it as there are some attributes not 
            // duplicated into vehicle pattern
            var consolidatedPattern = ConsolidatePatterns(sinComPattern, vehiclePattern);
            if (!string.IsNullOrEmpty(pattern))
            {
                if (!EvaluateRule(pattern, consolidatedPattern, vmkCodes, !string.IsNullOrEmpty(vehiclePattern)))
                    return false;
            }

            foreach (var mod in modifications)
            {
                foreach (var rule in mod.Activations)
                {
                    // Does this apply to this vehicle
                    if (EvaluateRule(rule.ActivationPattern, consolidatedPattern, vmkCodes,
                            !string.IsNullOrEmpty(vehiclePattern)))
                    {
                        // Does this vehicle have the data needed
                        if (vehicleModificationFilters.ContainsKey(rule.ActivationCode))
                        {
                            // Before or after rule?
                            if (mod.Type == "C")
                            {
                                // C means stops at so if data is past this then it is invisible
                                if (int.Parse(rule.ActivationSpec) <=
                                    int.Parse(vehicleModificationFilters[rule.ActivationCode]))
                                {
                                    return false;
                                }
                            }

                            if (mod.Type == "D")
                            {
                                // C means after a date if data is before this then it is invisible
                                if (int.Parse(rule.ActivationSpec) > int.Parse(vehicleModificationFilters[rule.ActivationCode]))
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }
    }
}
