using ECommercePlatform.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ECommercePlatform.Application.Common.Extensions
{
    /// <summary>
    /// Extension methods for PagedResponse to support conversion between entity and DTO types
    /// </summary>
    public static class PagedResponseExtensions
    {
        /// <summary>
        /// Maps a PagedResponse of one type to another using a mapping function
        /// </summary>
        public static PagedResponse<TTarget> MapTo<TSource, TTarget>(
            this PagedResponse<TSource> source,
            Func<TSource, TTarget> mapFunction)
        {
            return new PagedResponse<TTarget>
            {
                Items = source.Items.Select(mapFunction).ToList(),
                TotalCount = source.TotalCount,
                PageNumber = source.PageNumber,
                PageSize = source.PageSize
            };
        }
    }
}