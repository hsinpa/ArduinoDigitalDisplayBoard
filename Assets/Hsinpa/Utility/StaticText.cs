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

            public const string SaveModalDescription = "<color=#2E49BC>{0}</color> was closesd, at time <color=#2E49BC>{1}</color>. Do you want to resume the last save state?";
        }

        public class Hint
        {
            public const string SearchBLE = "Searching<br>Connecting to BLE Board . . .";
            public const string ReconnectionBLE = "Connection failed<br>Try reconnecting . . .";

            public const string SliderTitle = "Rotation Speed: {0} seconds";
        }
    }
}