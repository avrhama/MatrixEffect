using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Threading;

namespace MatrixEffect
{
    class Program
    {
        Random rand = new Random();
        static void Main(string[] args)
        {


            Program p = new Program();
            Console.CursorVisible = false;
            Console.WindowWidth = Console.LargestWindowWidth;
            Console.WindowHeight = Console.LargestWindowHeight;
            p.start();



            Console.ReadLine();


        }

        public void start()
        {
            lettersStructs = createLetters();
            Console.WriteLine("Enter you name:");
            string name = Console.ReadLine();
            name = name.ToUpper().Trim();
            int offsetX = 50;
            int offsetY = 20;
            Console.Clear();
            pixelLetterList = new List<PixelLetter>();
            pixelLetterListMatch = new Dictionary<char, List<RainDrop.RainLetter>>();
            pixelLetterListMatchCompleted = new Dictionary<char, List<RainDrop.RainLetter>>();
            for (int i = 0; i < name.Length; i++)
            {
                if (name[i] >= 65 && name[i] <= 90)
                {
                    byte[][] letterStruct = lettersStructs[name[i] - 65];
                    for (int row = 0; row < letterStruct.Length; row++)
                    {
                        byte[] rowArr = letterStruct[row];
                        for (int col = 0; col < rowArr.Length; col++)
                        {
                            char letter = ' ';
                            /* Console.SetCursorPosition(col + offsetX, row + offsetY);
                             if (letterStruct[row][col] == 1)
                             {
                                 int a = rand.Next(0, 2) == 0 ? 'A' : 'a';
                                 letter = (char)(a + rand.Next(0, 26));
                                 //letter = name[i];
                             }

                             Console.Write(letter);*/
                            if (letterStruct[row][col] == 1)
                            {
                                lettersSum++;
                            }
                        }
                    }
                    PixelLetter pl = new PixelLetter(name[i], new Point(offsetX, offsetY), new Size(letterStruct[0].Length, letterStruct.Length));
                    pixelLetterList.Add(pl);
                    offsetX += 1 + letterStruct[0].Length;
                }
            }
            rainDropList = new List<RainDrop>();
            maxRainDrops = 10;
            Timer t = new Timer(TimerCallback, null, 0, 30);
            Console.ReadLine();
            return;

            do
            {

                int count = rand.Next(2, 4);
                for (int i = 0; i < count && rainDropList.Count < maxRainDrops; i++)
                {
                    RainDrop rd = createRainDrop();
                    rainDropList.Add(rd);
                }
                List<RainDrop> rainDropListRemove = new List<RainDrop>();

                for (int i = 0; i < rainDropList.Count; i++)
                {
                    RainDrop rd = rainDropList[i];
                    if (drawRainDrop(rd))
                        rainDropListRemove.Add(rd);
                }
                foreach (RainDrop rd in rainDropListRemove)
                    rainDropList.Remove(rd);
                rainDropListRemove.Clear();

                foreach (char letter in pixelLetterListMatch.Keys)
                {
                    List<RainDrop.RainLetter> list = pixelLetterListMatch[letter];
                    for (int i = 0; i < list.Count; i++)
                    {
                        RainDrop.RainLetter rl = list[i];
                        if (rl.letter != letter)
                        {
                            int a = rand.Next(0, 2) == 0 ? 'A' : 'a';
                            char nextLetter = (char)(a + rand.Next(0, 26));
                            rl.letter = nextLetter;
                            list[i] = rl;
                        }
                        else
                        {
                            rl.color = ConsoleColor.White;
                        }
                        Console.SetCursorPosition(rl.pos.X, rl.pos.Y);


                        Console.ForegroundColor = rl.color;
                        Console.Write(rl.letter);



                    }
                }

                rainDropListRemove.Clear();

                //  Thread.Sleep(10);
            } while (true);






            Console.SetCursorPosition(0, 0);
            Console.WriteLine("Hello World!");
            Console.ReadKey();
        }
        List<RainDrop> rainDropList;
        int maxRainDrops;
        object locker = new object();
        bool timerTicked = false;
        bool keepRain = true;
        int lettersSum = 0;
        public void TimerCallback(Object o)
        {

            lock (locker)
            {
                if (timerTicked == true)
                    return;
                timerTicked = true;
            }
            // Display the date/time when this method got called.
            // Console.WriteLine("In TimerCallback: " + DateTime.Now);
            // Force a garbage collection to occur for this demo.
            int count = rand.Next(2, 4);
            for (int i = 0; i < count && rainDropList.Count < maxRainDrops && keepRain; i++)
            {
                RainDrop rd = createRainDrop();
                rainDropList.Add(rd);
            }
           /* List<RainDrop> rainDropListRemove = new List<RainDrop>();

            for (int i = 0; i < rainDropList.Count; i++)
            {
                RainDrop rd = rainDropList[i];
                if (drawRainDrop(rd))
                    rainDropListRemove.Add(rd);
            }
            foreach (RainDrop rd in rainDropListRemove)
                rainDropList.Remove(rd);
            rainDropListRemove.Clear();*/

            foreach (char letter in pixelLetterListMatch.Keys)
            {
                List<RainDrop.RainLetter> list = pixelLetterListMatch[letter];

                for (int i = 0; i < list.Count; i++)
                {
                    RainDrop.RainLetter rl = list[i];
                    if (rl.letter != letter)
                    {
                        int a = rand.Next(0, 2) == 0 ? 'A' : 'a';
                        char nextLetter = (char)(a + rand.Next(0, 26));
                        rl.letter = nextLetter;
                        list[i] = rl;
                        Console.SetCursorPosition(rl.pos.X, rl.pos.Y);
                        Console.ForegroundColor = rl.color;
                        Console.Write(rl.letter);
                    }
                    else
                    {

                        rl.color = ConsoleColor.White;
                        if (!pixelLetterListMatchCompleted.ContainsKey(letter))
                            pixelLetterListMatchCompleted[letter] = new List<RainDrop.RainLetter>();
                        pixelLetterListMatchCompleted[letter].Add(rl);
                        list.RemoveAt(i);
                        i--;
                        lettersSum--;
                        keepRain = lettersSum != 0;
                    }

                }  
            }
            foreach (char letter in pixelLetterListMatchCompleted.Keys)
            {
               List<RainDrop.RainLetter> list = pixelLetterListMatchCompleted[letter];
                for (int i = 0; i < list.Count; i++)
                {
                    RainDrop.RainLetter rl = list[i];
                    Console.SetCursorPosition(rl.pos.X, rl.pos.Y);
                    Console.ForegroundColor = rl.color;
                    Console.Write(rl.letter);


                }
            }

            List<RainDrop> rainDropListRemove = new List<RainDrop>();

            for (int i = 0; i < rainDropList.Count; i++)
            {
                RainDrop rd = rainDropList[i];
                if (drawRainDrop(rd))
                    rainDropListRemove.Add(rd);
            }
            foreach (RainDrop rd in rainDropListRemove)
                rainDropList.Remove(rd);
            rainDropListRemove.Clear();
            GC.Collect();
            lock (locker)
            {
                timerTicked = false;
            }

        }
        public RainDrop createRainDrop()
        {
            //checks theres no overlapping when rainDropCreated[avoiding the case when dropSpeed is the same].
            int size = rand.Next(7, 10);
            //int x = rand.Next(0, Console.WindowWidth);
            int x = rand.Next(pixelLetterList[0].pos.X - 2, pixelLetterList[pixelLetterList.Count - 1].pos.X + pixelLetterList[pixelLetterList.Count - 1].size.Width + 2);
            // int y = rand.Next(0, Console.WindowHeight);
            int coin = rand.Next(1, 7);
            // int y = rand.Next(10, Console.WindowHeight);
            int thirdScreen = Console.WindowHeight / 3;
            int j = 0;
            if (coin % 3 == 0)
            {
                j = 1;
            }
            else if (coin % 6 == 0)
            {
                // j = 2;
            }

            int y = rand.Next(j * thirdScreen, (j + 1) * thirdScreen);
            // int  y = rand.Next(0, Console.WindowHeight);
            // y = 0;
            int dropSpeed = rand.Next(1, 5);

            int replaceLetterSpeed = rand.Next(1, 5);
            bool allWhite = rand.Next(1, 5) == 1 ? true : false;
            // allWhite = false;
            RainDrop rd = new RainDrop(new Point(x, y), size, dropSpeed, replaceLetterSpeed, allWhite);
            return rd;
        }
        List<PixelLetter> pixelLetterList;
        Dictionary<char, List<RainDrop.RainLetter>> pixelLetterListMatch;
        Dictionary<char, List<RainDrop.RainLetter>> pixelLetterListMatchCompleted;
        byte[][][] lettersStructs;
        public bool drawRainDrop(RainDrop rd)
        {
            while (!rd.completeCycle && rd.changed)
            {
                RainDrop.RainLetter rl = rd.getLetter();

                Console.SetCursorPosition(rl.pos.X, rl.pos.Y);
                Console.ForegroundColor = rl.color;
                Console.Write(rl.letter);
                //checks if letter colides with name.
                foreach (PixelLetter pl in pixelLetterList)
                {

                    if ((rl.pos.X >= pl.pos.X) && rl.pos.X < (pl.pos.X + pl.size.Width))
                    {
                        if (((rl.pos.Y >= pl.pos.Y) && rl.pos.Y < (pl.pos.Y + pl.size.Height)))
                        {
                            if (lettersStructs[pl.letter - 65][rl.pos.Y - pl.pos.Y][rl.pos.X - pl.pos.X] == 0)
                                break;
                            if (!pixelLetterListMatch.ContainsKey(pl.letter) )
                                pixelLetterListMatch[pl.letter] = new List<RainDrop.RainLetter>();

                            if (!pixelLetterListMatch[pl.letter].Contains(rl))
                            {
                                if(!pixelLetterListMatchCompleted.ContainsKey(pl.letter)|| !pixelLetterListMatchCompleted[pl.letter].Contains(rl))
                                pixelLetterListMatch[pl.letter].Add(rl);
                            }
                            break;
                        }

                    }
                }
            }

            return rd.moveDown();
        }
        public class RainDrop
        {
            static Random rander = new Random();
            Point headPos;
            int size;
            int dropSpeed;
            int replaceLetterSpeed;
            int fallingLength;
            bool allWhite;
            public bool completeCycle;
            int headLetterIndex;
            int currLetterIndex;
            char[] letters;
            public bool changed = true;
            public RainDrop(Point headPos, int size, int dropSpeed, int replaceLetterSpeed, bool allWhite)
            {
                this.headPos = headPos;
                this.size = size;
                this.dropSpeed = dropSpeed;
                this.replaceLetterSpeed = replaceLetterSpeed;
                fallingLength = 0;
                this.allWhite = allWhite;
                completeCycle = false;
                letters = new char[size];
                headLetterIndex = 0;
                setRandLetters();
            }
            public void setRandLetters()
            {
                for (int i = headLetterIndex; i < size - 1; i++)
                {
                    int a = rander.Next(0, 2) == 0 ? 'A' : 'a';
                    letters[i] = (char)(a + rander.Next(0, 26));
                }
                letters[size - 1] = ' ';
            }
            public RainLetter getLetter()
            {


                Point pos = new Point(headPos.X, headPos.Y - currLetterIndex);
                ConsoleColor color = ConsoleColor.White;
                if (!allWhite)
                {
                    color = ConsoleColor.Green;
                    //head rainDrop should be green.
                    if (currLetterIndex < 2)
                        color = ConsoleColor.White;
                }
                RainLetter rl = new RainLetter { pos = pos, letter = letters[currLetterIndex], color = color };
                currLetterIndex++;
                if (currLetterIndex == size || currLetterIndex > fallingLength || pos.Y == 0)
                {
                    currLetterIndex = headLetterIndex;
                    completeCycle = true;
                }
                return rl;
            }
            public bool moveDown()
            {
                //returns true when rain drop should be removed.
                fallingLength++;
                changed = false;
                if ((fallingLength % dropSpeed) == 0)
                {
                    changed = true;

                    completeCycle = false;
                    headPos.Y++;
                    //if curr dropRain head reached the bottom look proceed the next letter.
                    if (Console.WindowHeight <= headPos.Y)
                    {
                        headLetterIndex = headPos.Y - Console.WindowHeight + 1;
                        currLetterIndex = headLetterIndex;

                    }
                    if (headPos.Y >= Console.WindowHeight + (size - 1))
                        return true;
                    else
                        return false;
                }
                if ((fallingLength % replaceLetterSpeed) == 0)
                {
                    changed = true;
                    setRandLetters();
                }
                return false;
            }
            public struct RainLetter
            {
                public Point pos;
                public char letter;
                public ConsoleColor color;
                public override bool Equals(object obj)
                {
                    if (obj.GetType() != this.GetType())
                        return false;
                    RainLetter other = (RainLetter)obj;


                    return (pos.X == other.pos.X) && (pos.Y == other.pos.Y);
                }
            }
        }
        public class PixelLetter
        {
            public char letter;
            public Point pos;
            public Size size;
            public PixelLetter(char letter, Point pos, Size size)
            {
                this.letter = letter;
                this.pos = pos;
                this.size = size;
            }

        }
        public byte[][][] createLetters()
        {
            byte[][][] lettersStructs = new byte[26][][];
            //A
            byte[][] letterStruct = new byte[8][];
            letterStruct[0] = new byte[6] { 0, 0, 1, 1, 0, 0 };
            letterStruct[1] = new byte[6] { 0, 1, 1, 1, 1, 0 };
            letterStruct[2] = new byte[6] { 1, 1, 1, 1, 1, 1 };
            letterStruct[3] = new byte[6] { 1, 1, 0, 0, 1, 1 };
            letterStruct[4] = new byte[6] { 1, 1, 1, 1, 1, 1 };
            letterStruct[5] = new byte[6] { 1, 1, 1, 1, 1, 1 };
            letterStruct[6] = new byte[6] { 1, 1, 0, 0, 1, 1 };
            letterStruct[7] = new byte[6] { 1, 1, 0, 0, 1, 1 };

            lettersStructs[0] = letterStruct;
            letterStruct = new byte[8][];
            //B
            letterStruct[0] = new byte[6] { 1, 1, 1, 1, 1, 0 };
            letterStruct[1] = new byte[6] { 1, 1, 1, 1, 1, 1 };
            letterStruct[2] = new byte[6] { 1, 1, 0, 0, 1, 1 };
            letterStruct[3] = new byte[6] { 1, 1, 1, 1, 1, 1 };
            letterStruct[4] = new byte[6] { 1, 1, 1, 1, 1, 1 };
            letterStruct[5] = new byte[6] { 1, 1, 0, 0, 1, 1 };
            letterStruct[6] = new byte[6] { 1, 1, 1, 1, 1, 1 };
            letterStruct[7] = new byte[6] { 1, 1, 1, 1, 1, 0 };
            lettersStructs[1] = letterStruct;
            letterStruct = new byte[8][];
            //C
            letterStruct[0] = new byte[6] { 0, 1, 1, 1, 1, 0 };
            letterStruct[1] = new byte[6] { 1, 1, 1, 1, 1, 1 };
            letterStruct[2] = new byte[6] { 1, 1, 0, 0, 1, 1 };
            letterStruct[3] = new byte[6] { 1, 1, 0, 0, 0, 0 };
            letterStruct[4] = new byte[6] { 1, 1, 0, 0, 0, 0 };
            letterStruct[5] = new byte[6] { 1, 1, 0, 0, 1, 1 };
            letterStruct[6] = new byte[6] { 1, 1, 1, 1, 1, 1 };
            letterStruct[7] = new byte[6] { 0, 1, 1, 1, 1, 0 };
            lettersStructs[2] = letterStruct;
            letterStruct = new byte[8][];
            //D
            letterStruct[0] = new byte[6] { 1, 1, 1, 1, 1, 0 };
            letterStruct[1] = new byte[6] { 1, 1, 1, 1, 1, 1 };
            letterStruct[2] = new byte[6] { 1, 1, 0, 0, 1, 1 };
            letterStruct[3] = new byte[6] { 1, 1, 0, 0, 1, 1 };
            letterStruct[4] = new byte[6] { 1, 1, 0, 0, 1, 1 };
            letterStruct[5] = new byte[6] { 1, 1, 0, 0, 1, 1 };
            letterStruct[6] = new byte[6] { 1, 1, 1, 1, 1, 1 };
            letterStruct[7] = new byte[6] { 1, 1, 1, 1, 1, 0 };
            lettersStructs[3] = letterStruct;
            letterStruct = new byte[8][];
            //E
            letterStruct[0] = new byte[6] { 1, 1, 1, 1, 1, 1 };
            letterStruct[1] = new byte[6] { 1, 1, 1, 1, 1, 1 };
            letterStruct[2] = new byte[6] { 1, 1, 0, 0, 0, 0 };
            letterStruct[3] = new byte[6] { 1, 1, 1, 1, 1, 0 };
            letterStruct[4] = new byte[6] { 1, 1, 1, 1, 1, 0 };
            letterStruct[5] = new byte[6] { 1, 1, 0, 0, 0, 0 };
            letterStruct[6] = new byte[6] { 1, 1, 1, 1, 1, 1 };
            letterStruct[7] = new byte[6] { 1, 1, 1, 1, 1, 1 };
            lettersStructs[4] = letterStruct;
            letterStruct = new byte[8][];
            //F
            letterStruct[0] = new byte[6] { 1, 1, 1, 1, 1, 1 };
            letterStruct[1] = new byte[6] { 1, 1, 1, 1, 1, 1 };
            letterStruct[2] = new byte[6] { 1, 1, 0, 0, 0, 0 };
            letterStruct[3] = new byte[6] { 1, 1, 1, 1, 1, 0 };
            letterStruct[4] = new byte[6] { 1, 1, 1, 1, 1, 0 };
            letterStruct[5] = new byte[6] { 1, 1, 0, 0, 0, 0 };
            letterStruct[6] = new byte[6] { 1, 1, 0, 0, 0, 0 };
            letterStruct[7] = new byte[6] { 1, 1, 0, 0, 0, 0 };
            lettersStructs[5] = letterStruct;
            letterStruct = new byte[8][];
            //G
            letterStruct[0] = new byte[6] { 0, 1, 1, 1, 1, 0 };
            letterStruct[1] = new byte[6] { 1, 1, 1, 1, 1, 1 };
            letterStruct[2] = new byte[6] { 1, 1, 0, 0, 1, 1 };
            letterStruct[3] = new byte[6] { 1, 1, 0, 0, 0, 0 };
            letterStruct[4] = new byte[6] { 1, 1, 0, 1, 1, 1 };
            letterStruct[5] = new byte[6] { 1, 1, 0, 0, 1, 1 };
            letterStruct[6] = new byte[6] { 1, 1, 1, 1, 1, 1 };
            letterStruct[7] = new byte[6] { 0, 1, 1, 1, 1, 0 };
            lettersStructs[6] = letterStruct;
            letterStruct = new byte[8][];
            //H
            letterStruct[0] = new byte[6] { 1, 1, 0, 0, 1, 1 };
            letterStruct[1] = new byte[6] { 1, 1, 0, 0, 1, 1 };
            letterStruct[2] = new byte[6] { 1, 1, 1, 1, 1, 1 };
            letterStruct[3] = new byte[6] { 1, 1, 1, 1, 1, 1 };
            letterStruct[4] = new byte[6] { 1, 1, 0, 0, 1, 1 };
            letterStruct[5] = new byte[6] { 1, 1, 0, 0, 1, 1 };
            letterStruct[6] = new byte[6] { 1, 1, 0, 0, 1, 1 };
            letterStruct[7] = new byte[6] { 1, 1, 0, 0, 1, 1 };
            lettersStructs[7] = letterStruct;
            letterStruct = new byte[8][];
            //I
            letterStruct[0] = new byte[6] { 0, 1, 1, 1, 1, 0 };
            letterStruct[1] = new byte[6] { 0, 1, 1, 1, 1, 0 };
            letterStruct[2] = new byte[6] { 0, 0, 1, 1, 0, 0 };
            letterStruct[3] = new byte[6] { 0, 0, 1, 1, 0, 0 };
            letterStruct[4] = new byte[6] { 0, 0, 1, 1, 0, 0 };
            letterStruct[5] = new byte[6] { 0, 0, 1, 1, 0, 0 };
            letterStruct[6] = new byte[6] { 0, 1, 1, 1, 1, 0 };
            letterStruct[7] = new byte[6] { 0, 1, 1, 1, 1, 0 };
            lettersStructs[8] = letterStruct;
            letterStruct = new byte[8][];
            //J
            letterStruct[0] = new byte[6] { 0, 1, 1, 1, 1, 1 };
            letterStruct[1] = new byte[6] { 0, 1, 1, 1, 1, 1 };
            letterStruct[2] = new byte[6] { 0, 0, 0, 1, 1, 0 };
            letterStruct[3] = new byte[6] { 0, 0, 0, 1, 1, 0 };
            letterStruct[4] = new byte[6] { 0, 0, 0, 1, 1, 0 };
            letterStruct[5] = new byte[6] { 1, 1, 0, 1, 1, 0 };
            letterStruct[6] = new byte[6] { 1, 1, 1, 1, 1, 0 };
            letterStruct[7] = new byte[6] { 0, 1, 1, 1, 0, 0 };
            lettersStructs[9] = letterStruct;
            letterStruct = new byte[8][];
            //K
            letterStruct[0] = new byte[6] { 1, 1, 0, 0, 1, 1 };
            letterStruct[1] = new byte[6] { 1, 1, 0, 1, 1, 1 };
            letterStruct[2] = new byte[6] { 1, 1, 1, 1, 1, 0 };
            letterStruct[3] = new byte[6] { 1, 1, 1, 1, 0, 0 };
            letterStruct[4] = new byte[6] { 1, 1, 1, 1, 0, 0 };
            letterStruct[5] = new byte[6] { 1, 1, 1, 1, 1, 0 };
            letterStruct[6] = new byte[6] { 1, 1, 0, 1, 1, 1 };
            letterStruct[7] = new byte[6] { 1, 1, 0, 0, 1, 1 };
            lettersStructs[10] = letterStruct;
            letterStruct = new byte[8][];
            //L
            letterStruct[0] = new byte[6] { 1, 1, 1, 0, 0, 0 };
            letterStruct[1] = new byte[6] { 1, 1, 1, 0, 0, 0 };
            letterStruct[2] = new byte[6] { 0, 1, 1, 0, 0, 0 };
            letterStruct[3] = new byte[6] { 0, 1, 1, 0, 0, 0 };
            letterStruct[4] = new byte[6] { 0, 1, 1, 0, 0, 0 };
            letterStruct[5] = new byte[6] { 0, 1, 1, 0, 0, 0 };
            letterStruct[6] = new byte[6] { 1, 1, 1, 1, 1, 1 };
            letterStruct[7] = new byte[6] { 1, 1, 1, 1, 1, 1 };
            lettersStructs[11] = letterStruct;
            letterStruct = new byte[8][];
            //M
            letterStruct[0] = new byte[8] { 1, 1, 0, 0, 0, 0, 1, 1 };
            letterStruct[1] = new byte[8] { 1, 1, 1, 0, 0, 1, 1, 1 };
            letterStruct[2] = new byte[8] { 1, 1, 1, 1, 1, 1, 1, 1 };
            letterStruct[3] = new byte[8] { 1, 1, 1, 1, 1, 1, 1, 1 };
            letterStruct[4] = new byte[8] { 1, 1, 0, 1, 1, 0, 1, 1 };
            letterStruct[5] = new byte[8] { 1, 1, 0, 0, 0, 0, 1, 1 };
            letterStruct[6] = new byte[8] { 1, 1, 0, 0, 0, 0, 1, 1 };
            letterStruct[7] = new byte[8] { 1, 1, 0, 0, 0, 0, 1, 1 };
            lettersStructs[12] = letterStruct;
            letterStruct = new byte[8][];
            //N
            letterStruct[0] = new byte[7] { 1, 1, 0, 0, 0, 1, 1 };
            letterStruct[1] = new byte[7] { 1, 1, 1, 0, 0, 1, 1 };
            letterStruct[2] = new byte[7] { 1, 1, 1, 1, 0, 1, 1 };
            letterStruct[3] = new byte[7] { 1, 1, 1, 1, 1, 1, 1 };
            letterStruct[4] = new byte[7] { 1, 1, 0, 1, 1, 1, 1 };
            letterStruct[5] = new byte[7] { 1, 1, 0, 0, 1, 1, 1 };
            letterStruct[6] = new byte[7] { 1, 1, 0, 0, 0, 1, 1 };
            letterStruct[7] = new byte[7] { 1, 1, 0, 0, 0, 1, 1 };
            lettersStructs[13] = letterStruct;
            letterStruct = new byte[8][];
            //O
            letterStruct[0] = new byte[6] { 0, 1, 1, 1, 1, 0 };
            letterStruct[1] = new byte[6] { 1, 1, 1, 1, 1, 1 };
            letterStruct[2] = new byte[6] { 1, 1, 0, 0, 1, 1 };
            letterStruct[3] = new byte[6] { 1, 1, 0, 0, 1, 1 };
            letterStruct[4] = new byte[6] { 1, 1, 0, 0, 1, 1 };
            letterStruct[5] = new byte[6] { 1, 1, 0, 0, 1, 1 };
            letterStruct[6] = new byte[6] { 1, 1, 1, 1, 1, 1 };
            letterStruct[7] = new byte[6] { 0, 1, 1, 1, 1, 0 };
            lettersStructs[14] = letterStruct;
            letterStruct = new byte[8][];
            //P
            letterStruct[0] = new byte[6] { 1, 1, 1, 1, 1, 0 };
            letterStruct[1] = new byte[6] { 1, 1, 1, 1, 1, 1 };
            letterStruct[2] = new byte[6] { 1, 1, 0, 0, 1, 1 };
            letterStruct[3] = new byte[6] { 1, 1, 0, 0, 1, 1 };
            letterStruct[4] = new byte[6] { 1, 1, 1, 1, 1, 0 };
            letterStruct[5] = new byte[6] { 1, 1, 1, 1, 1, 0 };
            letterStruct[6] = new byte[6] { 1, 1, 0, 0, 0, 0 };
            letterStruct[7] = new byte[6] { 1, 1, 0, 0, 0, 0 };
            lettersStructs[15] = letterStruct;
            letterStruct = new byte[8][];
            //Q
            letterStruct[0] = new byte[7] { 0, 1, 1, 1, 1, 0, 0 };
            letterStruct[1] = new byte[7] { 1, 1, 1, 1, 1, 1, 0 };
            letterStruct[2] = new byte[7] { 1, 1, 0, 0, 1, 1, 0 };
            letterStruct[3] = new byte[7] { 1, 1, 0, 0, 1, 1, 0 };
            letterStruct[4] = new byte[7] { 1, 1, 0, 0, 1, 1, 0 };
            letterStruct[5] = new byte[7] { 1, 1, 0, 0, 1, 1, 0 };
            letterStruct[6] = new byte[7] { 1, 1, 1, 1, 1, 1, 0 };
            letterStruct[7] = new byte[7] { 0, 1, 1, 1, 1, 1, 1 };
            lettersStructs[16] = letterStruct;
            letterStruct = new byte[8][];
            //R
            letterStruct[0] = new byte[6] { 1, 1, 1, 1, 1, 0 };
            letterStruct[1] = new byte[6] { 1, 1, 1, 1, 1, 1 };
            letterStruct[2] = new byte[6] { 1, 1, 0, 0, 1, 1 };
            letterStruct[3] = new byte[6] { 1, 1, 0, 0, 1, 1 };
            letterStruct[4] = new byte[6] { 1, 1, 1, 1, 1, 0 };
            letterStruct[5] = new byte[6] { 1, 1, 1, 1, 1, 1 };
            letterStruct[6] = new byte[6] { 1, 1, 0, 0, 1, 1 };
            letterStruct[7] = new byte[6] { 1, 1, 0, 0, 1, 1 };
            lettersStructs[17] = letterStruct;
            letterStruct = new byte[8][];
            //S
            letterStruct[0] = new byte[6] { 0, 1, 1, 1, 1, 1 };
            letterStruct[1] = new byte[6] { 1, 1, 1, 1, 1, 1 };
            letterStruct[2] = new byte[6] { 1, 1, 0, 0, 0, 0 };
            letterStruct[3] = new byte[6] { 1, 1, 1, 1, 1, 0 };
            letterStruct[4] = new byte[6] { 0, 1, 1, 1, 1, 1 };
            letterStruct[5] = new byte[6] { 0, 0, 0, 1, 1, 1 };
            letterStruct[6] = new byte[6] { 1, 1, 1, 1, 1, 1 };
            letterStruct[7] = new byte[6] { 1, 1, 1, 1, 1, 0 };
            lettersStructs[18] = letterStruct;
            letterStruct = new byte[8][];
            //T
            letterStruct[0] = new byte[6] { 1, 1, 1, 1, 1, 1 };
            letterStruct[1] = new byte[6] { 1, 1, 1, 1, 1, 1 };
            letterStruct[2] = new byte[6] { 0, 0, 1, 1, 0, 0 };
            letterStruct[3] = new byte[6] { 0, 0, 1, 1, 0, 0 };
            letterStruct[4] = new byte[6] { 0, 0, 1, 1, 0, 0 };
            letterStruct[5] = new byte[6] { 0, 0, 1, 1, 0, 0 };
            letterStruct[6] = new byte[6] { 0, 0, 1, 1, 0, 0 };
            letterStruct[7] = new byte[6] { 0, 0, 1, 1, 0, 0 };
            lettersStructs[19] = letterStruct;
            letterStruct = new byte[8][];
            //U
            letterStruct[0] = new byte[6] { 1, 1, 0, 0, 1, 1 };
            letterStruct[1] = new byte[6] { 1, 1, 0, 0, 1, 1 };
            letterStruct[2] = new byte[6] { 1, 1, 0, 0, 1, 1 };
            letterStruct[3] = new byte[6] { 1, 1, 0, 0, 1, 1 };
            letterStruct[4] = new byte[6] { 1, 1, 0, 0, 1, 1 };
            letterStruct[5] = new byte[6] { 1, 1, 1, 1, 1, 1 };
            letterStruct[6] = new byte[6] { 1, 1, 1, 1, 1, 1 };
            letterStruct[7] = new byte[6] { 0, 1, 1, 1, 1, 0 };
            lettersStructs[20] = letterStruct;
            letterStruct = new byte[8][];
            //V
            letterStruct[0] = new byte[8] { 1, 1, 0, 0, 0, 0, 1, 1 };
            letterStruct[1] = new byte[8] { 1, 1, 0, 0, 0, 0, 1, 1 };
            letterStruct[2] = new byte[8] { 1, 1, 1, 0, 0, 1, 1, 1 };
            letterStruct[3] = new byte[8] { 1, 1, 1, 0, 0, 1, 1, 1 };
            letterStruct[4] = new byte[8] { 0, 1, 1, 0, 0, 1, 1, 0 };
            letterStruct[5] = new byte[8] { 0, 1, 1, 1, 1, 1, 1, 0 };
            letterStruct[6] = new byte[8] { 0, 0, 1, 1, 1, 1, 0, 0 };
            letterStruct[7] = new byte[8] { 0, 0, 0, 1, 1, 0, 0, 0 };
            lettersStructs[21] = letterStruct;
            letterStruct = new byte[8][];
            //W
            letterStruct[0] = new byte[8] { 1, 1, 0, 0, 0, 0, 1, 1 };
            letterStruct[1] = new byte[8] { 1, 1, 0, 0, 0, 0, 1, 1 };
            letterStruct[2] = new byte[8] { 1, 1, 0, 0, 0, 0, 1, 1 };
            letterStruct[3] = new byte[8] { 1, 1, 0, 1, 1, 0, 1, 1 };
            letterStruct[4] = new byte[8] { 1, 1, 1, 1, 1, 1, 1, 1 };
            letterStruct[5] = new byte[8] { 1, 1, 1, 1, 1, 1, 1, 1 };
            letterStruct[6] = new byte[8] { 1, 1, 1, 0, 0, 1, 1, 1 };
            letterStruct[7] = new byte[8] { 1, 1, 0, 0, 0, 0, 1, 1 };
            lettersStructs[22] = letterStruct;
            letterStruct = new byte[8][];
            //X
            letterStruct[0] = new byte[8] { 1, 1, 0, 0, 0, 0, 1, 1 };
            letterStruct[1] = new byte[8] { 1, 1, 1, 0, 0, 1, 1, 1 };
            letterStruct[2] = new byte[8] { 0, 1, 1, 1, 1, 1, 1, 0 };
            letterStruct[3] = new byte[8] { 0, 0, 1, 1, 1, 1, 0, 0 };
            letterStruct[4] = new byte[8] { 0, 1, 1, 1, 1, 1, 1, 0 };
            letterStruct[5] = new byte[8] { 1, 1, 1, 0, 0, 1, 1, 1 };
            letterStruct[6] = new byte[8] { 1, 1, 1, 0, 0, 1, 1, 1 };
            letterStruct[7] = new byte[8] { 1, 1, 0, 0, 0, 0, 1, 1 };
            lettersStructs[23] = letterStruct;
            letterStruct = new byte[8][];
            //Y
            letterStruct[0] = new byte[6] { 1, 1, 0, 0, 1, 1 };
            letterStruct[1] = new byte[6] { 1, 1, 0, 0, 1, 1 };
            letterStruct[2] = new byte[6] { 1, 1, 0, 0, 1, 1 };
            letterStruct[3] = new byte[6] { 1, 1, 1, 1, 1, 1 };
            letterStruct[4] = new byte[6] { 1, 1, 1, 1, 1, 1 };
            letterStruct[5] = new byte[6] { 0, 0, 1, 1, 0, 0 };
            letterStruct[6] = new byte[6] { 0, 0, 1, 1, 0, 0 };
            letterStruct[7] = new byte[6] { 0, 0, 1, 1, 0, 0 };
            lettersStructs[25] = letterStruct;
            letterStruct = new byte[8][];
            //Z
            letterStruct[0] = new byte[6] { 1, 1, 1, 1, 1, 1 };
            letterStruct[1] = new byte[6] { 1, 1, 1, 1, 1, 1 };
            letterStruct[2] = new byte[6] { 0, 0, 0, 1, 1, 1 };
            letterStruct[3] = new byte[6] { 0, 1, 1, 1, 1, 0 };
            letterStruct[4] = new byte[6] { 0, 1, 1, 1, 1, 0 };
            letterStruct[5] = new byte[6] { 0, 0, 1, 1, 0, 0 };
            letterStruct[6] = new byte[6] { 1, 1, 1, 0, 0, 0 };
            letterStruct[7] = new byte[6] { 1, 1, 1, 1, 1, 1 };
            lettersStructs[25] = letterStruct;
            /* for (int row = 0; row < letterStruct.Length; row++)
             {
                 byte[] rowArr = letterStruct[row];
                 for (int col = 0; col < rowArr.Length; col++)
                 {
                     char letter = ' ';
                     Console.SetCursorPosition(col, row);
                     if (letterStruct[row][col] == 1)
                     {
                         int a = rand.Next(0, 2) == 0 ? 'A' : 'a';
                         letter = (char)(a + rand.Next(0, 26));
                     }

                     Console.Write(letter);
                 }
             }*/
            return lettersStructs;
            
        }
    }
}
