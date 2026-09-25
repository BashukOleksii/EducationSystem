using EducationSystem.DTOs.Common;
using EducationSystem.Models;
using EducationSystem.Repositories.Interfaces;

namespace EducationSystem.Services;

public sealed class GroupService
{
    private readonly IGroupRepository _groupRepository;

    public GroupService(
        IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }

    public Task<PagedResult<Group>> GetPagedAsync(
        string? search,
        int page,
        int pageSize)
    {
        if (page < 1)
        {
            page = 1;
        }

        if (pageSize < 1)
        {
            pageSize = 10;
        }

        return _groupRepository.GetPagedAsync(
            search,
            page,
            pageSize
        );
    }

    public async Task<int> CreateAsync(
        string prefix,
        int number)
    {
        Validate(
            prefix,
            number
        );

        Group group =
            new Group
            {
                Prefix = prefix.Trim(),
                Number = (byte)number
            };

        return await _groupRepository.CreateAsync(
            group
        );
    }

    public async Task UpdateAsync(
        int id,
        string prefix,
        int number)
    {
        Validate(
            prefix,
            number
        );

        Group group =
            new Group
            {
                Id = id,
                Prefix = prefix.Trim(),
                Number = (byte)number
            };

        await _groupRepository.UpdateAsync(
            group
        );
    }

    public Task DeleteAsync(
        int id)
    {
        return _groupRepository.DeleteAsync(
            id
        );
    }

    private static void Validate(
        string prefix,
        int number)
    {
        if (string.IsNullOrWhiteSpace(prefix))
        {
            throw new ArgumentException(
                "Префікс групи не може бути порожнім."
            );
        }

        if (prefix.Trim().Length > 5)
        {
            throw new ArgumentException(
                "Префікс групи не може бути довшим за 5 символів."
            );
        }

        if (number is < 1 or > 255)
        {
            throw new ArgumentException(
                "Номер групи повинен бути від 1 до 255."
            );
        }
    }
}