﻿using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace JeremyTCD.DotNet.CommandLine.Tests.UnitTests
{
    public class CommandUnitTests
    {
        MockRepository _mockRepository = new MockRepository(MockBehavior.Default) { DefaultValue = DefaultValue.Mock };

        [Theory]
        [MemberData(nameof(PrintsParseExceptionAndGetHelpTipIfParseResultContainsAParseExceptionInstanceData))]
        public void Run_PrintsParseExceptionAndGetHelpTipIfParseResultContainsAParseExceptionInstance(bool dummyIsDefault,
            string expectedTargetPosValue, string dummyAndExpectedName)
        {
            // Arrange
            ParseException dummyParseException = new ParseException();
            ParseResult dummyParseResult = new ParseResult(dummyParseException, null);

            Mock<IAppPrinter> mockAppPrinter = _mockRepository.Create<IAppPrinter>();
            mockAppPrinter.Setup(a => a.AppendHeader()).Returns(mockAppPrinter.Object);
            mockAppPrinter.Setup(a => a.AppendLine()).Returns(mockAppPrinter.Object);
            mockAppPrinter.Setup(a => a.AppendParseException(dummyParseException)).Returns(mockAppPrinter.Object);
            mockAppPrinter.Setup(a => a.AppendGetHelpTip(expectedTargetPosValue, dummyAndExpectedName)).Returns(mockAppPrinter.Object);
            mockAppPrinter.Setup(a => a.Print());

            AppContext dummyAppContext = new AppContext(null, null, mockAppPrinter.Object);

            DummyCommand dummyCommand = new DummyCommand(dummyAndExpectedName, isDefault: dummyIsDefault);

            // Act
            int result = dummyCommand.Run(dummyParseResult, dummyAppContext);

            // Assert
            _mockRepository.VerifyAll();
            Assert.Equal(0, result);
        }

        public static IEnumerable<object[]> PrintsParseExceptionAndGetHelpTipIfParseResultContainsAParseExceptionInstanceData()
        {
            string dummyName = "dummyName";

            yield return new object[] { true, "this application", null };
            yield return new object[] { false, "this command", dummyName };
        }

        [Fact]
        public void Run_PrintsAppHelpIfHelpIsTrueAndCommandIsDefaultCommand()
        {
            // Arrange
            ParseResult dummyParseResult = new ParseResult(null, null);

            Mock<IAppPrinter> mockAppPrinter = _mockRepository.Create<IAppPrinter>();
            mockAppPrinter.Setup(a => a.AppendHeader()).Returns(mockAppPrinter.Object);
            mockAppPrinter.Setup(a => a.AppendLine()).Returns(mockAppPrinter.Object);
            mockAppPrinter.Setup(a => a.AppendAppHelp(null, 2)).Returns(mockAppPrinter.Object);
            mockAppPrinter.Setup(a => a.Print());

            AppContext dummyAppContext = new AppContext(null, null, mockAppPrinter.Object);

            DummyCommand dummyCommand = new DummyCommand(isDefault: true, help: true);

            // Act
            int result = dummyCommand.Run(dummyParseResult, dummyAppContext);

            // Assert
            _mockRepository.VerifyAll();
            Assert.Equal(1, result);
        }

        [Fact]
        public void Run_PrintsCommandHelpIfHelpIsTrueAndCommandIsNotDefaultCommand()
        {
            // Arrange
            string dummyName = "dummyName";
            ParseResult dummyParseResult = new ParseResult(null, null);

            Mock<IAppPrinter> mockAppPrinter = _mockRepository.Create<IAppPrinter>();
            mockAppPrinter.Setup(a => a.AppendHeader()).Returns(mockAppPrinter.Object);
            mockAppPrinter.Setup(a => a.AppendLine()).Returns(mockAppPrinter.Object);
            mockAppPrinter.Setup(a => a.AppendCommandHelp(dummyName, null, 2)).Returns(mockAppPrinter.Object);
            mockAppPrinter.Setup(a => a.Print());

            AppContext dummyAppContext = new AppContext(null, null, mockAppPrinter.Object);

            DummyCommand dummyCommand = new DummyCommand(name: dummyName, help: true);

            // Act
            int result = dummyCommand.Run(dummyParseResult, dummyAppContext);

            // Assert
            _mockRepository.VerifyAll();
            Assert.Equal(1, result);
        }

        [Fact]
        public void Run_CallsRunCommand()
        {
            // Arrange
            int exitCode = 1;

            Mock<IAppPrinter> mockAppPrinter = _mockRepository.Create<IAppPrinter>();
            mockAppPrinter.Setup(a => a.AppendHeader()).Returns(mockAppPrinter.Object);
            mockAppPrinter.Setup(a => a.AppendLine()).Returns(mockAppPrinter.Object);
            mockAppPrinter.Setup(a => a.Print());

            ParseResult dummyParseResult = new ParseResult(null, null);
            AppContext dummyAppContext = new AppContext(null, null, mockAppPrinter.Object);

            Mock<DummyCommand> dummyCommand = _mockRepository.Create<DummyCommand>();
            dummyCommand.Setup(c => c.RunCommand(dummyParseResult, dummyAppContext)).Returns(exitCode);
            dummyCommand.CallBase = true;

            // Act
            int result = dummyCommand.Object.Run(dummyParseResult, dummyAppContext);

            // Assert
            _mockRepository.VerifyAll();
            Assert.Equal(exitCode, result);
        }

        public class DummyCommand : Command
        {
            public override string Name { get; }
            public override string Description { get; }
            public override bool IsDefault { get; }
            
            public DummyCommand() : this(null, null, false, false)
            {

            }

            public DummyCommand(string name = null, string description = null, bool isDefault = false, bool help = false)
            {
                Name = name;
                Description = description;
                IsDefault = isDefault;
                Help = help;
            } 

            public override int RunCommand(ParseResult parseResult, AppContext appContext)
            {
                throw new NotImplementedException();
            }
        }
    }
}