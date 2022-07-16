using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hsinpa.Bluetooth
{
    public class SportGridBtn : MonoBehaviour
    {
        [SerializeField]
        UIStyle.UIStylesheet uiStylesheet;

        public string id;

        public void SetBtnAction(System.Action action) {
            uiStylesheet.onClick.RemoveAllListeners();
            uiStylesheet.onClick.AddListener(() => action());
        }
    }
}