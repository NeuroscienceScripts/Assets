using UnityEngine;
using System;

namespace Classes
{
    /// <summary>
    /// Internal container for GridLocation coordinates
    /// </summary>
    [Serializable]
    public struct Coordinate
    {

        [field: SerializeField]
        public int X { get; set; }
        [field: SerializeField]
        public int Y { get; set; }

        public static bool operator==(Coordinate a, Coordinate b) => a.X == b.X && a.Y == b.Y;
        public static bool operator !=(Coordinate a, Coordinate b) => a.X != b.X || a.Y != b.Y;

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Casts Coordinate to Grid Location where (0,0) is the lower left corner A1
        /// </summary>
        public GridLocation ToGridLocation()
        {
            return new GridLocation(Constants.LETTERS[Constants.GRID_LENGTH-1-Y], X+1);
        }

        /// <summary>
        /// String version of ToGridLocation
        /// </summary>
        /// <returns>string corresponding to GridLocation</returns>
        public string GridLocString()
        {
            return ToGridLocation().GetString();
        }
    }

    /// <summary>
    /// Used to represent the scene as a 7x7 Grid Space.  Y-direction is represented
    /// with a letter, starting with 'A' at the top and moving to 'G' at the bottom.
    /// X-direction is number based. This results in a letter/number combo for each
    /// 1x1 square of the grid (i.e. 'A1', 'G2', 'B6', etc).
    ///
    /// Contains functions to get the Unity-space location for a GridLocation object,
    /// along with a function to check what object (by name) is located at each location. 
    ///  
    /// </summary>
    [Serializable]
    public struct GridLocation
    {
        private Coordinate pos;
        private string y;
        private int x;

        public GridLocation(string y, int x)
        {
            pos = new Coordinate
            {
                X = x - (Constants.GRID_WIDTH / 2 + 1),
                Y = Constants.GRID_LENGTH/2 - Array.IndexOf(Constants.LETTERS, y)
            };
            this.y = y;
            this.x = x;
        }

        public GridLocation(int x, int y)
        {
            pos = new Coordinate
            {
                X = x,
                Y = y
            };
            this.y = Constants.LETTERS[Constants.GRID_LENGTH/2 - y];
            this.x = x + Constants.GRID_WIDTH/2 + 1;
        }

        public static bool operator ==(GridLocation a, GridLocation b) => a.GetX() == b.GetX() && a.GetY() == b.GetY();
        public static bool operator !=(GridLocation a, GridLocation b) => a.GetX() != b.GetX() || a.GetY() != b.GetY();

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public int GetX()
        {
            return pos.X;
        }

        public int GetY()
        {
            return pos.Y;
        }

        public string GetTarget() 
        {
            //TODO: Update with paintings
            switch (y)
            {
                case "A":
                    if (x == 1)
                        return "Stove";
                    else if (x==6)
                        return "Piano";
                    break;
                case "B":
                    if (x==2)
                        return "Picnic Table";
                    break;
                case "C":
                    if (x == 6)
                        return "Bookshelf";
                    else if (x==7)
                        return "Trashcan";
                    break;
                case "D":
                    if (x == 1)
                        return "Telescope";
                    else if (x==4)
                        return "WheelBarrow";
                    break;
                case "E":
                    if (x == 6)
                        return "Harp";
                    break;
                case "F": 
                    if (x==1)
                        return "Plant";
                    else if (x==6)
                        return "Well";
                    break;
                case "G":
                    if (x==2)
                        return "Mailbox";
                    else if (x == 5)
                        return "Chair";
                    break;
            }
            
            Debug.Log("Target not identifiable, check target location");
            return "Target not identifiable, check target location";  
        }

        /// <summary>
        /// Useful for printing GridLocation meaningfully ("A1", "B2", etc)
        /// </summary>
        /// <returns>string (y+x) (i.e. "A1", "B2", etc.)</returns>
        public string GetString()
        {
            return y + x; 
        }

   
    }
}