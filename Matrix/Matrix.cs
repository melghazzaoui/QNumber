using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algebra
{
    class Matrix : IQIndexable
    {
        // Attributs
        private int m;
        private int n;
        private QNumber[,] values;

        // Constructeurs

        public Matrix(string filename)
        {
            int rows = 0;
            int columns = 0;
            List<string> qdecimals = new List<string>();
            System.IO.TextReader txtReader = File.OpenText(filename);
            while(txtReader.Peek() != -1)
            {
                string line = txtReader.ReadLine();
                line = line.Trim();
                if (line.StartsWith("//") || line.StartsWith("#"))
                {
                    // line is a comment
                }
                else if (line != "")
                {
                    rows++;
                    string[] items = line.Split(new char[] { ' ', '\t' });
                    List<string> itemList = new List<string>();
                    foreach(string s in items)
                    {
                        if (s != "")
                        {
                            itemList.Add(s);
                        }
                    }
                    if (columns == 0)
                    {
                        columns = itemList.Count;
                    }
                    else if (columns != itemList.Count)
                    {
                        txtReader.Close();
                        throw new Exception("File wrong fromat (columns count is not same for all rows)");
                    }
                    qdecimals.AddRange(itemList);
                }
            }

            if (rows > 0 && columns > 0 && qdecimals.Count >= rows * columns)
            {
                values = new QNumber[rows, columns];
                this.m = rows;
                this.n = columns;
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {
                        values[i, j] = QNumber.fromString(qdecimals[j + columns * i]);
                    }
                }
            }
            else
            {
                txtReader.Close();
                throw new Exception("Matrix dimension is zero");
            }

            txtReader.Close();
        }

        public Matrix(QNumber[,] values)
        {
            this.m = (int)values.GetLongLength(0);
            this.n = (int)values.GetLongLength(1);
            this.values = values;
        }

        public Matrix(int m, int n)
        {
            this.m = m;
            this.n = n;
            values = new QNumber[m, n];
            for(int i=0;i<m; ++i)
            {
                for(int j=0; j<n; ++j)
                {
                    values[i, j] = new QNumber(0);
                }
            }

        }
        // Méthodes
        public void Set(int i, int j, QNumber valeur)
        {
            if (i >= 0 && i < m && j >= 0 && j < n)
            {
                values[i, j] = valeur;
            }
        }

        public void SetUnsafe(int i, int j, QNumber valeur)
        {
            values[i, j] = valeur;
        }


        public override string ToString()
        {
            string txt = "";

            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    txt += values[i, j] + " \t"; // Il détecte le double et le convertit implicitement en string

                }
                txt += "\n"; // retourne à la ligne
            }

            return txt;
        }

        public void ToFile(string filename)
        {
            FileStream fs = File.OpenWrite(filename);
            fs.Write(Encoding.ASCII.GetBytes(ToString()));
            fs.Close();
        }

        public QNumber GetUnsafe(int i, int j)
        {
            return values[i, j];
        }

        public QNumber GetSafe(int i, int j)
        {
            if (i >= 0 && i < m && j >= 0 && j < n)
            {
                return values[i, j];
            }
            else
            {
                throw new Exception("Index out of range");
            }
        }

        public Matrix Add(Matrix B)
        {
            if (m == B.m && n == B.n)
            {
                Matrix C = new Matrix(m, n);

                for (int i = 0; i < m; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        C.values[i, j] = values[i, j].Add(B.values[i, j]);
                    }
                }
                return C;
            }
            else
            {
                throw new Exception("Les matrices non pas les mêmes dimensions");
            }
        }

        public Matrix MultTerms(Matrix B)
        {
            if (m == B.m && n == B.n)
            {
                Matrix C = new Matrix(m, n);

                for (int i = 0; i < m; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        C.values[i, j] = values[i, j].Mult(B.values[i, j]);
                    }
                }
                return C;
            }
            else
            {
                throw new Exception("MultTerms error : Different dimensions");
            }
        }

        public Matrix AddScalar(QNumber scalar)
        {
            QNumber qscalar = scalar;
            Matrix C = new Matrix(m, n);

            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    C.values[i, j] = values[i, j].Add(qscalar);
                }
            }
            return C;
        }

        public void AddScalarInplace(QNumber scalar)
        {
            QNumber qscalar = scalar;
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    values[i, j] = values[i, j].Add(qscalar);
                }
            }
        }

        public Matrix Substract(Matrix B)
        {

            if (m == B.m && n == B.n)
            {
                Matrix C = new Matrix(m, n);

                for (int i = 0; i < m; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        C.values[i, j] = values[i, j].Sub(B.values[i, j]);
                    }
                }
                return C;
            }
            else
            {
                throw new Exception("Les matrices non pas les mêmes dimensions");
            }
        }


        public Matrix Mult(Matrix B)
        {
            if (n == B.m)
            {
                Matrix C = new Matrix(m, B.n);

                for (int i = 0; i < C.m; i++)
                {
                    for (int j = 0; j < C.n; j++)
                    {
                        QNumber res = new QNumber();

                        for (int k = 0; k < n; k++)
                        {
                            res = res.Add(values[i, k].Mult(B.values[k, j]));
                        }
                        C.values[i, j] = res;
                    }
                }

                return C;
            }
            throw new Exception("Mult error : incorrect dimensions ");
        }

        public Matrix MultScalar(QNumber scalar)
        {
            QNumber qscalar = scalar;
            Matrix C = new Matrix(m, n);

            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    C.values[i, j] = values[i, j].Mult(qscalar);
                }
            }
            return C;
        }

        public void MultScalarInplace(QNumber scalar)
        {
            QNumber qscalar = scalar;
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    values[i, j] = values[i, j].Mult(qscalar);
                }
            }
        }

        public Matrix DivScalar(QNumber scalar)
        {
            if (scalar.Value == 0)
            {
                throw new InvalidOperationException("Div by zero");
            }
            Matrix C = new Matrix(m, n);

            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    C.values[i, j] = values[i, j].Div(scalar);
                }
            }
            return C;
        }

        public void DivScalarInplace(QNumber scalar)
        {
            if (scalar.Value == 0)
            {
                throw new InvalidOperationException("Div by zero");
            }

            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    values[i, j] = values[i, j].Div(scalar);
                }
            }
        }

        public Matrix Transpose()
        {
            Matrix t = new Matrix(n, m);
            for (int i = 0; i < t.m; ++i)
            {
                for (int j = 0; j < t.n; ++j)
                {
                    t[i, j] = values[j, i];
                }
            }
            return t;
        }

        public void TransposeInplace()
        {
            int tm = n;
            int tn = m;
            QNumber[,] tvalues = new QNumber[tm, tn];
            for (int i = 0; i < tm; ++i)
            {
                for (int j = 0; j < tn; ++j)
                {
                    tvalues[i, j] = values[j, i];
                }
            }

            m = tm;
            n = tn;
            values = tvalues;
        }

        public Matrix GetSub(int i0, int j0)
        {
            if (m>1 && n>1)
            {
                int m1 = m - 1;
                int n1 = n - 1;
                Matrix sub = new Matrix(m1, n1);
                int i1 = 0;
                for (int i = 0; i < m; ++i)
                {
                    if (i != i0)
                    {
                        int j1 = 0;
                        for (int j = 0; j < n; ++j)
                        {
                            if (j != j0)
                            {
                                sub[i1, j1] = values[i, j];
                                j1++;
                            }
                        }
                        i1++;
                    }
                }
                return sub;
            }
            return null;
        }

        public QNumber Determinant()
        {
            if (m != n)
            {
                throw new InvalidOperationException("Can not inverse non-square matrix");
            }

            if(m == 2)
            {
                return (values[0, 0].Mult(values[1, 1])).Sub(values[1, 0].Mult(values[0, 1]));
            }
            else if (m == 1) 
            {
                return values[0, 0]; 
            }

            QNumber det = new QNumber();

            int i = 0;
            Int64 signC = 1;
            for (int j = 0; j < m; ++j)
            {
                QNumber detSub = GetSub(i, j).Determinant();
                det = det.Add(values[i,j].Mult(detSub).Mult(signC));

                signC = -signC;
            }

            return det;
        }

        public Matrix GetComatrix()
        {
            if (m != n)
            {
                throw new InvalidOperationException("Can not inverse non-square matrix");
            }

            Matrix comat = new Matrix(m, m);

            Int64 signR = 1;
            for(int i=0; i<m; ++i)
            {
                Int64 signC = signR;
                for(int j=0; j<m; ++j)
                {
                    QNumber detSub = GetSub(i, j).Determinant();
                    comat[i, j] = detSub.Mult(signC);

                    signC = -signC;
                }

                signR = -signR;
            }

            return comat;
        }

        public Matrix GetInv()
        {
            QNumber det = Determinant();
            Console.WriteLine("det = " + det);
            if (det.Value == 0)
            {
                return null;
            }

            // inv(A) = (1/det) * t(Comat(A))
            Matrix inv = GetComatrix(); // comatrix
            Console.WriteLine("comat = \n" + inv);
            inv.TransposeInplace(); // transpose comatrix
            inv.DivScalarInplace(det);
            return inv;
        }

        // Propriétés
        public int Rows
        {
            get { return m; }
        }

        public int Columns
        {
            get { return n; }
        }

        public QNumber this[int i, int j] 
        {
            get { return values[i, j]; }
            set { values[i, j] = value; }
        }
    }
}
