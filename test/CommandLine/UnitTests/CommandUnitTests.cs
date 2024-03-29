﻿// Copyright (c) JeremyTCD. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Moq;
using Xunit;

namespace JeremyTCD.DotNet.CommandLine.Tests
{
    public class CommandUnitTests
    {
        private MockRepository _mockRepository = new MockRepository(MockBehavior.Default);

        [Fact]
        public void Run_PrintsParseExceptionAndCommandGetHelpTipIfParseResultContainsAParseExceptionAndCommandIsNotDefaultCommand()
        {
            // Arrange
            bool dummyIsDefault = false;
            string dummyTargetPosValue = "this command";
            string dummyCommandName = "dummyCommandName";
            ParseException dummyParseException = new ParseException();

            Mock<IParseResult> mockParseResult = _mockRepository.Create<IParseResult>();
            mockParseResult.Setup(p => p.ParseException).Returns(dummyParseException);

            Mock<ICommandLineAppPrinter> mockCommandLineAppPrinter = _mockRepository.Create<ICommandLineAppPrinter>();
            mockCommandLineAppPrinter.Setup(a => a.AppendHeader()).Returns(mockCommandLineAppPrinter.Object);
            mockCommandLineAppPrinter.Setup(a => a.AppendLine()).Returns(mockCommandLineAppPrinter.Object);
            mockCommandLineAppPrinter.Setup(a => a.AppendParseException(dummyParseException)).Returns(mockCommandLineAppPrinter.Object);
            mockCommandLineAppPrinter.Setup(a => a.AppendGetHelpTip(dummyTargetPosValue, dummyCommandName)).Returns(mockCommandLineAppPrinter.Object);
            mockCommandLineAppPrinter.Setup(a => a.Print());

            Mock<ICommandLineAppContext> mockCommandLineAppContext = _mockRepository.Create<ICommandLineAppContext>();
            mockCommandLineAppContext.Setup(c => c.CommandLineAppPrinter).Returns(mockCommandLineAppPrinter.Object);

            Mock<Command> testSubject = _mockRepository.Create<Command>();
            testSubject.Setup(c => c.Name).Returns(dummyCommandName);
            testSubject.Setup(c => c.IsDefault).Returns(dummyIsDefault);
            testSubject.CallBase = true;

            // Act
            int result = testSubject.Object.Run(mockParseResult.Object, mockCommandLineAppContext.Object);

            // Assert
            _mockRepository.VerifyAll();
            Assert.Equal(0, result);
        }

        [Fact]
        public void Run_PrintsParseExceptionAndAppGetHelpTipIfParseResultContainsAParseExceptionAndCommandIsDefaultCommand()
        {
            // Arrange
            bool dummyIsDefault = true;
            string dummyTargetPosValue = "this application";
            ParseException dummyParseException = new ParseException();

            Mock<IParseResult> mockParseResult = _mockRepository.Create<IParseResult>();
            mockParseResult.Setup(p => p.ParseException).Returns(dummyParseException);

            Mock<ICommandLineAppPrinter> mockCommandLineAppPrinter = _mockRepository.Create<ICommandLineAppPrinter>();
            mockCommandLineAppPrinter.Setup(a => a.AppendHeader()).Returns(mockCommandLineAppPrinter.Object);
            mockCommandLineAppPrinter.Setup(a => a.AppendLine()).Returns(mockCommandLineAppPrinter.Object);
            mockCommandLineAppPrinter.Setup(a => a.AppendParseException(dummyParseException)).Returns(mockCommandLineAppPrinter.Object);
            mockCommandLineAppPrinter.Setup(a => a.AppendGetHelpTip(dummyTargetPosValue, null)).Returns(mockCommandLineAppPrinter.Object);
            mockCommandLineAppPrinter.Setup(a => a.Print());

            Mock<ICommandLineAppContext> mockCommandLineAppContext = _mockRepository.Create<ICommandLineAppContext>();
            mockCommandLineAppContext.Setup(c => c.CommandLineAppPrinter).Returns(mockCommandLineAppPrinter.Object);

            Mock<Command> testSubject = _mockRepository.Create<Command>();
            testSubject.Setup(c => c.IsDefault).Returns(dummyIsDefault);
            testSubject.CallBase = true;

            // Act
            int result = testSubject.Object.Run(mockParseResult.Object, mockCommandLineAppContext.Object);

            // Assert
            _mockRepository.VerifyAll();
            Assert.Equal(0, result);
        }

