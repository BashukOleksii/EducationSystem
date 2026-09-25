using System;
using System.Collections.Generic;
using System.Text;

namespace EducationSystem.DTOs.Common
{
    public sealed class PagedResult<T>
    {
        public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();

        public int TotalCount { get; init; }
        public int Page { get; init; }
        public int PageSize { get; init; }

        public int TotalPages {
            get
            {
                if (PageSize <= 0)
                    return 0;

                return (int)Math.Ceiling((double)TotalCount / PageSize);
            }
        }

        public bool HasPreviousPage => Page > 1;
        public bool HasNextPage => Page < TotalPages;
    }
}
