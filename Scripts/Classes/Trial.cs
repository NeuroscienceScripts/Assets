using UnityEngine;

namespace Classes
{
    /// <summary>
     /// A simple pair of start/end locations using GridLocation
     /// </summary>
     /// <param name="start">Starting GridLocation</param>
     /// <param name="end">Ending GridLocation</param>
    public struct Trial
    {
        public GridLocation start;
        public GridLocation end;
        public bool stressTrial;

        
        public Trial(GridLocation start, GridLocation end)
        {
            this.start = start;
            this.end = end;
            this.stressTrial = false;
        }
        
        public Trial(GridLocation start, GridLocation end, bool stressTrial)
        {
            this.start = start;
            this.end = end;
            this.stressTrial = stressTrial;
        }

        public override string ToString()
        {
            return start.GetString() + "," + end.GetString();
        }
    }
}