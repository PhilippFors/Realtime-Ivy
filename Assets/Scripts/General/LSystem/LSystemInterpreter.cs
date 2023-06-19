using VineGeneration.Generators.LSystemGenerator;

namespace General.LSystem
{
	public class LSystemInterpreter
	{
		public void Interpret(LSystemData data, LSystemGenerator generator)
		{
			if (data.finished) {
				return;
			}

			for (int i = data.lastInterpreted; i < data.result.Count; i++) {
				var rule = data.result[i];
				if (!rule.ApplyRule(data, this, generator)) {
					data.lastInterpreted = i + 1;
					return;
				}
			}

			if (data.lastInterpreted >= data.result.Count) {
				data.finished = true;
				data.branch.alive = false;
			}
		}
	}
}
