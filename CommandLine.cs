using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SapphireEngine
{
    public static class CommandLine
    {
        // Fields
        private static string commandline = "";

        private static bool initialized = false;
        private static Dictionary<string, string> switches = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        // Methods
        public static void Force(string val)
        {
            commandline = val;
            initialized = false;
        }

        public static string GetSwitch(string strName, string strDefault)
        {
            Initalize();
            string str = "";
            if (!switches.TryGetValue(strName, out str))
            {
                return strDefault;
            }
            return str;
        }

        public static Dictionary<string, string> GetSwitches()
        {
            Initalize();
            return switches;
        }

        public static int GetSwitchInt(string strName, int iDefault)
        {
            Initalize();
            string str = "";
            if (!switches.TryGetValue(strName, out str))
            {
                return iDefault;
            }
            int result = iDefault;
            if (!int.TryParse(str, out result))
            {
                return iDefault;
            }
            return result;
        }

        public static bool HasSwitch(string strName)
        {
            Initalize();
            return switches.ContainsKey(strName);
        }

        private static void Initalize()
        {
            if (!initialized)
            {
                initialized = true;
                if (commandline == "")
                {
                    foreach (string str2 in Environment.GetCommandLineArgs())
                    {
                        commandline = commandline + "\"" + str2 + "\" ";
                    }
                }
                if (commandline != "")
                {
                    string key = "";
                    foreach (string str3 in commandline.SplitQuotesStrings())
                    {
                        if (str3.Length != 0)
                        {
                            if ((str3[0] == '-') || (str3[0] == '+'))
                            {
                                if ((key != "") && !switches.ContainsKey(key))
                                {
                                    switches.Add(key, "");
                                }
                                key = str3;
                            }
                            else if (key != "")
                            {
                                if (!switches.ContainsKey(key))
                                {
                                    switches.Add(key, str3);
                                }
                                key = "";
                            }
                        }
                    }
                    if ((key != "") && !switches.ContainsKey(key))
                    {
                        switches.Add(key, "");
                    }
                }
            }
        }

        // Properties
        public static string Full
        {
            get
            {
                Initalize();
                return commandline;
            }
        }

        public static string[] SplitQuotesStrings(this string input)
        {
            input = input.Replace("\\\"", "&qute;");
            MatchCollection matchs = new Regex("\"([^\"]+)\"|'([^']+)'|\\S+").Matches(input);
            string[] strArray = new string[matchs.Count];
            for (int i = 0; i < matchs.Count; i++)
            {
                char[] trimChars = new char[] {' ', '"'};
                strArray[i] = matchs[i].Groups[0].Value.Trim(trimChars);
                strArray[i] = strArray[i].Replace("&qute;", "\"");
            }
            return strArray;
        }
    }
}