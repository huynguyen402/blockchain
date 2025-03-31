using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blockchain
{
    public class BLockChain
    {
        public List<Block> Chain { get; set; }

        public BLockChain()
        {
            Chain = new List<Block>();
            Chain.Add(CreateGenesisBlock());
        }

        public Block CreateGenesisBlock()
        {
            Block genesisBlock = new Block(DateTime.Now, null, new List<Transaction>());
            genesisBlock.MineBlock(0);
            return genesisBlock;
        }

        public Block GetLatestBlock()
        {
            return Chain[Chain.Count - 1];
        }

        public void AddBlock(Block block)
        {
            Block latestBlock = GetLatestBlock();
            block.Index = latestBlock.Index + 1;
            block.PreviousHash = latestBlock.MerkleRootHash;
            block.MineBlock(10);
            Chain.Add(block);
        }

        
        
    }
}
