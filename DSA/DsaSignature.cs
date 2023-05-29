using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
namespace DSA
{
    internal class DsaSignature
    {
        public BigInteger Q;
        public BigInteger P;
        public BigInteger Y;
        private BigInteger G;
        private BigInteger X;
        public DsaSignature()
        {

        }
        public DsaSignature(BigInteger q)
        {
            if (!MillerRabinTest(q, 10))
            {
                throw new Exception("q должно быть простое");
            }
            
            Q = q;
            P = GetP();
            G = GetG();
            X = GetX();
            Y = GetY();
        }
        private BigInteger GetP()
        {
            BigInteger p = Q + 1;
            while (true)
            {
                if (MillerRabinTest(p, 10) && (p - 1) % Q == 0)
                    return p;
                else
                    p++;
            }
        }
        private BigInteger GetG()
        {
            BigInteger h = 2;
            BigInteger g;
            do
            {
                g = BigInteger.ModPow(h, (P - 1) / Q, P);
            }
            while (g <= 1) ;
            return g;
        }
        private BigInteger GetY()
        {
            return BigInteger.ModPow(G, X, P);
        }
        private BigInteger GetX()
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            Random random = new Random();
            byte[] _a = new byte[P.ToByteArray().LongLength];
            BigInteger x;

            do
            {
                random.NextBytes(_a);
                x = new BigInteger(_a);
            }
            while (x <= 0 || x >= Q);

            return x;
        }
        public (string,BigInteger,BigInteger) Signature(string str)
        {
            BigInteger r;
            BigInteger s;
            do
            {
                BigInteger k = 2;
                BigInteger h = str.GetHashCode();
                r = BigInteger.ModPow(G,k,P) % Q;
                s = Mod((Invmod(k,Q) * (h + X * r)),Q); 

            } while (r == 0 || s == 0);
            return (str, r, s);
        }
        public bool CheckSignature((string, BigInteger, BigInteger) sms,(BigInteger, BigInteger,BigInteger) publicKey)
        {
            string message = sms.Item1;
            BigInteger r = sms.Item2;
            BigInteger s = sms.Item3;
            BigInteger P = publicKey.Item1;
            BigInteger Q = publicKey.Item2;
            BigInteger Y = publicKey.Item3;
            BigInteger w = Invmod(s,Q);
            BigInteger u1 = Mod((message.GetHashCode() * w) , Q);
            BigInteger u2 = Mod((r * w), Q);
            BigInteger v = BigInteger.Pow(G, (int)u1) * BigInteger.Pow(Y, (int)u2) % P % Q;
            if (r == v)
                return true;
            else
                return false;
        }
        private bool MillerRabinTest(BigInteger n, int k)
        {
            if (n == 2 || n == 3)
                return true;
            if (n < 2 || n % 2 == 0)
                return false;

            BigInteger t = n - 1;
            int s = 0;
            while (t % 2 == 0)
            {
                t /= 2;
                s++;
            }

            for (int i = 0; i < k; i++)
            {
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                byte[] _a = new byte[n.ToByteArray().LongLength];
                BigInteger a;

                do
                {
                    rng.GetBytes(_a);
                    a = new BigInteger(_a);
                }
                while (a < 2 || a >= n - 2);

                BigInteger x = BigInteger.ModPow(a, t, n);

                if (x == 1 || x == n - 1)
                    continue;

                for (int r = 1; r < s; r++)
                {
                    x = BigInteger.ModPow(x, 2, n);
                    if (x == 1)
                        return false;
                    if (x == n - 1)
                        break;
                }

                if (x != n - 1)
                    return false;
            }
            return true;
        }
        private (BigInteger, BigInteger, BigInteger) GCD(BigInteger a, BigInteger b)
        {
            if (a == 0)
                return (b, 0, 1);
            (BigInteger gcd, BigInteger x, BigInteger y) = GCD(b % a, a);
            return (gcd, y - (b / a) * x, x);
        }
        private BigInteger Invmod(BigInteger a, BigInteger m)
        {
            if (a < 0)
                a *= -1;
            if (m < 0)
                m *= -1;
            (BigInteger g, BigInteger x, BigInteger y) = GCD(a, m);
            return g > 1 ? 0 : Mod(x,m); 
        }
        private BigInteger Mod(BigInteger a, BigInteger m)
        {
            return a < 0 ? m - (-a) % m : a % m;
        }
    }
}
