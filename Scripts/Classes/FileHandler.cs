using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

/// <summary>
/// Used to handle writing to files.  
/// </summary>
public class FileHandler
{
    /// <summary>
    /// Creates file (or directory) if it doesn't exist
    /// Then appends line 
    /// </summary>
    public void AppendLine(string path, string newLine)
    {
        if (!File.Exists(path))
        {
            string directory = Path.GetDirectoryName(path); 
            if(directory!=null & !Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            using (StreamWriter file = File.CreateText(path))
            {
                file.WriteLine(newLine);
            }
        }

        else
        {
            using (System.IO.StreamWriter file =
                   new System.IO.StreamWriter(@path, true))
            {
                file.WriteLine(newLine);
            }
        }
    }

    public void RemoveLastLine(string path, int numLines)
    {
        List<string> lineList = new List<string>(File.ReadAllLines(path));
        lineList.RemoveAt(lineList.Count - numLines);
        File.WriteAllLines(path, lineList.ToArray());
    }

    public void RemoveLastLine(string path)
    {
        RemoveLastLine(path, 1);
    }
}
