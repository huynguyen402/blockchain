using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace blockchain
{
    public class Block
    {
        public int Index { get; set; }
        public DateTime TimeStamp { get; set; }
        public string PreviousHash { get; set; }
        public string MerkleRootHash { get; set; }
        public List<Transaction> Transactions { get; set; }
        public int Nonce { get; set; }
        public int Difficulty { get; set; }

        public Block(DateTime timeStamp, string previousHash, List<Transaction> transactions)
        {
            Index = 0;
            TimeStamp = timeStamp;
            PreviousHash = previousHash;
            Transactions = transactions;
            MerkleRootHash = CalculateMRootWNonce();
            Nonce = 0;
            Difficulty = 0;

        }

        public string MineBlock(int diff = 3)
        {
            this.Difficulty = diff;
            string HashAttempt;
            List<string> transactionHashes = new List<string>();
            foreach (var tx in Transactions)
            {
                transactionHashes.Add(tx.Hash);
            }
            string combinedHashes = string.Join("", transactionHashes) + this.Nonce.ToString();
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combinedHashes));
                HashAttempt = BitConverter.ToString(bytes).Replace("-", "");
                BigInteger hashValue = BigInteger.Parse("0" + HashAttempt, System.Globalization.NumberStyles.HexNumber);
                BigInteger target = BigInteger.Pow(2, 256 - diff);
                if (hashValue < target)
                {
                    return HashAttempt;
                }
                else
                {
                    this.Nonce++;
                    return MineBlock(diff);
                }
            }
        }


        public string CalculateMRootWNonce()
        {
            List<string> transactionHashes = new List<string>();
            foreach (var tx in Transactions)
            {
                transactionHashes.Add(tx.ComputeHash() );
            }
            string combinedHashes = string.Join("", transactionHashes) + this.Nonce.ToString();
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combinedHashes));
                return BitConverter.ToString(bytes).Replace("-", "");
            }
        }






    }
}
