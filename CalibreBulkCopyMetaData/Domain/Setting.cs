using System;
using System.Collections.Specialized;
using System.Configuration;

namespace CalibreSetMetaData.Domain
{
    class Setting
{
        private NameValueCollection Configuration = ConfigurationManager.AppSettings;

        public Setting()
        {
        }

        //Calibreインストールパス
        public string CalibrePath => Configuration["CalibrePath"];

        //Calibreライブラリパス
        public string CalibreLibraryPath => Configuration["CalibreLibraryPath"];

        //コピー元カラム
        public string SourceColumn => Configuration["SourceColumn"];

        //コピー元カラムがカスタムカラムか否か
        public bool SrcClmCustomClmFlg => bool.Parse(Configuration["SrcClmCustomClmFlg"]);

        //コピー先カラム
        public string DestinationColumn => Configuration["DestinationColumn"];

        //コピー先カラムがカスタムカラムか否か
        public bool DestClmCustomClmFlg => bool.Parse(Configuration["DestClmCustomClmFlg"]);

        //部分一致時該当値コピー処理実施
        public bool CopyWhenIncludedFlg => bool.Parse(Configuration["CopyWhenIncludedFlg"]);

        //除去処理フラグ
        public bool ReplaceColumnDataFlg => bool.Parse(Configuration["ReplaceColumnDataFlg"]);

        //除去対象
        public string[] ReplaceColumnData => Configuration["ReplaceColumnData"].Split(',');

        //数字、記号、アルファベット半角変換
        public bool ConvertHalfWidth => bool.Parse(Configuration["ConvertHalfWidth"]);
    }
}
