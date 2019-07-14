using Jsbeautifier;
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
            Expression<Func<string, object>> expr = o => Regex.IsMatch(o, ".*"[0].ToString());

            var js = Expression2JS.ToJs(expr);

            Console.WriteLine(js);
            Console.WriteLine("----------");
            Console.WriteLine(new Beautifier().Beautify(js));
            Console.ReadKey(true);
        }
    }
}
