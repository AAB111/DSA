using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
namespace DSA
{
    internal class MainContext : Context
    {
        private Command? signatureCommand;
        private Command? checkCommand;
        private DsaSignature? dsa;
        private string? r;
        private string? s;
        private string? p;
        private string? qcheck;
        private string? y;
        private string message;
        public uint Q { get; set; }
        public string? SMS { get; set; }
        public string Message { get { return message; } set { message = value; OnPropertyChanged(); } }
        public string P { get { return p; } set { p = value; OnPropertyChanged(); } }
        public string Qcheck { get { return qcheck; } set { qcheck = value; OnPropertyChanged(); } }
        public string Y { get { return y; } set { y = value; OnPropertyChanged(); } }
        public string R { get { return r; } set { r = value;OnPropertyChanged(); } }
        public string S { get { return s; } set { s = value; OnPropertyChanged(); } }
        public Command SignatureCommand
        {
            get
            {
                return signatureCommand ??= new Command((obj) =>
                {
                    try
                    {
                        if (this.dsa?.Q != Q || this.dsa == null)
                        {
                            DsaSignature dsa = new(Q);
                            this.dsa = dsa;
                        }
                        var smsWithSignature = dsa.Signature(SMS);
                        R = smsWithSignature.Item2.ToString();
                        S = smsWithSignature.Item3.ToString();
                        P = dsa.P.ToString();
                        Qcheck = dsa.Q.ToString();
                        Y = dsa.Y.ToString();
                        Message = SMS;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                });
            }
        }
        public Command CheckCommand
        {
            get
            {
                return checkCommand ??= new Command((obj) =>
                {
                    try
                    {
                        _ = (bool)(dsa?.CheckSignature(new(Message, BigInteger.Parse(R), BigInteger.Parse(S)), new(BigInteger.Parse(P), BigInteger.Parse(Qcheck), BigInteger.Parse(Y)))) ? MessageBox.Show("Это оригинал") : MessageBox.Show("Подделка");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    
                });
            }
        }
    }
}