        [Fact]
        public void Run_PrintsAppHelpIfHelpIsTrueAndCommandIsDefaultCommand()
        {
            Mock<IParseResult> dummyParseResult = _mockRepository.Create<IParseResult>();

            Mock<ICommandLineAppPrinter> mockCommandLineAppPrinter = _mockRepository.Create<ICommandLineAppPrinter>();
            mockCommandLineAppPrinter.Setup(a => a.AppendHeader()).Returns(mockCommandLineAppPrinter.Object);
            mockCommandLineAppPrinter.Setup(a => a.AppendLine()).Returns(mockCommandLineAppPrinter.Object);
            mockCommandLineAppPrinter.Setup(a => a.AppendAppHelp(null, 2)).Returns(mockCommandLineAppPrinter.Object);
            mockCommandLineAppPrinter.Setup(a => a.Print());

            Mock<ICommandLineAppContext> mockCommandLineAppContext = _mockRepository.Create<ICommandLineAppContext>();
            mockCommandLineAppContext.Setup(c => c.CommandLineAppPrinter).Returns(mockCommandLineAppPrinter.Object);

            Mock<Command> testSubject = _mockRepository.Create<Command>();
            testSubject.Setup(c => c.IsDefault).Returns(true);
            testSubject.Setup(c => c.Help).Returns(true);
            testSubject.CallBase = true;

            // Act
            int result = testSubject.Object.Run(dummyParseResult.Object, mockCommandLineAppContext.Object);

            // Assert
            _mockRepository.VerifyAll();
            Assert.Equal(1, result);
        }

        [Fact]
        public void Run_PrintsCommandHelpIfHelpIsTrueAndCommandIsNotDefaultCommand()
        {
            // Arrange
            string dummyName = "dummyName";
            Mock<IParseResult> dummyParseResult = _mockRepository.Create<IParseResult>();

            Mock<ICommandLineAppPrinter> mockCommandLineAppPrinter = _mockRepository.Create<ICommandLineAppPrinter>();
            mockCommandLineAppPrinter.Setup(a => a.AppendHeader()).Returns(mockCommandLineAppPrinter.Object);
            mockCommandLineAppPrinter.Setup(a => a.AppendLine()).Returns(mockCommandLineAppPrinter.Object);
            mockCommandLineAppPrinter.Setup(a => a.AppendCommandHelp(dummyName, null, 2)).Returns(mockCommandLineAppPrinter.Object);
            mockCommandLineAppPrinter.Setup(a => a.Print());

            Mock<ICommandLineAppContext> mockCommandLineAppContext = _mockRepository.Create<ICommandLineAppContext>();
            mockCommandLineAppContext.Setup(c => c.CommandLineAppPrinter).Returns(mockCommandLineAppPrinter.Object);

            Mock<Command> testSubject = _mockRepository.Create<Command>();
            testSubject.Setup(c => c.Name).Returns(dummyName);
            testSubject.Setup(c => c.Help).Returns(true);
            testSubject.CallBase = true;

            // Act
            int result = testSubject.Object.Run(dummyParseResult.Object, mockCommandLineAppContext.Object);

            // Assert
            _mockRepository.VerifyAll();
            Assert.Equal(1, result);
        }

        [Fact]
        public void Run_CallsRunCommand()
        {
            // Arrange
            int exitCode = 1;

            Mock<ICommandLineAppPrinter> mockCommandLineAppPrinter = _mockRepository.Create<ICommandLineAppPrinter>();
            mockCommandLineAppPrinter.Setup(a => a.AppendHeader()).Returns(mockCommandLineAppPrinter.Object);
            mockCommandLineAppPrinter.Setup(a => a.AppendLine()).Returns(mockCommandLineAppPrinter.Object);
            mockCommandLineAppPrinter.Setup(a => a.Print());

            Mock<IParseResult> dummyParseResult = _mockRepository.Create<IParseResult>();

            Mock<ICommandLineAppContext> mockCommandLineAppContext = _mockRepository.Create<ICommandLineAppContext>();
            mockCommandLineAppContext.Setup(c => c.CommandLineAppPrinter).Returns(mockCommandLineAppPrinter.Object);

            Mock<Command> testSubject = _mockRepository.Create<Command>();
            testSubject.Setup(c => c.RunCommand(dummyParseResult.Object, mockCommandLineAppContext.Object)).Returns(exitCode);
            testSubject.CallBase = true;

            // Act
            int result = testSubject.Object.Run(dummyParseResult.Object, mockCommandLineAppContext.Object);

            // Assert
            _mockRepository.VerifyAll();
            Assert.Equal(exitCode, result);
        }
    }
}
