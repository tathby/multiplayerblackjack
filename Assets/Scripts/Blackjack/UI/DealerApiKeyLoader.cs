using System;
using System.IO;
using UnityEngine;

public static class DealerApiKeyLoader
{
    private const string HuggingFaceTokenVariable = "HUGGINGFACE_API_KEY";
    private const string ShortHuggingFaceTokenVariable = "HF_API_KEY";
    private const string OfficialHuggingFaceTokenVariable = "HF_TOKEN";

    public static bool TryLoad(string filePath, out string apiKey, out string source)
    {
        apiKey = string.Empty;
        source = string.Empty;

        if (TryLoadFromEnvironment(OfficialHuggingFaceTokenVariable, out apiKey, out source) ||
            TryLoadFromEnvironment(HuggingFaceTokenVariable, out apiKey, out source) ||
            TryLoadFromEnvironment(ShortHuggingFaceTokenVariable, out apiKey, out source))
        {
            return true;
        }

        string resolvedPath = ResolveProjectPath(filePath);
        source = resolvedPath;
        if (string.IsNullOrWhiteSpace(resolvedPath) || !File.Exists(resolvedPath))
        {
            return false;
        }

        apiKey = File.ReadAllText(resolvedPath).Trim();
        return !string.IsNullOrWhiteSpace(apiKey);
    }

    public static string ResolveProjectPath(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            return string.Empty;
        }

        if (Path.IsPathRooted(filePath))
        {
            return filePath;
        }

        string projectRoot = Directory.GetParent(Application.dataPath)?.FullName;
        return string.IsNullOrWhiteSpace(projectRoot) ? filePath : Path.Combine(projectRoot, filePath);
    }

    private static bool TryLoadFromEnvironment(string variableName, out string apiKey, out string source)
    {
        apiKey = Environment.GetEnvironmentVariable(variableName);
        source = variableName;
        return !string.IsNullOrWhiteSpace(apiKey);
    }
}
