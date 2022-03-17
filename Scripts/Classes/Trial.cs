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


        
        public Trial(GridLocation start, GridLocation end)
        {
            this.start = start;
            this.end = end;
        }

        public override string ToString()
        {
            return start.GetString() + "," + end.GetString();
        }
    }
}