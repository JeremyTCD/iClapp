﻿// Copyright (c) JeremyTCD. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace JeremyTCD.DotNet.CommandLine
{
    public class Parser : IParser
    {
        private readonly IArgumentAccessorFactory _argumentsAccessorFactory;
        private readonly ICommandMapper _commandMapper;

        /// <summary>
        /// Creates a <see cref="Parser"/> instance.
        /// </summary>
        /// <param name="argumentsAccessorFactory"></param>
        /// <param name="commandMapper"></param>
        public Parser(IArgumentAccessorFactory argumentsAccessorFactory, ICommandMapper commandMapper)
        {
            _argumentsAccessorFactory = argumentsAccessorFactory;
            _commandMapper = commandMapper;
        }

        /// <summary>
        /// Parses <paramref name="args"/>.
        /// </summary>
        /// <param name="args"></param>
        /// <param name="commandDictionary"></param>
        /// <returns>
        /// <see cref="ParseResult"/>
        /// </returns>
        public virtual IParseResult Parse(string[] args, ICommandDictionary commandDictionary)
        {
            ICommand command = null;
            ParseException parseException = null;

            try
            {
                IArgumentAccessor argumentAccessor = _argumentsAccessorFactory.Create(args);
                command = GetCommand(argumentAccessor.CommandName, commandDictionary);

                _commandMapper.Map(argumentAccessor, command);
            }
            catch (Exception exception)
            {
                parseException = exception is ParseException ? exception as ParseException : new ParseException(exception);
            }

            return new ParseResult(parseException, command);
        }

        /// <summary>
        /// Gets <see cref="ICommand"/> with name <paramref name="commandName"/> from <paramref name="commandDictionary"/>.
        /// </summary>
        /// <param name="commandName"></param>
        /// <param name="commandDictionary"></param>
        /// <returns>
        /// <see cref="ICommand"/> with name <paramref name="commandName"/> if <paramref name="commandName"/> is not null, default
        /// <see cref="ICommand"/> otherwise.
        /// </returns>
        /// <exception cref="ParseException">
        /// Thrown if no command with name <paramref name="commandName"/> exists.
        /// </exception>
        internal virtual ICommand GetCommand(string commandName, ICommandDictionary commandDictionary)
        {
            ICommand result;

            if (commandName == null)
            {
                result = commandDictionary.DefaultCommand;
            }
            else
            {
                commandDictionary.TryGetValue(commandName, out result);

                if (result == null)
                {
                    throw new ParseException(string.Format(Strings.ParseException_CommandDoesNotExist, commandName));
                }
            }

            return result;
        }
    }
}
