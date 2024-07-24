using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

namespace Jenga {
    public static class DebugConsole  {

        // class ActionEnvironment {
        //     public Dictionary<string, string> vars = new();
        //     public T? Get<T>(string key) where T : IConvertable 
        //         => vars.ContainsKey(key) 
        //             ? vars[key] == "" 
        //                 ? default(T)
        //                 : (T)Convert.ChangeType(vars[key], typeof(T))
        //             : default(T);


        //     public static int TryMatch(
        //         string fmt, string input, out ActionEnvironment env
        //     ) {
        //         env = new();
        //         var fmtWords = fmt.Split(' ');
        //         var inpWords = input.Split(' ');

        //         var fmtLen = fmtWords.Length;
        //         var inpLen = inpWords.Length;

        //         if (inpLen > fmtLen) return 0;

        //         var matchedLetters = 0;

        //         for (int i = 0; i < fmtLen; ++i) {
        //             var fmt = fmtWords[i];
        //             var inp = inpWords[i];

        //             if (fmt != '<' && fmt == inp) continue;  
        //             if (fmt != '<' && !fmt.StartsWith(inp)) return 0; 
        //             matchedLetters += inp.Length;
        //             if (fmt != '<') return matchedLetters;

        //             var pattern = @"\<(?<option>\w+)(\[=(?<default>\w+)\])?\>";
        //             var match = Regex.Match(pattern, fmt);
        //             var option = match.Groups["option"].Value;
        //             // var deflt = match.Groups["default"].Value;

        //             env.vars.Add(option, inp);
        //         }

        //         for (int i = inpLen; i < fmtLen; ++i) {
        //             var fmt = fmtWords[i];

        //             if (fmt != '<') return matchedLetters;

        //             var pattern = @"\<(?<option>\w+)(\[=(?<default>\w+)\])?\>";
        //             var match = Regex.Match(pattern, fmt);
        //             var option = match.Groups["option"].Value;
        //             var deflt = match.Groups["default"].Value;

        //             env.vars.Add(option, deflt);
        //         }
        //     }
        // }

        // class Action {
        //     public string format;
        //     public string description;
        //     public System.Action<ActionEnvironment> action;
        // }

        // // static List<> actions

    }
}