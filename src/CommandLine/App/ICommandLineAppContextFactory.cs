﻿// Copyright (c) JeremyTCD. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace JeremyTCD.DotNet.CommandLine
{
    /// <summary>
    /// Represents a factory that creates <see cref="ICommandLineAppContext"/>s.
    /// </summary>
    public interface ICommandLineAppContextFactory
    {
        /// <summary>
        /// Creates an instance of a class that implements <see cref="ICommandLineAppContext"/>.
        /// </summary>
        /// <param name="commandDictionary">The <see cref="ICommandLineAppContext"/>'s <see cref="ICommandDictionary"/>.</param>
        /// <param name="appOptions">The <see cref="ICommandLineAppContext"/>'s <see cref="CommandLineAppOptions"/>.</param>
        /// <returns>The new <see cref="ICommandLineAppContext"/>.</returns>
        ICommandLineAppContext Create(ICommandDictionary commandDictionary, CommandLineAppOptions appOptions);
    }
}
