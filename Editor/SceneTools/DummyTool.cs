using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {

    [RegisterInToolSelector("Dummy")]
    public class DummyTool : SceneTool { 
        public override string category => "SceneTool";
        public override string title => "Dummy";
        public override int order => 100; 

    }
    
}