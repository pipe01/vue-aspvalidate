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
            string asd = "hello";
            Expression<Func<string, object>> expr = o => Regex.IsMatch("asd", ".*").ToString();

            var js = ExpressionExtensions.ToJs(expr);

            Console.WriteLine($"({js})(0)");
            Console.WriteLine("----------");
            Console.WriteLine(new Beautifier().Beautify(js));
            Console.ReadKey(true);
        }
    }
}
