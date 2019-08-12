using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DitherMeThis
{
    public enum SeemDirection
    {
        UP, LEFT, RIGHT
    }

    public class SeemPixelRecord : IComparable<SeemPixelRecord>
    {
        public int best_path_cost;
        public int x;
        public int y;
        
        public int CompareTo(SeemPixelRecord p)
        {
            if (best_path_cost < p.best_path_cost)
                return -1;
            if (best_path_cost > p.best_path_cost)
                return 1;
            return 0;
        }

        public SeemPixelRecord(int cost, int x, int y)
        {
            this.best_path_cost = cost;
            this.x = x;
            this.y = y;
        }
    }
     
    public class Seem
    {
        private SeemPixelRecord start;
        List<SeemDirection> steps;  // from the bottom, which direction we go moving up

        public List<SeemDirection> Steps
        {
            get { return steps; }
        }

        public Seem(SeemPixelRecord start)
        {
            this.start = start;
        }

        
    }
}
