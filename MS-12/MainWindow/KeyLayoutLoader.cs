using System;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

using TRS.TMS12.Resources;

namespace TRS.TMS12
{
    internal static class KeyLayoutLoader
    {
        internal static KeyTab LoadFromXmlFile(string path, int keyCount, Action<string, string> error, Action<string, string> warning, Action<string, string> information)
        {
            XDocument xDocument;
            try
            {
                xDocument = XDocument.Load(path);
            }
            catch
            {
                error.Invoke("ファイル \"" + path + "\" が見つかりませんでした。", "キーレイアウト設定に関するエラー");
                return new KeyTab("（名前無し）", new List<KeyInfo>());
            }

            XElement layout = xDocument.Element("KeyLayout");
            if (layout == null)
            {
                error.Invoke("ファイル \"" + path + "\" はキーレイアウトファイルではありません。\n" +
                    "KeyLayout タグが見つかりません。", "キーレイアウト設定に関するエラー");
                return new KeyTab("（名前無し）", new List<KeyInfo>());
            }

            string header = (string)layout.Attribute("Header") ?? "";
            if (header == "")
            {
                header = "（名前無し）";
                warning.Invoke("ファイル \"" + path + "\" にヘッダーが設定されていないか、空白になっています。", "キーレイアウト設定に関するエラー");
            }

            List <KeyInfo> keyList = new List<KeyInfo>();

            IEnumerable<XElement> keys = layout.Elements("Key");
            foreach (XElement key in keys)
            {
                keyList.Add(new KeyInfo()
                {
                    KeyName = (string)key.Attribute("Name") ?? "",
                    Command = (string)key.Attribute("Command") ?? "",
                });
            }
            while (keyList.Count < keyCount)
            {
                keyList.Add(new KeyInfo()
                {
                    KeyName = "",
                    Command = null,
                });
            }

            return new KeyTab(header, keyList);
        }
    }
}
