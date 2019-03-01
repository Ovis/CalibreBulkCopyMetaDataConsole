using CalibreSetMetaData.Domain;
using Codeplex.Data;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace CalibreSetMetaData
{
    class Program
    {
        static void Main(string[] args)
        {
            var setting = new Setting();

            string calibreDbPath = Path.Combine(setting.CalibrePath, "calibredb.exe");

            ProcessStartInfo psInfo = new ProcessStartInfo();

            string output = "";

            // 実行するファイル
            psInfo.FileName = calibreDbPath;

            //引数
            psInfo.Arguments = @"list --library-path=" + setting.CalibreLibraryPath + " --fields "
                + (setting.SrcClmCustomClmFlg ? "*" : "") + setting.SourceColumn + ","
                + (setting.DestClmCustomClmFlg ? "*" : "") + setting.DestinationColumn + " --for-machine";

            // コンソール・ウィンドウを開かない
            psInfo.CreateNoWindow = true;
            // シェル機能を使用しない
            psInfo.UseShellExecute = false;

            // 標準出力をリダイレクト
            psInfo.RedirectStandardOutput = true;

            //エンコーディング設定
            psInfo.StandardOutputEncoding = Encoding.UTF8;

            // アプリの実行開始
            using (Process p = Process.Start(psInfo))
            {
                output = p.StandardOutput.ReadToEnd();

                output = Regex.Unescape(output.Replace(@"*", "")); // カスタム列のアスタリスク削除

                if (p.ExitCode != 0)
                {
                    Console.WriteLine("Error! Calibreを実行中の可能性あり。");
                    ConsoleWait();
                    return;
                }
            }

            var jsonData = DynamicJson.Parse(output);

            psInfo.StandardOutputEncoding = null;

            foreach (var item in jsonData)
            {
                //対象データが存在するもののみ処理を行う
                if (item.IsDefined("id") & item.IsDefined(setting.SourceColumn))
                {
                    //コピー先カラムに値がある場合は処理対象外1
                    if (!item.IsDefined(setting.DestinationColumn))
                    {
                        //Calibreに値をセットするために引数を設定
                        psInfo.Arguments = @"set_metadata --field " + (setting.DestClmCustomClmFlg ? "#" : "")
                            + setting.DestinationColumn + ":\"" + item[setting.SourceColumn] + "\"" + " --library-path="
                            + setting.CalibreLibraryPath + " " + item["id"];

                        try
                        {
                            // アプリの実行開始
                            using (Process p = Process.Start(psInfo))
                            {
                                output = p.StandardOutput.ReadToEnd();

                                if (p.ExitCode != 0)
                                {
                                    Console.WriteLine("Error!");
                                    ConsoleWait();
                                    return;
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            ConsoleWait();
                        }
                    }
                }
            }

            Console.WriteLine("Finish.");
            ConsoleWait();

        }

        /// <summary>
        /// コンソール用待機処理
        /// </summary>
        public static void ConsoleWait()
        {

            Console.WriteLine("続行するには何かキーを押してください．．．");
            Console.ReadKey();
        }
    }
}
