using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Sample.AspNetCore
{
    public static class HtmlHelperExtensions
    {
        private static Dictionary<Type, IEnumerable<SelectListItem>> cache 
            = new Dictionary<Type, IEnumerable<SelectListItem>>();

        /// <summary>
        /// Returns a select list for the given metadataType.
        /// </summary>
        /// <param name="metadataType">Type to generate a select list for.</param>
        /// <param name="html">The source Html Helper instance.</param>
        /// <returns>A sequence containing the select list for the given metadataType.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown if metadataType has no public static members.
        /// </exception>
        public static IEnumerable<SelectListItem> GetSelectListFromStaticMembers(this IHtmlHelper html, Type metadataType)
        {
            string getDisplayText(MemberInfo member)
            {
                return member
                    .GetCustomAttribute<DisplayAttribute>()
                    ?.Name
                    ?? member.Name;
            };

            // !! 임시구현.
            // 중복 실행이 성능/동작에 크게 영향을 주지 않으므로
            // thread safety를 고려하지 않고 단순 dictionary로 구현합니다.
            if (!cache.TryGetValue(metadataType, out IEnumerable<SelectListItem> value))
            {
                value = metadataType
                    .GetFields(BindingFlags.Public | BindingFlags.Static)
                    .Select(f => new SelectListItem
                    {
                        Text = getDisplayText(f),
                        Value = f.GetValue(null)?.ToString()
                    })
                    .Concat(metadataType
                        .GetProperties(BindingFlags.Public | BindingFlags.Static)
                        .Where(p => p.CanRead)
                        .Select(p => new SelectListItem
                        {
                            Text = getDisplayText(p),
                            Value = p.GetValue(null)?.ToString()
                        }))
                    .ToList();
                cache[metadataType] = value;
            }
            return value;
        }

        /// <summary>
        /// Returns a select list for the given metadataType.
        /// </summary>
        /// <typeparam name="TMetadata">Type to generate a select list for.</typeparam>
        /// <param name="html">The source Html Helper instance.</param>
        /// <returns>A sequence containing the select list for the given metadataType.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown if metadataType has no public static members.
        /// </exception>
        public static IEnumerable<SelectListItem> GetSelectListFromStaticMembers<TMetadata>(this IHtmlHelper html)
            where TMetadata : class
        {
            return GetSelectListFromStaticMembers(html, typeof(TMetadata));
        }
    }
}
