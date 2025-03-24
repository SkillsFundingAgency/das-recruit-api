using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using Recruit.Api.Domain.Extensions;
using Recruit.Api.Domain.Models;

namespace Recruit.Api.Domain.Tests.Extensions
{
    [TestFixture]
    public class QueryableExtensionsTests
    {
        private Mock<DbSet<TestEntity>> _mockDbSet;
        private Mock<DbContext> _mockContext;
        private IQueryable<TestEntity> _queryable;

        [SetUp]
        public void SetUp()
        {
            var data = new List<TestEntity>
            {
                new TestEntity { Id = 1, Name = "A" },
                new TestEntity { Id = 2, Name = "B" },
                new TestEntity { Id = 3, Name = "C" }
            }.AsQueryable();

            _mockDbSet = new Mock<DbSet<TestEntity>>();
            _mockDbSet.As<IQueryable<TestEntity>>().Setup(m => m.Provider).Returns(data.Provider);
            _mockDbSet.As<IQueryable<TestEntity>>().Setup(m => m.Expression).Returns(data.Expression);
            _mockDbSet.As<IQueryable<TestEntity>>().Setup(m => m.ElementType).Returns(data.ElementType);
            _mockDbSet.As<IQueryable<TestEntity>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            _mockContext = new Mock<DbContext>();
            _mockContext.Setup(c => c.Set<TestEntity>()).Returns(_mockDbSet.Object);

            _queryable = _mockContext.Object.Set<TestEntity>();
        }

        [Test]
        public async Task GetPagedAsync_ShouldReturnPaginatedList()
        {
            // Arrange
            int pageNumber = 1;
            int pageSize = 2;
            string sortColumn = "Name";
            bool isAscending = true;
            var cancellationToken = CancellationToken.None;

            // Act
            var result = await _queryable.GetPagedAsync(pageNumber, pageSize, sortColumn, isAscending, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(2);
            result.TotalCount.Should().Be(3);
            result.PageIndex.Should().Be(1);
            result.PageSize.Should().Be(2);
            result.TotalPages.Should().Be(2);
            result.HasPreviousPage.Should().BeFalse();
            result.HasNextPage.Should().BeTrue();
        }

        [Test]
        public async Task GetPagedAsync_ShouldSortDescending()
        {
            // Arrange
            int pageNumber = 1;
            int pageSize = 2;
            string sortColumn = "Name";
            bool isAscending = false;
            var cancellationToken = CancellationToken.None;

            // Act
            var result = await _queryable.GetPagedAsync(pageNumber, pageSize, sortColumn, isAscending, cancellationToken);

            // Assert
            result.Should().NotBeNull();
            result.Items.Should().HaveCount(2);
            result.Items.First().Name.Should().Be("C");
            result.Items.Last().Name.Should().Be("B");
        }

        public class TestEntity
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
