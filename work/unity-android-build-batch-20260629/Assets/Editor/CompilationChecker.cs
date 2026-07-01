#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Text;
using System.IO;

namespace CatLife.Editor
{
    /// <summary>
    /// 编译检查工具
    /// 菜单: CatLife/Check Compilation
    /// 命令行: Unity.exe -batchmode -projectPath "..." -executeMethod CatLife.Editor.CompilationChecker.Check
    /// </summary>
    public class CompilationChecker
    {
        private static readonly string ResultPath = Path.Combine(
            Application.temporaryCachePath, "compilation_result.txt");

        [MenuItem("CatLife/Check Compilation")]
        public static void Check()
        {
            RunCompilationCheck();
        }

        public static void RunCompilationCheck()
        {
            Debug.Log("[CompilationChecker] 开始检查...");

            StringBuilder report = new StringBuilder();
            int errorCount = 0;
            int warningCount = 0;

            // Editor.log 通常在 %LOCALAPPDATA%\Unity\Editor\
            string editorLog = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Unity", "Editor", "Editor.log");

            if (!File.Exists(editorLog))
            {
                report.AppendLine($"[ERROR] 未找到 Editor.log: {editorLog}");
                Debug.LogError($"[CompilationChecker] 未找到 Editor.log: {editorLog}");
                File.WriteAllText(ResultPath, report.ToString(), Encoding.UTF8);
                EditorApplication.Exit(1);
                return;
            }

            // 读 Editor.log 末尾 5000 行（用共享模式读取，兼容 Unity 正在写入的情况）
            string[] allLines;
            try
            {
                using (var fs = new FileStream(editorLog, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var sr = new StreamReader(fs, Encoding.UTF8))
                {
                    allLines = sr.ReadToEnd().Split('\n');
                }
            }
            catch (IOException)
            {
                report.AppendLine("[ERROR] 无法读取 Editor.log，可能被 Unity 占用");
                File.WriteAllText(ResultPath, report.ToString(), Encoding.UTF8);
                Debug.LogError("[CompilationChecker] 无法读取 Editor.log，可能被 Unity 占用");
                EditorApplication.Exit(1);
                return;
            }

            int startIdx = Math.Max(0, allLines.Length - 5000);

            for (int i = startIdx; i < allLines.Length; i++)
            {
                string line = allLines[i];

                if (line.Contains("error CS"))
                {
                    errorCount++;
                    string relPath = ExtractRelPath(line);
                    report.AppendLine($"[ERROR] {relPath}");
                }
                else if (line.Contains("warning CS"))
                {
                    warningCount++;
                    if (warningCount <= 30)
                    {
                        string relPath = ExtractRelPath(line);
                        report.AppendLine($"[WARN]  {relPath}");
                    }
                }
            }

            // 汇总
            report.AppendLine();
            report.AppendLine("=== 编译汇总 ===");
            report.AppendLine($"错误: {errorCount} 个");
            report.AppendLine($"警告: {warningCount} 个");
            report.AppendLine(errorCount == 0 ? "状态: 编译成功" : $"状态: 编译失败 ({errorCount} 个错误)");

            try
            {
                File.WriteAllText(ResultPath, report.ToString(), Encoding.UTF8);
                Debug.Log($"[CompilationChecker] 结果已写入: {ResultPath}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[CompilationChecker] 写入失败: {ex.Message}");
            }

            Debug.Log(report.ToString());
            EditorApplication.Exit(errorCount > 0 ? 1 : 0);
        }

        private static string ExtractRelPath(string line)
        {
            int idx = line.IndexOf("Assets");
            return idx >= 0 ? line.Substring(idx) : line;
        }
    }
}
#endif