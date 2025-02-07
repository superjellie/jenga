using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {
    [CreateAssetMenu(
        fileName = "New Level Sequencer",
        menuName = "Jenga/Level Sequencer",
        order = 1010
    )]
    public class LevelSequencer : ScriptableObject {
        public MonoGeneratorReference<LevelContext> levelGenerator;
    }
}