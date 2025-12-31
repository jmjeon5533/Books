using System;
using System.Data;

namespace Script
{
    using System.Reflection;

    public static class Utility
    {
        public static int FindAliasIdx(this DataRow rootRow, string alias, int columnCount)
        {
            for (int i = 0; i < columnCount; i++)
            {
                if (rootRow[i].ToString().ToLower() != alias.ToLower())
                    continue;
                return i;
            }
            return -1;
        }
    }
}