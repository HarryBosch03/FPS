namespace Code.Scripts.Utility
{
    public class Pair<TA, TB>
    {
        public TA a;
        public TB b;

        public Pair () { }

        public Pair (TA a, TB b)
        {
            this.a = a;
            this.b = b;
        }
    }
}
