using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEA_prototype_pathfinding
{
    
    class Program
    {
        static List<string> maze = new List<string>(){
               "___________________",
               "|G       |        |",
               "| -- --- | --- -- |",
               "| -- --- | --- -- |",
               "|                 |",
               "| -- | ----- | -- |",
               "|    |   |   |    |",
               "|--- |-- | --| ---|",
               "|##| |       | |##|",
               "|---   -- --   ---|",
               "|      |   |      |",
               "|--- | ----- | ---|",
               "|##| |       | |##|",
               "|--- | ----- | ---|",
               "|        |        |",
               "|        |        |",
               "| -- --- - --- -- |",
               "|  |           |  |",
               "|- | | ----- | | -|",
               "|    |   |P  |    |",
               "| ------ | ------ |",
               "|                 |",
               "-------------------",
            };

        public static string[] buffer = maze.ToArray();
        static List<Cell> WalkableCells(List<string> maze, Cell Current, Cell Target)
        {
            List<Cell> Possible = new List<Cell>() //create list of possible nodes that can be moved to surrounding the current selected node
            {
                new Cell {x = Current.x, y = Current.y-1, parent = Current, G = Current.G++},
                new Cell {x = Current.x, y = Current.y+1, parent = Current, G = Current.G++},
                new Cell {x = Current.x - 1, y = Current.y, parent = Current, G = Current.G++},
                new Cell {x = Current.x + 1, y = Current.y, parent = Current, G = Current.G++}
               
                
            };

            for (int i = 0; i < Possible.Count; i++)
            {
                Possible[i].CalculateH(Target.x, Target.y); //works out the heuristic for each of those nodes
            }
            int maxX = maze.Last().Length - 1;
            int maxY = maze.Count() - 1; //borders of the maze
            foreach (Cell cells in Possible.ToList())
            {
                if (cells.x > maxX || cells.x < 0)
                {
                    Possible.Remove(cells); //removes cells outside of the maze
                }
                else if (cells.y > maxY || cells.y < 0)
                {
                    Possible.Remove(cells); //removes cells outside of the maze
                }
                else if (maze[cells.y][cells.x] != ' ' && maze[cells.y][cells.x] != 'G' && maze[cells.y][cells.x] != 'P')
                {
                    Possible.Remove(cells); //removes cells that are walls
                }
            }
            return Possible;
        }

        static void display(string[] buffer) //removes stuttering by printing the whole maze at once
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Use WASD or Arrow Keys to move\n");
            for (int i = 0; i < buffer.Length; i++)
            {
                sb.AppendLine(buffer[i]);
            }
            Console.CursorLeft = 0;
            Console.CursorTop = 0;
            Console.WriteLine(sb);
        }

        static string ReplaceAt(string input, int index, string newString) //replaces a character at a specified index in a 
        {                                                                  //string with a different character
            return input.Remove(index, 1).Insert(index, newString);
        }
        static void Main(string[] args)
        {
            display(buffer);
            Cell ghost = new Cell();
            ghost.y = maze.FindIndex(x => x.Contains("G")); //finds the coordinates of the ghost node
            ghost.x = maze[ghost.y].IndexOf("G");
            Cell pac = new Cell();
            pac.y = maze.FindIndex(x => x.Contains("P")); //finds the coordinates of the pacman node
            pac.x = maze[pac.y].IndexOf("P");
            ghost.CalculateH(pac.x, pac.y); //works out estimated distance from ghost node to pacman node
            while (true)
            {
                List<Cell> open = new List<Cell>(); //list of nodes that are being visited/need to be visited
                List<Cell> closed = new List<Cell>(); //list of nodes already visited
                open.Add(ghost);
                ConsoleKeyInfo choice = Console.ReadKey(true); //used to allow arrow keys as inputs
                if ((choice.Key == ConsoleKey.UpArrow || choice.Key == ConsoleKey.W) && pac.y > 0)
                {
                    if (maze[pac.y - 1][pac.x] == ' ')
                    {
                        maze[pac.y] = maze[pac.y].Replace('P', ' '); //moves the pacman node up
                        pac.y--;
                        maze[pac.y] = ReplaceAt(maze[pac.y], pac.x, "P");
                    }
                }
                if ((choice.Key == ConsoleKey.DownArrow || choice.Key == ConsoleKey.S) && pac.y < maze.Count - 1)
                {
                    if (maze[pac.y + 1][pac.x] == ' ') //moves the pacman node down
                    {
                        maze[pac.y] = maze[pac.y].Replace('P', ' ');
                        pac.y++;
                        maze[pac.y] = ReplaceAt(maze[pac.y], pac.x, "P");
                    }

                }
                if ((choice.Key == ConsoleKey.LeftArrow || choice.Key == ConsoleKey.A) && pac.x > 0)
                {
                    string newMaze = maze[pac.y].Replace(" P", "P "); //moves the pacman node to the right
                    if (maze[pac.y] != newMaze) pac.x--;
                    maze[pac.y] = newMaze;
                }
                if ((choice.Key == ConsoleKey.RightArrow || choice.Key == ConsoleKey.D) && pac.x < maze[0].Length - 1)
                {
                    string newMaze = maze[pac.y].Replace("P ", " P"); //moves the pacman node to the left
                    if (maze[pac.y] != newMaze) pac.x++;
                    maze[pac.y] = newMaze;
                }
                buffer = maze.ToArray();
                while (open.Count != 0) //while there are nodes to be visited
                {
                    Cell check = open.OrderBy(x => x.F).First(); //gets the node with the lowest F value (lowest total cost)
                    if (check.x == pac.x && check.y == pac.y) //path has been found
                    {
                        Cell path = check;
                        while (path.parent != null)
                        {
                            if (path.x != pac.x || path.y != pac.y)
                            {
                                buffer[path.y] = ReplaceAt(buffer[path.y], path.x, "*"); //displays a * at every node that was taken in the path to pacman node
                            }
                            path = path.parent;
                        }
                        display(buffer);
                        break;
                    }
                    closed.Add(check); //adds it to list of viisted nodes
                    open.Remove(check);
                    List<Cell> Walkable = WalkableCells(maze, check, pac); //finds all walkable nodes surrounding the current node
                    foreach (Cell WalkableCell in Walkable)
                    {
                        if (closed.Any(x => x.x == WalkableCell.x && x.y == WalkableCell.y)) continue; //ignores it if its in the closed list
                        if (open.Any(x => x.x == WalkableCell.x && x.y == WalkableCell.y)) //if node is in list of visiting
                        {
                            Cell existing = open.First(x => x.x == WalkableCell.x && x.y == WalkableCell.y);
                            if (existing.F > check.F) //if current node has a lower F value than the same node on a different path
                            {
                                open.Remove(existing);
                                open.Add(WalkableCell);
                            }
                        }
                        else
                        {
                            open.Add(WalkableCell);
                        }
                    }
                }
            }
        }
    }
}
