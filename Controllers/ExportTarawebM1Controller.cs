using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

using Taraweb.Data;

namespace Taraweb.Controllers
{
    public partial class ExportTarawebM1Controller : ExportController
    {
        private readonly TarawebM1Context context;
        private readonly TarawebM1Service service;

        public ExportTarawebM1Controller(TarawebM1Context context, TarawebM1Service service)
        {
            this.service = service;
            this.context = context;
        }

        [HttpGet("/export/TarawebM1/galleries/csv")]
        [HttpGet("/export/TarawebM1/galleries/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportGalleriesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetGalleries(), Request.Query), fileName);
        }

        [HttpGet("/export/TarawebM1/galleries/excel")]
        [HttpGet("/export/TarawebM1/galleries/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportGalleriesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetGalleries(), Request.Query), fileName);
        }

        [HttpGet("/export/TarawebM1/languages/csv")]
        [HttpGet("/export/TarawebM1/languages/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportLanguagesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetLanguages(), Request.Query), fileName);
        }

        [HttpGet("/export/TarawebM1/languages/excel")]
        [HttpGet("/export/TarawebM1/languages/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportLanguagesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetLanguages(), Request.Query), fileName);
        }

        [HttpGet("/export/TarawebM1/pagecategories/csv")]
        [HttpGet("/export/TarawebM1/pagecategories/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportPageCategoriesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetPageCategories(), Request.Query), fileName);
        }

        [HttpGet("/export/TarawebM1/pagecategories/excel")]
        [HttpGet("/export/TarawebM1/pagecategories/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportPageCategoriesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetPageCategories(), Request.Query), fileName);
        }

        [HttpGet("/export/TarawebM1/posts/csv")]
        [HttpGet("/export/TarawebM1/posts/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportPostsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetPosts(), Request.Query), fileName);
        }

        [HttpGet("/export/TarawebM1/posts/excel")]
        [HttpGet("/export/TarawebM1/posts/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportPostsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetPosts(), Request.Query), fileName);
        }
    }
}
