using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Linq.Expressions;
using System.Security.Cryptography;
using NCalc;
using System.Text.RegularExpressions;

namespace IntegralApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int _n;
        private double _a, _b, _h;
        private string _f;

        public int N
        {
            get { return _n; }
            set { _n = value; }
        }

        public double A
        {
            get { return _a; }
            set { _a = value; }
        }

        public double B
        {
            get { return _b; }
            set { _b = value; }
        }

        public double H
        {
            get { return _h; }
            set { _h = value; }
        }

        public string F
        {
            get { return _f; }
            set { _f = value; }
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void NTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(NTextBox.Text, out int n) && n > 0)
            {
                N = n;
            }
            else
            {
                MessageBox.Show($"N должно быть целым числом больше 0!");
            }
        }

        private void ATextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (double.TryParse(ATextBox.Text, out double a))
            {
                A = a;
            }
        }

        private void BTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (double.TryParse(BTextBox.Text, out double b))
            {
                B = b;
            }
        }

        private void FTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            F = FTextBox.Text;
        }

        private void SButton_Click(object sender, RoutedEventArgs e)
        {
            Integral i = new Integral();
            
            H = i.GetH(A, B, N);
            double[] x = i.GetArrayOfX(N, A, H);
            double[] y = i.GetArrayOfY(x, F, N);
            double res = i.Trapezoid(H, y);

            MessageBox.Show($"~ {res}");
        }
    }

    class Integral
    {
        public double GetH(double a, double b, int n) => (b - a) / n;

        public double[] GetArrayOfX(int n, double a, double h)
        {
            double x0 = a;
            double[] array = new double[n + 1];
            array[0] = x0;

            for (int i = 1; i <= n; i++)
            {
                array[i] = array[i - 1] + h;
            }
            
            return array;
        }

        public double[] GetArrayOfY(double[] x, string f, int n)
        {
            double[] array = new double[n + 1];
            
            for (int i = 0; i <= n; i++)
            {
                array[i] = Evaluate(f, x[i]);
            }

            return array;
        }

        private double Evaluate(string f, double x)
        {
            string[] patterns = { @"\b(x)\b", @"\b(sin)\b", @"\b(cos)\b" };

            // Define replacements for each pattern
            string[] replacements = { "[x]", "Sin", "Cos" };

            // Apply replacements using regular expressions
            for (int i = 0; i < patterns.Length; i++)
            {
                f = Regex.Replace(f, patterns[i], replacements[i]);
            }
            
            if (f.Contains("^")) f = ReplaceExponentiation(f);

            NCalc.Expression e = new NCalc.Expression(f);
            e.Parameters["x"] = x;
            
            double res = Convert.ToDouble(e.Evaluate());
            return res;
        }

        private string ReplaceExponentiation(string formula)
        {
            // Define a regular expression pattern to match the exponentiation pattern 'x^2'
            string pattern = @"\[(\w+)\]\^(\d+)";

            // Use a MatchEvaluator delegate to replace matches with 'Pow' function
            string replacedFormula = Regex.Replace(formula, pattern, match =>
            {
                string baseValue = match.Groups[1].Value;
                string exponent = match.Groups[2].Value;
                return $"(Pow({baseValue},{exponent}))";
            });

            return replacedFormula;
        }

        public double Trapezoid(double h, double[] y)
        {
            double sum = 0, res = 0, trapezoidPart = (y[0] + y[y.Length - 1]) / 2;
            
            for (int i = 1; i < y.Length; i++)
            {
                sum += y[i];
            }
            
            res = h * (sum + trapezoidPart);
            return res;
        }
    }
}
