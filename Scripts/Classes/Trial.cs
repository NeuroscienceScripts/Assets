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
        public GridLocation blockedLocation; 
        
        public Trial(GridLocation start, GridLocation end)
        {
            this.start = start;
            this.end = end;
            this.stressTrial = false;
            this.blockedLocation = new GridLocation("A", 1);
        }
        
        public Trial(GridLocation start, GridLocation end, bool stressTrial, GridLocation blockedLocation)
        {
            this.start = start;
            this.end = end;
            this.stressTrial = stressTrial;
            this.blockedLocation = blockedLocation;
        }

        public override string ToString()
        {
            return start.GetString() + "," + end.GetString();
        }
    }
}