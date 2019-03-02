using CalibreSetMetaData.Domain;
using Codeplex.Data;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
            if (!ExecCalibreCommand(psInfo))
            {
                return;
            }

            var jsonData = DynamicJson.Parse(output);

            psInfo.StandardOutputEncoding = null;

            var includeTbl = new HashSet<string>();
            if (setting.CopyWhenIncludedFlg)
            {
                foreach (var item in jsonData)
                {
                    //対象データが存在するもののみ処理を行う
                    if (item.IsDefined("id") & item.IsDefined(setting.DestinationColumn))
                    {
                        includeTbl.Add(item[setting.DestinationColumn]);
                    }
                }
            }

            foreach (var item in jsonData)
            {
                //対象データが存在するもののみ処理を行う
                if (item.IsDefined("id") & item.IsDefined(setting.SourceColumn))
                {
                    if (setting.ConvertHalfWidth)
                    {
                        var sourceData = ConvHalfWidth((string)item[setting.SourceColumn]);

                        if(sourceData != (string)item[setting.SourceColumn])
                        {
                            //Calibreに値をセットするために引数を設定
                            psInfo.Arguments = @"set_metadata --field " + (setting.DestClmCustomClmFlg ? "#" : "")
                                + setting.DestinationColumn + ":\"" + sourceData + "\"" + " --library-path="
                                + setting.CalibreLibraryPath + " " + item["id"];

                            ExecCalibreCommand(psInfo);
                        }
                    }
                    else if (setting.ReplaceColumnDataFlg)
                    {
                        var sourceData = (string)item[setting.SourceColumn];

                        if (!setting.ReplaceColumnData.Any(sourceData.Contains))
                        {
                            continue;
                        }

                        sourceData = setting.ReplaceColumnData.Aggregate(
                            sourceData, (s, c) => s.Replace(c.ToString(), ""));
                        
                    }
                    else if (setting.CopyWhenIncludedFlg)
                    {
                        //コピー先カラムに値がある場合は処理対象外
                        if (!item.IsDefined(setting.DestinationColumn))
                        {
                            var sourceData = (string)item[setting.SourceColumn];

                            if (!includeTbl.Any(sourceData.Contains))
                            {
                                continue;
                            }

                            var setData = includeTbl.Where(p => sourceData.Contains(p)).First();

                            //Calibreに値をセットするために引数を設定
                            psInfo.Arguments = @"set_metadata --field " + (setting.DestClmCustomClmFlg ? "#" : "")
                                + setting.DestinationColumn + ":\"" + setData + "\"" + " --library-path="
                                + setting.CalibreLibraryPath + " " + item["id"];

                            ExecCalibreCommand(psInfo);
                        }
                    }
                    else
                    {
                        //コピー先カラムに値がある場合は処理対象外
                        if (!item.IsDefined(setting.DestinationColumn))
                        {
                            //Calibreに値をセットするために引数を設定
                            psInfo.Arguments = @"set_metadata --field " + (setting.DestClmCustomClmFlg ? "#" : "")
                                + setting.DestinationColumn + ":\"" + item[setting.SourceColumn] + "\"" + " --library-path="
                                + setting.CalibreLibraryPath + " " + item["id"];

                            ExecCalibreCommand(psInfo);
                        }
                    }
                }
            }

            Console.WriteLine("Finish.");
            ConsoleWait();
        }

        /// <summary>
        /// Calibre実行
        /// </summary>
        static bool ExecCalibreCommand(ProcessStartInfo psInfo, string replaceStr = null)
        {
            var output = "";
            try
            {
                // アプリの実行開始
                using (Process p = Process.Start(psInfo))
                {
                    output = p.StandardOutput.ReadToEnd();

                    if (replaceStr != null)
                    {
                        output = Regex.Unescape(output.Replace(replaceStr, ""));
                    }

                    if (p.ExitCode != 0)
                    {
                        Console.WriteLine("Error! Calibreを実行中の可能性あり。");
                        ConsoleWait();
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ConsoleWait();
            }
            return true;
        }

        /// <summary>
        /// 半角変換
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        static string ConvHalfWidth(string str)
        {
            Regex re = new Regex("[０-９Ａ-Ｚａ-ｚ：！？（）－　]+");
            string output = re.Replace(str, HalfWidthReplacer);

            return output;
        }

        /// <summary>
        /// 半角変換マッチ
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        static string HalfWidthReplacer(Match m)
        {
            return Strings.StrConv(m.Value, VbStrConv.Narrow, 0);
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
