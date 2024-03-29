﻿// Copyright (c) JeremyTCD. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Moq;
using Xunit;

namespace JeremyTCD.DotNet.CommandLine.Tests
{
    public class CommandLineAppPrinterUnitTests
    {
        private MockRepository _mockRepository = new MockRepository(MockBehavior.Default) { DefaultValue = DefaultValue.Mock };

        [Fact]
        public void AppendHeader_AppendsHeader()
        {
            // Arrange
            string dummyFullName = "dummyFullName";
            string dummyVersion = "dummyVersion";
            CommandLineAppOptions dummyAppOptions = new CommandLineAppOptions() { FullName = dummyFullName, Version = dummyVersion };

            CommandLineAppPrinter testSubject = CreateCommandLineAppPrinter(commandLineAppOptions: dummyAppOptions);

            // Act
            testSubject.AppendHeader();
            string result = testSubject.ToString();

            // Assert
            Assert.Equal(string.Format(Strings.Printer_Header, dummyFullName, dummyVersion), result);
        }

        [Fact]
        public void AppendAppHelp_AppendsAppHelp()
        {
            // Arrange
            int columnGap = 2;
            string columnSeparator = new string(' ', columnGap);
            string rowPrefix = "    ";
            string dummyCommandName = "dummyCommandName";
            string dummyCommandDescription = "dummyCommandDescription";
            string dummyExecutableName = "dummyExecutableName";
            string dummyOptionLongName = "dummyOptionLongName";
            string dummyOptionDescription = "dummyOptionDescription";
            CommandLineAppOptions dummyAppOptions = new CommandLineAppOptions() { ExecutableName = dummyExecutableName };

            Mock<ICommand> mockDefaultCommand = _mockRepository.Create<ICommand>();
            mockDefaultCommand.Setup(m => m.IsDefault).Returns(true);

            Mock<ICommand> mockNamedCommand = _mockRepository.Create<ICommand>();
            mockNamedCommand.Setup(m => m.Name).Returns(dummyCommandName);
            mockNamedCommand.Setup(m => m.Description).Returns(dummyCommandDescription);

            ICommand[] dummyCommands = new[] { mockDefaultCommand.Object, mockNamedCommand.Object };

            Mock<ICommandDictionary> mockCommandDictionary = _mockRepository.Create<ICommandDictionary>();
            mockCommandDictionary.Setup(c => c.Values).Returns(dummyCommands);
            mockCommandDictionary.Setup(c => c.DefaultCommand).Returns(mockDefaultCommand.Object);

            Mock<IOption> mockOption = _mockRepository.Create<IOption>();
            mockOption.Setup(o => o.LongName).Returns(dummyOptionLongName);
            mockOption.Setup(o => o.Description).Returns(dummyOptionDescription);

            OptionCollection dummyOptionCollection = new OptionCollection { mockOption.Object };

            Mock<IOptionCollectionFactory> mockOptionCollectionFactory = _mockRepository.Create<IOptionCollectionFactory>();
            mockOptionCollectionFactory.Setup(o => o.Create(mockDefaultCommand.Object)).Returns(dummyOptionCollection);

            CommandLineAppPrinter testSubject = CreateCommandLineAppPrinter(mockCommandDictionary.Object, dummyAppOptions, mockOptionCollectionFactory.Object);

            // Act
            testSubject.AppendAppHelp(rowPrefix, columnGap);

            // Assert
            string result = testSubject.ToString();
            string expected = $"Usage: '{dummyExecutableName} [command] [command options]'{Environment.NewLine}" +
                              $"Usage: '{dummyExecutableName} [options]'{Environment.NewLine}" +
                              $"{Environment.NewLine}" +
                              $"Commands:{Environment.NewLine}" +
                              $"{rowPrefix}{dummyCommandName}{columnSeparator}{dummyCommandDescription}{Environment.NewLine}" +
                              $"{Environment.NewLine}" +
                              $"Options:{Environment.NewLine}" +
                              $"{rowPrefix}-{dummyOptionLongName}{columnSeparator}{dummyOptionDescription}{Environment.NewLine}" +
                              $"{Environment.NewLine}" +
                              $"Run '{dummyExecutableName} [command] -help' for more information about a command.";
            Assert.Equal(expected, result);
            _mockRepository.VerifyAll();
        }

