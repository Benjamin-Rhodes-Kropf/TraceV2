using System;
using System.Collections;
using System.Collections.Generic;
using SA.Foundation.Tests;
using UnityEngine;


namespace SA.Foundation.Tests
{
    public class SA_TestsManager : MonoBehaviour
    {
        [SerializeField] SA_TestSuiteConfig m_config = null;
        private readonly List<SA_TestGroupConfig> m_testGroups = new List<SA_TestGroupConfig>();

        public SA_TestSuiteConfig Config {
            get {
                return m_config;
            }
        }

        private void Start() {
           foreach(var group in m_config.TestGroups) {
                if(group.Enabled) {
                    m_testGroups.Add(group);
                }     
           }

        }
    }
}