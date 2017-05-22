﻿namespace LivestreamerTwitchViewer.Exceptions
{
    #region using directives
    using System;
    #endregion
    /// <summary>Exception representing a provided scope was not permitted.</summary>
    public class BadScopeException : Exception
    {
        /// <summary>Exception constructor</summary>
        public BadScopeException(string data)
            : base(data)
        {
        }
    }
}