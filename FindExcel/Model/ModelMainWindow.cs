using ClosedXML.Excel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FindExcel
{
    public class ModelMainWindow
    {
        public List<FileExcel> GetFiles()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Excel Worksheets|*.xlsx"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                var list = new List<FileExcel>(openFileDialog.FileNames.Length);
                list.AddRange(openFileDialog.FileNames.Select(p => new FileExcel() { Name = Path.GetFileName(p), Path = p }));
                return list;
            }

            return null;
        }

        public string GetFileForResult(ProgressReport progressReport)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = false,
                Filter = "Excel Worksheets|*.xlsx"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    using (var fileStream = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                    { }
                }
                catch (IOException ex)
                {
                    progressReport.ProcessedFile = openFileDialog.FileName;
                    progressReport.ProcessedMessage = ex.Message;
                    progressReport.ProcessType = ProcessType.Error;
                    return null;
                }
                return openFileDialog.FileName;
            }
            return null;
        }

        public void EditMasterFile(DataEdit dataEdit, ProgressReport progressReport)
        {
            try
            {
                progressReport.ProcessedFile = dataEdit.FileForResult;

                if (!File.Exists(dataEdit.FileForResult))
                {
                    progressReport.ProcessType = ProcessType.Error;
                    progressReport.ProcessedMessage = $"Файл {dataEdit.FileForResult} не найден.";
                    return;
                }

                using (var excel = new XLWorkbook(dataEdit.FileForResult))
                {
                    var resultIsCheck = dataEdit.CollectionResults.Where(s => s.IsCheck);
                    var resultIsCheckMasterFile = dataEdit.CollectionResolutMasterFile.Where(s => s.IsCheck);

                    foreach (var item in resultIsCheckMasterFile)
                    {
                        string str = string.Empty;
                        foreach (var itemResult in resultIsCheck)
                        {
                            str += itemResult.NameFile + " ";
                        }

                        excel.Worksheet(item.WorkSheetName).Cell(item.RowNumber, dataEdit.ColumnForResult).Value = str;
                    }
                    excel.Save();
                    progressReport.ProcessType = ProcessType.Completed;
                    progressReport.ProcessedMessage = "Выполнение записи завершено!";
                }
            }
            catch (IOException)
            {
                progressReport.ProcessType = ProcessType.Error;
                progressReport.ProcessedMessage = $"Файл {dataEdit.FileForResult} открыт! Повторите попытку после закрытия данного файла.";
            }
            catch (Exception ex)
            {
                progressReport.ProcessType = ProcessType.Error;
                progressReport.ProcessedMessage = ex.Message;
            }
        }

        public void ReadFile(DataRead dataRead, ProgressReport progressReport)
        {
            Task task = new Task(() =>
            {
                try
                {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    progressReport.ProcessType = ProcessType.Working;

                    foreach (var file in dataRead.CollectionFiles)
                    {
                        progressReport.ProcessedFile = file.Name;
                        progressReport.ProgressValue = dataRead.CollectionFiles.IndexOf(file) + 1;



                        //var excel = new XLWorkbook(file.Path);
                        //var sheet = excel.Worksheets.FirstOrDefault();
                        // var a = sheet.CellsUsed(true);
                        // var firstTableCell = sheet.FirstCellUsed();
                        // var lastTableCell = sheet.LastCellUsed();
                        //  var rngData = sheet.Range(firstTableCell.Address, lastTableCell.Address);

                        //var a = sheet.Range("A1:M1").Columns();
                        //var rngData = sheet.PageSetup.PrintAreas.FirstOrDefault();
                        //sheet.c
                        // var a = sheet.Columns("A:M");
                        // sheet.Range("N:WVV").Delete(XLShiftDeletedCells.ShiftCellsLeft);
                        //  var b = sheet.Pictures;
                        // excel.Worksheets.Add("New");
                        //  var b = excel.Worksheets.FirstOrDefault(s => s.Name == "New");
                        // b.Cell(1, 1).Value = a;


                        // excel.Save();

                        using (var fileStream = new FileStream(file.Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            var start = stopwatch.ElapsedMilliseconds;

                            using (var excel = new XLWorkbook(fileStream))
                            {
                                var stop = stopwatch.ElapsedMilliseconds;
                                Debug.WriteLine($"Is Open File {stop - start}");

                                foreach (var sheet in excel.Worksheets)
                                {
                                    start = stopwatch.ElapsedMilliseconds;

                                    var rows = sheet.RowsUsed();

                                    stop = stopwatch.ElapsedMilliseconds;
                                    Debug.WriteLine($"Is received rows {stop - start}");

                                    foreach (var row in rows)
                                    {
                                        dataRead.CollectionResults.Add(new Result()
                                        {
                                            NameFile = Path.GetFileNameWithoutExtension(file.Path),
                                            FoundString = row.Cell(dataRead.ColumnSearch).Value.ToString(),
                                            Address = row.ToString(),
                                            RowNumber = row.RowNumber(),
                                            WorkSheetName = row.Worksheet.Name,
                                        });
                                    }
                                }
                            }
                        }
                  }





                    progressReport.ProcessedFile = dataRead.FileForResult;

                    if (!string.IsNullOrEmpty(dataRead.FileForResult))
                    {
                        using (var fileStream = new FileStream(dataRead.FileForResult, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            using (var excel = new XLWorkbook(fileStream))
                            {
                                foreach (var sheet in excel.Worksheets)
                                {
                                    var rows = sheet.RowsUsed();
                                    foreach (var row in rows)
                                    {
                                        dataRead.CollectionResolutMasterFile.Add(new Result()
                                        {
                                            FoundString = row.Cell(dataRead.ColumnSearchMastrFile).Value.ToString(),
                                            Address = row.ToString(),
                                            WorkSheetName = row.Worksheet.Name,
                                            RowNumber = row.RowNumber(),
                                            CellForResult = string.IsNullOrEmpty(dataRead.ColumnForResult) ? string.Empty : row.Cell(dataRead.ColumnForResult).Value.ToString()
                                        });
                                    }
                                }
                            }
                        }

                    }

                    progressReport.ProcessType = ProcessType.Completed;
                    progressReport.ProcessedMessage = "Выполнение чтения завершено!";

                    stopwatch.Stop();
                }
                catch (Exception ex)
                {
                    progressReport.ProcessType = ProcessType.Error;
                    progressReport.ProcessedMessage = ex.Message;
                }
                
            });


            task.Start();
        }
    }
}