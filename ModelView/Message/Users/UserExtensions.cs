using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FinalApi;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ModelView;

namespace ModelView
{
    public static class UserExtensions
    {
        // Extension Method لتطبيق الفلترة والـPagination والـSorting
        public static async Task<PaginatedUsersResult> ApplyFilterAndPaginationAsync(
            this IQueryable<User> query,
            string search,
            int page,
            int pageSize,
            UserManager<User> userManager,
            string orderByColumn = "Name", // العمود الافتراضي
            bool isAscending = true)
        {
            // الفلترة
            if (!string.IsNullOrWhiteSpace(search))
            {
                // تقسيم النص المدخل إلى كلمات
                var searchTerms = search.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                // تطبيق الفلترة مع تجاهل الفرق بين الحروف الكبيرة والصغيرة
                query = query.Where(u =>
                    searchTerms.All(term =>
                        u.Name.ToLower().Contains(term.ToLower()) ||
                        u.Email.ToLower().Contains(term.ToLower()) ||
                        (userManager.GetRolesAsync(u).Result.Any(role => role.ToLower().Contains(term.ToLower())))
                    )
                );
            }



            // الترتيب الديناميكي
            query = query.OrderBys(orderByColumn, isAscending);

            // حساب العدد الإجمالي بعد الفلترة
            var totalUsers = await query.CountAsync();

            // تطبيق الـPagination
            var users = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // جلب الأدوار لكل مستخدم
            var userViewModels = new List<UserViewModel>();
            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);
                userViewModels.Add(new UserViewModel
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = roles.FirstOrDefault() ?? "No Role"
                });
            }

            // إرجاع النتيجة مع الفلترة والـPagination
            return new PaginatedUsersResult
            {
                TotalUsers = totalUsers,
                Users = userViewModels
            };
        }

        // Extension Method لترتيب البيانات بناءً على اسم العمود
        public static IQueryable<T> OrderBys<T>(this IQueryable<T> source, string columnName, bool isAscending)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
            MemberExpression property = Expression.Property(parameter, columnName);
            LambdaExpression lambda = Expression.Lambda(property, parameter);

            string methodName = isAscending ? "OrderBy" : "OrderByDescending";
            MethodCallExpression methodCall = Expression.Call(
                typeof(Queryable),
                methodName,
                new Type[] { typeof(T), property.Type },
                source.Expression,
                Expression.Quote(lambda)
            );

            return source.Provider.CreateQuery<T>(methodCall);
        }
    }
}
