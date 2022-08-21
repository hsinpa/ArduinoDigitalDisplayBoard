using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleEvent.ID
{
    public class StaticText
    {
        public class Functions {
            public const string ExtraFoulTitle = "Team {0} fouls : {1}";
            public const string SubstitutionTitle = "Team {0}: Substitution";
        }

        public class Hint
        {
            public const string SearchBLE = "Searching<br>Connecting to BLE Board . . .";
            public const string ReconnectionBLE = "Connection failed<br>Try reconnecting . . .";
        }
    }
}