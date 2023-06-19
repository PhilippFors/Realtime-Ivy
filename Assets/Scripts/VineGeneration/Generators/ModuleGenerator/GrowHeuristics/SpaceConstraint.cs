using VineGeneration.Core;

namespace VineGeneration.Generators.ModuleGenerator.GrowHeuristics
{
    public class SpaceConstraint : BaseGrowHeuristic
    {
        public override bool Execute(ref BranchQueryInfo info)
        {
            var branch = info.branch;
            var origin = info.point;
            var direction = info.direction;
            var length = info.stepLength;
            
            // TODO: find ways to check if a point is crossing a different branch
            
            return true;
        }
    }
}