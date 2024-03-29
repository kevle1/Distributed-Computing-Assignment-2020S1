﻿// Filename: Blockchain.cs
// Project:  DC Assignment (COMP3008)
// Purpose:  Represents the blockchain 
// Author:   Kevin Le (19472960)
//
// Date:     28/05/2020

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace dc_p8_wallet.Blockchain
{
    class Blockchain
    {
        private static List<Block> blockchain = new List<Block>();

        /// <summary>
        /// Initialize blockchain with a genesis block 
        /// </summary>
        static Blockchain()
        {
            Block b = new Block();
            b.blockID = 0;

            b.AddTask(new string[] { "", "" });
            b.AddTask(new string[] { "", "" });
            b.AddTask(new string[] { "", "" });
            b.AddTask(new string[] { "", "" });
            b.AddTask(new string[] { "", "" });

            b.offset = 0;
            b.prevHash = "";

            b.hash = "";

            b = GenerateHash(b);

            blockchain.Add(b);

            Logger.Log("Added Block " + b.ToHashString());
        }

        /// <summary>
        /// Return end block 
        /// </summary>
        /// <returns></returns>
        public static Block GetEndBlock()
        {
            return blockchain.Last();
        }

        /// <summary>
        /// Return number of blocks 
        /// </summary>
        /// <returns></returns>
        public static int GetNumBlocks()
        {
            return blockchain.Count();
        }

        /// <summary>
        /// Get the ID of the largest block
        /// Used for validating a new block to be inserted
        /// </summary>
        /// <returns></returns>
        public static uint GetLargestBlockID()
        {
            uint largestID = 0;

            foreach (Block b in blockchain)
            {
                if (b.blockID > largestID)
                {
                    largestID = b.blockID;
                }
            }

            return largestID;
        }

        /// <summary>
        /// Return the blockchain 
        /// </summary>
        /// <returns>List of blocks</returns>
        public static List<Block> GetChain()
        {
            return blockchain;
        }

        /// <summary>
        /// Adds a new block to the blockchain, performing proper validation
        /// </summary>
        /// <param name="b"></param>
        public static void AddBlock(Block b)
        {
            if (b.blockID > GetLargestBlockID() && //Block ID larger than all other block IDs

                b.blockID >= 0 && //All numbers non negative 
                b.offset >= 0 &&

                b.prevHash == GetEndBlock().hash && //Previous hash matches with last block in current chain 

                b.hash.StartsWith("12345") && //Starts with 12345 

                b.hash == CheckHash(b) //Check if hash in block is correct for the block 
                )
            {
                blockchain.Add(b);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Error: Invalid block");
            }
        }

        /// <summary>
        /// Replaces chain with another chain 
        /// </summary>
        /// <param name="newBlockchain"></param>
        public static void UpdateChain(List<Block> newBlockchain)
        {
            blockchain = newBlockchain;
        }

        /// <summary>
        /// Generates a new hash by brute force. If no hash begins with 12345, increment offset by 1 and check again. 
        /// </summary>
        /// <param name="b"></param>
        /// <returns>Block with hash</returns>
        public static Block GenerateHash(Block b)
        {
            Boolean hashGenerated = false;
            SHA256 sha256 = SHA256.Create();

            Block outB = b;

            while (!hashGenerated)
            {
                try
                {
                    //If hash doesn't start with 12345 or exists keep attempting to generate 
                    while (!outB.hash.StartsWith("12345") || outB.hash == "")
                    {
                        outB.offset = outB.offset + 1;

                        //Convert SHA256 to ulong, then to string  
                        string hash = (BitConverter.ToUInt64(sha256.ComputeHash(Encoding.UTF8.GetBytes(outB.ToHashString())), 0)).ToString();

                        outB.hash = hash;
                    }

                    hashGenerated = true;
                }
                catch (ArgumentOutOfRangeException)
                {
                    System.Diagnostics.Debug.WriteLine("Out of string bounds");
                }
            }

            return outB;
        }

        /// <summary>
        /// Simply hashes block data. Used to confirm if hash is correct. 
        /// </summary>
        /// <param name="b"></param>
        /// <returns>String hash</returns>
        public static string CheckHash(Block b)
        {
            SHA256 sha256 = SHA256.Create();
            return (BitConverter.ToUInt64(sha256.ComputeHash(Encoding.UTF8.GetBytes(b.ToHashString())), 0)).ToString();
        }
    }
}
