namespace Util
{
    public class RandomState
    {
        public float Value => Range(0f, 1f);

        private int seed;
        private System.Random random;
        
        public RandomState(int seed)
        {
            this.seed = seed;
            random = new System.Random(seed);
        }
        
        public void SetSeed(int s)
        {
            seed = s;
            random = new System.Random(seed);
        }
        
        public float Range(float min, float max)
        {
            var value = random.NextDouble() * (max - min) + min;
            return (float)value;
        }
        
        public int Range(int min, int max)
        {
            return random.Next(min, max);
        }
    }
}