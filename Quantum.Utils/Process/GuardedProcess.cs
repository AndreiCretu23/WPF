using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Quantum.Utils
{
    /// <summary>
    /// A wrapper class over a Process. It has internal handling of various exceptions that processes can throw and
    /// a deadlock guarded StandardOutput/StandardError reading mechanism. For an example of how to use it, check
    /// IMX6ShaderCompiler/ApolloLakeShaderCompiler/RCarD3ShaderCompiler in cgi_studio_scenecomposer_extensions.
    /// </summary>
    public class GuardedProcess
    {
        public string FileName { get; set; }
        public string Arguments { get; set; }
        public bool UseShellExecute { get; set; }
        public bool CreateNoWindow { get; set; }
        public bool RedirectStandardOutput { get; set; }
        public string WorkingDirectory { get; set; }
        public int ExecutableTimeout { get; set; } = 30000;


        public delegate void ProcessOutputHandler(StringBuilder standardOutput, StringBuilder standardError);
        public delegate void Win32ExceptionHandler(Win32Exception win32e);


        public Win32ExceptionHandler OnProcessStartWin32Exception { get; set; }
        public ProcessOutputHandler ProcessOutput { get; set; }
        public Action ProcessExit { get; set; }
        public Action OnProcessTimeout { get; set; }

        public void Execute()
        {
            using (Process process = new Process())
            {
                process.StartInfo.FileName = FileName;
                process.StartInfo.Arguments = Arguments;
                process.StartInfo.UseShellExecute = UseShellExecute;
                process.StartInfo.CreateNoWindow = CreateNoWindow;
                process.StartInfo.RedirectStandardError = RedirectStandardOutput;
                process.StartInfo.RedirectStandardOutput = RedirectStandardOutput;
                if (WorkingDirectory != null)
                {
                    process.StartInfo.WorkingDirectory = WorkingDirectory;
                }

                if (!RedirectStandardOutput)
                {
                    ExecuteImpl_NoOutput(process);
                }

                else
                {
                    ExecuteImpl_ReadOutput(process);
                }
            }
        }

        private void ExecuteImpl_NoOutput(Process process)
        {

            try
            {
                process.Start();
            }
            catch (Win32Exception win32e)
            {
                OnProcessStartWin32Exception?.Invoke(win32e);
                return;
            }
            catch (Exception)
            {
                return;
            }

            if (process.WaitForExit(ExecutableTimeout))
            {
                ProcessExit?.Invoke();
            }
            else
            {
                try
                {
                    process.Kill();
                    OnProcessTimeout?.Invoke();
                }
                catch (Win32Exception) { }
                catch (NotSupportedException) { }
                catch (InvalidOperationException) { }
            }
        }

        private void ExecuteImpl_ReadOutput(Process process)
        {
            StringBuilder output = new StringBuilder();
            StringBuilder error = new StringBuilder();

            using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
            using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
            {
                process.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data == null)
                    {
                        outputWaitHandle.Set();
                    }
                    else
                    {
                        output.AppendLine(e.Data);
                    }
                };

                process.ErrorDataReceived += (sender, e) =>
                {
                    if (e.Data == null)
                    {
                        errorWaitHandle.Set();
                    }
                    else
                    {
                        error.AppendLine(e.Data);
                    }
                };

                try
                {
                    process.Start();
                }
                catch (Win32Exception win32e)
                {
                    OnProcessStartWin32Exception?.Invoke(win32e);
                    return;
                }
                catch (Exception)
                {
                    return;
                }

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                if (process.WaitForExit(ExecutableTimeout) &&
                      outputWaitHandle.WaitOne() &&
                      errorWaitHandle.WaitOne())
                {
                    ProcessOutput?.Invoke(output, error);
                    ProcessExit?.Invoke();
                }
                else
                {
                    try
                    {
                        process.Kill();
                        OnProcessTimeout?.Invoke();
                    }
                    catch (Win32Exception) { }
                    catch (NotSupportedException) { }
                    catch (InvalidOperationException) { }
                }
            }

        }
    }
}