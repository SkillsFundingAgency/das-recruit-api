using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Recruit.Api.Domain.Tests.Extensions
{
    [TestFixture]
    public class IQueryableExtensionsTests
    {
        private class TestEntity
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
        }

        private DbContextOptions<TestDbContext> GetDbOptions()
        {
            return new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }

        private class TestDbContext : DbContext
        {
            public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }
            public DbSet<TestEntity> TestEntities { get; set; }
        }

        [Test]
        public async Task GetPagedAsync_Returns_CorrectPage()
        {
            var options = GetDbOptions();

            using (var context = new TestDbContext(options))
            {
                context.TestEntities.AddRange(
                    Enumerable.Range(1, 20).Select(i => new TestEntity { Id = i, Name = $"Name{i}" })
                );
                await context.SaveChangesAsync();
            }

            using (var context = new TestDbContext(options))
            {
                var query = context.TestEntities.AsQueryable();
                var pagedResult = await query.GetPagedAsync(2, 5, "Id", true);

                Assert.Equal(20, pagedResult.TotalRecords);
                Assert.Equal(2, pagedResult.PageNumber);
                Assert.Equal(5, pagedResult.PageSize);
                Assert.Equal(5, pagedResult.Items.Count);
                Assert.Equal(6, pagedResult.Items.First().Id);
            }
        }

        [Test]
        public async Task GetPagedAsync_Sorts_Descending()
        {
            var options = GetDbOptions();

            using (var context = new TestDbContext(options))
            {
                context.TestEntities.AddRange(
                    Enumerable.Range(1, 20).Select(i => new TestEntity { Id = i, Name = $"Name{i}" })
                );
                await context.SaveChangesAsync();
            }

            using (var context = new TestDbContext(options))
            {
                var query = context.TestEntities.AsQueryable();
                var pagedResult = await query.GetPagedAsync(1, 5, "Id", false);

                Assert.Equal(20, pagedResult.TotalRecords);
                Assert.Equal(1, pagedResult.PageNumber);
                Assert.Equal(5, pagedResult.PageSize);
                Assert.Equal(5, pagedResult.Items.Count);
                Assert.Equal(20, pagedResult.Items.First().Id);
            }
        }
    }
}
