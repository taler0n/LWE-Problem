using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace LWE_Problem
{
    public class RegevCryptosystem
    {
        public class VectorIntPair
        {
            public int[] A { get; private set; }
            public int B { get; private set; }

            public VectorIntPair(int[] a, int b)
            {
                A = a;
                B = b;
            }

            public override string ToString()
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("(a = { ");
                for (int j = 0; j < A.Length - 1; j++)
                {
                    builder.Append(A[j]);
                    builder.Append(", ");
                }
                builder.Append(A[A.Length - 1]);
                builder.Append(" }, b = ");
                builder.Append(B);
                return builder.ToString();
            }
        }

        private readonly int _N;
        private readonly int _module;
        private readonly int _dimension;
        private ZqSecureRNG _rng;
        private int[] _secret;
        private VectorIntPair[] _open;
        public RegevCryptosystem(int n, int q)
        {
            _N = n;
            _module = q;
            double dimension = (1 + Double.Epsilon) * (1 + n) * Math.Log(q);
            _dimension = Convert.ToInt32(Math.Round(dimension));
            _rng = new ZqSecureRNG(q);
            _secret = _rng.GenerateVector(n);
            GenerateOpenKey();
        }

        private void GenerateOpenKey()
        {
            _open = new VectorIntPair[_dimension];
            for (int i = 0; i < _dimension; i++)
            {
                int[] a = _rng.GenerateVector(_N);
                //вот тут волшебная неправильная ошибка
                int error = _rng.GenerateInt(2);
                int b = (CalculateScalar(a, _secret) + error) % _module;
                _open[i] = new VectorIntPair(a, b);
            }
        }

        public void PrintParams()
        {
            Console.WriteLine("Размерность m: {0}", _dimension);
            StringBuilder builder = new StringBuilder();
            builder.Append('{');
            for (int i = 0; i < _secret.Length - 1; i++)
            {
                builder.Append(_secret[i]);
                builder.Append(", ");
            }
            builder.Append(_secret[_secret.Length - 1]);
            builder.Append('}');
            Console.WriteLine("Закрытый ключ: {0}", builder.ToString());
            Console.WriteLine("Открытый ключ:");
            for (int i = 0; i < _open.Length; i++)
            {
                Console.WriteLine("\t{0}. {1}", i + 1, _open[i].ToString());
            }
        }

        public VectorIntPair[] Encrypt(string message)
        {
            //всю строку в бинарь
            string binary = string.Join("", Encoding.UTF8.GetBytes(message).Select(n => Convert.ToString(n, 2).PadLeft(8, '0')));
            VectorIntPair[] output = new VectorIntPair[binary.Length];  
            for (int i = 0; i < binary.Length; i++)
            {
                //каждый бит - в пару вектор-число
                List<int> subset = _rng.GetSubset(_dimension);
                int[] a = new int[_N];
                int b = int.Parse(binary[i].ToString()) + _module;
                if (b % 2 == 1)
                {
                    b += _module;
                }
                b /= 2;
                for (int j = 0; j < subset.Count; j++)
                {
                    a = CalculateVectorSum(a, _open[subset[j]].A);
                    b += _open[subset[j]].B;
                }
                b %= _module;
                output[i] = new VectorIntPair(a, b);
            }
            return output;
        }

        public string Decrypt(VectorIntPair[] message)
        {
            List<char> bits = new List<char>();
            for (int i = 0; i < message.Length; i++)
            {
                int dividend = (((message[i].B - CalculateScalar(message[i].A, _secret)) % _module) + _module) % _module;
                double x = (double)dividend / _module;
                if (x < 0.25 || x > 0.75)
                {
                    bits.Add('0');
                }
                else
                {
                    bits.Add('1');
                }
            }
            byte[] bytes = new byte[message.Length / 8];
            for (int i = 0; i < message.Length / 8; i++)
            {
                StringBuilder bitsForByte = new StringBuilder();
                for (int j = 0; j < 8; j++)
                {
                    bitsForByte.Append(bits[i * 8 + j]);
                }
                bytes[i] = Convert.ToByte(bitsForByte.ToString(), 2);
            }
            return Encoding.UTF8.GetString(bytes);
        }

        private int[] CalculateVectorSum(int[] v1, int[] v2)
        {
            if (v1.Length == v2.Length)
            {
                int[] sum = new int[v1.Length];
                for (int i = 0; i < v1.Length; i++)
                {
                    sum[i] = (v1[i] + v2[i]) % _module;
                }
                return sum;
            }
            return null;
        }

        private int CalculateScalar(int[] v1, int[] v2)
        {
            if (v1.Length == v2.Length)
            {
                int sum = 0;
                for (int i = 0; i < v1.Length; i++)
                {
                    sum += v1[i] * v2[i];
                }
                return sum;
            }
            return -1;
        }
    }
}
