using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace HWEmu
{
    public static class Chips
    {
        public static List<Chip> ChipList = new List<Chip>();

        public static void AddChips()
        {
            Program.ProjectChips.Add(HWEmu.Chips.ChipList[0]);
            Program.ProjectChips[0].Rectangle = new Rectangle(400f, 400f, Program.ProjectChips[0].Rectangle.Width, Program.ProjectChips[0].Rectangle.Height);
            Chip.IOPositionOffsetSetFromChip(Program.ProjectChips[0]);

            Program.ProjectChips.Add(HWEmu.Chips.ChipList[1]);
            Program.ProjectChips[1].Rectangle = new Rectangle(1200f, 400f, Program.ProjectChips[1].Rectangle.Width, Program.ProjectChips[1].Rectangle.Height);
            Chip.IOPositionOffsetSetFromChip(Program.ProjectChips[1]);

            Program.ProjectChips.Add(HWEmu.Chips.ChipList[2]);
            Program.ProjectChips[2].Rectangle = new Rectangle(400f, 1200f, Program.ProjectChips[2].Rectangle.Width, Program.ProjectChips[2].Rectangle.Height);
            Chip.IOPositionOffsetSetFromChip(Program.ProjectChips[2]);

            Program.ProjectChips.Add(HWEmu.Chips.ChipList[3]);
            Program.ProjectChips[3].Rectangle = new Rectangle(1800f, 1200f, Program.ProjectChips[3].Rectangle.Width, Program.ProjectChips[3].Rectangle.Height);
            Chip.IOPositionOffsetSetFromChip(Program.ProjectChips[3]);
        }

        public static void LoadChips()
        {
            // load chips data
            string path = System.AppDomain.CurrentDomain.BaseDirectory + "chips";
            string[] files = Directory.GetFiles(path, "*.md");

            int chipfileIterator = 0;

            foreach (string chipFile in files)
            {
                ChipList.Add(new Chip { binaryStateTable = new Dictionary<string, string>(), ChipName = Path.GetFileNameWithoutExtension(chipFile) });

                var lines = File.ReadAllLines(chipFile);
                int lineIterator = 0;
                foreach (string line in lines)
                {
                    string trimmedLine = line.Replace(" ", String.Empty);

                    // Header line
                    if (lineIterator == 0)
                    {
                        bool inputs = true;
                        string IOName = string.Empty;
                        foreach (char c in trimmedLine)
                        {
                            switch (c)
                            {
                                case '|':
                                    if (IOName != "")
                                    {
                                        if (inputs)
                                        {
                                            ChipList[chipfileIterator].Inputs.Add(new Input
                                            {
                                                Position = new Vector2(),
                                                State = false,
                                                Name = IOName,
                                                Guid = Guid.NewGuid(),
                                                Parent = ChipList[chipfileIterator],
                                            });
                                        }
                                        else
                                        {
                                            ChipList[chipfileIterator].Outputs.Add(new Output
                                            {
                                                Position = new Vector2(),
                                                State = false,
                                                Name = IOName,
                                                Guid = Guid.NewGuid(),
                                                Parent = ChipList[chipfileIterator],
                                            });
                                        }
                                        IOName = string.Empty;
                                    }
                                    break;

                                case '→':
                                    inputs = false;
                                    break;

                                default:
                                    IOName += c;
                                    break;
                            }
                        }
                    }
                    // Do nothing
                    //if(lineIterator == 1) { }

                    if (lineIterator > 1)
                    {
                        string inputsBinary = "";
                        string binaryToAdd = "";

                        foreach (char c in trimmedLine)
                        {
                            switch (c)
                            {
                                case '→':
                                    inputsBinary = binaryToAdd;
                                    binaryToAdd = "";
                                    break;
                                case '|':  // Ignore
                                    break;
                                default:
                                    binaryToAdd += c;
                                    break;
                            }
                        }

                        ChipList[chipfileIterator].binaryStateTable.Add(inputsBinary, binaryToAdd);

                        // Add default state for the chip // TODO move out of the loop
                        if (lineIterator == 2)
                        {
                            ChipList[chipfileIterator].CurrentBinaryInputState = inputsBinary;
                        }
                    }

                    lineIterator++;
                }

                // Check the default output values
                var newOutputString = ChipList[chipfileIterator].binaryStateTable[ChipList[chipfileIterator].CurrentBinaryInputState];

                string oldOutputString = "";

                foreach (var item in ChipList[chipfileIterator].Outputs)
                {
                    oldOutputString += "0";
                }

                //////////////////////////////
                // Update values
                List<int> outputIndexesToChange = new List<int>();

                int outputIndex = 0;
                foreach (char output in oldOutputString)
                {
                    if (newOutputString[outputIndex] == oldOutputString[outputIndex])
                    {
                        // No need to change this index
                    }
                    else
                    {
                        outputIndexesToChange.Add(outputIndex);
                    }
                    outputIndex++;
                }

                List<Output> OutputsToChange = new List<Output>();
                foreach (var indexItem in outputIndexesToChange)
                {
                    OutputsToChange.Add(ChipList[chipfileIterator].Outputs[indexItem]);
                }

                foreach (Output output in OutputsToChange)
                {
                    output.State = !output.State;
                }

                ///////////////////////////////////////

                // Add positions for IO's
                var inputCount = ChipList[chipfileIterator].Inputs.Count();
                var outputCount = ChipList[chipfileIterator].Outputs.Count();

                if (inputCount + outputCount > 4)
                {
                    ChipList[chipfileIterator].Rectangle = new Rectangle(0f, 0f, 400f, 800f);
                }
                else
                {
                    ChipList[chipfileIterator].Rectangle = new Rectangle(0f, 0f, 400f, 400f);
                }



                float yPositionInputs = 0f;
                float yPositionOutputs = 0f;

                float yPositionIncrementInputs = ChipList[chipfileIterator].Rectangle.Height / (inputCount + 1);
                float yPositionIncrementOutputs = ChipList[chipfileIterator].Rectangle.Height / (outputCount + 1);

                foreach (var input in ChipList[chipfileIterator].Inputs)
                {
                    yPositionInputs += yPositionIncrementInputs;
                    input.Position = new Vector2(ChipList[chipfileIterator].Rectangle.X - 100, yPositionInputs);
                }

                foreach (var output in ChipList[chipfileIterator].Outputs)
                {
                    yPositionOutputs += yPositionIncrementInputs;
                    output.Position = new Vector2(ChipList[chipfileIterator].Rectangle.X + ChipList[chipfileIterator].Rectangle.Width + 100, yPositionOutputs);
                }

                chipfileIterator++;
            }
        }
    }
}
