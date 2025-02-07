using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {
	// int
	[AddTypeMenu(typeof(MonoGenerator<int>), "General/Nothing", 0)]
	public class NothingGeneratorInt : NothingGenerator<int> { }
	[AddTypeMenu(typeof(MonoGenerator<int>), "General/Int", 1), InlinePropertyEditor]
	public class ItemGeneratorInt : ItemGenerator<int> { }
	[AddTypeMenu(typeof(MonoGenerator<int>), "General/Repeat", 2)]
	public class RepeatGeneratorInt : RepeatGenerator<int> { }
	[AddTypeMenu(typeof(MonoGenerator<int>), "General/Sequence", 2)]
	public class SequenceGeneratorInt : SequenceGenerator<int> { }
	[AddTypeMenu(typeof(MonoGenerator<int>), "General/Take Random", 2)]
	public class TakeRandomGeneratorInt : TakeRandomGenerator<int> { }
	[AddTypeMenu(typeof(MonoGenerator<int>), "General/Repeat Forever", 3)]
	public class RepeatForeverGeneratorInt : RepeatForeverGenerator<int> { }
	[AddTypeMenu(typeof(MonoGenerator<int>), "General/Generate While", 4)]
	public class GenerateWhileGeneratorInt : GenerateWhileGenerator<int> { }
	[AddTypeMenu(typeof(MonoGenerator<int>), "General/Generate Until", 5)]
	public class GenerateUntilGeneratorInt : GenerateUntilGenerator<int> { }
	[AddTypeMenu(typeof(MonoGenerator<int>), "General/Optional", 5)]
	public class OptionalGeneratorInt : OptionalGenerator<int> { }

	// LevelContext
	[AddTypeMenu(typeof(MonoGenerator<LevelContext>), "General/Nothing", 0)]
	public class NothingGeneratorLevelContext : NothingGenerator<LevelContext> { }
	[AddTypeMenu(typeof(MonoGenerator<LevelContext>), "General/Level", 1)]
	public class ItemGeneratorLevelContext : ItemGenerator<LevelContext> { }
	[AddTypeMenu(typeof(MonoGenerator<LevelContext>), "General/Repeat", 2)]
	public class RepeatGeneratorLevelContext : RepeatGenerator<LevelContext> { }
	[AddTypeMenu(typeof(MonoGenerator<LevelContext>), "General/Sequence", 2)]
	public class SequenceGeneratorLevelContext : SequenceGenerator<LevelContext> { }
	[AddTypeMenu(typeof(MonoGenerator<LevelContext>), "General/Take Random", 2)]
	public class TakeRandomGeneratorLevelContext : TakeRandomGenerator<LevelContext> { }
	[AddTypeMenu(typeof(MonoGenerator<LevelContext>), "General/Repeat Forever", 3)]
	public class RepeatForeverGeneratorLevelContext : RepeatForeverGenerator<LevelContext> { }
	[AddTypeMenu(typeof(MonoGenerator<LevelContext>), "General/Generate While", 4)]
	public class GenerateWhileGeneratorLevelContext : GenerateWhileGenerator<LevelContext> { }
	[AddTypeMenu(typeof(MonoGenerator<LevelContext>), "General/Generate Until", 5)]
	public class GenerateUntilGeneratorLevelContext : GenerateUntilGenerator<LevelContext> { }
	[AddTypeMenu(typeof(MonoGenerator<LevelContext>), "General/Optional", 5)]
	public class OptionalGeneratorLevelContext : OptionalGenerator<LevelContext> { }

}