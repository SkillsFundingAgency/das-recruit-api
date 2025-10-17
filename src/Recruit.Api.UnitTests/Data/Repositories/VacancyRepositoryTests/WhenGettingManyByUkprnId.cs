using Microsoft.EntityFrameworkCore;
using SFA.DAS.Recruit.Api.Data;
using SFA.DAS.Recruit.Api.Data.Repositories;
using SFA.DAS.Recruit.Api.Domain.Entities;
using SFA.DAS.Recruit.Api.Domain.Enums;

namespace SFA.DAS.Recruit.Api.UnitTests.Data.Repositories.VacancyRepositoryTests;
[TestFixture]
internal class WhenGettingManyByUkprnId
{
    private DbContextOptions<RecruitDataContext> _dbOptions;
    private RecruitDataContext _context;
    private VacancyRepository _repository;

    [SetUp]
    public void SetUp()
    {
        _dbOptions = new DbContextOptionsBuilder<RecruitDataContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new TestDataContext(_dbOptions);

        // seed some test data
        _context.VacancyEntities.AddRange(new[]
        {
            new VacancyEntity { Id = Guid.NewGuid(), OwnerType = OwnerType.Provider, Ukprn = 123, Title = "Vac A", ClosingDate = new DateTime(2025, 1, 1), Status = VacancyStatus.Approved },
            new VacancyEntity { Id = Guid.NewGuid(), OwnerType = OwnerType.Provider, Ukprn = 123, Title = "Vac B", ClosingDate = new DateTime(2025, 2, 1), Status = VacancyStatus.Approved },
            new VacancyEntity { Id = Guid.NewGuid(), OwnerType = OwnerType.Provider,  Ukprn = 123, Title = "Vac C", ClosingDate = new DateTime(2025, 3, 1), Status = VacancyStatus.Closed },
            new VacancyEntity { Id = Guid.NewGuid(), OwnerType = OwnerType.Provider, Ukprn = 999, Title = "Other Ukprn", ClosingDate = new DateTime(2025, 4, 1), Status = VacancyStatus.Live }
        });
        _context.SaveChanges();

        _repository = new VacancyRepository(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    [Test]
    public async Task GetManyByUkprnIdAsync_Should_Filter_ByUkprn()
    {
        // Act
        var result = await _repository.GetManyByUkprnIdAsync(123, 1, 25, v => v.CreatedDate, SortOrder.Asc, FilteringOptions.All,"", CancellationToken.None);

        // Assert
        result.TotalCount.Should().Be(3);
        result.Items.Should().OnlyContain(x => x.Ukprn == 123);
    }

    [Test]
    public async Task GetManyByUkprnIdAsync_Should_ApplyDefaultPaging()
    {
        // Act
        var result = await _repository.GetManyByUkprnIdAsync(123, 1, 25, v => v.CreatedDate, SortOrder.Asc, FilteringOptions.All, "", CancellationToken.None);

        // Assert
        result.PageIndex.Should().Be(1);
        result.PageSize.Should().Be(25);
        result.Items.Should().HaveCount(3);
    }

    [Test]
    public async Task GetManyByUkprnIdAsync_Should_ApplyPaging()
    {
        // Act
        var result = await _repository.GetManyByUkprnIdAsync(123, page: 2, pageSize: 2, v => v.CreatedDate);

        // Assert
        result.PageIndex.Should().Be(2);
        result.PageSize.Should().Be(2);
        result.Items.Should().HaveCount(1);
    }

    [Test]
    public async Task GetManyByUkprnIdAsync_Should_OrderDescending_WhenSortOrderDesc()
    {
        // Act
        var result = await _repository.GetManyByUkprnIdAsync(
            123,
            orderBy: v => v.ClosingDate,
            sortOrder: SortOrder.Desc);

        // Assert
        result.Items.First().ClosingDate.Should().Be(new DateTime(2025, 3, 1));
    }

    [Test]
    public async Task GetManyByUkprnIdAsync_Should_OrderAscending_WhenSortOrderAsc()
    {
        // Act
        var result = await _repository.GetManyByUkprnIdAsync(
            123,
            orderBy: v => v.ClosingDate,
            sortOrder: SortOrder.Asc);

        // Assert
        result.Items.First().ClosingDate.Should().Be(new DateTime(2025, 1, 1));
    }

    [Test]
    public async Task GetManyByUkprnIdAsync_Should_ReturnEmptyList_WhenNoMatches()
    {
        // Act
        var result = await _repository.GetManyByUkprnIdAsync(555, 1, 25, v => v.CreatedDate, SortOrder.Asc, FilteringOptions.All, "", CancellationToken.None);

        // Assert
        result.TotalCount.Should().Be(0);
        result.Items.Should().BeEmpty();
    }

    public class TestDataContext(DbContextOptions<RecruitDataContext> options) : RecruitDataContext(options)
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {}
    }
}
