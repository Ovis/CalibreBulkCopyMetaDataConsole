using Microsoft.VisualBasic;
using System.Text.RegularExpressions;

namespace CalibreBulkCopyMetaData.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// 半角変換
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ConvHalfWidth(string str)
        {
            Regex re = new Regex("[０-９Ａ-Ｚａ-ｚ：！？’（）－　]+");
            string output = re.Replace(str, HalfWidthReplacer);

            return output;
        }

        /// <summary>
        /// 半角変換マッチ
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static string HalfWidthReplacer(Match m)
        {
            return Strings.StrConv(m.Value, VbStrConv.Narrow, 0);
        }
    }
}
