using System;
using System.Collections;
using System.Collections.Generic;

namespace PureCat.Context
{
    /// <summary>
    /// Cat上下文
    /// </summary>
    public class CatContext
    {
        /// <summary>
        /// 消息根ID
        /// </summary>
        public string CatRootId { get; set; }
        /// <summary>
        /// 上级消息ID
        /// </summary>
        public string CatParentId { get; set; }
        /// <summary>
        /// 当前消息ID
        /// </summary>
        public string CatChildId { get; set; }
        /// <summary>
        /// 当前的上下文名称
        /// </summary>
        public string ContextName { get; set; }
        /// <summary>
        /// 初始化Cat上下文
        /// </summary>
        /// <param name="contextName"></param>
        public CatContext(string contextName)
        {
            ContextName = contextName ?? Environment.MachineName;
        }

        public CatContext()
            : this(null)
        { }

    }
}