        [Theory]
        [MemberData(nameof(AppendGetHelpTip_AppendsGetHelpTip_Data))]
        public void AppendGetHelpTip_AppendsGetHelpTip(string dummyCommandPosValue, string dummyTargetPosValue, string dummyExecutableName, string expected)
        {
            // Arrange
            CommandLineAppOptions dummyAppOptions = new CommandLineAppOptions() { ExecutableName = dummyExecutableName };

            CommandLineAppPrinter testSubject = CreateCommandLineAppPrinter(commandLineAppOptions: dummyAppOptions);

            // Act
            testSubject.AppendGetHelpTip(dummyTargetPosValue, dummyCommandPosValue);
            string result = testSubject.ToString();

            // Assert
            Assert.Equal(expected, result);
        }

        public static IEnumerable<object[]> AppendGetHelpTip_AppendsGetHelpTip_Data()
        {
            string dummyCommandPosValue = "dummyCommandPosValue";
            string dummyTargetPosValue = "dummyTargetPosValue";
            string dummyExecutableName = "dummyExecutableName";

            yield return new object[]
            {
                dummyCommandPosValue,
                dummyTargetPosValue,
                dummyExecutableName,
                string.Format(Strings.Printer_GetHelpTip, dummyExecutableName, dummyCommandPosValue + " ", dummyTargetPosValue)
            };
            yield return new object[]
            {
                null,
                dummyTargetPosValue,
                dummyExecutableName,
                string.Format(Strings.Printer_GetHelpTip, dummyExecutableName, string.Empty, dummyTargetPosValue)
            };
        }

        [Theory]
        [MemberData(nameof(AppendUsage_AppendsUsage_Data))]
        public void AppendUsage_AppendsUsage(string dummyOptionsPosValue, string dummyCommandPosValue, string dummyExecutableName, string expected)
        {
            // Arrange
            CommandLineAppOptions dummyAppOptions = new CommandLineAppOptions() { ExecutableName = dummyExecutableName };

            CommandLineAppPrinter testSubject = CreateCommandLineAppPrinter(commandLineAppOptions: dummyAppOptions);

            // Act
            testSubject.AppendUsage(dummyOptionsPosValue, dummyCommandPosValue);
            string result = testSubject.ToString();

            // Assert
            Assert.Equal(expected, result);
        }

        public static IEnumerable<object[]> AppendUsage_AppendsUsage_Data()
        {
            string dummyExecutableName = "dummyExecutableName";
            string dummyOptionsPosValue = "dummyOptionsPosValue";
            string dummyCommandPosValue = "dummyCommandPosValue";

            yield return new object[]
            {
                dummyOptionsPosValue,
                dummyCommandPosValue,
                dummyExecutableName,
                string.Format(Strings.Printer_Usage, dummyExecutableName, dummyCommandPosValue + " ", dummyOptionsPosValue)
            };
            yield return new object[]
            {
                dummyOptionsPosValue,
                null,
                dummyExecutableName,
                string.Format(Strings.Printer_Usage, dummyExecutableName, string.Empty, dummyOptionsPosValue)
            };
        }

        [Fact]
        public void AppendDescription_AppendsDescription()
        {
            // Arrange
            string dummyDescription = "dummyDescription";

            CommandLineAppPrinter testSubject = CreateCommandLineAppPrinter();

            // Act
            testSubject.AppendDescription(dummyDescription);

            // Assert
            string result = testSubject.ToString();
            Assert.Equal(string.Format(Strings.Printer_Description, dummyDescription), result);
        }

