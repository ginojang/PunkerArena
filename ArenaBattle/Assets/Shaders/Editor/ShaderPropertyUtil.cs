using UnityEditor;
using UnityEngine;

public static class ShaderPropertyUtil
{
    public static bool[] ExistPropertyName(Shader shader, params string[] args)
    {
        var result = new bool[args.Length];
        var count = ShaderUtil.GetPropertyCount(shader);
        for (int i = 0; i < count; ++i)
        {
            var propertyName = ShaderUtil.GetPropertyName(shader, i);

            for (int j = 0; j < args.Length; ++j)
            {
                if (result[j] == false && propertyName == args[j])
                {
                    result[j] = true;
                    break;
                }
            }
        }

        return result;
    }
}