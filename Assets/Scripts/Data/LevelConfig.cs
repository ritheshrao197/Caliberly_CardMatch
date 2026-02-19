using System.Collections.Generic;
using UnityEngine;
namespace MemoryGame
{
    /// <summary>
    /// Definition of a single level in the Memory Game
    /// </summary>
  
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "MemoryGame/LevelConfig")]
    public class LevelConfig : ScriptableObject
    {
        public List<LevelDef> levels = new List<LevelDef>()
    {
        // Easy Levels (1-10)
        new LevelDef{ rows=2, cols=2 },                    // Level 1: 4 cards
        new LevelDef{ rows=2, cols=3 },                    // Level 2: 6 cards
        new LevelDef{ rows=2, cols=4 },                    // Level 3: 8 cards
        new LevelDef{ rows=3, cols=3 },                    // Level 4: 9 cards
        new LevelDef{ rows=3, cols=4 },                    // Level 5: 12 cards
        new LevelDef{ rows=4, cols=3 },                    // Level 6: 12 cards
        new LevelDef{ rows=4, cols=4 },                    // Level 7: 16 cards
        new LevelDef{ rows=4, cols=5 },                    // Level 8: 18 cards
        new LevelDef{ rows=5, cols=5 },                    // Level 9: 20 cards
        new LevelDef{ rows=5, cols=6 },                    // Level 10: 20 cards

        // Medium Levels (11-20)
        new LevelDef{ rows=4, cols=4, moveLimit=32 },      // Level 11: 16 cards, 32 moves
        new LevelDef{ rows=4, cols=5, moveLimit=40 },      // Level 12: 20 cards, 40 moves
        new LevelDef{ rows=5, cols=4, timeLimitSec=120 },  // Level 13: 20 cards, 2min
        new LevelDef{ rows=4, cols=6, moveLimit=48 },      // Level 14: 24 cards, 48 moves
        new LevelDef{ rows=5, cols=5, timeLimitSec=150 },  // Level 15: 25 cards, 2.5min
        new LevelDef{ rows=6, cols=4, moveLimit=48 },      // Level 16: 24 cards, 48 moves
        new LevelDef{ rows=5, cols=6, timeLimitSec=180 },  // Level 17: 30 cards, 3min
        new LevelDef{ rows=6, cols=5, moveLimit=60 },      // Level 18: 30 cards, 60 moves
        new LevelDef{ rows=6, cols=6, timeLimitSec=210 },  // Level 19: 36 cards, 3.5min
        new LevelDef{ rows=7, cols=5, moveLimit=70 },      // Level 20: 35 cards, 70 moves

        // Hard Levels (21-30)
        new LevelDef{ rows=6, cols=6, moveLimit=54, timeLimitSec=180 },  // Level 21: 36 cards, 54 moves, 3min
        new LevelDef{ rows=7, cols=5, moveLimit=60, timeLimitSec=165 },  // Level 22: 35 cards, 60 moves, 2.75min
        new LevelDef{ rows=6, cols=7, moveLimit=63, timeLimitSec=180 },  // Level 23: 42 cards, 63 moves, 3min
        new LevelDef{ rows=7, cols=6, moveLimit=70, timeLimitSec=195 },  // Level 24: 42 cards, 70 moves, 3.25min
        new LevelDef{ rows=8, cols=5, moveLimit=65, timeLimitSec=180 },  // Level 25: 40 cards, 65 moves, 3min
        new LevelDef{ rows=7, cols=7, moveLimit=77, timeLimitSec=210 },  // Level 26: 49 cards, 77 moves, 3.5min
        new LevelDef{ rows=8, cols=6, moveLimit=72, timeLimitSec=195 },  // Level 27: 48 cards, 72 moves, 3.25min
        new LevelDef{ rows=6, cols=9, moveLimit=81, timeLimitSec=225 },  // Level 28: 54 cards, 81 moves, 3.75min
        new LevelDef{ rows=8, cols=7, moveLimit=84, timeLimitSec=210 },  // Level 29: 56 cards, 84 moves, 3.5min
        new LevelDef{ rows=8, cols=8, moveLimit=96, timeLimitSec=240 }   // Level 30: 64 cards, 96 moves, 4min
    };

        public int MaxCardsPerLevel()
        {
            int max = 0;
            for (int i = 0; i < levels.Count; i++)
            {
                int total = levels[i].rows * levels[i].cols;
                if (total > max) max = total;
            }
            return max;
        }
    }
}