        [Fact]
        public void AppendCommandHelp_AppendsCommandHelp()
        {
            // Arrange
            int columnGap = 2;
            string columnSeparator = new string(' ', columnGap);
            string rowPrefix = "    ";
            string dummyCommandName = "dummyCommandName";
            string dummyDescription = "dummyDescription";
            string dummyExecutableName = "dummyExecutableName";
            string dummyOptionLongName = "dummyOptionLongName";
            string dummyOptionDescription = "dummyOptionDescription";
            CommandLineAppOptions dummyAppOptions = new CommandLineAppOptions() { ExecutableName = dummyExecutableName };

            Mock<ICommand> mockCommand = _mockRepository.Create<ICommand>();
            mockCommand.Setup(c => c.Description).Returns(dummyDescription);

            ICommand outValue = mockCommand.Object;
            Mock<ICommandDictionary> mockCommandDictionary = _mockRepository.Create<ICommandDictionary>();
            mockCommandDictionary.Setup(c => c.TryGetValue(dummyCommandName, out outValue)).Returns(true);

            Mock<IOption> mockOption = _mockRepository.Create<IOption>();
            mockOption.Setup(o => o.LongName).Returns(dummyOptionLongName);
            mockOption.Setup(o => o.Description).Returns(dummyOptionDescription);

            OptionCollection dummyOptionCollection = new OptionCollection { mockOption.Object };

            Mock<IOptionCollectionFactory> mockOptionCollectionFactory = _mockRepository.Create<IOptionCollectionFactory>();
            mockOptionCollectionFactory.Setup(o => o.Create(mockCommand.Object)).Returns(dummyOptionCollection);

            CommandLineAppPrinter testSubject = CreateCommandLineAppPrinter(mockCommandDictionary.Object, dummyAppOptions, mockOptionCollectionFactory.Object);

            // Act
            testSubject.AppendCommandHelp(dummyCommandName, rowPrefix, columnGap);

            // Assert
            string result = testSubject.ToString();
            string expected = $"Description: {dummyDescription}{Environment.NewLine}" +
                              $"{Environment.NewLine}" +
                              $"Usage: '{dummyExecutableName} {dummyCommandName} [options]'{Environment.NewLine}" +
                              $"{Environment.NewLine}" +
                              $"Options:{Environment.NewLine}" +
                              $"{rowPrefix}-{dummyOptionLongName}{columnSeparator}{dummyOptionDescription}";
            Assert.Equal(expected, result);
            _mockRepository.VerifyAll();
        }

        [Fact]
        public void AppendCommandHelp_ThrowsInvalidOperationExceptionIfNoCommandWithNameCommandNameExists()
        {
            // Arrange
            string dummyCommandName = "dummyCommandName";

            ICommand dummyOutValue = null;
            Mock<ICommandDictionary> mockCommandDictionary = _mockRepository.Create<ICommandDictionary>();
            mockCommandDictionary.Setup(c => c.TryGetValue(dummyCommandName, out dummyOutValue)).Returns(false);

            CommandLineAppPrinter testSubject = CreateCommandLineAppPrinter(mockCommandDictionary.Object);

            // Act and assert
            InvalidOperationException result = Assert.Throws<InvalidOperationException>(() => testSubject.AppendCommandHelp(dummyCommandName));
            Assert.Equal(string.Format(Strings.Exception_CommandDoesNotExist, dummyCommandName), result.Message);
            _mockRepository.VerifyAll();
        }

        [Theory]
        [MemberData(nameof(AppendParseException_AppendsParseException_Data))]
        public void AppendParseException_AppendsParseException(ParseException parseException, string expected)
        {
            // Arrange
            CommandLineAppPrinter testSubject = CreateCommandLineAppPrinter();

            // Act
            testSubject.AppendParseException(parseException);
            string result = testSubject.ToString();

            // Assert
            Assert.Equal(expected, result);
        }

        public static IEnumerable<object[]> AppendParseException_AppendsParseException_Data()
        {
            string dummyMessage = "dummyMessage";

            yield return new object[] { new ParseException(dummyMessage), dummyMessage };
            yield return new object[] { new ParseException(new ParseException(dummyMessage)), dummyMessage };
        }

        [Theory]
        [MemberData(nameof(GetOptionNames_GetsOptionNames_Data))]
        public void GetOptionNames_GetsOptionNames(string dummyOptionShortName, string dummyOptionLongName, string expected)
        {
            // Arrange
            Mock<IOption> mockOption = _mockRepository.Create<IOption>();
            mockOption.Setup(o => o.LongName).Returns(dummyOptionLongName);
            mockOption.Setup(o => o.ShortName).Returns(dummyOptionShortName);

            CommandLineAppPrinter testSubject = CreateCommandLineAppPrinter();

            // Act
            string result = testSubject.GetOptionNames(mockOption.Object);

            // Assert
            Assert.Equal(expected, result);
            _mockRepository.VerifyAll();
        }

