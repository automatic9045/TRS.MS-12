# TRS.MS-12
![メインメニュー](https://user-images.githubusercontent.com/67314487/111030751-58113800-8447-11eb-9fb2-451313fd5809.png)

【開発中】駅の窓口で見かける座席予約端末をモチーフにした、POS向け汎用端末です。

ダウンロード：https://github.com/Automatic9045/TRS.MS-12/releases
サイト内の紹介ページ：https://Automatic9045.github.io/contents/software/software.html

## 実行に必要なもの
- .NET Framework 4.8
- [Microsoft Point of Service for .NET（POS for .NET）](https://www.microsoft.com/en-us/download/details.aspx?id=55758)

## 注意

実行する際は、以下のDLLファイルについてプロパティからブロックの解除を行って下さい。  

- Plugins\Basic\OneTimePrinting\OneTimePrinting.dll
- Plugins\TRS\NumberedTickets.NumberedTicket\NumberedTicket.dll
- Plugins\TRS\TRSConnector.dll
- Plugins\TRS\TRSTicket.dll
- Printers\Printer.dll
- Printers\PrinterPlugin.PrinterEmulator.dll
- Printers\TRSConnector.dll
- Printers\TRSTicket.dll

解除されていないDLLはソフトウェアから動的に読み込めません。

![ブロックの解除方法](https://user-images.githubusercontent.com/67314487/111025068-5898d700-8425-11eb-9a1b-e9053d2cd63a.png)
