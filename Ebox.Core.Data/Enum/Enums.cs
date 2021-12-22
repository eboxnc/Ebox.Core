
using System.ComponentModel;

namespace Ebox.Core.Data
{
    /// <summary>
    /// 启用标志。
    /// </summary>
    public enum StateFlags
    {
        /// <summary>
        /// 启用
        /// </summary>
        [Description("启用")]
        Enabled = 1,
        /// <summary>
        /// 禁用
        /// </summary>
        [Description("禁用")]
        Disabled = 0
    }

    /// <summary>
    /// 是否在线。
    /// </summary>
    public enum onLineState
    {
        /// <summary>
        /// 在线
        /// </summary>
        [Description("在线")]
        On = 1,
        /// <summary>
        /// 离线
        /// </summary>
        [Description("离线")]
        Off = 0
    }
}
