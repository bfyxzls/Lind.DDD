// Microsoft Developer & Platform Evangelism
//=================================================================================== 
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
//===================================================================================
// Copyright (c) Microsoft Corporation.  All Rights Reserved.
// This code is released under the terms of the MS-LPL license, 
// http://microsoftnlayerapp.codeplex.com/license
//===================================================================================

using Lind.DDD.IoC.Implements;
using System;
using System.Collections.Generic;


namespace Lind.DDD.IoC
{
    /// <summary>
    /// IoCFactory  implementation 
    /// </summary>
    public sealed class IoCFactory
    {
        #region Singleton
        static IoCFactory instance;
        static object lockObj = new object();
        /// <summary>
        /// Get singleton instance of IoCFactory
        /// </summary>
        public static IoCFactory Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObj)
                    {
                        if (instance == null)
                        {
                            instance = new IoCFactory();
                        }
                    }
                }
                return instance;
            }
        }
        #endregion

        #region Members

        IContainer _CurrentContainer;

        /// <summary>
        /// Get current configured IContainer
        /// </summary>
        public IContainer CurrentContainer
        {
            get
            {
                return _CurrentContainer;
            }
        }

        #endregion

        #region Constructor

        private IoCFactory()
        {
            switch (ConfigConstants.ConfigManager.Config.IocContaion.IoCType)
            {
                case 0:
                    _CurrentContainer = new UnityAdapterContainer();
                    break;
                case 1:
                    _CurrentContainer = new AutofacAdapterContainer();
                    break;
                default:
                    throw new ArgumentException("不支持此IoC类型");
            }
        }
        #endregion
    }
}
