using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {
    [System.Serializable]
    public class ToolChannelState {
        public string channel;
        public string state;

        public string shortName => channel.Length > 0 ? $"{channel[0]}" : "?";

        public ToolChannelState(string channel, string state) {
            this.channel = channel;
            this.state = state;
        }

        public bool SameAs(ToolChannelState other) 
            => channel == other.channel && state == other.state;

        public bool IsIn(ToolChannelState[] states) {
            foreach (var state in states)
                if (state.SameAs(this))
                    return true;
            return false;
        }

        public bool HasSameChannelAsIn(ToolChannelState[] states) {
            foreach (var state in states)
                if (state.channel == channel)
                    return true;
                    
            return false;
        }

        public static ToolChannelState[] Intersect(
            ToolChannelState[] list1, ToolChannelState[] list2
        ) {
            List<ToolChannelState> both = new();
            foreach (var state in list1)
                if (state.IsIn(list2))
                    both.Add(state);
            return both.ToArray();
        }
    }
}