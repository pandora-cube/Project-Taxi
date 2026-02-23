using System.Text;

namespace Util
{
    public static class StringUtil
    {
        /// <summary>
        /// 언더바로 구분된 카멜 케이스를 일반적 카멜 케이스로 변환합니다.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToCleanCamelCase(this string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            StringBuilder sb = new StringBuilder();
            // 언더바를 기준으로 단어를 나눕니다.
            string[] words = input.Split('_');

            foreach (string word in words)
            {
                if (word.Length <= 0) continue;

                // 첫 글자는 대문자로, 나머지는 소문자로 변환하여 합칩니다.
                sb.Append(char.ToUpper(word[0]));
                if (word.Length > 1)
                {
                    sb.Append(word.Substring(1).ToLower());
                }
            }

            return sb.ToString();
        }
    }
}