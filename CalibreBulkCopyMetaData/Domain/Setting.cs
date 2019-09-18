using Microsoft.Extensions.Configuration;
using System;

namespace CalibreSetMetaData.Domain
{
    class Setting
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("App.config.json", optional: true)
            .Build();

        public Setting()
        {
        }

        //Calibreインストールパス
        public string CalibrePath => configuration["Setting:Calibre:Path"];

        //Calibreライブラリパス
        public string CalibreLibraryPath => configuration["Setting:Calibre:LibraryPath"];



        //数字、記号、アルファベット半角変換
        public bool ConvertHalfWidth => bool.Parse(configuration["Setting:ConvertHalfWidth:Enabled"]);

        //対象カラム
        public string ConvertHalfWidthSource => configuration["Setting:ConvertHalfWidth:Source"];

        //カラムデータコピー
        public bool DataCopy => bool.Parse(configuration["Setting:DataCopy:Enabled"]);

        //データコピー対象カラム
        public string DataCopySourceColumn => configuration["Setting:DataCopy:Source"];

        //データコピー先カラム
        public string DataCopyDestination => configuration["Setting:DataCopy:Destination"];



        //リプレース処理
        //除去処理フラグ
        public bool ReplaceColumnDataFlg => bool.Parse(configuration["Setting:Replace:Enabled"]);

        //リプレース対象カラム
        public string ReplaceColumn => configuration["Setting:Replace:Source"];

        //テキストリプレースかカラム内容リプレースか
        public bool IsTextReplace => bool.Parse(configuration["Setting:Replace:IsTextReplace"]);

        //リプレース対象の値が入ったカラム名
        public string ReplaceDestinationColumn => configuration["Setting:Replace:Destination"];

        //(カラム内容の場合のみ)開始文字列
        public string ReplaceStartWith => configuration["Setting:Replace:StartWith"];

        //(カラム内容の場合のみ)終了文字列文字列
        public string ReplaceEndWith => configuration["Setting:Replace:EndWith"];

        //リプレース対象テキスト
        public string[] ReplaceColumnData => configuration["Setting:Replace:ReplaceText"].Split(',');


        //一部文字列コピー
        //有効無効
        public bool CopyWhenIncludedFlg => bool.Parse(configuration["Setting:CopyWhenIncluded:Enabled"]);

        //対象カラム
        public string CopyWhenIncludedSource => configuration["Setting:CopyWhenIncluded:Source"];

        //データコピー先カラム(既存データカラムを兼ねる)
        public string CopyWhenIncludedDestination => configuration["Setting:CopyWhenIncluded:Destination"];

    }
}
