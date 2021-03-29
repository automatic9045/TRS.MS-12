using System;

namespace TRS.TMS12.Interfaces
{
    /// <summary>
    /// クラスに金額を定義します。
    /// </summary>
    public interface IChargeable
    {
        /// <summary>
        /// 金額を取得します。
        /// </summary>
        int Amount { get; }
    }
}
