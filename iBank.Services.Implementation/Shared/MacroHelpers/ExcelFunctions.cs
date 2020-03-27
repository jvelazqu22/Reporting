using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using com.ciswired.libraries.CISLogger;
using CODE.Framework.Core.Utilities;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Excel;
using Microsoft.Vbe.Interop;
using System.Threading;

namespace iBank.Services.Implementation.Shared.MacroHelpers
{
    public class ExcelFunctions
    {
        private static readonly com.ciswired.libraries.CISLogger.ILogger LOG = new LogManagerWrapper().GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        [DllImport("user32.dll")]
        static extern int GetWindowThreadProcessId(int hWnd, out int lpdwProcessId);

        public void KillOldExcelProcesses()
        {
            LOG.Info($"InactivateOldExcelProcesses - KillOldExcelProcesses() - start ");

            if (!new ExcelProcessChecker().IsItOkayToInactivateOldProcesses()) return;

            foreach (var process in Process.GetProcessesByName("EXCEL"))
            {
                // if the process is not older than 6 hours then, skip it. This is to make sure that we only kill processes
                // that have been there longer than 6 hours. This way we won't kill processes that are in the middle of being used.
                if (DateTime.Now.AddHours(-6) < process.StartTime) continue;

                try
                {
                    LOG.Info($"InactivateOldExcelProcesses - gettting ready to kill process id: {process.Id}");
                    process.Kill();
                    LOG.Info($"InactivateOldExcelProcesses - killed process id: {process.Id}");
                }
                catch (Exception ex)
                {
                    LOG.Warn($"error trying to kill excel process", ex);
                }
            }
            LOG.Info($"InactivateOldExcelProcesses - KillOldExcelProcesses() was ran.");
        }

        private Process GetExcelProcess(Microsoft.Office.Interop.Excel.Application excelApp)
        {
            int id;
            GetWindowThreadProcessId(excelApp.Hwnd, out id);
            return Process.GetProcessById(id);
        }

        private void TerminateExcelProcess(Microsoft.Office.Interop.Excel.Application excelApp)
        {
            try
            {
                var process = GetExcelProcess(excelApp);
                process?.Kill();
            }
            catch (Exception ex)
            {
                LOG.Warn($"error trying to kill excel process", ex);
            }
        }

        public bool ApplyMacros(string fileName, List<string> MacroDataList, string macroKey)
        {
            var excel = new Microsoft.Office.Interop.Excel.Application();
            Workbook workbook = null;
            try
            {
                excel.Visible = false;
                excel.AutomationSecurity = MsoAutomationSecurity.msoAutomationSecurityLow;
                workbook = excel.Workbooks.Open(fileName);
            }
            catch (Exception ex)
            {
                LOG.Error(ex);
                workbook?.Close(false);
                excel.Quit();
                LoggingMediator.Log("Excel Interop failed.", LogEventType.Exception);
                return false;
            }

            try
            {
                var newStandardModule = workbook.VBProject.VBComponents.Add(vbext_ComponentType.vbext_ct_StdModule);
                var codeModule = newStandardModule.CodeModule;

                foreach (var macroStr in MacroDataList)
                {
                    codeModule.AddFromString(macroStr);
                    workbook.Save();

                    //data in the macroStr is in vb, so it will begin with Sub -- so strip that out to get the name of the macro
                    var j = macroStr.IndexOf("Sub ", StringComparison.CurrentCultureIgnoreCase) + 3;

                    var macroName = macroStr.Substring(j + 1);
                    var whiteSpace = new[] { '\n', '\r', ' ', '(' };
                    macroName = macroName.Substring(0, macroName.IndexOfAny(whiteSpace));
                    var m = $"{workbook.Name}!{newStandardModule.Name}.{macroName}";
                    excel.Run(m);
                }
                DeleteMacros(workbook);
                workbook.Save();
                workbook.Saved = true;
            }
            catch (Exception ex)
            {
                LOG.Error(ex);
                DeleteMacros(workbook);
                LoggingMediator.Log("Specified macrokey " + macroKey + " failed.", LogEventType.Exception);
                return false;
            }
            finally
            {
                try
                {
                    // sometimes the following exception happens in prod only:
                    // System.Runtime.InteropServices.COMException(0x800AC472): Exception from HRESULT: 0x800AC472     
                    // at Microsoft.Office.Interop.Excel.ApplicationClass.set_DisplayAlerts(Boolean RHS)
                    excel.DisplayAlerts = false;
                    //Excel is a slow, not sure if this will fix, but try it first.
                    Thread.Sleep(50);
                    workbook?.Close(false); // If the macro was successfully run, it was saved above, if not, then we don't want to save it anyway.
                    excel.Quit();
                }
                catch (Exception ex)
                {
                    LOG.Error(ex);
                }

                LOG.Info($"InactivateOldExcelProcesses - TerminateExcelProcess() - start ");
                TerminateExcelProcess(excel);
                LOG.Info($"InactivateOldExcelProcesses - TerminateExcelProcess() - end ");

                //while (Marshal.ReleaseComObject(excel.Sheets) > 0) { }
                //while (Marshal.ReleaseComObject(workbook) > 0) { }
                //while (Marshal.ReleaseComObject(excel.Workbooks) > 0) { }

                //excel.Quit();

                //while (Marshal.ReleaseComObject(excel) > 0) { }
            }

            return true;
        }

        private static void DeleteMacros(Workbook workbook)
        {
            //Remove the macro. IF the component can't be removed, then the lines will have to be removed.
            VBProject project = workbook.VBProject;

            for (int i = project.VBComponents.Count; i >= 1; i--)
            {
                var component = project.VBComponents.Item(i);
                try
                {
                    project.VBComponents.Remove(component);
                }
                catch (ArgumentException)
                {
                }
            }

            for (int i = project.VBComponents.Count; i >= 1; i--)
            {
                VBComponent component = project.VBComponents.Item(i);
                component.CodeModule.DeleteLines(1, component.CodeModule.CountOfLines);
            }

        }

    }
}
