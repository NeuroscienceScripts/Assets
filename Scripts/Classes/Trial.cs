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
        public int augmentation;
        
        public Trial(GridLocation start, GridLocation end)
        {
            this.start = start;
            this.end = end;
            this.stressTrial = false;
            isWallTrial = false;
            this.augmentation = 0;
        }
        
        public Trial(GridLocation start, GridLocation end, bool stressTrial)
        {
            this.start = start;
            this.end = end;
            this.stressTrial = stressTrial;
            isWallTrial = false;
            this.augmentation = 0;
        }
        public Trial(GridLocation start, GridLocation end, bool stressTrial, bool isWallTrial)
        {
            this.start = start;
            this.end = end;
            this.stressTrial = stressTrial;
            this.isWallTrial = isWallTrial;
            this.augmentation = 0;
        }
        
        public Trial(GridLocation start, GridLocation end, bool stressTrial, bool isWallTrial, int augmentation)
        {
            this.start = start;
            this.end = end;
            this.stressTrial = stressTrial;
            this.isWallTrial = isWallTrial;
            this.augmentation = augmentation;
        }
        

        public override string ToString()
        {
            return start.GetString() + "," + end.GetString();
        }
    }
}