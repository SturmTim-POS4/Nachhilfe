using System;
using System.Collections.Generic;
using System.IO;

namespace Nachhilfe
{
    public class Configuration
    {
        public static int[] Levels { get; } = {1, 2, 3, 4, 5};

        public static List<String> Subject { get; } = new List<string>();

        static Configuration()
        {
            var subjectStrings = File.ReadAllText(@"csv_Images/csv/subjects.csv").Split(';');
            foreach (var subject in subjectStrings)
            {
                Subject.Add(subject);
            }
            
        }
    }
}