using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Hsinpa.Bluetooth.Sport
{
    public interface ISport
    {
        void SetupLayout();
        void ExecNextTime(DigitalBoardDataType.CharacterirticsData scoreType);
    }
}