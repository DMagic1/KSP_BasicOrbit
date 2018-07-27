using System;
using System.Collections.Generic;
using UnityEngine;
using BasicOrbit.Unity.Unity;

namespace BasicOrbit.Unity
{
    public class BasicPanelManager : MonoBehaviour
    {
        List<BasicOrbit_Panel> _activePanels = new List<BasicOrbit_Panel>();

        private int _updateCounter;

        private static BasicPanelManager _instance;

        public static BasicPanelManager Instance
        {
            get { return _instance; }
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
        }

        private void OnDestroy()
        {
            if (_instance == this)
                _instance = null;

            _activePanels.Clear();
        }

        public void RegisterPanel(BasicOrbit_Panel panel)
        {
            if (!_activePanels.Contains(panel))
                _activePanels.Add(panel);
        }

        public void UnregisterPanel(BasicOrbit_Panel panel)
        {
            if (_activePanels.Contains(panel))
                _activePanels.Remove(panel);
        }

        private void Update()
        {
            if (_activePanels == null || _activePanels.Count <= 0)
                return;

            _updateCounter++;

            if (_updateCounter >= _activePanels.Count)
                _updateCounter = 0;

            _activePanels[_updateCounter].OnUpdate();
        }
    }
}
