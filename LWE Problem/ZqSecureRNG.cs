using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace LWE_Problem
{
    class ZqSecureRNG
    {
        private RNGCryptoServiceProvider _rng;
        private readonly int _module;
        public ZqSecureRNG(int q)
        {
            _rng = new RNGCryptoServiceProvider();
            _module = q;
        }

        public int GenerateInt()
        {
            byte[] bytes = new byte[4];
            _rng.GetBytes(bytes);
            return Math.Abs(BitConverter.ToInt32(bytes)) % _module;
        }

        public int GenerateInt(int module)
        {
            byte[] bytes = new byte[4];
            _rng.GetBytes(bytes);
            return Math.Abs(BitConverter.ToInt32(bytes)) % module;
        }

        private int GenerateBit()
        {
            byte[] bytes = new byte[1];
            _rng.GetBytes(bytes);
            return Math.Abs(bytes[0]) % 2;
        }

        public int[] GenerateVector(int length)
        {
            int[] vector = new int[length];
            for (int i = 0; i < length; i++)
            {
                vector[i] = GenerateInt();
            }
            return vector;
        }

        public List<int> GetSubset(int length)
        {
            List<int> list = new List<int>();
            for (int i = 0; i < length; i++)
            {
                int decider = GenerateBit();
                if (decider == 1)
                {
                    list.Add(i);
                }
            }
            return list;
        }
    }
}
