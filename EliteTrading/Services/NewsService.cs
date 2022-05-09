using EliteTrading.Extensions;
using EliteTrading.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace EliteTrading.Services {
    public class NewsService {
        
        

        // *************************************************
        // Base object functions
        // *************************************************

        public News GetNews() {
            DefaultCacheProvider _cache = new DefaultCacheProvider();
            var news = (News)_cache.Get("news");
            if (news == null) {
                using (ApplicationDbContext db = new ApplicationDbContext()) {
                    news = (from d in db.News
                                where d.ShowFrom <= DateTime.Now && d.ShowTo >= DateTime.Now
                                select d).FirstOrDefault();
                    if (news != null) {
                        _cache.Set("news", news, 1);
                    }
                }
                
            }
            return news;
        }

        public News GetAdminNews() {
            using (ApplicationDbContext db = new ApplicationDbContext()) {
                News notification = (from d in db.News
                                             select d).FirstOrDefault();
                return notification;
            }
            
        }

        public int SetNews(News model) {
            using (ApplicationDbContext db = new ApplicationDbContext()) {
                List<News> old = db.News.ToList();
                db.News.RemoveRange(old);
                db.News.Add(model);
                db.SaveChanges();

                DefaultCacheProvider _cache = new DefaultCacheProvider();
                _cache.Invalidate("news");
                return model.Id;
            }
        }

        public void Clear() {
            using (ApplicationDbContext db = new ApplicationDbContext()) {
                var news = db.News.ToList();
                db.News.RemoveRange(news);
                db.SaveChanges();

                DefaultCacheProvider _cache = new DefaultCacheProvider();
                _cache.Invalidate("news");
            }
            
        }
    }
}