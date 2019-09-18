using CalibreBulkCopyMetaData.Domain;
using CalibreBulkCopyMetaData.Extensions;
using CalibreSetMetaData.Domain;
using Codeplex.Data;
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

            // 実行するファイル
            psInfo.FileName = calibreDbPath;

            //対象カラム指定
            var argColumn = (setting.ConvertHalfWidth ? GetColumnName(setting.ConvertHalfWidthSource, true) + "," : "")
                + (setting.DataCopy ? GetColumnName(setting.DataCopySourceColumn, true) + "," : "")
                + (setting.DataCopy ? GetColumnName(setting.DataCopyDestination, true) + "," : "")
                + (setting.ReplaceColumnDataFlg ? GetColumnName(setting.ReplaceColumn, true) + "," : "")
                + (setting.ReplaceColumnDataFlg ? GetColumnName(setting.ReplaceDestinationColumn, true) + "," : "")
                + (setting.CopyWhenIncludedFlg ? GetColumnName(setting.CopyWhenIncludedSource, true) + "," : "")
                + (setting.CopyWhenIncludedFlg ? GetColumnName(setting.CopyWhenIncludedDestination, true) : "");

            //引数
            psInfo.Arguments = @"list --library-path=" + setting.CalibreLibraryPath + " --fields "
                + argColumn.TrimEnd(',')
                + " --for-machine";

            // コンソール・ウィンドウを開かない
            psInfo.CreateNoWindow = true;
            // シェル機能を使用しない
            psInfo.UseShellExecute = false;

            // 標準出力をリダイレクト
            psInfo.RedirectStandardOutput = true;

            //エンコーディング設定
            psInfo.StandardOutputEncoding = Encoding.UTF8;

            psInfo.StandardOutputEncoding = null;

            // アプリの実行開始
            (bool ret, string output) = ExecCalibreCommand(psInfo, "*");
            if (!ret)
            {
                return;
            }

            var jsonData = DynamicJson.Parse(output);


            var includeTbl = new HashSet<string>();
            var replaceTbl = new HashSet<string>();

            if (setting.CopyWhenIncludedFlg || (setting.ReplaceColumnDataFlg && !setting.IsTextReplace))
            {
                //カラム内の値を登録
                foreach (var item in jsonData)
                {
                    if (setting.CopyWhenIncludedFlg)
                    {
                        //対象データが存在するもののみ処理を行う
                        if (item.IsDefined("id") & item.IsDefined(setting.CopyWhenIncludedDestination))
                        {
                            includeTbl.Add(item[setting.CopyWhenIncludedDestination]);
                        }
                    }
                    if (setting.ReplaceColumnDataFlg && !setting.IsTextReplace)
                    {
                        //対象データが存在するもののみ処理を行う
                        if (item.IsDefined("id") & item.IsDefined(setting.ReplaceDestinationColumn))
                        {
                            replaceTbl.Add(setting.ReplaceStartWith + item[setting.ReplaceDestinationColumn] + setting.ReplaceEndWith);
                        }
                    }
                }
            }


            foreach (var item in jsonData)
            {
                if (!item.IsDefined("id"))
                {
                    continue;
                }

                //別カラム丸ごとコピー
                if (setting.DataCopy)
                {
                    //コピー先カラムに値がある場合は処理対象外
                    if (!item.IsDefined(setting.DataCopyDestination))
                    {
                        Console.WriteLine("別カラム丸ごとコピー実施");
                        Console.WriteLine("文字列：" + item[setting.DataCopySourceColumn]);
                        //Calibreに値をセットするために引数を設定
                        psInfo.Arguments = @"set_metadata --field " + GetColumnName(setting.DataCopyDestination)
                            + ":\"" + item[setting.DataCopySourceColumn] + "\"" + " --library-path="
                            + setting.CalibreLibraryPath + " " + item["id"];

                        ExecCalibreCommand(psInfo);

                        Console.WriteLine("--------------------------------------------------------");
                    }
                }

                //半角変換
                if (setting.ConvertHalfWidth && item.IsDefined(setting.ConvertHalfWidthSource))
                {
                    var convertString = StringExtensions.ConvHalfWidth((string)item[setting.ConvertHalfWidthSource]);

                    //半角変換した値が今登録されている値と異なる場合はセットする
                    if (convertString != (string)item[setting.ConvertHalfWidthSource])
                    {
                        Console.WriteLine("半角変換実施");
                        Console.WriteLine("変換前文字列：" + (string)item[setting.ConvertHalfWidthSource]);
                        Console.WriteLine("変換後文字列：" + convertString);

                        //Calibreに値をセットするために引数を設定
                        psInfo.Arguments = @"set_metadata --field " + GetColumnName(setting.ConvertHalfWidthSource)
                            + ":\"" + convertString + "\""
                            + " --library-path=" + setting.CalibreLibraryPath + " " + item["id"];

                        ExecCalibreCommand(psInfo);

                        Console.WriteLine("--------------------------------------------------------");
                    }
                }

                //カラムデータリプレース
                if (setting.ReplaceColumnDataFlg && item.IsDefined(setting.ReplaceColumn))
                {
                    var sourceData = (string)item[setting.ReplaceColumn];

                    if (setting.IsTextReplace)
                    {
                        if (setting.ReplaceColumnData.Any(sourceData.Contains))
                        {
                            sourceData = setting.ReplaceColumnData.Aggregate(
                              sourceData, (s, c) => s.Replace(c.ToString(), ""));

                            Console.WriteLine("カラムデータリプレース実施");
                            Console.WriteLine("変換前文字列：" + (string)item[setting.ReplaceColumn]);
                            Console.WriteLine("変換後文字列：" + sourceData);

                            //Calibreに値をセットするために引数を設定
                            psInfo.Arguments = @"set_metadata --field " + GetColumnName(setting.ReplaceColumn)
                                + ":\"" + sourceData + "\"" + " --library-path="
                                + setting.CalibreLibraryPath + " " + item["id"];

                            ExecCalibreCommand(psInfo);

                            Console.WriteLine("--------------------------------------------------------");
                        }
                    }
                    else
                    {
                        if (replaceTbl.Any(sourceData.Contains))
                        {
                            sourceData = replaceTbl.Aggregate(
                              sourceData, (s, c) => s.Replace(c.ToString(), "")).Trim();

                            Console.WriteLine("カラムデータリプレース実施");
                            Console.WriteLine("変換前文字列：" + (string)item[setting.ReplaceColumn]);
                            Console.WriteLine("変換後文字列：" + sourceData);

                            //Calibreに値をセットするために引数を設定
                            psInfo.Arguments = @"set_metadata --field " + GetColumnName(setting.ReplaceColumn)
                                + ":\"" + sourceData + "\"" + " --library-path="
                                + setting.CalibreLibraryPath + " " + item["id"];

                            ExecCalibreCommand(psInfo);

                            Console.WriteLine("--------------------------------------------------------");
                        }
                    }


                }

                //既存文字列を含む場合、対象カラムにその文字列を登録
                if (setting.CopyWhenIncludedFlg)
                {
                    //コピー先カラムに値がある場合は処理対象外
                    if (!item.IsDefined(setting.CopyWhenIncludedDestination))
                    {
                        var sourceData = (string)item[setting.CopyWhenIncludedSource];

                        if (!includeTbl.Any(sourceData.Contains))
                        {
                            continue;
                        }

                        var setData = includeTbl.OrderByDescending(o => o.Length).Where(p => sourceData.Contains(p)).First();

                        Console.WriteLine("既存文字列セット実施");
                        Console.WriteLine("元の文字列：" + (string)item[setting.CopyWhenIncludedSource]);
                        Console.WriteLine("対象文字列：" + setData);

                        //Calibreに値をセットするために引数を設定
                        psInfo.Arguments = @"set_metadata --field " + GetColumnName(setting.CopyWhenIncludedDestination)
                            + ":\"" + setData + "\"" + " --library-path="
                            + setting.CalibreLibraryPath + " " + item["id"];

                        ExecCalibreCommand(psInfo);

                        Console.WriteLine("--------------------------------------------------------");
                    }
                }
            }

            Console.WriteLine("Finish.");
            ConsoleWait();
        }

        /// <summary>
        /// カスタムカラムか否かの判定
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        static bool IsCustomColumn(string columnName)
        {
            if (Enum.TryParse<CalibreColumnNameEnum>(columnName, out _))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 引数に渡すカラム名を取得
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        static string GetColumnName(string columnName, bool init = false)
        {
            if (IsCustomColumn(columnName))
            {
                return (init ? "*" : "#") + columnName;
            }
            {
                return columnName;
            }
        }

        /// <summary>
        /// Calibre実行
        /// </summary>
        static (bool ret, string output) ExecCalibreCommand(ProcessStartInfo psInfo, string replaceStr = null)
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

                    if (output.Contains("CantOpenError: unable to open database file"))
                    {
                        ExecCalibreCommand(psInfo);
                    }
                    else if (p.ExitCode != 0)
                    {
                        Console.WriteLine("Error! Calibreを実行中の可能性あり。");
                        Console.WriteLine(output);
                        ConsoleWait();
                        return (false, "");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ConsoleWait();
                return (false, "");
            }
            return (true, output);
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
