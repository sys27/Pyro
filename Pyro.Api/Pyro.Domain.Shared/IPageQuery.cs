// Copyright (c) Dmytro Kyshchenko. All rights reserved.
// Licensed under the GPL-3.0 license. See LICENSE file in the project root for full license information.

using FluentValidation;

namespace Pyro.Domain.Shared;

public interface IPageQuery<out TOffset>
{
    int Size { get; }

    TOffset? Before { get; }

    TOffset? After { get; }
}

public class PageQueryValidator<TOffset> : AbstractValidator<IPageQuery<TOffset>>
{
    public PageQueryValidator()
    {
        RuleFor(x => x.Size)
            .InclusiveBetween(10, 50);

        RuleFor(x => x)
            .Must(x => x.Before is null || x.After is null)
            .WithMessage("Only one of 'Before' or 'After' can be specified.");
    }
}