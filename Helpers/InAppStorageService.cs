namespace ProjectAPI.Helpers
{
    public class InAppStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment env;
        private readonly IHttpContextAccessor httpContextAccessor;

        public InAppStorageService(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            this.env = env;
            this.httpContextAccessor = httpContextAccessor;
        }
        public Task DeleteFile(string fileRoute, string containerName)
        {
            if (string.IsNullOrEmpty(fileRoute))
            {
                return Task.CompletedTask;
            }
            var fileName = Path.GetFileName(fileRoute);
            var fileDirectory = Path.Combine(env.WebRootPath, containerName);
            if (File.Exists(fileDirectory))
            {
                File.Delete(fileDirectory);
            }
            return Task.CompletedTask;
        }

        public async Task<string> EditFile(string fileRoute, string containerName, IFormFile file)
        {
            await DeleteFile(fileRoute, containerName);
            return await SaveFile(containerName, file);
        }

        public async Task<string> SaveFile(string containerName, IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{extension}";
            string folder = Path.Combine(env.WebRootPath, containerName);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            string route = Path.Combine(folder, fileName);
            using(var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                var content = ms.ToArray();
                await File.WriteAllBytesAsync(route, content);
            }
            var url = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}";
            var routeForDB = Path.Combine(url, containerName, fileName).Replace("\\", "/");
            return routeForDB;
        }
        public async Task<string> SaveManyFiles(string containerName, List<IFormFile> files)
        {
            List<string> routesForDB = new List<string>();
            foreach(var file in files)
            {
                var extension = Path.GetExtension(file.FileName);
                var fileName = $"{Guid.NewGuid()}{extension}";
                string folder = Path.Combine(env.WebRootPath, containerName);

                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                string route = Path.Combine(folder, fileName);
                using (var ms = new MemoryStream())
                {
                    await file.CopyToAsync(ms);
                    var content = ms.ToArray();
                    await File.WriteAllBytesAsync(route, content);
                }
                var url = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}";
                var routeForDB = Path.Combine(url, containerName, fileName).Replace("\\", "/");
                routesForDB.Add(routeForDB);
            }
            var routesString = string.Join("Ω", routesForDB);
            return routesString;
            //var extension = Path.GetExtension(file.FileName);
            //var fileName = $"{Guid.NewGuid()}{extension}";
            //string folder = Path.Combine(env.WebRootPath, containerName);

            //if (!Directory.Exists(folder))
            //{
            //    Directory.CreateDirectory(folder);
            //}
            //string route = Path.Combine(folder, fileName);
            //using (var ms = new MemoryStream())
            //{
            //    await file.CopyToAsync(ms);
            //    var content = ms.ToArray();
            //    await File.WriteAllBytesAsync(route, content);
            //}
            //var url = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}";
            //var routeForDB = Path.Combine(url, containerName, fileName).Replace("\\", "/");
            //return routeForDB;
        }
    }
}
