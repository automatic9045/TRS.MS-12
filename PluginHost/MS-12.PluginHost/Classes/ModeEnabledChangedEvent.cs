using System;

namespace TRS.TMS12.Interfaces
{
    /// <summary>
    /// 各モードの有効・無効が変更されたときに発生する <see cref="IPluginHost.ModeEnabledChanged"/> イベントを処理するメソッドを表します。
    /// </summary>
    /// <param name="e">イベントデータを格納している <see cref="ModeEnabledChangedEventArgs"/>。</param>
    public delegate void ModeEnabledChangedEventHandler(ModeEnabledChangedEventArgs e);

    /// <summary>
    /// <see cref="IPluginHost.ModeEnabledChanged"/> イベントのデータを提供します。
    /// </summary>
    public class ModeEnabledChangedEventArgs : EventArgs
    {
        /// <summary>
        /// 有効・無効が変更されたモードを取得します。
        /// </summary>
        public Mode TargetMode { get; }

        /// <summary>
        /// 対象のモードが有効になったかを取得します。
        /// </summary>
        public bool IsModeEnabled { get; }

        public ModeEnabledChangedEventArgs(Mode targetMode, bool isModeEnabled)
        {
            TargetMode = targetMode;
            IsModeEnabled = isModeEnabled;
        }
    }
}
