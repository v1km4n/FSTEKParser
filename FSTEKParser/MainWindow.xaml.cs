using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using OfficeOpenXml;

namespace FSTEKParser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int currentPage = 0;
        public int recordsPerPage = 35;
        private string localFileName = "thrlist.xlsx";
        private const string databaseURL = "https://bdu.fstec.ru/files/documents/thrlist.xlsx";
        public MainWindow()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            InitializeComponent();
            DownloadDatabase();
            ParseDatabase();
            Title = "Угрозы безопасности ФСТЭК";
            ShortThreatListGrid.ItemsSource = Pagination(0, recordsPerPage);
        }
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;
            KeyValuePair<string, string> rowData = (KeyValuePair<string, string>)row.Item; // получили данные из ряда, по которому кликнули
            int threatID = Int32.Parse(rowData.Key.Substring(4));

            SingleThreatInfo singleThreatInfoWindow = new SingleThreatInfo(Threat.AllThreats[threatID - 1]);
            singleThreatInfoWindow.Show();
        }
        private void DownloadDatabase()
        {
            Downloader.Download(databaseURL, localFileName);
        }
        private void ParseDatabase()
        {
            using (ExcelPackage table = new ExcelPackage(new FileInfo(localFileName)))
            {
                ExcelWorksheet main = table.Workbook.Worksheets[0]; // первый и единственный лист в .xlsx файле
                var totalRows = main.Dimension.End.Row;
                // список начинается с 3 строки
                for (int rowNum = 3; rowNum <= totalRows; rowNum++)
                {
                    // 1 столбец - int
                    // 2-5 - string
                    // 6-8 - bool
                    Threat.AllThreats.Add(new Threat(
                        Int32.Parse(main.Cells[rowNum, 1].Value.ToString()),
                        main.Cells[rowNum, 2].Value.ToString(),
                        main.Cells[rowNum, 3].Value.ToString(),
                        main.Cells[rowNum, 4].Value.ToString(),
                        main.Cells[rowNum, 5].Value.ToString(),
                        main.Cells[rowNum, 6].Value.ToString() == "1",
                        main.Cells[rowNum, 7].Value.ToString() == "1",
                        main.Cells[rowNum, 8].Value.ToString() == "1")
                    );
                    Threat.AllThreatsShort.Add($"УБИ.{Int32.Parse(main.Cells[rowNum, 1].Value.ToString())}", main.Cells[rowNum, 2].Value.ToString());
                }
            }
        }
        private void OpenThreatListGrid_Click(object sender, RoutedEventArgs e)
        {
            ThreatList threatList = new ThreatList();
            threatList.Show();
        }

        private void UpdateThreatList_Click(object sender, RoutedEventArgs e)
        {
            string tempFile = "temp.xlsx";
            Downloader.Download(databaseURL, tempFile);
            using (ExcelPackage table = new ExcelPackage(new FileInfo(tempFile)))
            //using (ExcelPackage table = new ExcelPackage(new FileInfo("test.xlsx")))
            {
                ExcelWorksheet main = table.Workbook.Worksheets[0];
                var totalRows = main.Dimension.End.Row;
                // список начинается с 3 строки
                int newThreats = 0;
                int changedThreats = 0;
                string result = "";
                for (int rowNum = 3; rowNum <= totalRows; rowNum++)
                {
                    if (Threat.AllThreatsShort.ContainsKey($"УБИ.{Int32.Parse(main.Cells[rowNum, 1].Value.ToString())}")) // если ключ уже есть, сравниваем поля
                    {
                        bool changed = false;
                        int threatID = Int32.Parse(main.Cells[rowNum, 1].Value.ToString()) - 1;

                        if (main.Cells[rowNum, 2].Value.ToString() != Threat.AllThreats[threatID].ThreatName)
                        {
                            if (!changed)
                            {
                                ++changedThreats;
                                result += $"УБИ.{Int32.Parse(main.Cells[rowNum, 1].Value.ToString())}\n";
                                changed = true;
                            }
                            result += $"Название:\n" +
                                $"\tБыло: {Threat.AllThreats[threatID].ThreatName}\n" +
                                $"\tСтало: {main.Cells[rowNum, 2].Value.ToString()}\n";
                            Threat.AllThreats[threatID].ThreatName = main.Cells[rowNum, 2].Value.ToString();
                        }
                        if (main.Cells[rowNum, 3].Value.ToString() != Threat.AllThreats[threatID].ThreatDescription)
                        {
                            if (!changed)
                            {
                                ++changedThreats;
                                result += $"УБИ.{Int32.Parse(main.Cells[rowNum, 1].Value.ToString())}\n";
                                changed = true;
                            }
                            result += $"Описание:\n" +
                                $"\tБыло: {Threat.AllThreats[threatID].ThreatDescription}\n" +
                                $"\tСтало: {main.Cells[rowNum, 3].Value.ToString()}\n";
                            Threat.AllThreats[threatID].ThreatDescription = main.Cells[rowNum, 3].Value.ToString();
                        }
                        if (main.Cells[rowNum, 4].Value.ToString() != Threat.AllThreats[threatID].ThreatSource)
                        {
                            if (!changed)
                            {
                                ++changedThreats;
                                result += $"УБИ.{Int32.Parse(main.Cells[rowNum, 1].Value.ToString())}\n";
                                changed = true;
                            }
                            result += $"Источник:\n" +
                                $"\tБыло: {Threat.AllThreats[threatID].ThreatSource}\n" +
                                $"\tСтало: {main.Cells[rowNum, 4].Value.ToString()}\n";
                            Threat.AllThreats[threatID].ThreatSource = main.Cells[rowNum, 4].Value.ToString();
                        }
                        if (main.Cells[rowNum, 5].Value.ToString() != Threat.AllThreats[threatID].ThreatTarget)
                        {
                            if (!changed)
                            {
                                ++changedThreats;
                                result += $"УБИ.{Int32.Parse(main.Cells[rowNum, 1].Value.ToString())}\n";
                                changed = true;
                            }
                            result += $"Объект:\n" +
                                $"\tБыло: {Threat.AllThreats[threatID].ThreatTarget}\n" +
                                $"\tСтало: {main.Cells[rowNum, 5].Value.ToString()}\n";
                            Threat.AllThreats[threatID].ThreatTarget = main.Cells[rowNum, 5].Value.ToString();
                        }
                        if (main.Cells[rowNum, 6].Value.ToString() == "1" != Threat.AllThreats[threatID].IsConfidentialityAffected)
                        {
                            if (!changed)
                            {
                                ++changedThreats;
                                result += $"УБИ.{Int32.Parse(main.Cells[rowNum, 1].Value.ToString())}\n";
                                changed = true;
                            }
                            result += $"Нарушение конфиденциальности:\n" +
                                $"\tБыло: {(Threat.AllThreats[threatID].IsConfidentialityAffected ? "Да" : "Нет")}\n" +
                                $"\tСтало: {(main.Cells[rowNum, 6].Value.ToString() == "1" ? "Да" : "Нет")}\n";
                            Threat.AllThreats[threatID].IsConfidentialityAffected = main.Cells[rowNum, 6].Value.ToString() == "1";
                        }
                        if (main.Cells[rowNum, 7].Value.ToString() == "1" != Threat.AllThreats[threatID].IsIntegrityAffected)
                        {
                            if (!changed)
                            {
                                ++changedThreats;
                                result += $"УБИ.{Int32.Parse(main.Cells[rowNum, 1].Value.ToString())}\n";
                                changed = true;
                            }
                            result += $"Нарушение целостности:\n" +
                                $"\tБыло: {(Threat.AllThreats[threatID].IsIntegrityAffected ? "Да" : "Нет")}\n" +
                                $"\tСтало: {(main.Cells[rowNum, 7].Value.ToString() == "1" ? "Да" : "Нет")}\n";
                            Threat.AllThreats[threatID].IsIntegrityAffected = main.Cells[rowNum, 7].Value.ToString() == "1";
                        }
                        if (main.Cells[rowNum, 8].Value.ToString() == "1" != Threat.AllThreats[threatID].IsAvailabilityAffected)
                        {
                            if (!changed)
                            {
                                ++changedThreats;
                                result += $"УБИ.{Int32.Parse(main.Cells[rowNum, 1].Value.ToString())}\n";
                                changed = true;
                            }
                            result += $"Нарушение доступности:\n" +
                                $"\tБыло: {(Threat.AllThreats[threatID].IsAvailabilityAffected ? "Да" : "Нет")}\n" +
                                $"\tСтало: {(main.Cells[rowNum, 8].Value.ToString() == "1" ? "Да" : "Нет")}\n";
                            Threat.AllThreats[threatID].IsAvailabilityAffected = main.Cells[rowNum, 8].Value.ToString() == "1";
                        }
                    }
                    else // если нет, добавляем его в оба списка
                    {
                        Threat.AllThreatsShort.Add($"УБИ.{Int32.Parse(main.Cells[rowNum, 1].Value.ToString())}", main.Cells[rowNum, 2].Value.ToString());
                        ++newThreats;
                        Threat.AllThreats.Add(new Threat(
                            Int32.Parse(main.Cells[rowNum, 1].Value.ToString()),
                            main.Cells[rowNum, 2].Value.ToString(),
                            main.Cells[rowNum, 3].Value.ToString(),
                            main.Cells[rowNum, 4].Value.ToString(),
                            main.Cells[rowNum, 5].Value.ToString(),
                            main.Cells[rowNum, 6].Value.ToString() == "1",
                            main.Cells[rowNum, 7].Value.ToString() == "1",
                            main.Cells[rowNum, 8].Value.ToString() == "1")
                        );
                    }
                }
                string messageBoxText = $"Обновление успешно.\n" +
                    $"Обновлено угроз: {changedThreats}\n" +
                    $"Новых угроз: {newThreats}";
                MessageBoxResult messageBoxResult = MessageBox.Show(messageBoxText, "Обновление списка угроз", MessageBoxButton.OK, MessageBoxImage.Information);
                if ((messageBoxResult == MessageBoxResult.OK) && (changedThreats != 0)) // после нажатия на ОК, если есть изменения, выводим их подробным логом
                {
                    SingleThreatInfo singleThreatInfoWindow = new SingleThreatInfo(result, "Изменения списка угроз");
                    singleThreatInfoWindow.Show();
                }
            }
            File.Delete(tempFile);
        }

        private void ExportToFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFilePath = new SaveFileDialog();
            saveFilePath.Filter = "Excel files (.xlsx)|*.xlsx";
            saveFilePath.FilterIndex = 1;
            string exportName = "";

            if (saveFilePath.ShowDialog() == true)
            {
                exportName = saveFilePath.FileName;         
            }
            else
            {
                return;
            }

            using (ExcelPackage pkg = new ExcelPackage(new FileInfo(exportName)))
            {
                try
                {
                    pkg.Workbook.Worksheets.Delete("Sheet");
                }
                catch (Exception)
                {
                    // А в этой штуке нет метода, чтобы проверить, существует ли лист, я не виноват
                }
                ExcelWorksheet ws = pkg.Workbook.Worksheets.Add("Sheet");
                ws.Cells["A1"].SetCellValue(0, 0, "Идентификатор УБИ");
                ws.Cells["A1"].SetCellValue(0, 1, "Наименование УБИ");
                ws.Cells["A1"].SetCellValue(0, 2, "Описание");
                ws.Cells["A1"].SetCellValue(0, 3, "Источник угрозы (характеристика и потенциал нарушителя)");
                ws.Cells["A1"].SetCellValue(0, 4, "Объект воздействия");
                ws.Cells["A1"].SetCellValue(0, 5, "Нарушение конфиденциальности");
                ws.Cells["A1"].SetCellValue(0, 6, "Нарушение целостности");
                ws.Cells["A1"].SetCellValue(0, 7, "Нарушение доступности");

                for (int row = 0; row < Threat.AllThreats.Count; row++)
                {
                    ws.Cells["A1"].SetCellValue(row + 1, 0, Threat.AllThreats[row].ThreatId);
                    ws.Cells["A1"].SetCellValue(row + 1, 1, Threat.AllThreats[row].ThreatName);
                    ws.Cells["A1"].SetCellValue(row + 1, 2, Threat.AllThreats[row].ThreatDescription);
                    ws.Cells["A1"].SetCellValue(row + 1, 3, Threat.AllThreats[row].ThreatSource);
                    ws.Cells["A1"].SetCellValue(row + 1, 4, Threat.AllThreats[row].ThreatTarget);
                    ws.Cells["A1"].SetCellValue(row + 1, 5, Threat.AllThreats[row].IsConfidentialityAffected ? 1 : 0);
                    ws.Cells["A1"].SetCellValue(row + 1, 6, Threat.AllThreats[row].IsIntegrityAffected ? 1 : 0);
                    ws.Cells["A1"].SetCellValue(row + 1, 7, Threat.AllThreats[row].IsAvailabilityAffected ? 1 : 0);
                }
                
                pkg.Save();
            }
            MessageBox.Show("Успешно экспортировано в файл", "Экспорт в файл", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }

        private void PaginationUp_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage <= Threat.AllThreatsShort.Count / recordsPerPage - 1)
            {
                currentPage++;
                ShortThreatListGrid.ItemsSource = Pagination(currentPage, recordsPerPage);
            }
        }

        private void PaginationDown_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage != 0)
            {
                currentPage--;
                ShortThreatListGrid.ItemsSource = Pagination(currentPage, recordsPerPage);
            }
        }
        public Dictionary<string, string> Pagination(int page, int recordsPerPage)
        {
            Dictionary<string, string> temp = new Dictionary<string, string>();
            // 0: 1 - 35 (0 * 35 + 1) = 1; (1 * 35) = 35
            // 1: 36 - 70 (1 * 35 + 1) = 36; (2 * 35) = 70 
            // от page * 35 + 1 до (page + 1) * 35 
            foreach (var pair in Threat.AllThreatsShort)
            {
                int threatID = Int32.Parse(pair.Key.Substring(4));
                if ((threatID >= page * recordsPerPage + 1) && (threatID <= (page + 1) * recordsPerPage))
                {
                    temp.Add(pair.Key, pair.Value);
                }
                if (threatID > (page + 1) * recordsPerPage)
                {
                    break;
                }
            }
            return temp;
        }
    }
}
