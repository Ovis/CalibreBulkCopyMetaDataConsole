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

        //コピー先カラム
        public string DestinationColumn => Configuration["DestinationColumn"];

    }
}
