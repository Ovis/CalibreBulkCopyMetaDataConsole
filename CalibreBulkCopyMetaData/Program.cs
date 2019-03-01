using CalibreSetMetaData.Domain;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using CalibreSetMetaData.Entity;
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
            //psInfo.Arguments = @"show_metadata --as-opf --library-path=" + setting.CalibreLibraryPath + " 4";
            psInfo.Arguments = @"list --library-path=" + setting.CalibreLibraryPath + " --fields " + setting.SourceColumn + ",*" + setting.DestinationColumn + " --for-machine";

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
                // 標準出力の読み取り
                output = p.StandardOutput.ReadToEnd();

                output = Regex.Unescape(output.Replace(@"*", "")); // カスタム列のアスタリスク削除

                if(p.ExitCode != 0)
                {
                    Console.WriteLine("Error! Calibreを実行中の可能性あり。");
                    return;
                }
            }

            var json = JsonConvert.DeserializeObject<List<CalibreList>>(output);

            //カラムに値の入っていないものを取得
            var targetData = json.Where(x => x.OriginalTitle == null).ToList();

            psInfo.StandardOutputEncoding = null;

            foreach (var target in targetData)
            {
                //Calibreに値をセットするために引数を設定
                psInfo.Arguments = @"set_metadata --field #" + setting.DestinationColumn + ":\"" + target.Title + "\"" +" --library-path=" + setting.CalibreLibraryPath + " " + target.Id;

                try
                {
                    // アプリの実行開始
                    using (Process p = Process.Start(psInfo))
                    {
                        // 標準出力の読み取り
                        output = p.StandardOutput.ReadToEnd();

                        if (p.ExitCode != 0)
                        {
                            Console.WriteLine("Error!");
                            return;
                        }
                    }
                }catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            Console.WriteLine("Finish.");
        }

    }
}
