using InBodyPDFExtractor.Models;
using Syncfusion.XlsIO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InBodyPDFExtractor.Utilities;

internal class ExcelUtilities
{
    public static Task WriteExcel(string outputPath, IList<Person> persons)
    {
        return Task.Run(() =>
        {
            //Create an instance of ExcelEngine
            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                IApplication application = excelEngine.Excel;

                application.DefaultVersion = ExcelVersion.Excel2016;

                //Create a workbook
                IWorkbook workbook = application.Workbooks.Create(1);
                IWorksheet worksheet = workbook.Worksheets[0];

                // Header Row

                worksheet.Range["A1"].Value = "Id";
                worksheet.Range["B1"].Value = "วันที่ตรวจ";
                worksheet.Range["C1"].Value = "ชื่อ-นามสกุล";
                worksheet.Range["D1"].Value = "อายุ";
                worksheet.Range["E1"].Value = "ความสูง";
                worksheet.Range["F1"].Value = "น้ำหนัก";
                worksheet.Range["G1"].Value = "ช่วงน้ำหนักปกติ-ต่ำ";
                worksheet.Range["H1"].Value = "ช่วงน้ำหนักปกติ-สูง";
                worksheet.Range["I1"].Value = "SkeletalMuscleMass";
                worksheet.Range["J1"].Value = "NormalSkeletalMuscleMass-Lower";
                worksheet.Range["K1"].Value = "NormalSkeletalMuscleMass-Upper";
                worksheet.Range["L1"].Value = "BodyFatMass";
                worksheet.Range["M1"].Value = "NormalBodyFatMass-Lower";
                worksheet.Range["N1"].Value = "NormalBodyFatMass-Upper";
                worksheet.Range["O1"].Value = "TotalBodyWater";
                worksheet.Range["P1"].Value = "NormalTotalBodyWater-Lower";
                worksheet.Range["Q1"].Value = "NormalTotalBodyWaters-Upper";
                worksheet.Range["R1"].Value = "FatFreeMass";
                worksheet.Range["S1"].Value = "NormalFatFreeMass-Lower";
                worksheet.Range["T1"].Value = "NormalFatFreeMass-Upper";

                for (var row = 0; row < persons.Count; row++)
                {
                    var person = persons[row];
                    var actualRow = row + 1;
                    worksheet.Range[$"A{actualRow}"].Value = person.Id;
                    worksheet.Range[$"B{actualRow}"].Value2 = person.DateTime;
                    worksheet.Range[$"C{actualRow}"].Value = person.Name;
                    worksheet.Range[$"D{actualRow}"].Value2 = person.Age;
                    worksheet.Range[$"E{actualRow}"].Value2 = person.Height;

                    worksheet.Range[$"F{actualRow}"].Value2 = person.Weight?.Value;
                    worksheet.Range[$"G{actualRow}"].Value2 = person.Weight?.Lower;
                    worksheet.Range[$"H{actualRow}"].Value2 = person.Weight?.Upper;

                    worksheet.Range[$"I{actualRow}"].Value2 = person.SkeletalMuscleMass?.Value;
                    worksheet.Range[$"J{actualRow}"].Value2 = person.SkeletalMuscleMass?.Lower;
                    worksheet.Range[$"K{actualRow}"].Value2 = person.SkeletalMuscleMass?.Upper;

                    worksheet.Range[$"L{actualRow}"].Value2 = person.BodyFatMass?.Value;
                    worksheet.Range[$"M{actualRow}"].Value2 = person.BodyFatMass?.Lower;
                    worksheet.Range[$"N{actualRow}"].Value2 = person.BodyFatMass?.Upper;
                    worksheet.Range[$"O{actualRow}"].Value2 = person.TotalBodyWater?.Value;
                    worksheet.Range[$"P{actualRow}"].Value2 = person.TotalBodyWater?.Lower;
                    worksheet.Range[$"Q{actualRow}"].Value2 = person.TotalBodyWater?.Upper;
                    worksheet.Range[$"R{actualRow}"].Value2 = person.FatFreeMass?.Value;
                    worksheet.Range[$"S{actualRow}"].Value2 = person.FatFreeMass?.Lower;
                    worksheet.Range[$"T{actualRow}"].Value2 = person.FatFreeMass?.Upper;
                }

                workbook.SaveAs(outputPath);
            }
        });
    }
}