        public static IEnumerable<object[]> GetOptionNames_GetsOptionNames_Data()
        {
            string dummyShortName = "dummyShortName";
            string dummyLongName = "dummyLongName";

            yield return new object[] { dummyShortName, dummyLongName, $"-{dummyShortName}|-{dummyLongName}" };
            yield return new object[] { null, dummyLongName, $"-{dummyLongName}" };
            yield return new object[] { dummyShortName, null, $"-{dummyShortName}" };
            yield return new object[] { " ", dummyLongName, $"-{dummyLongName}" };
            yield return new object[] { dummyShortName, " ", $"-{dummyShortName}" };
        }

        [Theory]
        [MemberData(nameof(GetNormalizedPosValue_GetsNormalizedPosValue_Data))]
        public void GetNormalizedPosValue_GetsNormalizedPosValue(string dummyPosValue, string expected)
        {
            // Arrange
            CommandLineAppPrinter testSubject = CreateCommandLineAppPrinter();

            // Act
            string result = testSubject.GetNormalizedPosValue(dummyPosValue);

            // Assert
            Assert.Equal(expected, result);
        }

        public static IEnumerable<object[]> GetNormalizedPosValue_GetsNormalizedPosValue_Data()
        {
            string dummyPosValue = "dummyPosValue";

            yield return new object[] { dummyPosValue, $"{dummyPosValue} " };
            yield return new object[] { null, string.Empty };
        }

        [Theory]
        [MemberData(nameof(AppendRows_AppendsRows_Data))]
        public void AppendRows_AppendsRows(string dummyRowPrefix, int dummyColumnGap, string expected)
        {
            // Arrange
            // Column 1 - Empty string, increasing length
            // Column 2 - Decreasing length
            // Column 3 - Strings with same length, increasing then decreasing length
            string[][] rows = new string[][]
            {
                new string[] { string.Empty, "123", "1" },
                new string[] { "1", "12", "12" },
                new string[] { "12", "1", "1" }
            };

            CommandLineAppPrinter testSubject = CreateCommandLineAppPrinter();

            // Act
            testSubject.AppendRows(rows, dummyColumnGap, dummyRowPrefix);
            string result = testSubject.ToString();

            // Assert
            Assert.Equal(expected, result);
        }

        public static IEnumerable<object[]> AppendRows_AppendsRows_Data()
        {
            string dummyRowPrefix = "    ";
            int dummyColumnGap = 2;
            string columnSeparator = new string(' ', dummyColumnGap);

            yield return new object[]
            {
                dummyRowPrefix,
                dummyColumnGap, $"{dummyRowPrefix}  {columnSeparator}123{columnSeparator}1 {Environment.NewLine}{dummyRowPrefix}1 {columnSeparator}12 {columnSeparator}12{Environment.NewLine}{dummyRowPrefix}12{columnSeparator}1  {columnSeparator}1 "
            };
            yield return new object[]
            {
                null,
                dummyColumnGap, $"  {columnSeparator}123{columnSeparator}1 {Environment.NewLine}1 {columnSeparator}12 {columnSeparator}12{Environment.NewLine}12{columnSeparator}1  {columnSeparator}1 "
            };
        }

        private CommandLineAppPrinter CreateCommandLineAppPrinter(ICommandDictionary commandDictionary = null, CommandLineAppOptions commandLineAppOptions = null, IOptionCollectionFactory optionCollectionFactory = null)
        {
            return new CommandLineAppPrinter(commandDictionary, commandLineAppOptions, optionCollectionFactory);
        }

        private class DummyCommandWithOptions : ICommand
        {
            public DummyCommandWithOptions(string name = null, string description = null, bool isDefault = false)
            {
                Name = name;
                Description = description;
                IsDefault = isDefault;
            }

            public string Name { get; }

            public string Description { get; }

            public bool IsDefault { get; }

            [Option(LongName = "DummyOptionLongName", Description = "DummyOptionDescription")]
            public string DummyOption { get; }

            public int Run(IParseResult parseResult, ICommandLineAppContext appContext)
            {
                throw new NotImplementedException();
            }
        }
    }
}
