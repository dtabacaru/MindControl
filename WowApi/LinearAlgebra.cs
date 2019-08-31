using System;
using System.Collections.Generic;

namespace ClassicWowNeuralParasite
{
    public class Matrix
    {
        private double[,] m_Data;

        public int NumberOfColumns { get; }

        public int NumberOfRows { get; }

        public Matrix(List<double[]> mList)
        {
            NumberOfRows = mList.Count;
            NumberOfColumns = mList[0].Length;
            m_Data = new double[NumberOfRows, NumberOfColumns];
            Array.Clear(m_Data, 0, m_Data.Length);

            for (int i = 0; i < NumberOfRows; i++)
                for (int j = 0; j < NumberOfColumns; j++)
                    m_Data[i, j] = mList[i][j];
        }

        public Matrix(int nRows, int nCols)
        {
            NumberOfColumns = nCols;
            NumberOfRows = nRows;
            m_Data = new double[NumberOfRows, NumberOfColumns];
            Array.Clear(m_Data, 0, m_Data.Length);
        }

        internal Matrix(Matrix m)
        {
            NumberOfColumns = m.NumberOfColumns;
            NumberOfRows = m.NumberOfRows;
            m_Data = new double[NumberOfRows, NumberOfColumns];
            Array.Clear(m_Data, 0, m_Data.Length);

            for (int i = 0; i < m.NumberOfRows; i++)
                for (int j = 0; j < m.NumberOfColumns; j++)
                    m_Data[i, j] = m[i, j];
        }

        public double this[int rowKey, int colKey]
        {
            get
            {
                return m_Data[rowKey, colKey];
            }
            set
            {
                m_Data[rowKey, colKey] = value;
            }
        }

        public Matrix Transpose()
        {
            Matrix output = new Matrix(this.NumberOfColumns, this.NumberOfRows);

            for (int i = 0; i < output.NumberOfRows; i++)
                for (int j = 0; j < output.NumberOfColumns; j++)
                    output[i, j] = this[j, i];

            return output;
        }

        public static Matrix operator !(Matrix m1)
        {
            Matrix thisCopy = new Matrix(m1);
            Matrix output = Identity(m1.NumberOfRows);

            for (int i = 0; i < m1.NumberOfRows; i++)
            {
                if (thisCopy[i, i] != 0)
                {
                    for (int j = 0; j < m1.NumberOfRows; j++)
                    {
                        double s = thisCopy[i, j];
                        thisCopy[i, j] = thisCopy[i, j];
                        thisCopy[i, j] = s;
                        s = output[i, j];
                        output[i, j] = output[i, j];
                        output[i, j] = s;
                    }

                    double t = 1 / thisCopy[i, i];
                    for (int j = 0; j < m1.NumberOfRows; j++)
                    {
                        thisCopy[i, j] = t * thisCopy[i, j];
                        output[i, j] = t * output[i, j];
                    }

                    for (int j = 0; j < m1.NumberOfRows; j++)
                    {
                        if (j != i)
                        {
                            t = -thisCopy[j, i];

                            for (int m = 0; m < m1.NumberOfRows; m++)
                            {
                                thisCopy[j, m] = thisCopy[j, m] + t * thisCopy[i, m];
                                output[j, m] = output[j, m] + t * output[i, m];
                            }
                        }
                    }
                }

            }

            return output;
        }

        public static Matrix Identity(int n)
        {
            Matrix output = new Matrix(n, n);

            for (int i = 0; i < n; i++)
                output[i, i] = 1;

            return output;
        }

        public static Vector operator *(Matrix m, Vector v)
        {
            Vector output = new Vector(m.NumberOfRows);

            for (int i = 0; i < m.NumberOfRows; i++)
                for (int j = 0; j < m.NumberOfColumns; j++)
                    output[i] += m[i, j] * v[j];

            return output;
        }

        public static Matrix operator *(Matrix m1, Matrix m2)
        {
            Matrix output = new Matrix(m1.NumberOfRows, m2.NumberOfColumns);

            for (int i = 0; i < m1.NumberOfRows; i++)
                for (int j = 0; j < m2.NumberOfColumns; j++)
                    for (int k = 0; k < m1.NumberOfColumns; k++)
                        output[i, j] += m1[i, k] * m2[k, j];

            return output;
        }

        public static Matrix operator *(Matrix m, double c)
        {
            Matrix output = new Matrix(m.NumberOfRows, m.NumberOfColumns);

            for (int i = 0; i < output.NumberOfRows; i++)
                for (int j = 0; j < output.NumberOfColumns; j++)
                    output[i, j] = m[i, j] * c;

            return output;
        }

        public static Matrix operator +(Matrix m1, Matrix m2)
        {
            Matrix output = new Matrix(m1.NumberOfRows, m1.NumberOfColumns);

            for (int i = 0; i < m1.NumberOfRows; i++)
                for (int j = 0; j < m1.NumberOfColumns; j++)
                    output[i, j] = m1[i, j] + m2[i, j];

            return output;
        }

        public static Matrix operator -(Matrix m1, Matrix m2)
        {
            Matrix output = new Matrix(m1.NumberOfRows, m1.NumberOfColumns);

            for (int i = 0; i < m1.NumberOfRows; i++)
                for (int j = 0; j < m1.NumberOfColumns; j++)
                    output[i, j] = m1[i, j] - m2[i, j];

            return output;
        }
    }

    public class Vector
    {
        private double[] m_Data;

        public int NumberOfElements { get; }

        public Vector(List<double> vList)
        {
            NumberOfElements = vList.Count;
            m_Data = new double[NumberOfElements];
            Array.Clear(m_Data, 0, m_Data.Length);

            for (int i = 0; i < NumberOfElements; i++)
                m_Data[i] = vList[i];
        }

        public Vector(int nElements)
        {
            NumberOfElements = nElements;
            m_Data = new double[NumberOfElements];
            Array.Clear(m_Data, 0, m_Data.Length);
        }

        public Vector(Vector v)
        {
            NumberOfElements = v.NumberOfElements;
            m_Data = new double[NumberOfElements];
            Array.Clear(m_Data, 0, m_Data.Length);

            for (int i = 0; i < NumberOfElements; i++)
                m_Data[i] = v[i];
        }

        public static Vector operator +(Vector v1, Vector v2)
        {
            Vector output = new Vector(v1.NumberOfElements);

            for (int i = 0; i < v1.NumberOfElements; i++)
                output[i] = v1[i] + v2[i];

            return output;
        }

        public static Vector operator -(Vector v1, Vector v2)
        {
            Vector output = new Vector(v1.NumberOfElements);

            for (int i = 0; i < v1.NumberOfElements; i++)
                output[i] = v1[i] - v2[i];

            return output;
        }

        public double this[int key]
        {
            get
            {
                return m_Data[key];
            }
            set
            {
                m_Data[key] = value;
            }
        }

    }

}
