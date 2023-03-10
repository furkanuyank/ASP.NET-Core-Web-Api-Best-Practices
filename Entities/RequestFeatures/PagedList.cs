using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.RequestFeatures
{
    public class PagedList<T> : List<T>
    {
        public MetaData MetaData { get; set; }

        public PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            MetaData = new MetaData()
            {
                TotalCount = count,
                PageSize = pageSize <= 0 ? count : pageSize,
                TotalPage = pageSize <= 0 ? 1 : (int)Math.Ceiling(count / (double)pageSize),
                CurrentPage = pageNumber <= 0 ? 1 : pageNumber
            };
            AddRange(items);
        }

        public static PagedList<T> ToPagedList(IEnumerable<T> source, int pageNumber, int pageSize)
        {
            var count = source.Count();
            var totalPage = pageSize <= 0 ? 1 : (int)Math.Ceiling(count / (double)pageSize);
            List<T> items;

            if (pageSize <= 0 && totalPage >= pageNumber)
                items = source.ToList();

            else
                items = source
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}
