using System;
using System.Transactions;

namespace Algebra
{
    class Program
    {
        static Matrix GenRandomMatrix(int m, int n, int min, int max)
        {
            Matrix A = new Matrix(m, n);
            Random r = new Random();
            for(int i=0; i<m; ++i)
            {
                for(int j=0; j<n; ++j)
                {
                    int signP = r.Next(0, 1000);
                    Int64 sign = 1;
                    if (signP >= 500)
                    {
                        sign = -1;
                    }

                    QNumber x = new QNumber(r.Next(min, max), r.Next(min == 0 ? min + 1 : min, max));
                    A[i, j] = x.Mult(sign);
                }
            }
            return A;
        }

        /*
         * Solve System A*X = B
         * X = inv(A)*B
         */
        static Matrix SolveSystem(Matrix A, Matrix B)
        {
            if (B.Columns > 1)
            {
                throw new InvalidOperationException("Matrix B should be a column (m,1)");
            }

            if (A.Rows != A.Columns)
            {
                throw new InvalidOperationException("Matrix A should be a square matrix (m,m)");
            }

            QNumber det = A.Determinant();
            if (det.Value == 0)
            {
                return null;
            }

            Matrix inv = A.GetComatrix();
            inv.TransposeInplace();
            Matrix X = inv.Mult(B);
            X.DivScalarInplace(det);
            return X;
        }

        static QNumber Q(string s)
        {
            return QNumber.fromString(s);
        }

        static QNumber Q(double f)
        {
            return new QNumber((Int64)f);
        }

        static QNumber Q(Int64 n)
        {
            return new QNumber(n);
        }

        static void Main(string[] args)
        {   
            try
            {
                QNumber qn = new QNumber(3, -2);
                Console.WriteLine("qn = " + qn);
                int min = 1;
                int max = 1000;
                int n = 4;
                //Matrix A = new Matrix("../../../data/A.mat");
                Matrix A = GenRandomMatrix(n, n, min, max);
                /*A[0, 0] = Q(-1); A[0, 1] = Q(-1); A[0, 2] = Q(-2);
                A[1, 0] = Q(1); A[1, 1] = Q("-1/2"); A[1, 2] = Q(2);
                A[2, 0] = Q("1/2"); A[2, 1] = Q(-1); A[2, 2] = Q(1);*/
                A.ToFile("../../../data/A.mat");

                //Matrix B = new Matrix("../../../data/B.mat");
                Matrix B = GenRandomMatrix(n, 1, min, max);
                B.ToFile("../../../data/B.mat");
                /*B[0, 0] = Q(2);
                B[1, 0] = Q(0);
                B[2, 0] = Q(1);*/

                Console.WriteLine("A = \n" + A);
                Console.WriteLine("B = \n" + B);
                Matrix X = SolveSystem(A, B);
                Console.WriteLine("X = " + (X!=null?("\n"+X.ToString()):"None"));
                Console.WriteLine("\nPrime numbers :");
                /*foreach(Int64 p in QDecimal.primeNumbers)
                {
                    Console.Write(p); Console.Write(" ");
                }*/
                Console.WriteLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
        }
    }
}
