using System;

namespace TRS.TMS12.Interfaces
{
    /// <summary>
    /// 操作種別が変更されたときに発生する <see cref="IPluginHost.SendTypeChanged"/> イベントを処理するメソッドを表します。
    /// </summary>
    /// <param name="e">イベントデータを格納している <see cref="SendTypeChangedEventArgs"/>。</param>
    public delegate void SendTypeChangedEventHandler(SendTypeChangedEventArgs e);

    /// <summary>
    /// <see cref="IPluginHost.SendTypeChanged"/> イベントのデータを提供します。
    /// </summary>
    public class SendTypeChangedEventArgs : EventArgs
    {
        /// <summary>
        /// 変更後の <see cref="SendTypes"/> を取得します。
        /// </summary>
        public SendTypes? SendType { get; }

        public SendTypeChangedEventArgs(SendTypes? newSendType)
        {
            SendType = newSendType;
        }
    }
}
