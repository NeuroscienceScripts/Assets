using Unity.VisualScripting;
using UnityEngine;

namespace Classes
{
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
    public struct GridLocation
    {
        public string y;
        public int x;

        public GridLocation(string y, int x)
        {
            this.y = y;
            this.x = x; 
        }

        public float GetX()
        {
            switch (x)
            {
                case 1:
                    return -3;
                case 2:
                    return -2;
                case 3:
                    return -1;
                case 4:
                    return 0;
                case 5:
                    return 1;
                case 6:
                    return 2;
                case 7:
                    return 3;
                default:
                    return 100;
            }

        }

        public float GetY()
        {
            switch (y)
            {
                case "A":
                    return 3;
                case "B":
                    return 2;
                case "C":
                    return 1;
                case "D":
                    return 0;
                case "E":
                    return -1;
                case "F":
                    return -2;
                case "G":
                    return -3; 
                default:
                    return 100; 
            }
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
                    else if (x == 6)
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