namespace Nachhilfe
{
    public class Service
    {
        public int Level { get; set; }
        public string Subject { get; set; }

        public override string ToString()
        {
            return $"{Subject} in {Level}.Klassen";
        }

        public string ToCsvString()
        {
            return $"{Subject};{Level}";
        }
    }
}