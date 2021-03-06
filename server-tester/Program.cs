﻿using Jsbeautifier;
using System;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using VueAspValidate.JS;

namespace server_tester
{
    class Program
    {
        static void Main(string[] args)
        {
            Expression<Func<string, object>> expr = o => Regex.IsMatch("asd", "\\w", RegexOptions.Multiline | RegexOptions.IgnoreCase);

            var js = expr.ToJs();

            Console.WriteLine($"( {js} )(0)");
            Console.WriteLine("----------");
            Console.WriteLine(new Beautifier().Beautify(js));
            Console.ReadKey(true);
        }
    }
}
