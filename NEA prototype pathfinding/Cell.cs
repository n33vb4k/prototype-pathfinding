using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEA_prototype_pathfinding
{
    class Cell
    {
        public int F => G + H; //total cost(distance)
        public int G { get; set; }//Distance from start to current node
        public int H { get; set; } //Distance from current to end node
        public int x { get; set; }
        public int y { get; set; }

        public Cell parent { get; set; }
        public void CalculateH(int X, int Y) //distance between end node and current node
        {
            H = Math.Abs(X - x) + Math.Abs(Y - y);
        }
    }
}
