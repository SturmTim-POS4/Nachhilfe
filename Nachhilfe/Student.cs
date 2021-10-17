using System;
using System.Collections.Generic;
using System.Text;

namespace Nachhilfe
{
    public class Student
    {
        public string Clazz { get; set; }

        public string ImagePath => $"csv_Images/Images/{Name.Replace(' ','_')}.jpg";

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string Name { get; set; }

        public List<Service> Services { get; set; } = new();
        

        public static Student Parse(String line)
        {
            String[] studentStrings = line.Split(';');
            String[] name = studentStrings[1].Split(' ');
            return new Student()
            {
                Clazz = studentStrings[0],
                Firstname = name[1],
                Lastname = name[0],
                Name = $"{name[0]} {name[1]}"
            };
        }

        public override string ToString()
        {
            return $"{Firstname} {Lastname}";
        }

        public string ToCsvString()
        {
            StringBuilder serviceString = new StringBuilder();
            foreach (Service service in Services)
            {
                if (serviceString.Length>0)
                {
                    serviceString.AppendLine("");
                }
                serviceString.Append($"{Name};{service.ToCsvString()};");
                
            }
            return serviceString.ToString(); 
        }
    }
}