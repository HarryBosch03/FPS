using System.IO;

namespace Code.Scripts.Utility
{
    public class Util
    {
        public static string PrettifyName (string text)
        {
            text = text.Replace(c => c.IsCapital(), c => $" {c}").Replace(' ', '_', '.', '-').Trim();
            text = text[0].ToString().ToUpper() + text.Substring(1, text.Length - 1);
            return text;
        }

        public static bool IsChildDirectoryOf (string child, string parent)
        {
            child = Path.GetFullPath(child);
            parent = Path.GetFullPath(parent);

            if (parent.Length > child.Length) return false;

            for (int i = 0; i < parent.Length; i++)
            {
                if (child[i] != parent[i]) return false;
            }

            return true;
        }

        public static string SimplifyName(string text) => SimplifyName(ref text);
        public static string SimplifyName(ref string text) => string.IsNullOrEmpty(text) ? string.Empty : text.Trim().Replace(" ", "").ToLower();
        
        public static bool CompareNames(string a, string b)
        {
            a = SimplifyName(a);
            b = SimplifyName(b);
            
            if (string.IsNullOrWhiteSpace(a) && string.IsNullOrWhiteSpace(b)) return true;
            if (string.IsNullOrWhiteSpace(a) || string.IsNullOrWhiteSpace(b)) return false;
            
            return a == b;
        }
    }
}
