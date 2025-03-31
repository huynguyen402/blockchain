using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace blockchain
{
    public partial class Form1 : Form
    {
        private int _blockCount = 1;
        // List to keep track of all added block panels.
        private List<Panel> _blockPanels = new List<Panel>();
        private List<Transaction> pendingTransactions = new List<Transaction>();
        private BLockChain GradeChain;

        public Form1()
        {
            InitializeComponent();
            GradeChain = new BLockChain();
        }

        private void AddToWaitList_Click(object sender, EventArgs e)
        {
            // Lấy dữ liệu nhập từ các control
            string msv = textBox1.Text.Trim();
            string mmh = textBox2.Text.Trim();
            string diemStr = textBox3.Text.Trim();
            DateTime ngayLuu = dateTimePicker1.Value;
            int lanThi = (int)numericUpDown1.Value;

            // Kiểm tra dữ liệu nhập: Mã sinh viên và mã môn học không được để trống
            if (string.IsNullOrEmpty(msv) || string.IsNullOrEmpty(mmh))
            {
                MessageBox.Show("Mã sinh viên và mã môn học không được để trống.");
                return;
            }

            // Kiểm tra giá trị điểm nhập vào
            if (!double.TryParse(diemStr, out double diem))
            {
                MessageBox.Show("Giá trị điểm không hợp lệ.");
                return;
            }

            // Tạo một đối tượng Transaction mới
            Transaction tx = new Transaction(msv, mmh, diem, ngayLuu, lanThi);

            // Thêm transaction vào danh sách điểm chờ
            pendingTransactions.Add(tx);

            // Cập nhật DataGridView: thêm dòng mới với các giá trị nhập
            PopulateDataGridView(pendingTransactions);

            // Xóa dữ liệu nhập sau khi thêm (nếu cần)
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            numericUpDown1.Value = 1;
        }



        private void AddBlockToChain_Click(object sender, EventArgs e)
        {
            if (pendingTransactions.Count < 3)
            {
                MessageBox.Show("There must be at least 3 transactions to create a block.");
                return;
            }
            // Create a new block.
            Block newBlock = new Block(DateTime.Now, "PreviousHash", pendingTransactions.GetRange(0,3));
            // Add the block to the blockchain.
            GradeChain.AddBlock(newBlock);
            // Update the block panel with the new block details.
            AddBlockToPanel(newBlock);
            // Remove the transaction from the pending list.
            pendingTransactions.RemoveRange(0, 3);
            PopulateDataGridView(pendingTransactions);



        }

        private void AddBlockToPanel(Block newBlock)
        {
            // Create a new Panel to represent a block.
            Panel blockPanel = new Panel
            {
                Size = new Size(220, 280),
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(10),
                AutoScroll = true
            };

            // Block Header label.
            Label lblBlockHeader = new Label
            {
                Text = $"Block Header (Block {_blockCount})",
                Location = new Point(10, 10),
                AutoSize = true,                
            };
            blockPanel.Controls.Add(lblBlockHeader);

            // Example labels for block details.
            Label lblPrevBlockHash = new Label
            {
                Text = $"PrevBlockHash: {newBlock.PreviousHash}",
                Location = new Point(10, 40),
                AutoSize = true
            };
            blockPanel.Controls.Add(lblPrevBlockHash);
            // Block Header Nonce.
            Label lblBlockNonce = new Label
            {
                Text = $"Block Nonce: {newBlock.Nonce}",
                Location = new Point(10, 60),
                AutoSize = true,
            };
            blockPanel.Controls.Add(lblBlockNonce);

            Label lblBlockDiff = new Label
            {
                Text = $"Block Difficulty: {newBlock.Difficulty}",
                Location = new Point(10, 80),
                AutoSize = true,

            };
            blockPanel.Controls.Add(lblBlockDiff);

            

            Label lblMerkleRoot = new Label
            {
                Text = $"MerkleRoot: {newBlock.MerkleRootHash}",
                Location = new Point(10, 100),
                AutoSize = true
            };
            blockPanel.Controls.Add(lblMerkleRoot);


            Label lblTimestamp = new Label
            {
                Text = $"Timestamp: {newBlock.TimeStamp}",
                Location = new Point(10, 120),
                AutoSize = true
            };
            blockPanel.Controls.Add(lblTimestamp);

            // Block Body label.
            Label lblBlockBody = new Label
            {
                Text = $"Block Body",
                Location = new Point(10, 140),
                AutoSize = true,
                Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Bold)
            };
            blockPanel.Controls.Add(lblBlockBody);

            for (int i = 0; i < newBlock.Transactions.Count; i++)
            {
                Label lblTransactions = new Label
                {

                    Text = $"{newBlock.Transactions[i].PrintTransaction()}",
                    Location = new Point(10, 160 + i* 15),
                    AutoSize = true
                };
                blockPanel.Controls.Add(lblTransactions);
            }

            // Add the block panel to the FlowLayoutPanel.
            flowLayoutPanel1.Controls.Add(blockPanel);

            // Also add the block panel to our list.
            _blockPanels.Add(blockPanel);

            // Increment the block counter.
            _blockCount++;
        }

        

        private void PopulateDataGridView(List<Transaction> transactions)
        {
            // Xóa hết các dòng hiện có
            dataGridView1.Rows.Clear();

            int stt = 1;
            foreach (var tx in transactions)
            {
                // Giả sử Transaction có các thuộc tính: StudentId, SubjectId, RecordDate, AttemptNumber
                dataGridView1.Rows.Add(
                    stt,
                    tx.StudentId,
                    tx.SubjectId,
                    tx.Score,
                    tx.RecordDate.ToShortDateString(),
                    tx.AttemptNumber
                );
                stt++;
            }
        }

        private bool ValidateChain(List<Block> chain)
        {
            for (int i = 1; i < chain.Count; i++)
            {
                Block currentBlock = chain[i];
                Block previousBlock = chain[i - 1];
                // Validate the hash of the current block
                if (currentBlock.MerkleRootHash != currentBlock.CalculateMRootWNonce())
                {
                    return false;
                }
                // Validate the previous hash of the current block
                if (currentBlock.PreviousHash != previousBlock.MerkleRootHash)
                {
                    return false;
                }
            }
            return true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Validate whether the data in blockchain is valid
            if (ValidateChain(GradeChain.Chain))
            {
                MessageBox.Show("The blockchain is valid.");
            }
            else
            {
                MessageBox.Show("The blockchain is not valid.");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //export the blockchain to a json file and ask the user where to save it
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JSON files|*.json";
            saveFileDialog.Title = "Save the blockchain to a JSON file";
            saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName != "")
            {
                // Inside the button4_Click method, replace the problematic line with:
                string json = JsonConvert.SerializeObject(GradeChain.Chain, Formatting.Indented);
                // Write the JSON string to the file
                System.IO.File.WriteAllText(saveFileDialog.FileName, json);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //import the blockchain from a json file and ask the user to select the file
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JSON files|*.json";
            openFileDialog.Title = "Select a JSON file to import the blockchain";
            openFileDialog.ShowDialog();

            if (openFileDialog.FileName != "")
            {
                // Read the JSON string from the file
                string json = System.IO.File.ReadAllText(openFileDialog.FileName);
                // Deserialize the JSON string to a List<Block>
                List<Block> chain = JsonConvert.DeserializeObject<List<Block>>(json);
                // Update the blockchain with the imported chain
                GradeChain.Chain = chain;
                //clear panel 
                flowLayoutPanel1.Controls.Clear();
                _blockPanels.Clear();
                _blockCount = 0;
                // Update the block panel with the imported 
                foreach (Block block in chain)
                {
                    AddBlockToPanel(block);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
