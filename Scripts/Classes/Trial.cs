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
        public bool isWallTrial;
        // ADDED
        public bool hasAudio;
        
        
        public void Trial1(GridLocation start, GridLocation end)
        {
            this.start = start;
            this.end = end;
            stressTrial = false;
            isWallTrial = false;
        }
        
        public Trial(GridLocation start, GridLocation end, bool stressTrial, bool hasAudio)
        {
            this.start = start;
            this.end = end;
            this.stressTrial = stressTrial;
            isWallTrial = false;
            this.hasAudio = hasAudio;
        }
        
        public Trial(GridLocation start, GridLocation end, bool stressTrial, bool isWallTrial, bool hasAudio) 
        {
            this.start = start;
            this.end = end;
            this.stressTrial = stressTrial;
            this.isWallTrial = isWallTrial;
            this.hasAudio = hasAudio;
        }
        

        public override string ToString()
        {
            return start.GetString() + "," + end.GetString();
        }
    }
}