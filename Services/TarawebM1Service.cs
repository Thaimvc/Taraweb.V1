using System;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Radzen;

using Taraweb.Data;

namespace Taraweb
{
    public partial class TarawebM1Service
    {
        TarawebM1Context Context
        {
           get
           {
             return this.context;
           }
        }

        private readonly TarawebM1Context context;
        private readonly NavigationManager navigationManager;

        public TarawebM1Service(TarawebM1Context context, NavigationManager navigationManager)
        {
            this.context = context;
            this.navigationManager = navigationManager;
        }

        public void Reset() => Context.ChangeTracker.Entries().Where(e => e.Entity != null).ToList().ForEach(e => e.State = EntityState.Detached);


        public async Task ExportGalleriesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/tarawebm1/galleries/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/tarawebm1/galleries/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportGalleriesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/tarawebm1/galleries/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/tarawebm1/galleries/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGalleriesRead(ref IQueryable<Taraweb.Models.TarawebM1.Gallery> items);

        public async Task<IQueryable<Taraweb.Models.TarawebM1.Gallery>> GetGalleries(Query query = null)
        {
            var items = Context.Galleries.AsQueryable();

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }

            OnGalleriesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnGalleryGet(Taraweb.Models.TarawebM1.Gallery item);

        public async Task<Taraweb.Models.TarawebM1.Gallery> GetGalleryById(int id)
        {
            var items = Context.Galleries
                              .AsNoTracking()
                              .Where(i => i.Id == id);

  
            var itemToReturn = items.FirstOrDefault();

            OnGalleryGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnGalleryCreated(Taraweb.Models.TarawebM1.Gallery item);
        partial void OnAfterGalleryCreated(Taraweb.Models.TarawebM1.Gallery item);

        public async Task<Taraweb.Models.TarawebM1.Gallery> CreateGallery(Taraweb.Models.TarawebM1.Gallery gallery)
        {
            OnGalleryCreated(gallery);

            var existingItem = Context.Galleries
                              .Where(i => i.Id == gallery.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Galleries.Add(gallery);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(gallery).State = EntityState.Detached;
                throw;
            }

            OnAfterGalleryCreated(gallery);

            return gallery;
        }

        public async Task<Taraweb.Models.TarawebM1.Gallery> CancelGalleryChanges(Taraweb.Models.TarawebM1.Gallery item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnGalleryUpdated(Taraweb.Models.TarawebM1.Gallery item);
        partial void OnAfterGalleryUpdated(Taraweb.Models.TarawebM1.Gallery item);

        public async Task<Taraweb.Models.TarawebM1.Gallery> UpdateGallery(int id, Taraweb.Models.TarawebM1.Gallery gallery)
        {
            OnGalleryUpdated(gallery);

            var itemToUpdate = Context.Galleries
                              .Where(i => i.Id == gallery.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(gallery);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterGalleryUpdated(gallery);

            return gallery;
        }

        partial void OnGalleryDeleted(Taraweb.Models.TarawebM1.Gallery item);
        partial void OnAfterGalleryDeleted(Taraweb.Models.TarawebM1.Gallery item);

        public async Task<Taraweb.Models.TarawebM1.Gallery> DeleteGallery(int id)
        {
            var itemToDelete = Context.Galleries
                              .Where(i => i.Id == id)
                              .Include(i => i.Posts)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnGalleryDeleted(itemToDelete);


            Context.Galleries.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterGalleryDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportLanguagesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/tarawebm1/languages/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/tarawebm1/languages/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportLanguagesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/tarawebm1/languages/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/tarawebm1/languages/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnLanguagesRead(ref IQueryable<Taraweb.Models.TarawebM1.Language> items);

        public async Task<IQueryable<Taraweb.Models.TarawebM1.Language>> GetLanguages(Query query = null)
        {
            var items = Context.Languages.AsQueryable();

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }

            OnLanguagesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnLanguageGet(Taraweb.Models.TarawebM1.Language item);

        public async Task<Taraweb.Models.TarawebM1.Language> GetLanguageById(int id)
        {
            var items = Context.Languages
                              .AsNoTracking()
                              .Where(i => i.Id == id);

  
            var itemToReturn = items.FirstOrDefault();

            OnLanguageGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnLanguageCreated(Taraweb.Models.TarawebM1.Language item);
        partial void OnAfterLanguageCreated(Taraweb.Models.TarawebM1.Language item);

        public async Task<Taraweb.Models.TarawebM1.Language> CreateLanguage(Taraweb.Models.TarawebM1.Language language)
        {
            OnLanguageCreated(language);

            var existingItem = Context.Languages
                              .Where(i => i.Id == language.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Languages.Add(language);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(language).State = EntityState.Detached;
                throw;
            }

            OnAfterLanguageCreated(language);

            return language;
        }

        public async Task<Taraweb.Models.TarawebM1.Language> CancelLanguageChanges(Taraweb.Models.TarawebM1.Language item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnLanguageUpdated(Taraweb.Models.TarawebM1.Language item);
        partial void OnAfterLanguageUpdated(Taraweb.Models.TarawebM1.Language item);

        public async Task<Taraweb.Models.TarawebM1.Language> UpdateLanguage(int id, Taraweb.Models.TarawebM1.Language language)
        {
            OnLanguageUpdated(language);

            var itemToUpdate = Context.Languages
                              .Where(i => i.Id == language.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(language);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterLanguageUpdated(language);

            return language;
        }

        partial void OnLanguageDeleted(Taraweb.Models.TarawebM1.Language item);
        partial void OnAfterLanguageDeleted(Taraweb.Models.TarawebM1.Language item);

        public async Task<Taraweb.Models.TarawebM1.Language> DeleteLanguage(int id)
        {
            var itemToDelete = Context.Languages
                              .Where(i => i.Id == id)
                              .Include(i => i.PageCategories)
                              
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnLanguageDeleted(itemToDelete);


            Context.Languages.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterLanguageDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportPageCategoriesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/tarawebm1/pagecategories/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/tarawebm1/pagecategories/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportPageCategoriesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/tarawebm1/pagecategories/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/tarawebm1/pagecategories/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnPageCategoriesRead(ref IQueryable<Taraweb.Models.TarawebM1.PageCategory> items);

        public async Task<IQueryable<Taraweb.Models.TarawebM1.PageCategory>> GetPageCategories(Query query = null)
        {
            var items = Context.PageCategories.AsQueryable();

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }

            OnPageCategoriesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnPageCategoryGet(Taraweb.Models.TarawebM1.PageCategory item);

        public async Task<Taraweb.Models.TarawebM1.PageCategory> GetPageCategoryById(int id)
        {
            var items = Context.PageCategories
                              .AsNoTracking()
                              .Where(i => i.Id == id);

                items = items.Include(i => i.Language);
  
            var itemToReturn = items.FirstOrDefault();

            OnPageCategoryGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnPageCategoryCreated(Taraweb.Models.TarawebM1.PageCategory item);
        partial void OnAfterPageCategoryCreated(Taraweb.Models.TarawebM1.PageCategory item);

        public async Task<Taraweb.Models.TarawebM1.PageCategory> CreatePageCategory(Taraweb.Models.TarawebM1.PageCategory pagecategory)
        {
            OnPageCategoryCreated(pagecategory);

            var existingItem = Context.PageCategories
                              .Where(i => i.Id == pagecategory.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.PageCategories.Add(pagecategory);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(pagecategory).State = EntityState.Detached;
                throw;
            }

            OnAfterPageCategoryCreated(pagecategory);

            return pagecategory;
        }

        public async Task<Taraweb.Models.TarawebM1.PageCategory> CancelPageCategoryChanges(Taraweb.Models.TarawebM1.PageCategory item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnPageCategoryUpdated(Taraweb.Models.TarawebM1.PageCategory item);
        partial void OnAfterPageCategoryUpdated(Taraweb.Models.TarawebM1.PageCategory item);

        public async Task<Taraweb.Models.TarawebM1.PageCategory> UpdatePageCategory(int id, Taraweb.Models.TarawebM1.PageCategory pagecategory)
        {
            OnPageCategoryUpdated(pagecategory);

            var itemToUpdate = Context.PageCategories
                              .Where(i => i.Id == pagecategory.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(pagecategory);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterPageCategoryUpdated(pagecategory);

            return pagecategory;
        }

        partial void OnPageCategoryDeleted(Taraweb.Models.TarawebM1.PageCategory item);
        partial void OnAfterPageCategoryDeleted(Taraweb.Models.TarawebM1.PageCategory item);

        public async Task<Taraweb.Models.TarawebM1.PageCategory> DeletePageCategory(int id)
        {
            var itemToDelete = Context.PageCategories
                              .Where(i => i.Id == id)
                              .Include(i => i.Posts)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnPageCategoryDeleted(itemToDelete);


            Context.PageCategories.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterPageCategoryDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportPostsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/tarawebm1/posts/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/tarawebm1/posts/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportPostsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/tarawebm1/posts/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/tarawebm1/posts/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnPostsRead(ref IQueryable<Taraweb.Models.TarawebM1.Post> items);

        public async Task<IQueryable<Taraweb.Models.TarawebM1.Post>> GetPosts(Query query = null)
        {
            var items = Context.Posts.AsQueryable();

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }

            
            OnPostsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnPostContentsRead(ref IQueryable<Taraweb.Models.TarawebM1.PostContent> items);
        public async Task<IQueryable<Taraweb.Models.TarawebM1.PostContent>> GetPostContents(Query query = null)
        {
            var items = Context.PostContents.AsQueryable();

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }

            OnPostContentsRead(ref items);

            return await Task.FromResult(items);
        }

        public async Task<IQueryable<Taraweb.Models.TarawebM1.PostContent>> GetPostContentByCatId(int catid,Query query = null)
        {
            var items = Context.PostContents.Where(w => w.Post.PageCategoryId == catid).AsQueryable();

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }

            OnPostContentsRead(ref items);

            return await Task.FromResult(items);
        }


        partial void OnPostGet(Taraweb.Models.TarawebM1.Post item);

        public async Task<IQueryable<Taraweb.Models.TarawebM1.Post>> GetPostByCategoryId(int catid, Query query=null)
        {
            var items = Context.Posts.Where(w=>w.PageCategoryId==catid).AsQueryable();
            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }
            OnPostsRead(ref items);

            return await Task.FromResult(items);
        }
        public async Task<Taraweb.Models.TarawebM1.PostContent> GetPostContentBySlug(string slug,string lang)
        {
            var items = Context.PostContents
                .Include(i => i.Post.Gallery)
                .Include(i => i.Post.PageCategory)
                              .AsNoTracking()
                              .Where(i => i.Slug == slug && i.Language.Code==lang).FirstOrDefault();

          
            

            OnPostContentGet(items);

            return await Task.FromResult(items);
        }
        partial void OnPostContentGet(Taraweb.Models.TarawebM1.PostContent item);

        public async Task<IQueryable<Taraweb.Models.TarawebM1.PostContent>> GetPostContentByCategoryId(int catid,string lang, Query query = null)
        {
            var items = Context.PostContents.Where(w => w.Language.Code==lang&& w.Post.PageCategoryId == catid).AsQueryable();
            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach (var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }
            OnPostContentsRead(ref items);
            return await Task.FromResult(items);
        }

        public async Task<Taraweb.Models.TarawebM1.Post> GetPostById(int id)
        {
            var items = Context.Posts
                              .AsNoTracking()
                              .Where(i => i.Id == id);

                items = items.Include(i => i.Gallery);
                
                items = items.Include(i => i.PageCategory);
            items = items.Include(i => i.PostContents);
            items = items.Include("PostContents.Language");

            var itemToReturn = items.FirstOrDefault();

            OnPostGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }
        public async Task<Taraweb.Models.TarawebM1.Post> GetPostBySlug(string slug)
        {
            var items = Context.Posts
                              .AsNoTracking()
                              .Where(i => i.PostContents.Any(w=>w.Slug == slug));

            items = items.Include(i => i.Gallery);
           
            items = items.Include(i => i.PageCategory);

            var itemToReturn = items.FirstOrDefault();

            OnPostGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }
        partial void OnPostCreated(Taraweb.Models.TarawebM1.Post item);
        partial void OnAfterPostCreated(Taraweb.Models.TarawebM1.Post item);

        public async Task<Taraweb.Models.TarawebM1.Post> CreatePost(Taraweb.Models.TarawebM1.Post post)
        {
            OnPostCreated(post);
            if (post.Gallery == null)
            {
                post.Gallery = await context.Galleries.Where(w => w.Id ==1).FirstOrDefaultAsync();
            }
            post.DateCreate = DateTime.Now;
            post.DateUpdate = DateTime.Now;
            var existingItem = Context.Posts
                              .Where(i => i.Id == post.Id)
                              .FirstOrDefault();

                     

            try
            {
                
                    
                    Context.Posts.Add(post);
                    Context.SaveChanges();
                  

               
                
            }
            catch(Exception e)
            {
                Context.Entry(post).State = EntityState.Detached;
                throw new Exception(e.Message);
            }


            OnAfterPostCreated(post);
            return post;
        }

        public async Task<Taraweb.Models.TarawebM1.Post> CancelPostChanges(Taraweb.Models.TarawebM1.Post item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnPostUpdated(Taraweb.Models.TarawebM1.Post item);
        partial void OnAfterPostUpdated(Taraweb.Models.TarawebM1.Post item);

        public async Task<Taraweb.Models.TarawebM1.Post> UpdatePost(int id, Taraweb.Models.TarawebM1.Post post)
        {
            OnPostUpdated(post);

            var itemToUpdate = Context.Posts.Include(i=>i.PostContents)
                              .Where(i => i.Id == post.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }

            foreach(var content in itemToUpdate.PostContents)
            {
                var currcontent = post.PostContents.Where(w => w.Id == content.Id).FirstOrDefault();
                if(currcontent != null)
                {
                    content.Title = currcontent.Title;
                    content.Slug = currcontent.Slug;
                    content.Content = currcontent.Content;
                }
            }

            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(post);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterPostUpdated(post);

            return post;
        }

        partial void OnPostDeleted(Taraweb.Models.TarawebM1.Post item);
        partial void OnAfterPostDeleted(Taraweb.Models.TarawebM1.Post item);

        public async Task<Taraweb.Models.TarawebM1.Post> DeletePost(int id)
        {
            var itemToDelete = Context.Posts
                              .Where(i => i.Id == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnPostDeleted(itemToDelete);


            Context.Posts.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterPostDeleted(itemToDelete);

            return itemToDelete;
        }
        }
}