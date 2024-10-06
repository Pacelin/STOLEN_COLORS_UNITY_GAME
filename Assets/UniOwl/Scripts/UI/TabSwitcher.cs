using UnityEngine;

namespace UniOwl.UI
{
    public class TabSwitcher : MonoBehaviour
    {
        [SerializeField] private GameObject[] tabs;

        public void SetActiveTab(GameObject newTab)
        {
            DisableTabs();
            newTab.SetActive(true);
        }

        public void DisableTabs()
        {
            foreach (var tab in tabs)
                tab.SetActive(false);
        }
    }
}
