using ProyectoERP.Helpers;

namespace MvcCoreUtilidades.Helpers
{
    public class HelperUploadFiles
    {
        private HelperPathProvider helperPath;
        
        public HelperUploadFiles(HelperPathProvider pathProvider)
        {
            this.helperPath = pathProvider;
        }
        public async Task<string> UploadFileAsync(IFormFile file, Folders folder)
        {
            string fileName = file.FileName;
            string path =
                this.helperPath.MapPath(fileName, folder);
            using (Stream stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return path;
        }

        public async Task<List<string>> UploadFileAsync(List<IFormFile> files, Folders folder)
        {
            List<string> paths = new List<string>();
            foreach (IFormFile file in files)
            {
                string fileName = file.FileName;
                string path = this.helperPath.MapPath(fileName, folder);
                using (Stream stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                paths.Add(path);
            }
            return paths;
        }
    }
}
