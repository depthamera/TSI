namespace TSI.Core
{
    public readonly struct PhysicsInterpolationMessage
    {
        public readonly float Alpha;
        public readonly int StepCount;

        public PhysicsInterpolationMessage(float alpha, int stepCount)
        {
            Alpha = alpha;
            StepCount = stepCount;
        }
        
    }
}
