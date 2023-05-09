using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelBusters.CoreLibrary
{
    public abstract class StringPopupAttribute : PropertyAttribute 
    {
        #region Static fields

        private     static      string[]        s_emptyOptions      = new string[0];

        #endregion

        #region Fields

        private     string[]    m_constantOptions;

        #endregion

        #region Properties

        public string PreferencePropertyName { get; private set; }

        public bool PreferencePropertyValue { get; private set; }

        public bool UsesConstantOptions { get; private set; }

        public string[] Options
        {
            get
            {
                var     options     = UsesConstantOptions ? m_constantOptions : GetDynamicOptions();
                return options ?? s_emptyOptions;
            }
        }

        #endregion

        #region Constructors

        public StringPopupAttribute(string preferencePropertyName = null, bool preferencePropertyValue = true, bool usesConstantOptions = true, params string[] constantOptions)
        {
            // set properties
            PreferencePropertyName  = preferencePropertyName;
            PreferencePropertyValue = preferencePropertyValue;
            UsesConstantOptions     = usesConstantOptions;
            m_constantOptions       = constantOptions;
        }

        #endregion

        #region Private methods

        protected virtual string[] GetDynamicOptions()
        {
            return null;
        }

        #endregion
    }
}