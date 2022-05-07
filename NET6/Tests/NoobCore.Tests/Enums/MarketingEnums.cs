using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoobCore.Tests.Enums
{
    [Flags] //指示可将枚举视为位域（即一组标志）。
    /// <summary>
    /// 卡券使用场景(1:线上,2:线下,3:[1|2:线上与线下])
    /// </summary>
    public enum TicketUseScope
    {
        /// <summary>
        /// 无(兼容之前)
        /// </summary>
        [Description("无")]
        None = 0,
        /// <summary>
        /// 仅线上可用
        /// </summary>
        [Description("仅线上可用")]
        Online = 1,//Math.Pow(2, 0)        
        /// <summary>
        /// 仅线下可用
        /// </summary>
        [Description("仅线下可用")]
        Offline = 2,// Math.Pow(2, 1)        
        /// <summary>
        /// 无限制
        /// </summary>
        [Description("无限制")]
        Limitless = Online | Offline
    }

    /// <summary>
    /// 赠品类型(1:优惠券,2:会员余额,4:会员积分)
    /// </summary>
    [Flags] //指示可将枚举视为位域（即一组标志）。
    public enum GiftType
    {
        /// <summary>
        /// 无
        /// </summary>
        [Description("无")]
        None = 0,
        /// <summary>
        /// 优惠券
        /// </summary>
        [Description("优惠券")]
        Ticket = 1 << 0, //Math.Pow(2,0)       
        /// <summary>
        /// 会员余额
        /// </summary>
        [Description("会员余额")]
        Balance = 1 << 1, //Math.Pow(2,1)        
        /// <summary>
        /// 会员积分
        /// </summary>
        Integral = 1 << 2, //Math.Pow(2,2)
    }
}
