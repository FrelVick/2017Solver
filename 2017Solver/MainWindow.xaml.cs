using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace _2017Solver
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>

    public class AritmSolver
    {
        public bool BySigns = false;

        public struct Record
        {
            public string Sequence { get; }
            public string Expression { get; }
            public double Value { get; }
            public int Digits { get; }
            public int Signs { get; }

            public Record(string seq, string exp, double val, int dig, int sig)
            {
                Sequence = seq;
                Expression = exp;
                Value = val;
                Digits = dig;
                Signs = sig;
            }
        }

        public List<Record> ExpRecords;

        public AritmSolver(string s)
        {
            ExpRecords = new List<Record>();
            Add_To_List(s);
        }

        public List<Record> GetRecords(string s)
        {
            if (ExpRecords.All(rec => rec.Sequence != s)) Add_To_List(s);
            return ExpRecords.Where(rec => rec.Sequence == s).ToList();
        }

        public void Add_To_List(string s)
        {
            if (ExpRecords.Any(rec => rec.Sequence == s)) { return; }

            ExpRecords.Add(new Record(s, s, double.Parse(s), s.Length, 0));
            ExpRecords.Add(new Record(s, "-" + s, -1 * double.Parse(s), s.Length, 1));
            try
            {
                if (int.Parse(s) < 12) ExpRecords.Add(new Record(s, s + "!", Factorial(int.Parse(s)), 1, 1));
                if (Math.Abs(Math.Sqrt(int.Parse(s)) % 1) <= (Double.Epsilon * 100)) ExpRecords.Add(new Record(s, "sqr(" + s + ")", Math.Sqrt(int.Parse(s)), 1, 1));
            }
            catch (Exception e)
            {
                //Console.WriteLine(e);
                //throw;
            }


            Remove_redondance();

            for (int i = 1; i < s.Length; i++)
            {
                var p1 = GetRecords(s.Substring(0, i));
                var p2 = GetRecords(s.Substring(i));
                foreach (var part1 in p1)
                {
                    foreach (var part2 in p2)
                    {
                        int digits = part1.Digits + part2.Digits;
                        int signs = part1.Signs + part2.Signs;
                        ExpRecords.Add(new Record(s, $"({part1.Expression})+({part2.Expression})", part1.Value + part2.Value, digits, signs + 1));
                        ExpRecords.Add(new Record(s, $"({part1.Expression})-({part2.Expression})", part1.Value - part2.Value, digits, signs + 1));
                        ExpRecords.Add(new Record(s, $"({part1.Expression})*({part2.Expression})", part1.Value * part2.Value, digits, signs + 1));
                        if (Math.Abs(part1.Value / part2.Value % 1) <= (Double.Epsilon * 100)) ExpRecords.Add(new Record(s, $"({part1.Expression})/({part2.Expression})", part1.Value / part2.Value, digits, signs + 1));
                        if (Math.Abs(Math.Pow(part1.Value, part2.Value) % 1) <= (Double.Epsilon * 100)) ExpRecords.Add(new Record(s, $"({part1.Expression})^({part2.Expression})", Math.Pow(part1.Value, part2.Value), digits, signs + 1));
                        //toAdd.Add($"({part1.Key}) root of ({part2.Key})", Math.Pow(part1.Value, 1 / part2.Value));

                    }
                }
                Remove_redondance();
            }



        }

        private static int Factorial(int i)
        {
            return i < 0 ? -1 : i == 0 || i == 1 ? 1 : Enumerable.Range(1, i).Aggregate((counter, value) => counter * value);
        }


        public void Remove_redondance()
        {
            if (BySigns)
            {
                ExpRecords = ExpRecords.Where(rec => Math.Abs(rec.Value) < 1000000).GroupBy(rec => rec.Value)
                    .Select(g => g.OrderBy(rec => rec.Signs).First()).ToList();
            }
            else
            {
                ExpRecords = ExpRecords.Where(rec => Math.Abs(rec.Value) < 1000000).GroupBy(rec => rec.Value)
                    .Select(g => g.OrderBy(rec => rec.Digits).First()).ToList();
            }
        }
    }

    public partial class MainWindow
    {
        public Dictionary<string, Dictionary<string, double>> Dict = new Dictionary<string, Dictionary<string, double>>();

        public MainWindow()
        {
            InitializeComponent();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            List<AritmSolver.Record> res = new List<AritmSolver.Record>();
            for (var i = '1'; i <= '9'; i++)
            {
                var s = i.ToString();
                var p = new AritmSolver(i.ToString());
                while (p.ExpRecords.All(rec => Math.Abs(rec.Value - 2017) > 0.0001))
                {
                    p.Add_To_List(s += i.ToString());
                }
                res.AddRange(p.ExpRecords.Where(rec => rec.Value == 2017).ToList());
            }
            for (var i = '1'; i <= '9'; i++)
            {
                var s = i.ToString();
                var p = new AritmSolver(i.ToString());
                p.BySigns = true;
                while (p.ExpRecords.All(rec => Math.Abs(rec.Value - 2017) > 0.0001))
                {
                    p.Add_To_List(s += i.ToString());
                }
                res.AddRange(p.ExpRecords.Where(rec => rec.Value == 2017).ToList());
            }
            Result.ItemsSource = res;

        }

        private void Result_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}
