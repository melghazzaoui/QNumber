using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Text;

namespace Algebra
{

    class QNumber
    {
        private Int64 numerator;
        private Int64 denominator;


        public QNumber()
        {
            numerator = 0;
            denominator = 1;
        }

        private static Int64 GCD(Int64 a, Int64 b)
        {
            if (b != 0)
                return GCD(b, a % b);
            else
                return a;
        }

        public QNumber(Int64 numerator, Int64 denominator)
        {
            if (denominator == 0)
                throw new InvalidOperationException("div by zero error");


            // compute Greatest Common Divisor
            Int64 gcd = getGCD(numerator, denominator);
            numerator /= gcd;
            denominator /= gcd;

            // Bring the sign to the numerator.
            // Denominator should be always positive
            if (denominator < 0)
            {
                numerator = -numerator;
                denominator = -denominator;
            }

            this.numerator = numerator;
            this.denominator = denominator;
        }

        public QNumber(Int64 a)
        {
            numerator = a;            
            denominator = 1;
        }

        public static Int64 Pow(Int64 a, Int64 b)
        {
            if (b > 0)
            {
                Int64 p = a;
                for(Int64 n =1; n<b; n++)
                {
                    p *= a;
                }
                return p;
            }
            else if (b == 0)
                return 1;
            else
                throw new InvalidOperationException("Pow with negative argument");
        }

        private static List<Int64> allPrimeNumbers = new List<Int64>(new Int64[] { 2, 3 });
        static Int64 startPoint = 5;

        public static bool isPrimeNumber(Int64 n)
        {
            double sqrtN = Math.Sqrt(n);
            foreach(Int64 p in allPrimeNumbers)
            {
                if ((double)p > sqrtN)
                    return true;
                if (n % p == 0)
                    return false;
            }
            return true;
        } 

        private static bool isNewPrimeNumber(Int64 n)
        {
            double sqrtN = Math.Sqrt(n);
            using(var enumerable = allPrimeNumbers.GetEnumerator())
            {
                if (enumerable.MoveNext()) // ignore p=2
                {
                    Int64 p = 0;
                    while (enumerable.MoveNext())
                    {
                        p = enumerable.Current;
                        if ((double)p > sqrtN)
                            return true;
                        else if (n % p == 0)
                            return false;
                    }
                }
            }
            return true;
            
        }
        public static void updateAllPrimeNumbers(Int64 limit)
        {
            while (startPoint <= limit)
            {
                if (isNewPrimeNumber(startPoint))
                    allPrimeNumbers.Add(startPoint);
                startPoint += 2;
            }
        }

        public static List<Int64> primeNumbers
        {
            get { return allPrimeNumbers; }
        }

        public static QNumber fromString(string s)
        {
            string msg = "Failed to convert string to QDecimal";
            Int64 numerator = 0;
            Int64 denominator = 1;
            string[] items = s.Split('/');

            if (items.Length > 2)
            {
                throw new Exception(msg);
            }

            if(items.Length == 1 && (s.Contains('.') || s.Contains(',')))
            {
                if (s.Contains('.'))
                    items = s.Split('.');
                else if(s.Contains(','))
                    items = s.Split(',');
                if (items.Length == 2)
                {
                    try
                    {
                        Int64 part1 = Int64.Parse(items[0]);
                        Int64 part2 = Int64.Parse(items[1]);
                        double full = 0;
                        if (!Double.TryParse(s, out full))
                        {
                            s = s.Replace('.', ',');
                            full = Double.Parse(s);
                        }
                        denominator = (Int64)Math.Pow(10, items[1].Length);
                        numerator = (Int64)(full * denominator);
                        return new QNumber(numerator, denominator);
                    }
                    catch (Exception)
                    {
                        throw new Exception(msg);
                    }
                }
                else
                    throw new Exception(msg);
            }

            if (items.Length >= 1)
            {
                try
                {
                    numerator = Int64.Parse(items[0]);
                    if (items.Length == 2)
                    {
                        denominator = Int64.Parse(items[1]);
                    }
                    return new QNumber(numerator, denominator);
                }
                catch
                {
                    throw new Exception(msg);
                }
            }
            return null;
        }


        public static SortedDictionary<Int64, Int64> primeFactorization(Int64 a)
        {
            updateAllPrimeNumbers(a);

            SortedDictionary<Int64, Int64> primeFactors = new SortedDictionary<Int64, Int64>();

            Int64 A = 1;
            Int64 a1 = a;
            foreach(Int64 p in allPrimeNumbers)
            {
                if (A != a)
                {
                    Int64 count = 0;
                    while(a1 % p == 0)
                    {
                        a1 /= p;
                        A *= p;
                        count++;
                    }
                    if(count > 0)
                    {
                        primeFactors.Add(p, count);
                    }
                }
                else
                {
                    break;
                }
            }

            return primeFactors;
        }

        /*
         * get the Greatest Common Divisor
         */
        public static Int64 getGCD(Int64 a, Int64 b)
        {
            return GCD(a, b);
        }

        /*
         * get the Least Common Multiple 
         */
        public static Int64 getLCM(Int64 a, Int64 b)
        {
            if (a == 0 || b == 0)
                return 0;
            return (a * b) / getGCD(a, b);
        }

        public static bool isInt(double a)
        {
            return Math.Round(a) == a;
        }

        public static QNumber Div(double a, double b, out double res)
        {
            res = 0;
            if (b != 0)
            {
                res = a / b;
                if (isInt(a) && isInt(b))
                {
                    return new QNumber((Int64)a, (Int64)b);
                }
                else if (isInt(res))
                {
                    return new QNumber((Int64)res, 1);
                }
                else
                {
                    return null;
                }
            }
            else
                throw new InvalidOperationException("div by zero");
        }

        public double Value
        {
            get { return (double)numerator/denominator; }
        }

        public QNumber Add(QNumber q)
        {
            Int64 up1 = numerator * q.denominator + q.numerator * denominator;
            Int64 down1 = denominator * q.denominator;
            QNumber res = new QNumber(up1, down1);
            return res;
        }

        public QNumber Add(Int64 n)
        {
            return Add(new QNumber(n));
        }

        public QNumber Sub(QNumber q)
        {
            Int64 up1 = (numerator * q.denominator) - (q.numerator * denominator);
            Int64 down1 = denominator * q.denominator;
            return new QNumber(up1, down1);
        }

        public QNumber Sub(Int64 n)
        {
            return Sub(new QNumber(n));
        }

        public QNumber Mult(QNumber q)
        {
            Int64 up1 = numerator * q.numerator;
            Int64 down1 = denominator * q.denominator;
            return new QNumber(up1, down1);
        }

        public QNumber Mult(Int64 n)
        {
            return Mult(new QNumber(n));
        }

        public QNumber Div(QNumber q)
        {
            return Mult(new QNumber(q.denominator, q.numerator));
        }

        public QNumber Div(Int64 n)
        {
            if (n != 0)
                return new QNumber(numerator, denominator * n);
            throw new Exception("Division by zero");
        }

        public override string ToString()
        {
            if (numerator == 0)
            {
                return ((int)0).ToString();
            }
            else
            {
                string strDown = (denominator != 1) ? ("/" + denominator.ToString()) : "";
                return numerator + strDown;
            }
        }
    }
}
