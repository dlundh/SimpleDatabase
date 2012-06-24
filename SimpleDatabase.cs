using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace SimpleDatabase
{
    class Program
    {
        static void Main(string[] args)
        {
            string s;            
            int currentBlock = 0;
            bool activeBlock = false;
            
            Dictionary<String, String> dictionary = new Dictionary<String, String>();
            Dictionary<String, String> transactionBlock = new Dictionary<String, String>();            
            Dictionary<int, Dictionary<String, String>> transactionBlocks = new Dictionary<int, Dictionary<String, String>>();
            
            while ((s = Console.ReadLine()) != null)
            {
                try
                {
                    string[] commands = s.ToUpper().Split(' ');
                    if (commands[0] == "BEGIN")
                    {
                        activeBlock = true;

                        if (transactionBlock.Values.Count > 0)
                        {
                            //avoid link between transactionBlock and transactionBlocks
                            Dictionary<String, String> temp = new Dictionary<String, String>();
                            foreach (KeyValuePair<String, String> pair in transactionBlock)
                            {
                                temp[pair.Key] = pair.Value;
                            }

                            //store current block for rollback
                            transactionBlocks[currentBlock] = temp;
                            
                            currentBlock++;
                        }
                    }
                    else if (commands[0] == "SET")
                    {
                        if (activeBlock == true)
                        {
                            //store transaction history since BEGIN is used
                            transactionBlock[commands[1]] = commands[2];
                        }
                        else
                        {
                            //store directly in database
                            dictionary[commands[1]] = commands[2];
                        }
                    }
                    else if (commands[0] == "GET")
                    {
                        try
                        {
                            //look in transaction block first if it is active
                            if (activeBlock == true)
                            {                                
                                Console.WriteLine(transactionBlock[commands[1]]);
                            }
                            else
                            {
                                //look in database
                                Console.WriteLine(dictionary[commands[1]]);
                            }
                        }
                        catch
                        {
                            try
                            {
                                //look in database
                                Console.WriteLine(dictionary[commands[1]]);
                            }
                            catch
                            {
                                Console.WriteLine("NULL");
                            }
                        }
                                                
                    }
                    else if (commands[0] == "UNSET")
                    {
                        if (activeBlock == true)
                        {
                            //store unset transaction in transaction block
                            transactionBlock[commands[1]] = "NULL";
                        }
                        else
                        {
                            dictionary[commands[1]] = "NULL";
                        }
                    }
                    else if (commands[0] == "ROLLBACK")
                    {
                        //get last dictionary if exist                        
                        if (currentBlock > 0)
                        {
                            currentBlock--;

                            //get last transaction block if exist
                            transactionBlock = transactionBlocks[currentBlock];

                            //remove block from block list
                            transactionBlocks.Remove(currentBlock);
                        }
                        else if (activeBlock == true)
                        {
                            activeBlock = false;
                            transactionBlock = new Dictionary<String, String>();  
                        }
                        else
                        {
                            Console.WriteLine("INVALID ROLLBACK");
                        }
                    }
                    else if (commands[0] == "COMMIT")
                    {
                        //store transaction block to database
                        foreach (KeyValuePair<String, String> pair in transactionBlock)
                        {
                            dictionary[pair.Key] = pair.Value;
                        }

                        //reset transactions
                        transactionBlocks = new Dictionary<int, Dictionary<String, String>>();
                        transactionBlock = new Dictionary<String, String>();
                        currentBlock = 0;
                        activeBlock = false;
                    }
                    else if (commands[0] == "END")
                    {
                        //quit console
                        Environment.Exit(0);
                    }
                    else
                    {
                        Console.WriteLine("Input is not supported.");
                    }
                }
                catch
                {
                    Console.WriteLine("Input is not supported.");
                }
            }
        }
    }
}
