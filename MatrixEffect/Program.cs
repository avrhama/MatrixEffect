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
        char[][] lettersBoard;
        ConsoleColor[][] lettersBoardColor;
        int nameDisplayStartX = 0;
        int nameDisplayY = 20;
        int nameDisplayEndX = 0;
        int nameDisplayHight = 8;
        char[][] nameStuct = new char[8][];
        string name;
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

            lettersBoard = new char[Console.WindowHeight][];
            lettersBoardColor = new ConsoleColor[Console.WindowHeight][];
            //fill the letters board with _(for a white space character, ' ' is used for RainDrop's tail)
            for (int row = 0; row < lettersBoard.Length; row++)
            {
                lettersBoard[row] = new char[Console.WindowWidth];
                lettersBoardColor[row] = new ConsoleColor[Console.WindowWidth];
                for (int col = 0; col < lettersBoard[row].Length; col++)
                {
                    lettersBoard[row][col] = '_';
                    lettersBoardColor[row][col] = ConsoleColor.White;
                }
            }
            //fill the array with array of letter structure.
            lettersStructs = createLetters();
            //the max letter width size is 8,  and we want  2 cols of rain in  the outter side for each col.
            int maxNameLength = Console.WindowWidth / 8 - 4;
            Console.WriteLine($"Enter you name(max:{maxNameLength}):");
            name = Console.ReadLine();
            if (name.Length > maxNameLength)
                name = name.Substring(0, maxNameLength);
            //its was more convenient, to represent ' '  as '[' because '[' comes next after 'Z' in ascii table so i can use it's 133 ascii code. 
            name = name.ToUpper().Trim().Replace(" ", "[");
            //the max letter width size is 8
            int nameSize = name.Length * 8;
            //pos the name in the middle of the screen.
            nameDisplayStartX = (Console.WindowWidth - nameSize) / 2;
            int offsetX = nameDisplayStartX;
            int offsetY = nameDisplayY;
            Console.Clear();
            pixelLetterList = new List<PixelLetter>();
            pixelLetterListMatch = new Dictionary<char, List<RainDrop.RainLetter>>();
            pixelLetterListMatchCompleted = new Dictionary<char, List<RainDrop.RainLetter>>();


            for (int i = 0; i < name.Length; i++)
            {
                if (name[i] >= 65 && name[i] <= 91)
                {
                    byte[][] letterStruct = lettersStructs[name[i] - 65];
                    for (int row = 0; row < letterStruct.Length; row++)
                    {
                        byte[] rowArr = letterStruct[row];
                        for (int col = 0; col < rowArr.Length; col++)
                        {
                            
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

            for (int row = 0; row < nameDisplayHight; row++)
            {
                nameStuct[row] = new char[offsetX];
            }

            int letterX = 0, letterY = 0, tempX = 0;
            for (int i = 0; i < name.Length; i++)
            {
                if (name[i] >= 65 && name[i] <= 91)
                {

                    byte[][] letterStruct = lettersStructs[name[i] - 65];
                    letterY = 0;
                    for (int row = 0; row < letterStruct.Length; row++, letterY++)
                    {
                        byte[] rowArr = letterStruct[row];
                        int col;
                        for (col = 0, letterX = tempX; col < rowArr.Length; col++,letterX++)
                        {
                            if (letterStruct[row][col] != 0)
                                nameStuct[letterY][letterX] = name[i];
                            

                        }
                    }
                    tempX += letterStruct[0].Length+1;
                }
            }
            nameDisplayEndX = offsetX;
            rainDropList = new List<RainDrop>();
            maxRainDrops = 100;
            int maxCounter = 0;
            // Timer t = new Timer(TimerCallback, null, 0, 50);
            while (true)
            {
                TimerCallback(null);
                Thread.Sleep(10);
              /*  if (maxCounter == 10)
                {
                    maxCounter = 0;
                    maxRainDrops++;
                }*/
                if (!keepRain && rainDropList.Count == 0)
                    break;
            }
            Console.ReadLine();
            Console.Clear();
            keepRain = true;
            lettersSum = 0;
            start();
            
        }
        List<RainDrop> rainDropList;
        int maxRainDrops;
        object locker = new object();
        bool timerTicked = false;
        bool keepRain = true;
        int lettersSum = 0;
        public void TimerCallback(Object o)
        {

            /* lock (locker)
             {
                 if (timerTicked == true)
                     return;
                 timerTicked = true;
             }*/
            // Display the date/time when this method got called.
            // Console.WriteLine("In TimerCallback: " + DateTime.Now);
            // Force a garbage collection to occur for this demo.
            int count = rand.Next(2, 4);
            for (int i = 0; i < count && rainDropList.Count < maxRainDrops && keepRain; i++)
            {
                RainDrop rd = createRainDrop();
                rainDropList.Add(rd);
            }


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
                        Console.ForegroundColor = ConsoleColor.Green;
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
                    //Console.SetCursorPosition(rl.pos.X, rl.pos.Y);
                    //Console.ForegroundColor = rl.color;
                    //Console.Write(rl.letter);
                    lettersBoard[rl.pos.Y][rl.pos.X] = rl.letter;
                    lettersBoardColor[rl.pos.Y][rl.pos.X] = ConsoleColor.White;

                }
            }


            List<RainDrop> rainDropListRemove = new List<RainDrop>();

            for (int i = 0; i < rainDropList.Count; i++)
            {
                RainDrop rd = rainDropList[i];
                
                
                if (drawRainDropVer3(rd))
                    rainDropListRemove.Add(rd);
            }
            foreach (RainDrop rd in rainDropListRemove)
                rainDropList.Remove(rd);
            rainDropListRemove.Clear();

            drawLettersVer2();
            //GC.Collect();
            /*  lock (locker)
              {
                  timerTicked = false;
              }*/

        }
        public void drawLettersVer1()
        {

            for (int row = 0; row < lettersBoard.Length; row++)
            {
                Console.SetCursorPosition(0, row);
                string greenLine = "";
                bool emptyLine = true;
                for (int col = 0; col < lettersBoard[row].Length; col++)
                {
                    char c = lettersBoard[row][col];
                    if (c != '_')
                        emptyLine = false;
                    if (c == '_')
                        c = ' ';
                    greenLine += c;
                }
                if (!emptyLine)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(greenLine);
                }
                Console.ForegroundColor = ConsoleColor.White;
                //Draw White Letters
                for (int col = 0; col < lettersBoard[row].Length; col++)
                {
                    char c = lettersBoard[row][col];

                    if (c == ' ')
                    {
                        lettersBoard[row][col] = '_';

                    }

                    if (c == '_' || lettersBoardColor[row][col] == ConsoleColor.Green)
                        continue;
                    Console.SetCursorPosition(col, row);

                    Console.Write(c);

                }
            }

        }
        public void drawLettersVer2()
        {
            string greenLine = "";
            for (int row = 0; row < lettersBoard.Length; row++)
            {
                /*Console.SetCursorPosition(0, row);
                string greenLine = "";*/
                for (int col = 0; col < lettersBoard[row].Length; col++)
                {
                    char c = lettersBoard[row][col];
                    if (c == '_')
                        c = ' ';


                    greenLine += c;
                    /* if (c == '_')
                         continue;
                     Console.SetCursorPosition(col, row);
                     Console.ForegroundColor = ConsoleColor.Green;
                     Console.Write(c);
                     if (c == ' ')
                         lettersBoard[row][col] = '_';*/
                }
                greenLine += "\n";
                //// Console.ForegroundColor = ConsoleColor.Green;
                /// Console.Write(greenLine);
                //Console.ForegroundColor = ConsoleColor.White;
                /* //Draw White Letters
                 for (int col = 0; col < lettersBoard[row].Length; col++)
                 {
                     char c = lettersBoard[row][col];

                     if (c == ' ')
                     {
                         lettersBoard[row][col] = '_';

                     }

                      if (c == '_'||lettersBoardColor[row][col]==ConsoleColor.Green)
                          continue;
                      Console.SetCursorPosition(col, row);

                      Console.Write(c);

                 }*/
            }
            greenLine = greenLine.Substring(0, greenLine.Length - 1);
            Console.SetCursorPosition(0, 0);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(greenLine);
            for (int row = 0; row < lettersBoard.Length; row++)
            {

                Console.ForegroundColor = ConsoleColor.White;
                //Draw White Letters
                for (int col = 0; col < lettersBoard[row].Length; col++)
                {
                    char c = lettersBoard[row][col];

                    if (c == ' ')
                    {
                        lettersBoard[row][col] = '_';

                    }

                    if (c == '_' || lettersBoardColor[row][col] == ConsoleColor.Green)
                        continue;
                    Console.SetCursorPosition(col, row);

                    Console.Write(c);

                }
            }
        }
        public RainDrop createRainDrop()
        {
            //checks theres no overlapping when rainDropCreated[avoiding the case when dropSpeed is the same].
            int size = rand.Next(7, 10);
            int x = rand.Next(pixelLetterList[0].pos.X - 2, pixelLetterList[pixelLetterList.Count - 1].pos.X + pixelLetterList[pixelLetterList.Count - 1].size.Width + 2);
            int coin = rand.Next(1, 7);
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
        
            int dropSpeed = rand.Next(1, 10);

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
        public bool drawRainDropVer1(RainDrop rd)
        {
            while (!rd.completeCycle && rd.changed)
            {
                RainDrop.RainLetter rl = rd.getLetter();

                //Console.SetCursorPosition(rl.pos.X, rl.pos.Y);
                //  Console.ForegroundColor = rl.color;
                //  Console.Write(rl.letter);

                lettersBoard[rl.pos.Y][rl.pos.X] = rl.letter;
                lettersBoardColor[rl.pos.Y][rl.pos.X] = rl.color;
                //checks if letter colides with name's letter.
                //can be improve by  build a array represent the name by location.
                foreach (PixelLetter pl in pixelLetterList)
                {
                    if ((rl.pos.X >= pl.pos.X) && rl.pos.X < (pl.pos.X + pl.size.Width))
                    {
                        if (((rl.pos.Y >= pl.pos.Y) && rl.pos.Y < (pl.pos.Y + pl.size.Height)))
                        {
                            if (lettersStructs[pl.letter - 65][rl.pos.Y - pl.pos.Y][rl.pos.X - pl.pos.X] == 0)
                                break;
                            if (!pixelLetterListMatch.ContainsKey(pl.letter))
                                pixelLetterListMatch[pl.letter] = new List<RainDrop.RainLetter>();

                            if (!pixelLetterListMatch[pl.letter].Contains(rl))
                            {
                                if (!pixelLetterListMatchCompleted.ContainsKey(pl.letter) || !pixelLetterListMatchCompleted[pl.letter].Contains(rl))
                                    pixelLetterListMatch[pl.letter].Add(rl);
                            }
                            break;
                        }

                    }
                }
                //lettersStructs

            }

            return rd.moveDown();
        }
        public bool drawRainDropVer3(RainDrop rd)
        {

            int counter = rd.dropSpeed;
            bool result = false;
            while (counter > 0)
            {
                
            while (!rd.completeCycle && rd.changed)
            {
                RainDrop.RainLetter rl = rd.getLetter();



                lettersBoard[rl.pos.Y][rl.pos.X] = rl.letter;
                lettersBoardColor[rl.pos.Y][rl.pos.X] = rl.color;
                //checks if letter colides with name's letter.
                //can be improve by  build a array represent the name by location.
               
                //lettersStructs
                if ((rl.pos.X >= nameDisplayStartX) && (rl.pos.X < nameDisplayEndX))
                {
                    if ((rl.pos.Y >= nameDisplayY) && (rl.pos.Y < nameDisplayY + nameDisplayHight))
                    {
                        int x = rl.pos.X - nameDisplayStartX;
                        int y = rl.pos.Y - nameDisplayY;
                        if (nameStuct[y][x] != 0)
                        {
                            PixelLetter pl = new PixelLetter(nameStuct[y][x], new Point(rl.pos.X, rl.pos.Y),new Size(0,0));
                            if (!pixelLetterListMatch.ContainsKey(pl.letter))
                                pixelLetterListMatch[pl.letter] = new List<RainDrop.RainLetter>();

                            if (!pixelLetterListMatch[pl.letter].Contains(rl))
                            {
                                if (!pixelLetterListMatchCompleted.ContainsKey(pl.letter) || !pixelLetterListMatchCompleted[pl.letter].Contains(rl))
                                    pixelLetterListMatch[pl.letter].Add(rl);
                            }
                        }
                    }
                }
            }
                if (rd.moveDown())
                    return true;
                counter--;
            }
            return false;
        }
        public class RainDrop
        {
            static Random rander = new Random();
            Point headPos;
            int size;
           public int dropSpeed;
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
                //when the letter had reach the bottom, we need to select the next letter to be the new head for cycle through all letters.
                headLetterIndex = 0;
                setRandLetters();
            }
            //fill the RainDrop with rands letters.
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
                /*
                 * currLetterIndex == size: we passed through all the (currently display) letters in the RainDrop.
                 * currLetterIndex > fallingLength: falling effect, we dont that all letters to appear at once.
                 * pos.Y == 0: the previous letter was in the first line of the screen so this letter can be display.
                 */
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
              
                    //indicates that the some changes occurs to the RainDrop.
                    changed = true;
                    //indicates that we passed through all the (currently display) letters in the RainDrop,thus we can move to the next RainDrop.
                    completeCycle = false;
                    headPos.Y++;
                    //if curr dropRain head reached the bottom proceed the next letter.
                    if (Console.WindowHeight <= headPos.Y)
                    {
                        headLetterIndex = headPos.Y - Console.WindowHeight + 1;
                        currLetterIndex = headLetterIndex;

                    }
                    //the last letter has cross the bottom line, RainDrop should be removed.
                    if (headPos.Y >= Console.WindowHeight + (size - 1))
                        return true;
                    /*else
                        return false;*/
               
                if ((fallingLength % replaceLetterSpeed) == 0)
                {
                    changed = true;

                    setRandLetters();
                }
               
                return false;
            }
            //struct represent single letter in rainDrop
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
        //class for a each letter in the name.
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
            byte[][][] lettersStructs = new byte[27][][];
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
            lettersStructs[24] = letterStruct;
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
            letterStruct = new byte[8][];
            //Space
            letterStruct[0] = new byte[2] { 0, 0 };
            letterStruct[1] = new byte[2] { 0, 0 };
            letterStruct[2] = new byte[2] { 0, 0 };
            letterStruct[3] = new byte[2] { 0, 0 };
            letterStruct[4] = new byte[2] { 0, 0 };
            letterStruct[5] = new byte[2] { 0, 0 };
            letterStruct[6] = new byte[2] { 0, 0 };
            letterStruct[7] = new byte[2] { 0, 0 };
            lettersStructs[26] = letterStruct;
            //test print letters
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
