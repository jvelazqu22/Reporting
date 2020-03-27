using Domain.Helper;
using Microsoft.Reporting.WinForms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UserDefinedReports;

namespace iBank.UnitTesting.iBank.UserDefinedReports
{
    [TestClass]
    public class ReportRendererTests
    {
        [TestMethod]
        public void output_type_is_pdf_render_pdf_file()
        {
            var fileName = "mytestfile.pdf";
            var report = new LocalReport();
            var output = DestinationSwitch.ClassicPdf;
            var sut = new Mock<ReportRenderer>(MockBehavior.Default, report, fileName);

            sut.Object.Render(output);

            sut.Verify(x => x.RenderToStream(It.Is<string>(s => s.Equals("pdf"))), Times.Once);
            sut.Verify(x => x.WriteAllBytes(It.Is<string>(s => s.Equals(fileName)), It.IsAny<byte[]>()), Times.Once);
            sut.Verify(x => x.DeleteFile(It.Is<string>(s => s.Equals("mytestfile.rdlc"))), Times.Once);
        }

        [TestMethod]
        public void output_type_is_xlsx_formatted_ext_is_xls_change_to_xlsx_and_render_xlsx_file()
        {
            var fileName = "mytestfile.xls";
            var report = new LocalReport();
            var output = DestinationSwitch.XlsxFormatted;
            var sut = new Mock<ReportRenderer>(MockBehavior.Default, report, fileName);

            sut.Object.Render(output);

            sut.Verify(x => x.RenderToStream(It.Is<string>(s => s.Equals("excelopenxml"))), Times.Once);
            sut.Verify(x => x.WriteAllBytes(It.Is<string>(s => s.Equals(fileName + "x")), It.IsAny<byte[]>()), Times.Once);
            sut.Verify(x => x.DeleteFile(It.Is<string>(s => s.Equals("mytestfile.rdlc"))), Times.Once);
        }
    }
}
