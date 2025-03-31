using System;
using System.Security.Cryptography;
using System.Text;

namespace blockchain
{
    public class Transaction
    {
        public string StudentId { get; set; }
        public string SubjectId { get; set; }
        public double Score { get; set; }
        public DateTime RecordDate { get; set; }
        public int AttemptNumber { get; set; }
        public string Hash { get; set; }
        public string DigitalSignature { get; set; }

        public Transaction(string studentId, string subjectId, double score, DateTime recordDate, int attemptNumber)
        {
            StudentId = studentId;
            SubjectId = subjectId;
            Score = score;
            RecordDate = recordDate;
            AttemptNumber = attemptNumber;
            Hash = ComputeHash();
            DigitalSignature = SignTransaction();
        }

        public string ComputeHash()
        {
            string input = $"{StudentId}{SubjectId}{Score}{RecordDate.ToString("o")}{AttemptNumber}";
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                return BitConverter.ToString(bytes).Replace("-", "");
            }
        }

        private string SignTransaction()
        {
            // Ví dụ minh họa: Tạo chữ ký số bằng cách sử dụng RSA.
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                byte[] dataBytes = Encoding.UTF8.GetBytes(Hash);
                // Dùng SHA256 để tính toán chữ ký
                byte[] signatureBytes = rsa.SignData(dataBytes, new SHA256CryptoServiceProvider());
                return Convert.ToBase64String(signatureBytes);
            }
        }

        public string PrintTransaction()
        {
            return $"{StudentId}-{SubjectId}-{Score}-{RecordDate}-{AttemptNumber}-{DigitalSignature}";
        }
    }
}
